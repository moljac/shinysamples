using System;
using Xamarin;

namespace Samples.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            FormsMaps.Init("INSERT_AUTHENTICATION_TOKEN_HERE");
            this.LoadApplication(new Samples.App());
        }
    }
}
