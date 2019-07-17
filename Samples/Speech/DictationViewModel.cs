using System;
using System.Windows.Input;
using Acr.UserDialogs.Forms;
using Shiny.SpeechRecognition;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Samples.Speech
{
    public class DictationViewModel : ViewModel
    {
        public DictationViewModel(ISpeechRecognizer speech, IUserDialogs dialogs)
        {
            IDisposable token = null;
            speech
                .WhenListeningStatusChanged()
                .SubOnMainThread(x => this.ListenText = x
                    ? "Stop Listening"
                    : "Start Dictation"
                );


            this.ToggleListen = ReactiveCommand.Create(()  =>
            {
                if (token == null)
                {
                    if (this.UseContinuous)
                    {
                        token = speech
                            .ContinuousDictation()
                            .SubOnMainThread(
                                x => this.Text += " " + x,
                                ex => dialogs.Alert(ex.ToString())
                            );
                    }
                    else
                    {
                        token = speech
                            .ListenUntilPause()
                            .SubOnMainThread(
                                x => this.Text = x,
                                ex => dialogs.Alert(ex.ToString())
                            );
                    }
                }
                else
                {
                    token.Dispose();
                    token = null;
                }
            });
        }


        public ICommand ToggleListen { get; }
        [Reactive] public bool UseContinuous { get; set; } = true;
        [Reactive] public string ListenText { get; private set; } = "Start Listening";
        [Reactive] public string Text { get; private set; }
    }
}
