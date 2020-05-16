using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.Push;


namespace Samples.Push
{
    public class SetupViewModel : ViewModel
    {
        readonly IPushManager? pushManager;


        public SetupViewModel(IPushManager? pushManager = null)
        {
            this.pushManager = pushManager;

            this.RequestAccess = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (this.pushManager == null)
                        return;

                    var result = await this.pushManager.RequestAccess();
                    this.AccessStatus = result.Status;
                    this.Refresh();
                }
            );
            this.UnRegister = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (this.pushManager == null)
                        return;

                    await this.pushManager.UnRegister();
                    this.AccessStatus = AccessState.Disabled;
                    this.Refresh();
                },
                this.WhenAny(
                    x => x.RegToken,
                    x => !x.GetValue().IsEmpty()
                )
            );
            this.UpdateTag = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (this.pushManager == null)
                        return;

                    await this.pushManager.TryUpdateTags(this.Tag);
                    this.Refresh();
                },
                this.WhenAny(
                    x => x.Tag,
                    x => x.RegToken,
                    (tag, token) =>
                        this.pushManager?.IsTagsSupport() ?? false &&
                        !tag.GetValue().IsEmpty() &&
                        !token.GetValue().IsEmpty()
                )
            );
        }


        public ICommand RequestAccess { get; }
        public ICommand UnRegister { get; }
        public ICommand UpdateTag { get; }

        public bool IsTagsSupported => this.pushManager?.IsTagsSupport() ?? false;
        [Reactive] public string Tag { get; set; }
        [Reactive] public string RegToken { get; private set; }
        [Reactive] public DateTime? RegDate { get; private set; }
        [Reactive] public DateTime? ExpiryDate { get; private set; }
        [Reactive] public AccessState AccessStatus { get; private set; } = AccessState.Unknown;


        void Refresh()
        {
            this.RegToken = pushManager?.CurrentRegistrationToken ?? "-";
            this.RegDate = pushManager?.CurrentRegistrationTokenDate;
            this.ExpiryDate = pushManager?.CurrentRegistrationExpiryDate;
            this.Tag = pushManager?.TryGetTags()?.FirstOrDefault() ?? "-";
        }
    }
}
