using Reversi.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xunit;

namespace Reversi.UI.Tests.Converters
{
    public class BoolToHiddenConverterTests
    {
        [Theory]
        [InlineData(true, Visibility.Visible)] 
        [InlineData(false, Visibility.Hidden)]
        public void ShouldConvertToCorrectValue(bool input, Visibility expected)
        {
            //Arrange
            var converter = new BoolToHiddenConverter();

            //Act
            var result = converter.Convert(input, null, null, null);

            //Assert
            Assert.Equal(expected, result);
        }
    }
}
