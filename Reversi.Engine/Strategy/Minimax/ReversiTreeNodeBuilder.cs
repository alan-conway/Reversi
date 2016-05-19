using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.Minimax
{
    /// <summary>
    /// Builds nodes in the game search tree
    /// </summary>
    public class ReversiTreeNodeBuilder : IReversiTreeNodeBuilder
    {
        private IValidMoveFinder _moveFinder;
        private IMoveOrdering _ordering;

        public ReversiTreeNodeBuilder(IValidMoveFinder moveFinder, IMoveOrdering ordering)
        {
            _moveFinder = moveFinder;
            _ordering = ordering;
        }

        /// <summary>
        /// Creates a single node, eg for the root of the tree
        /// </summary>
        public IReversiTreeNode CreateSingleTreeNode(int moveLocation, IGameContext context, IGameEngine engine)
        {
            return new ReversiTreeNode(moveLocation, context, engine, this);
        }

        /// <summary>
        /// For a given board(/context), finds all the next possible moves and 
        /// creates a tree node for each one
        /// </summary>
        /// <param name="context"></param>
        /// <param name="engine"></param>
        /// <returns></returns>
        public List<IReversiTreeNode> CreateNextTreeNodes(IGameContext context, IGameEngine engine)
        {
            var nodes = new List<IReversiTreeNode>();
            var allMoves = _moveFinder.FindAllValidMoves(context);
            var orderedMoves = _ordering.OrderMoves(context, allMoves);

            foreach (int moveLocation in orderedMoves)
            {
                var childContext = context.Clone();
                var move = moveLocation == -1 ? Move.PassMove : new Move(moveLocation);
                var response = engine.UpdateBoardWithMove(move, childContext);
                UpdateContextWithResponse(childContext, response);
                var childNode = new ReversiTreeNode(moveLocation, childContext, engine, this);
                nodes.Add(childNode);
            }
            return nodes;
        }

        private void UpdateContextWithResponse(IGameContext childContext, Response response)
        {
            for (int i = 0; i < response.Squares.Length; i++)
            {
                childContext.SetPiece(i, response.Squares[i].Piece);
            }
        }
    }
}
