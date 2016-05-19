using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Core
{
    public enum GameStatus
    {
        NewGame,
        InProgress,
        BlackWins,
        WhiteWins,
        Draw
    }
}
