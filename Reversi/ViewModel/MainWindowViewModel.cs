using Prism.Events;
using Reversi.Engine.Interfaces;
using Reversi.Services;
using Reversi.Services.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.ViewModel
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(IEventAggregator eventAggregator, IGameEngine engine,
            IMessageDialogService dialogService, IStatusMessageFormatter statusMsgFormatter)
        {
            Game = new GameViewModel(eventAggregator, engine, dialogService, statusMsgFormatter);
            Options = new OptionsViewModel(engine);
        }

        public GameViewModel Game { get; }

        public OptionsViewModel Options { get;  }
    }
}
