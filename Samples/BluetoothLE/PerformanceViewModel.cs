using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Humanizer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.BluetoothLE.Central;


namespace Samples.BluetoothLE
{
    public class PerformanceViewModel : ViewModel
    {
        const string DefaultServiceUuid = "A495FF20-C5B1-4B44-B512-1370F02D74DE";
        const string DefaultCharacteristicUuid = "A495FF21-C5B1-4B44-B512-1370F02D74DE";

        readonly ICentralManager centralManager;
        int bytes;
        IDisposable notifySub;
        IDisposable speedSub;

        public PerformanceViewModel(ICentralManager centralManager)
        {
            this.centralManager = centralManager;

            this.Permissions = ReactiveCommand.CreateFromTask(async () =>
                this.Status = await this.centralManager.RequestAccess().ToTask()
            );

            this.WhenAnyValue(x => x.IsRunning)
                .Skip(1)
                .Subscribe(x =>
                {
                    if (!x)
                        this.speedSub?.Dispose();
                    else
                    {
                        this.speedSub = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                        {
                            this.Speed = (this.bytes / 2).Bytes().Humanize();
                            Interlocked.Exchange(ref this.bytes, 0);
                        });
                    }
                });

            this.WriteTest = this.DoWork(async (ch, ct) =>
            {
                // need to send MTU size
                await ch.Write(new byte[0x01], true).ToTask(ct);
                return 1;
            });
            this.WriteWithoutResponseTest = this.DoWork(async (ch, ct) =>
            {
                // need to send MTU size
                await ch.Write(new byte[0x01], false).ToTask(ct);
                return 1;
            });
            this.ReadTest = this.DoWork(async (ch, ct) =>
            {
                var read = await ch.Read().ToTask(ct);
                return read.Data.Length;
            });

            this.NotifyTest = ReactiveCommand
                .CreateFromTask(
                    async () =>
                    {
                        this.IsRunning = true;
                        var characteristic = await this.SetupCharacteristic(this.cancelSrc.Token);

                        this.notifySub = characteristic
                            .RegisterAndNotify(true)
                            .Subscribe(x =>
                                Interlocked.Add(ref this.bytes, x.Data.Length)
                            );
                    },
                    this.CanRun()
            );

            this.Stop = ReactiveCommand.Create(
                () =>
                {
                    this.IsRunning = false;
                    this.cancelSrc?.Cancel();
                    this.notifySub?.Dispose();
                    this.notifySub = null;
                },
                this.WhenAny(
                    x => x.IsRunning,
                    x => x.GetValue()
                )
            );
        }
        public ICommand WriteTest { get; }
        public ICommand WriteWithoutResponseTest { get; }
        public ICommand ReadTest { get; }
        public ICommand NotifyTest { get; }
        public ICommand Stop { get; }
        public ICommand Permissions { get; }

        [Reactive] public string DeviceName { get; set; } = "DABOMB";
        [Reactive] public bool IsConnected { get; private set; }
        [Reactive] public int MTU { get; private set; }
        [Reactive] public string Speed { get; private set; }
        [Reactive] public string ServiceUuid { get; set; } = DefaultServiceUuid;
        [Reactive] public string CharacteristicUuid { get; set; } = DefaultCharacteristicUuid;
        [Reactive] public bool IsRunning { get; private set; }
        [Reactive] public AccessState Status { get; private set; }


        CancellationTokenSource cancelSrc;
        ICommand DoWork(Func<IGattCharacteristic, CancellationToken, Task<int>> func) => ReactiveCommand
            .CreateFromTask(async () =>
            {
                this.IsRunning = true;
                try
                {
                    this.cancelSrc = new CancellationTokenSource();
                    var characteristic = await this.SetupCharacteristic(this.cancelSrc.Token);

                    while (!this.cancelSrc.IsCancellationRequested)
                    {
                        var length = await func(characteristic, this.cancelSrc.Token);
                        Interlocked.Add(ref this.bytes, length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    this.IsRunning = false;
                }
            },
            this.CanRun()
        );


        async Task<IGattCharacteristic> SetupCharacteristic(CancellationToken cancelToken)
        {
            var suuid = Guid.Parse(this.ServiceUuid);
            var cuuid = Guid.Parse(this.CharacteristicUuid);

            var peripheral = await this.centralManager
                .ScanUntilPeripheralFound(this.DeviceName)
                .ToTask(cancelToken);

            await peripheral
                .ConnectWait()
                .ToTask(cancelToken);

            this.MTU = await peripheral
                .RequestMtu(512)
                .ToTask(cancelToken);

            var characteristic = await peripheral
                .GetKnownCharacteristics(
                    suuid,
                    cuuid
                )
                .ToTask(cancelToken);

            return characteristic;
        }


        IObservable<bool> CanRun() => this.WhenAny(
            x => x.Status,
            x => x.DeviceName,
            x => x.ServiceUuid,
            x => x.CharacteristicUuid,
            x => x.IsRunning,
            (stat, dn, suuid, cuuid, run) =>
                stat.GetValue() == AccessState.Available &&
                !dn.GetValue().IsEmpty() &&
                Guid.TryParse(suuid.GetValue(), out _) &&
                Guid.TryParse(cuuid.GetValue(), out _) &&
                !run.GetValue()
        );
    }
}