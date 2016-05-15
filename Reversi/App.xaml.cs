using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Reversi.Startup;
using Reversi.View;
using Microsoft.Practices.Unity;
using Reversi.ViewModel;

namespace Reversi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IUnityContainer container = new Bootstrapper().Container;
            var gameWindow = container.Resolve<Game>();
            var gameViewModel = container.Resolve<GameViewModel>();
            gameWindow.DataContext = gameViewModel;
            gameWindow.Show();

        }
    }
}
