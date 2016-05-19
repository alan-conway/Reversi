using Reversi.Converters;
using Reversi.Engine;
using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xunit;

namespace Reversi.UI.Tests.Converters
{
    public class PieceToColourConverterTests
    {
        [Theory]
        [InlineData(Piece.Black, 255, 0, 0, 0 )] // Brushes.Black
        [InlineData(Piece.White, 255, 255, 255, 255)] // Brushes.White
        [InlineData(Piece.None, 0, 255, 255, 255)] // Brushes.Transparant
        public void ShouldConvertToCorrectColour(Piece inputPiece, byte A, byte R, byte G, byte B)
        {
            //Arrange
            var converter = new PieceToColourConverter();

            //Act
            var result = converter.Convert(inputPiece, null, null, null);

            //Assert
            Assert.Equal(A, (result as SolidColorBrush).Color.A);
            Assert.Equal(R, (result as SolidColorBrush).Color.R);
            Assert.Equal(G, (result as SolidColorBrush).Color.G);
            Assert.Equal(B, (result as SolidColorBrush).Color.B);
        }
    }
}

