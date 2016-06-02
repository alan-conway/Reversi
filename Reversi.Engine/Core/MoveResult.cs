using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Core
{
    public struct MoveResult
    {
        public MoveResult(GameStatus status, IGameContext context)
        {
            Status = status;
            Context = context;
        }

        public GameStatus Status { get; }
        public IGameContext Context { get; }
    }
}
