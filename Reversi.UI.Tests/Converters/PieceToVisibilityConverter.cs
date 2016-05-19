using Reversi.Converters;
using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xunit;

namespace Reversi.UI.Tests.Converters
{
    public class PieceToVisibilityConverterTests
    {
        [Theory]
        [InlineData(Piece.Black, Visibility.Visible)] 
        [InlineData(Piece.White, Visibility.Visible)] 
        [InlineData(Piece.None, Visibility.Collapsed)] 
        public void ShouldConvertToCorrectColour(Piece inputPiece, Visibility expected)
        {
            //Arrange
            var converter = new PieceToVisibilityConverter();

            //Act
            var result = converter.Convert(inputPiece, null, null, null);

            //Assert
            Assert.Equal(expected, result);
        }
    }
}

