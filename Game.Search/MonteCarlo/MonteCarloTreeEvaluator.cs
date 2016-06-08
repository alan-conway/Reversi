using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Search.Interfaces;

namespace Game.Search.MonteCarlo
{
    public class MonteCarloTreeEvaluator : IMonteCarloTreeEvaluator
    {
        private ISelector _selector;
        private IExpander _expander;
        private ISimulator _simulator;

        /*
            parentNode = root;
            leaf = selectLeafNode(parentNode)
            expandLeaf(leaf)
            var result = simulateGameToCompletionFromNode(leaf)
            backpropagateResults(leaf, status)

            selection:
            selectLeafNode(node)
            {
                if (expandedChildren.Count = 0)
                    return node;
                else
                    return selectLeafNode(children.OrderBy(n => n.UCT).First());
            }

            expandLeaf(node)
            {
                foreach(child in originalNode)
                {
                    node.AddChild(new WrappedNode(child))
                }
            }

            simulateGameToCompletion(node)
            {
                var branchContext = originalContext.Clone();
                while(branchContext.Status == InProgress)
                {   
                    Move move = GetMove(IRandomMoveStrategy);
                    engine.UpdateBoardWithMove(move, branchContext);
                    DetermineGameStatus(branchContext);
                }
            }

            backpropagation(node, status):
            node.Played++;
            node.WinsBlack++;
            node.WinsWhite??
            node.UCT = CalcUCT(..)
            backpropagation(node.Parent, status):
            */

        public MonteCarloTreeEvaluator(ISelector selector, IExpander expander, 
            ISimulator simulator)
        {
            _selector = selector;
            _expander = expander;
            _simulator = simulator;
        }

        public MonteCarloResult EvaluateTree(ITreeNode root, bool isPlayer1, double durationSeconds)
        {
            DateTime startTime = DateTime.UtcNow;
            var elapsedSeconds = 0.0;

            var rootNode = new WrappedNode(root, isPlayer1);

            while(elapsedSeconds < durationSeconds)
            {
                var leaf = _selector.SelectNode(rootNode);
                var node = _expander.ExpandRandomNode(leaf);
                PlayoutAndBackPropagate(node, isPlayer1);

                elapsedSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            }

            return GetEvaluationResult(rootNode);
        }

        private void PlayoutAndBackPropagate(WrappedNode node, bool isPlayer1)
        {
            var score = _simulator.SimulateGameToCompletionFromNode(node.OriginalTreeNode);
            node.PropagateResults(score);
        }

        private MonteCarloResult GetEvaluationResult(WrappedNode root)
        {
            var mostPlayedChildNode = root
                .Children
                .OrderByDescending(c => c.NumCompletedGames)
                .First();

            return new MonteCarloResult(
                root.NumCompletedGames,
                mostPlayedChildNode.NumCompletedGames,
                mostPlayedChildNode.OriginalTreeNode);
        }

       
    }
}
