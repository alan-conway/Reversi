﻿using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Core
{
    public class GameOptions : IGameOptions
    {
        public bool UserPlaysAsBlack { get; set; } = true;

        public IGameOptions Clone()
        {
            return new GameOptions()
            {
                UserPlaysAsBlack = this.UserPlaysAsBlack
            };
        }
    }
}
