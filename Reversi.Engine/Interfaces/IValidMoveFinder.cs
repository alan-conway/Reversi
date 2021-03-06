﻿using Reversi.Engine.Core;
using System.Collections.Generic;

namespace Reversi.Engine.Interfaces
{
    public interface IValidMoveFinder
    {
        /// <summary>
        /// Determines whether playing in a particular location would be valid for the
        /// current player
        /// </summary>
        bool IsValidMove(IGameContext context, int location);

        /// <summary>
        /// Determines whether playing in a particular location would be valid for the
        /// specified player
        /// </summary>
        bool IsValidMove(IGameContext context, int location, Piece relativePiece);

        /// <summary>
        /// Looks at the whole board and identifies all valid moves for the current
        /// player
        /// </summary>
        IEnumerable<int> FindAllValidMoves(IGameContext context);

        /// <summary>
        /// Looks at the whole board and identifies all valid moves for the specified
        /// player
        /// </summary>
        IEnumerable<int> FindAllValidMoves(IGameContext context, Piece relativePiece);

        /// <summary>
        /// Looks at the whole board and identifies whether there exists a valid move
        /// </summary>
        bool IsAnyMoveValid(IGameContext context);
    }
}