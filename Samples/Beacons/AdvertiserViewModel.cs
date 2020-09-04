using System;
using System.Windows.Input;
using Shiny.Beacons.Advertising;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Infrastructure;


namespace Samples.Beacons
{
    public class AdvertiserViewModel : ViewModel
    {
        const string StartText = "Start Advertising";
        const string StopText = "Stop Advertising";


        public AdvertiserViewModel(IDialogs dialogs, IBeaconAdvertiser? advertiser = null)
        {
            this.Toggle = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (advertiser == null)
                    {
                        await dialogs.Alert("Beacon advertising not supported on this platform");
                        return;
                    }
                    if (advertiser.Uuid != null)
                    {
                        this.ToggleText = StartText;
                        await advertiser.Stop();
                    }
                    else
                    {
                        var uuid = Guid.Parse(this.Uuid);
                        var major = UInt16.Parse(this.Major);
                        var minor = UInt16.Parse(this.Minor);
                        Byte.Parse(this.TxPower);

                        await advertiser.Start(uuid, major, minor);
                        this.ToggleText = StopText;
                    }
                },
                this.WhenAny(
                    x => x.Uuid,
                    x => x.Major,
                    x => x.Minor,
                    x => x.TxPower,
                    (uuid, major, minor, tx) =>
                    {
                        if (!Guid.TryParse(uuid.GetValue(), out var _))
                            return false;

                        if (!UInt16.TryParse(major.GetValue(), out var M) && M > 0)
                            return false;

                        if (!UInt16.TryParse(minor.GetValue(), out var m) && m > 0)
                            return false;

                        if (!Byte.TryParse(tx.GetValue(), out var _))
                            return false;

                        return true;
                    }
                )
            );
        }


        public ICommand Toggle { get; }
        [Reactive] public string ToggleText { get; private set; } = StartText;
        [Reactive] public string Uuid { get; set; } = Constants.EstimoteUuid;
        [Reactive] public string Major { get; set; }
        [Reactive] public string Minor { get; set; }
        [Reactive] public string TxPower { get; set; } = "0";
    }
}
