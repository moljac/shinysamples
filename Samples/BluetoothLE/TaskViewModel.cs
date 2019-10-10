using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Samples.BluetoothLE
{
    public class TaskViewModel : ViewModel
    {
        CancellationTokenSource cancelSrc;


        public TaskViewModel(string text, Func<CancellationToken, Task> run, IObservable<bool> whenAny = null)
        {
            this.Text = text;

            this.WhenAnyValue(x => x.IsBusy)
                .Select(x => x ? "Stop" : "Start")
                .ToPropertyEx(this, x => x.ButtonText);

            this.Run = ReactiveCommand.CreateFromTask(async () =>
            {
                if (this.cancelSrc == null)
                {
                    this.cancelSrc = new CancellationTokenSource();
                    await run(this.cancelSrc.Token);
                }
                else
                {
                    this.Cancel();
                }
            }, whenAny);

            this.BindBusyCommand(this.Run);
        }


        public IReactiveCommand Run { get; }
        public string Text { get; }
        public string ButtonText { [ObservableAsProperty] get; }


        public void Cancel()
        {
            try
            {
                this.IsBusy = false;
                this.cancelSrc?.Cancel();
                this.cancelSrc = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

