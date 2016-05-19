using Game.Search.Interfaces;
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
    /// Represents a node in the game search tree
    /// </summary>
    public class ReversiTreeNode : ITreeNode, IReversiTreeNode
    {
        private List<IReversiTreeNode> _children;
        private IReversiTreeNodeBuilder _treeNodeBuilder;
        private IGameEngine _engine;

        public ReversiTreeNode(int moveLocation, IGameContext context, 
            IGameEngine engine, IReversiTreeNodeBuilder treeBuilder)
        {
            _engine = engine;
            MoveLocation = moveLocation;
            Context = context;
            _treeNodeBuilder = treeBuilder;
        }

        public int MoveLocation { get; }
        public int Depth { get; set; }
        public IGameContext Context { get; }

        public IEnumerable<ITreeNode> GetChildren()
        {
            if (_children == null)
            {
                _children = _treeNodeBuilder.CreateNextTreeNodes(Context, _engine);
                _children.ForEach(c => c.Depth = Depth + 1);
            }
            return _children;
        }

    }
}
