using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using GoodMovie.Contracts;
using GoodMovie.Logger;
using GoodMovie.ViewModels.ViewModels;
using GoodMovie.Views;

namespace GoodMovie
{
    public sealed partial class App
    {
        private WinRTContainer _container;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "GoodMovie.Views",
                DefaultSubNamespaceForViewModels = "GoodMovie.ViewModels.ViewModels"
            };
            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);
            RegisterViewModels();
            RegisterServices();
        
            }

        private void RegisterViewModels()
        {
            _container.PerRequest<HomeScreenViewModel>();
           
        }

        private void RegisterServices()
        {
            _container.Singleton<IAppLogger, AppLogger>();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;
            throw new Exception("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            DisplayRootView<HomeScreenView>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = base.SelectAssemblies().ToList();
            assemblies.Add(typeof (HomeScreenViewModel).GetTypeInfo().Assembly);
            return assemblies;
        }
    }
}