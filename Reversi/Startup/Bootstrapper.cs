using Microsoft.Practices.Unity;
using Prism.Events;
using Reversi.Engine;
using Reversi.View;
using Reversi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy;
using Reversi.Engine.Helpers;
using Reversi.MessageDialogs;

namespace Reversi.Startup
{
    class Bootstrapper
    {
        public Bootstrapper()
        {
            Container = new UnityContainer();

            Container.RegisterInstance<IEventAggregator>(new EventAggregator());
            Container.RegisterInstance<IGameContext>(new GameContext());
            Container.RegisterInstance<IRandomiser>(new Randomiser());

            Container.RegisterType<IMessageDialogService, MessageDialogService>();            
            Container.RegisterType<ILocationHelper, LocationHelper>();
            Container.RegisterType<ICaptureHelper, CaptureHelper>();
            Container.RegisterType<IValidMoveFinder, ValidMoveFinder>();
            Container.RegisterType<IMoveStrategy, RandomMoveStrategy>();
            Container.RegisterType<IGameStatusExaminer, GameStatusExaminer>();
            Container.RegisterType<IGameEngine, GameEngine>();
            Container.RegisterType<BoardViewModel>();
            Container.RegisterType<Game>();
            Container.RegisterType<GameViewModel>();
        }

        public IUnityContainer Container { get; set; }
    }
}
