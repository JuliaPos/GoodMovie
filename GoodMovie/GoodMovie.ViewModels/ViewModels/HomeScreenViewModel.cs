using Caliburn.Micro;
using GoodMovie.Contracts;

namespace GoodMovie.ViewModels.ViewModels
{
    public class HomeScreenViewModel:Screen
    {
        private readonly IAppLogger _appLogger;

        public HomeScreenViewModel(IAppLogger appLogger)
        {
            _appLogger = appLogger;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _appLogger.Info("you are good baby");
            _appLogger.Error("my boy");
        }
    }
}