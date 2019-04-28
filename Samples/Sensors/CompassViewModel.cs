using System;
using System.Reactive.Disposables;
using ReactiveUI.Fody.Helpers;
using Shiny.Sensors;


namespace Samples.Sensors
{
    public class CompassViewModel : ViewModel
    {
        readonly ICompass compass;
        public CompassViewModel(ICompass compass)
            => this.compass = compass;


        [Reactive] public double Rotation { get; private set; }
        [Reactive] public double Heading { get; private set; }


        public override void OnAppearing()
        {
            base.OnAppearing();
            this.compass
                .WhenReadingTaken()
                .Subscribe(x =>
                {
                    this.Rotation = 360 - x.MagneticHeading;
                    this.Heading = x.MagneticHeading;
                })
                .DisposeWith(this.DeactivateWith);
        }
    }
}
