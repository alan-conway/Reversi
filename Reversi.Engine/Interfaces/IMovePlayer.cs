using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Interfaces
{
    public interface IMovePlayer
    {
        MoveResult PlayMove(Move move, IGameContext moveContext);
    }
}
