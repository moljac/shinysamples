using Shiny.Locations;
using System;


namespace Samples.Gps
{
    //https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map
    public class MapViewModel : ViewModel
    {
        readonly IGpsManager gpsManager;


        public MapViewModel(IGpsManager gpsManager)
        {
            this.gpsManager = gpsManager;
        }
    }
}
