using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;


namespace Samples.ShinySetup
{
    public interface IFullService
    {

    }


    public class FullService : ReactiveObject, IFullService, IShinyStartupTask
    {
        [Reactive] public int Count { get; set;}
        public void Start()
        {
            this.Count++;
            Console.WriteLine("Startup Count today is " + this.Count);
        }
    }
}
