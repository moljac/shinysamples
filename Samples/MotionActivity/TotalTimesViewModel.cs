using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Locations;


namespace Samples.MotionActivity
{
    public class TotalTimesViewModel : ViewModel
    {
        public TotalTimesViewModel(IMotionActivity activityManager)
        {
            this.Load = ReactiveCommand.CreateFromTask(async () =>
            {
                var data = await activityManager.GetTotalsForRange(this.Date.Date, this.Date.Date.AddDays(1));
                this.List = data
                    .OrderBy(x => x.Key.ToString())
                    .Select(x => new CommandItem
                    {
                        Text = x.Key.ToString(),
                        Detail = x.Value.ToString()
                    })
                    .ToList();
            });
            this.BindBusyCommand(this.Load);
            this.WhenAnyValue(x => x.Date)
                .InvokeCommand((ICommand)this.Load);
        }


        public IReactiveCommand Load { get; }
        [Reactive] public DateTime Date { get; set; } = DateTime.Now;
        [Reactive] public IList<CommandItem> List { get; private set; }
    }
}
