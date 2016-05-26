using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Core
{
    public class GameOptions : IGameOptions
    {
        public string StrategyName { get; set; }
        public int StrategyLevel { get; set; }
        public bool UserPlaysAsBlack { get; set; } = true;

        public IGameOptions Clone()
        {
            return new GameOptions()
            {
                UserPlaysAsBlack = UserPlaysAsBlack,
                StrategyName = StrategyName,
                StrategyLevel = StrategyLevel
            };
        }
    }
}
