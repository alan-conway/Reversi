﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Core;

namespace Reversi.Engine.Strategy.Random
{
    /// <summary>
    /// Represents the strategy of choosing any random (but valid) move
    /// </summary>
    public class RandomMoveStrategy : IRandomMoveStrategy
    {
        private IValidMoveFinder _moveFinder;
        private IRandomiser _random;

        public RandomMoveStrategy(IRandomiser random, IValidMoveFinder moveFinder)
        {
            _random = random;
            _moveFinder = moveFinder;
        }

        /// <summary>
        /// Choose any random valid move
        /// </summary>
        public Move ChooseMove(IGameContext context, IGameEngine engine)
        {
            var allMoves = _moveFinder.FindAllValidMoves(context);
            if (allMoves.Any())
            {
                int randomIndex = _random.Next(0, allMoves.Count());
                return new Move(allMoves.ElementAt(randomIndex));
            }
            else
            {
                return Move.PassMove;
            }
        }
    }
}