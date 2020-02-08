using System;
using System.Reactive;
using Shiny;

namespace Samples.ShinyDelegates
{
    public static class BackgroundEvents
    {
        public static IObservable<Unit> OnBgEvent(this IMessageBus bus, string eventName)
            => bus.Listener<Unit>(eventName);
    }
}
