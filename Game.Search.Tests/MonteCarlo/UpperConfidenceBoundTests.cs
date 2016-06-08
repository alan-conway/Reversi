using Game.Search.MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Game.Search.Tests.MonteCarlo
{
    public class UpperConfidenceBoundTests
    {
        [Theory]
        [InlineData(0, 1, 1, 0)]
        [InlineData(1, 2, 3, 1.548)]
        [InlineData(2, 5, 9, 1.337)]
        public void ShouldCalculatedExpectedUCT(int wins, int numGames, int numParentGames,
            double expectedResult)
        {
            //Arrange
            IUpperConfidenceBoundCalculator calculator = new WrappedNode(null);
            var constant = Math.Sqrt(2);

            //Act
            var result = calculator.CalculateUpperConfidenceBound(wins, numGames, numParentGames, constant);

            //Assert
            Assert.Equal(expectedResult, Math.Round(result, 3));
        }
    }
}
