using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Xunit;
using Reversi.Engine.Core;

namespace Reversi.Engine.Tests.Core
{
    public class GameContextTests
    {
        [Fact]
        public void ShouldInitialiseToBlankSquaresAndMove1()
        {
            //Arrange 
            var context = new GameContext();

            //Act

            //Assert
            Assert.True(context.Squares.All(s => s.Piece == Piece.None));
            Assert.Equal(1, context.MoveNumber);
            Assert.Equal(Piece.Black, context.CurrentPiece);
            Assert.Equal(Piece.White, context.EnemyPiece);
        }

        [Fact]
        public void ShouldIncrementMoveNumberWhenMoveIsPlayed()
        {
            //Arrange 
            var context = new GameContext();
            var moveNumber = context.MoveNumber;

            //Act
            context.SetMovePlayed();

            //Assert
            Assert.Equal(moveNumber + 1, context.MoveNumber);
        }

        [Theory]
        [InlineData(1, Piece.Black, Piece.White)]
        [InlineData(2, Piece.White, Piece.Black)]
        [InlineData(10, Piece.White, Piece.Black)]
        public void ShouldIdentifyCurrentAndEnemyPieces(int move, Piece current, Piece enemy)
        {
            //Arrange 
            var context = new GameContext();

            //Act
            while(context.MoveNumber < move)
            {
                context.SetMovePlayed();
            }

            //Assert
            Assert.Equal(current, context.CurrentPiece);
            Assert.Equal(enemy, context.EnemyPiece);
        }


        [Fact]
        public void ShouldHaveNoValidMovesAfterSetAllInvalid()
        {
            //Arrange 
            var context = new GameContext();
            context.SetValid(0, true);
            context.SetValid(1, true);
            context.SetValid(2, true);

            //Act
            context.SetAllInvalid();

            //Assert
            Assert.False(context.Squares.Any(s => s.IsValidMove));
        }

        [Fact]
        public void ShouldCreateDeepCopyOnClone()
        {
            //Arrange
            var context1 = new GameContext();
            context1.SetPiece(0, Piece.Black);
            context1.SetValid(10, true);
            context1.SetMovePlayed();

            context1.SetPiece(1, Piece.White);
            context1.SetValid(11, true);
            context1.SetMovePlayed();

            //Act
            var context2 = context1.Clone();
            context2.SetPiece(0, Piece.White);
            context2.SetPiece(2, Piece.White);
            context2.SetMovePlayed();

            //Assert
            Assert.NotEqual(context1.MoveNumber, context2.MoveNumber);
            Assert.NotEqual(context1[0].Piece, context2[0].Piece);
            Assert.NotEqual(context1[2].Piece, context2[2].Piece);
            Assert.Equal(3, context1.MoveNumber);
            Assert.Equal(4, context2.MoveNumber);
            Assert.Equal(context1[1].Piece, context2[1].Piece);
            Assert.Equal(context1[10].IsValidMove, context2[10].IsValidMove);


        }

    }
}
