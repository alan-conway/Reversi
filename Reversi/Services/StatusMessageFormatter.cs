using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services
{
    public class StatusMessageFormatter : IStatusMessageFormatter
    {
        private IScoreCalculator _scoreCalculator;

        public StatusMessageFormatter(IScoreCalculator scoreCalculator)
        {
            _scoreCalculator = scoreCalculator;
        }

        /// <summary>
        /// Calculates the scores and returns the appropriate message
        /// </summary>
        public string GetStatusMessage(GameStatus status, Square[] squares)
        {
            if(status == GameStatus.NewGame)
            {
                return string.Empty;
            }

            int blackScore = _scoreCalculator.CalculateScoreForPlayer(Piece.Black, status, squares);
            int whiteScore = _scoreCalculator.CalculateScoreForPlayer(Piece.White, status, squares);

            var score = $"Black: {blackScore}  White: {whiteScore}";

            switch (status)
            {
                case GameStatus.Draw: return $"Game is a draw  ({score})";
                case GameStatus.BlackWins: return $"Black wins  ({score})";
                case GameStatus.WhiteWins: return $"White wins  ({score})";
                default: return score;
            }
        }
    }
}
