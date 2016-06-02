using Game.Search.Interfaces;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.Minimax
{
    /// <summary>
    /// Represents a node in the game search tree
    /// </summary>
    /// <remarks>
    /// Child nodes are created lazily when requested, rather than upfront
    /// </remarks>
    public class ReversiTreeNode : ITreeNode, IReversiTreeNode
    {
        private List<IReversiTreeNode> _children;
        private IReversiTreeNodeBuilder _treeNodeBuilder;
        private IMovePlayer _movePlayer;

        public ReversiTreeNode(int moveLocation, IGameContext context,
            IMovePlayer movePlayer, IReversiTreeNodeBuilder treeBuilder)
        {
            _movePlayer = movePlayer;
            MoveLocation = moveLocation;
            Context = context;
            _treeNodeBuilder = treeBuilder;
        }

        public int MoveLocation { get; }
        public int Depth { get; set; }
        public IGameContext Context { get; }

        public IEnumerable<ITreeNode> GetChildren()
        {
            // construct child nodes lazily when requested
            if (_children == null)
            {
                _children = _treeNodeBuilder.CreateNextTreeNodes(Context, _movePlayer);
                _children.ForEach(c => c.Depth = Depth + 1);
            }
            return _children;
        }

    }
}
