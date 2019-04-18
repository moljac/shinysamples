using System;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Maps;


namespace Samples.Gps
{
    public partial class MapPage : ContentPage
    {
        IDisposable gpsSub;


        public MapPage()
        {
            this.InitializeComponent();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.gpsSub = this.ViewModel
                .WhenAnyValue(x => x.Position)
                .Subscribe(x =>
                {
                    myMap.Pins.Clear();
                    myMap.Pins.Add(new Pin
                    {
                        Label = "YOU",
                        Position = new Position(x.Latitude, x.Longitude)
                    });
                });
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.gpsSub.Dispose();
        }

        public MapViewModel ViewModel => this.BindingContext as MapViewModel;
    }
}