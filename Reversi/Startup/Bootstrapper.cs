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

namespace Reversi.Startup
{
    class Bootstrapper
    {
        public Bootstrapper()
        {
            Container = new UnityContainer();

            Container.RegisterInstance<IEventAggregator>(new EventAggregator());
            Container.RegisterInstance<IGameContext>(new GameContext());

            Container.RegisterType<ILocationHelper, LocationHelper>();
            Container.RegisterType<ICaptureHelper, CaptureHelper>();
            Container.RegisterType<IValidMoveFinder, ValidMoveFinder>();
            Container.RegisterType<IMoveChooser, MoveChooserRandom>();
            Container.RegisterType<IGameStatusExaminer, GameStatusExaminer>();
            Container.RegisterType<IGameEngine, GameEngine>();
            Container.RegisterType<BoardViewModel>();
            Container.RegisterType<Game>();
            Container.RegisterType<GameViewModel>();
        }

        public IUnityContainer Container { get; set; }
    }
}
