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
using Reversi.Services.MessageDialogs;
using Reversi.Engine.Strategy.Minimax;
using Game.Search.Interfaces;
using Game.Search.Minimax;
using Reversi.Engine.Core;
using Reversi.Services;
using Reversi.Engine.Strategy.Minimax.Heuristics;
using Reversi.Engine.Strategy.Minimax.Interfaces;

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
            Container.RegisterInstance<IMoveOrdering>(new MoveOrdering());

            Container.RegisterType<IMessageDialogService, MessageDialogService>();            
            Container.RegisterType<ILocationHelper, LocationHelper>();
            Container.RegisterType<ICaptureHelper, CaptureHelper>();
            Container.RegisterType<IValidMoveFinder, ValidMoveFinder>();
            Container.RegisterType<IGameStatusExaminer, GameStatusExaminer>();

            Container.RegisterType<IHeuristic, WinLoseHeuristic>("WinLoseHeuristic");
            Container.RegisterType<IHeuristic, CornerHeuristic>("CornerHeuristic");
            Container.RegisterType<IHeuristic, MobilityHeuristic>("MobilityHeuristic");
            Container.RegisterType<IScoreProvider, ReversiScoreProvider>(
                new InjectionConstructor(
                    new ResolvedParameter<IHeuristic>("WinLoseHeuristic"),
                    new ResolvedParameter<IHeuristic>("CornerHeuristic"),
                    new ResolvedParameter<IHeuristic>("MobilityHeuristic")
                ));

            Container.RegisterType<IMinimaxTreeEvaluator, MinimaxTreeEvaluator>();
            Container.RegisterType<IReversiTreeNodeBuilder, ReversiTreeNodeBuilder>();
            Container.RegisterType<IMoveStrategy, MinimaxMoveStrategy>(
                new InjectionConstructor(
                    new ResolvedParameter<IMinimaxTreeEvaluator>(),
                    new ResolvedParameter<IValidMoveFinder>(),                    
                    new ResolvedParameter<IScoreProvider>(),
                    new ResolvedParameter<IGameStatusExaminer>(),
                    new ResolvedParameter<IReversiTreeNodeBuilder>(),
                    6)); //RandomMoveStrategy>();

            Container.RegisterType<IGameEngine, GameEngine>();
            Container.RegisterType<BoardViewModel>();
            Container.RegisterType<GameView>();
            Container.RegisterType<IScoreCalculator, ScoreCalculator>();
            Container.RegisterType<IStatusMessageFormatter, StatusMessageFormatter>();
            Container.RegisterType<GameViewModel>();
        }

        public IUnityContainer Container { get; set; }
    }
}
