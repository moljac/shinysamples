using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using Acr.UserDialogs.Forms;
using Shiny;
using Shiny.Logging;


namespace Samples
{
    public static class Extensions
    {
        public static async Task<bool> RequestAccess(this IUserDialogs dialogs, Func<Task<AccessState>> request)
        {
            var access = await request();
            return await dialogs.AlertAccess(access);
        }


        public static async Task<bool> AlertAccess(this IUserDialogs dialogs,  AccessState access)
        {
            switch (access)
            {
                case AccessState.Available:
                    return true;

                case AccessState.Restricted:
                    await dialogs.Alert("WARNING: Access is restricted");
                    return true;

                default:
                    await dialogs.Alert("Invalid Access State: " + access);
                    return false;
            }
        }


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext);


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext, Action<Exception> onError)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext, onError);


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext, Action<Exception> onError, Action onComplete)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext, onError, onComplete);


        public static async Task Navigate(this INavigationService nav, string uri, INavigationParameters parms, bool useModal = false)
        {
            var result = await nav.NavigateAsync(uri, parms, useModal);
            if (!result.Success)
                Log.Write(result.Exception);
        }


        public static async Task Navigate(this INavigationService nav, string uri, bool useModal = false, params (string, object)[] args)
        {
            var result = await nav.NavigateAsync(uri, ToParameters(args), useModal);
            if (!result.Success)
                Log.Write(result.Exception);
        }


        public static ICommand NavigateCommand(this INavigationService nav, string uri, Action<INavigationParameters> getArgs = null, bool useModal = false)
            => ReactiveCommand.CreateFromTask(async _ =>
            {
                var parms = new NavigationParameters();
                getArgs?.Invoke(parms);
                await nav.Navigate(uri, parms, useModal);
            });


        public static ICommand NavigateCommand<T>(this INavigationService nav, string uri, Action<T, INavigationParameters> getArgs = null, bool useModal = false)
            => ReactiveCommand.CreateFromTask<T>(async arg =>
            {

                var parms = new NavigationParameters();
                getArgs?.Invoke(arg, parms);
                await nav.Navigate(uri, parms, useModal);
            });


        public static ICommand GoBackCommand(this INavigationService nav, Action<INavigationParameters> getArgs = null)
            => ReactiveCommand.CreateFromTask(async _ =>
            {
                var parms = new NavigationParameters();
                getArgs?.Invoke(parms);
                await nav.GoBack(false, parms);
            });


        public static Task GoBack(this INavigationService nav, bool toRoot = false, params (string, object)[] args)
        {
            var parms = ToParameters(args);
            return nav.GoBack(toRoot, parms);
        }


        public static async Task GoBack(this INavigationService nav, bool toRoot = false, INavigationParameters parms = null)
        {
            if (toRoot)
                await nav.GoBackToRootAsync(parms);
            else
                await nav.GoBackAsync(parms);
        }


        public static INavigationParameters Set(this INavigationParameters parms, string key, object value)
        {
            parms.Add(key, value);
            return parms;
        }


        public static INavigationParameters ToParameters(params (string, object)[] args)
        {
            var parms = new NavigationParameters();
            if (args != null)
                foreach (var arg in args)
                    parms.Add(arg.Item1, arg.Item2);

            return parms;
        }
    }
}
