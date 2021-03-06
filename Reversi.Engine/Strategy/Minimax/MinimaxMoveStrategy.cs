﻿using Game.Search.Interfaces;
using Game.Search.Minimax;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.Minimax
{
    /// <summary>
    /// Calls the minimax search to evaluate which next move is best
    /// </summary>
    public class MinimaxMoveStrategy : IMoveStrategy
    {
        private const string Name = "Minimax";
        private const bool IsMultiLevel = true;
        
        private IMinimaxTreeEvaluator _minimax;
        private IValidMoveFinder _moveFinder;
        private IScoreProvider _scoreProvider;
        private IGameStatusExaminer _statusExaminer;
        private IReversiTreeNodeBuilder _treeNodeBuilder;
        
        public MinimaxMoveStrategy(IMinimaxTreeEvaluator minimax,
            IValidMoveFinder moveFinder, IScoreProvider scoreProvider, 
            IGameStatusExaminer statusExaminer, IReversiTreeNodeBuilder treeNodeBuilder)
        {
            _minimax = minimax;
            _moveFinder = moveFinder;
            _scoreProvider = scoreProvider;
            _treeNodeBuilder = treeNodeBuilder;
            _statusExaminer = statusExaminer;

            SetLevel(1);
        }

        public StrategyInfo StrategyInfo { get; private set; }

        public int MaxSearchDepth { get; private set; }

        public void SetLevel(int level)
        {
            MaxSearchDepth = level;
            StrategyInfo = new StrategyInfo(Name, IsMultiLevel, level);            
        }

        /// <summary>
        /// Calls out to the external IMinimaxTreeEvaluator
        /// </summary>
        public Move ChooseMove(IGameContext context, IMovePlayer movePlayer)
        {
            if (!_moveFinder.IsAnyMoveValid(context))
            {
                return Move.PassMove;
            }
            bool isPlayer1 = context.CurrentPiece == Piece.Black;
            var treeNode = _treeNodeBuilder.CreateRootTreeNode(context, movePlayer);
            var result = _minimax.EvaluateTree(treeNode, IsLeafNode, _scoreProvider, isPlayer1);
            return new Move(((IReversiTreeNode)result.TreeNode).MoveLocation);
        }

        /// <summary>
        /// Stop evaluation once the game is over or the depth is too deep
        /// </summary>
        private bool IsLeafNode(ITreeNode node)
        {
            var treeNode = node as IReversiTreeNode;
            return treeNode.Depth >= MaxSearchDepth ||
                _statusExaminer.DetermineGameStatus(treeNode.Context) != GameStatus.InProgress;
        }
    }
}
