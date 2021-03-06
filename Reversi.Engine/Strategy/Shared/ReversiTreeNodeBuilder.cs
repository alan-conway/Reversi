﻿using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.Shared
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
        public IReversiTreeNode CreateRootTreeNode(IGameContext context, IMovePlayer movePlayer)
        {
            return new ReversiTreeNode(-1, context, movePlayer, this);
        }

        /// <summary>
        /// For a given board(/context), finds all the next possible moves and 
        /// creates a tree node for each one
        /// </summary>
        /// <param name="context"></param>
        /// <param name="engine"></param>
        /// <returns></returns>
        public List<IReversiTreeNode> CreateNextTreeNodes(IGameContext context, IMovePlayer movePlayer)
        {
            var nodes = new List<IReversiTreeNode>();
            var allMoves = _moveFinder.FindAllValidMoves(context);
            var orderedMoves = _ordering.OrderMoves(context, allMoves);

            foreach (int moveLocation in orderedMoves)
            {
                var childContext = context.Clone();
                var move = moveLocation == -1 ? Move.PassMove : new Move(moveLocation);
                var result = movePlayer.PlayMove(move, childContext);
                UpdateContextWithResult(childContext, result);
                var childNode = new ReversiTreeNode(moveLocation, childContext, movePlayer, this);
                nodes.Add(childNode);
            }
            return nodes;
        }

        private void UpdateContextWithResult(IGameContext childContext, MoveResult result)
        {
            for (int i = 0; i < result.Context.Squares.Length; i++)
            {
                childContext.SetPiece(i, result.Context.Squares[i].Piece);
            }
        }
    }
}
