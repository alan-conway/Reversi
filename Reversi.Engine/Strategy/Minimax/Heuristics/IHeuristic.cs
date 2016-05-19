using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy.Minimax.Heuristics
{
    public interface IHeuristic
    {
        int GetScore(IGameContext context, Piece relativePiece);
    }
}