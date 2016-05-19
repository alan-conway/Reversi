using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Core;

namespace Reversi.Engine.Tests.Factories
{
    public static class GameContextFactory
    {
        public static IGameContext CreateGameContext()
        {
            return new GameContext();
        }

        public static IGameContext CreateGameContext(int[] blackPieces, int[] whitePieces)
        {
            var context = new GameContext();
            foreach(int index in blackPieces)
            {
                context.SetPiece(index, Piece.Black);
            }
            foreach (int index in whitePieces)
            {
                context.SetPiece(index, Piece.White);
            }
            return context;
        }
    }
}
