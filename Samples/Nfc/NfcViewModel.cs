using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.Nfc;

using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Samples.Nfc
{
    public class NfcViewModel : ViewModel
    {
        public NfcViewModel(INfcManager? nfcManager = null)
        {
            this.CheckPermission = ReactiveCommand.CreateFromTask(() => this.DoCheckPermission(nfcManager));

            this.Clear = ReactiveCommand.Create(() =>
                this.NDefRecords.Clear()
            );

            this.Listen = ReactiveCommand.CreateFromTask(async () =>
            {
                await this.DoCheckPermission(nfcManager);
                if (this.Access != AccessState.Available)
                    return;

                if (this.IsListening)
                {
                    this.IsListening = false;
                    this.Deactivate();
                }
                else
                {
                    nfcManager
                        .Reader()
                        .SelectMany(x => x.Select(y => new NDefItemViewModel(y)))
                        .SubOnMainThread(this.NDefRecords.Add)
                        .DisposeWith(this.DeactivateWith);
                    this.IsListening = true;
                }
            });
        }



        public ICommand Clear { get; }
        public ICommand Listen { get; }
        public ICommand CheckPermission { get; }
        public ObservableList<NDefItemViewModel> NDefRecords { get; } = new ObservableList<NDefItemViewModel>();
        [Reactive] public AccessState Access { get; private set; } = AccessState.Unknown;
        [Reactive] public bool IsListening { get; private set; }


        async Task DoCheckPermission(INfcManager? manager = null)
        {
            if (manager == null)
                this.Access = AccessState.NotSupported;
            else
                this.Access = await manager.RequestAccess().ToTask();
        }
    }
}
