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
using Reversi.Engine.Strategy.Shared;
using Reversi.Engine.Strategy.Random;
using Game.Search.MonteCarlo;
using Reversi.Engine.Strategy.MonteCarlo;

namespace Reversi.Startup
{
    class Bootstrapper
    {
        public Bootstrapper()
        {
            var userOptions = new GameOptions()
            {
                UserPlaysAsBlack = true,
                StrategyName = "Minimax",
                StrategyLevel = 4
            };

            Container = new UnityContainer();

            Container.RegisterInstance<IEventAggregator>(new EventAggregator());
            Container.RegisterInstance<IGameContext>(new GameContext());
            Container.RegisterInstance<IRandomiser>(new Randomiser());
            Container.RegisterInstance<IMoveOrdering>(new MoveOrdering());
            Container.RegisterInstance<IGameOptions>(userOptions);

            Container.RegisterType<IDialogWindow, OptionsWindow>();
            Container.RegisterType<IMessageDialogService, MessageDialogService>();   
            Container.RegisterType<ILocationHelper, LocationHelper>();
            Container.RegisterType<ICaptureHelper, CaptureHelper>();
            Container.RegisterType<IValidMoveFinder, ValidMoveFinder>();
            Container.RegisterType<IGameStatusExaminer, GameStatusExaminer>();
            Container.RegisterType<IMovePlayer, MovePlayer>();

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

            Container.RegisterType<ISelector, Selector>();
            Container.RegisterType<IExpander, NodeExpander>();
            Container.RegisterType<ISimulator, MonteCarloPlayoutSimulator>();
            Container.RegisterType<IRandomMoveStrategy, RandomMoveStrategy>();
            Container.RegisterType<IMonteCarloTreeEvaluator, MonteCarloTreeEvaluator>();
            
            Container.RegisterType<IMoveStrategy, MinimaxMoveStrategy>("Minimax");
            Container.RegisterType<IMoveStrategy, RandomMoveStrategy>("Random");
            Container.RegisterType<IMoveStrategy, MonteCarloMoveStrategy>("Monte Carlo");

            Container.RegisterType<IStrategyProvider, StrategyProvider>(
                new InjectionConstructor(
                    new ResolvedArrayParameter<IMoveStrategy>(
                        new ResolvedParameter<IMoveStrategy>("Minimax"), 
                        new ResolvedParameter<IMoveStrategy>("Monte Carlo"),
                        new ResolvedParameter<IMoveStrategy>("Random")
                )));
                

            Container.RegisterType<IGameEngine, GameEngine>();
            Container.RegisterType<BoardViewModel>();
            
            Container.RegisterType<IScoreCalculator, ScoreCalculator>();
            Container.RegisterType<IStatusMessageFormatter, StatusMessageFormatter>();
            Container.RegisterType<IDelayProvider, DelayProvider>();
            Container.RegisterType<IConfigurationService, ConfigurationService>();
            Container.RegisterType<GameViewModel>();
            Container.RegisterType<OptionsViewModel>();
            Container.RegisterType<GameView>();
        }

        public IUnityContainer Container { get; set; }
    }
}
