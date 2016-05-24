using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Tests.Extensions
{
    public static class GameContextExtension
    {
        public static IGameContext SetPiece(this IGameContext context, Piece piece, params int[] locations)
        {
            foreach(var location in locations)
            {
                context.SetPiece(location, piece);
            }
            return context;
        }
    }
}
