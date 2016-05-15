using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests
{
    public class LocationHelperTests
    {
        [Theory]
        [InlineData(0, new int[] { })]
        [InlineData(16, new[] { 8, 0 })]
        [InlineData(25, new[] { 17, 9, 1 })]
        [InlineData(34, new[] { 26, 18, 10, 2 })]
        [InlineData(63, new[] { 55, 47, 39, 31, 23, 15, 7 })]
        public void ShouldListAllLocationsUpFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsUp(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }

        [Theory]
        [InlineData(60, new int[] { })]
        [InlineData(40, new[] { 48, 56 })]
        [InlineData(25, new[] { 33, 41, 49, 57 })]
        [InlineData(36, new[] { 44, 52, 60 })]
        [InlineData(6, new[] { 14, 22, 30, 38, 46, 54, 62 })]
        public void ShouldListAllLocationsDownFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsDown(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }

        [Theory]
        [InlineData(16, new int[] { })]
        [InlineData(25, new[] { 24 })]
        [InlineData(36, new[] { 35, 34, 33, 32 })]
        [InlineData(6, new[] { 5, 4, 3, 2, 1, 0 })]
        public void ShouldListAllLocationsLeftFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsLeft(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }

        [Theory]
        [InlineData(7, new int[] { })]
        [InlineData(25, new[] { 26,27,28,29,30,31 })]
        [InlineData(36, new[] { 37, 38, 39 })]
        [InlineData(6, new[] { 7 })]
        public void ShouldListAllLocationsRightFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsRight(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }

        [Theory]
        [InlineData(8, new int[] { })]
        [InlineData(25, new[] { 16 })]
        [InlineData(36, new[] { 27, 18, 9, 0 })]
        [InlineData(6, new int[] { })]
        public void ShouldListAllLocationsUpLeftFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsUpLeft(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }

        [Theory]
        [InlineData(8, new[] { 1 })]
        [InlineData(25, new[] { 18, 11, 4 })]
        [InlineData(36, new[] { 29, 22, 15 })]
        [InlineData(55, new int[] { })]
        [InlineData(58, new[] { 51,44,37,30,23 })]
        public void ShouldListAllLocationsUpRightFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsUpRight(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }

        [Theory]
        [InlineData(8, new int[] { })]
        [InlineData(25, new[] { 32 })]
        [InlineData(36, new[] { 43, 50, 57})]
        [InlineData(55, new[] { 62 })]
        [InlineData(58, new int[] { })]
        public void ShouldListAllLocationsDownLeftFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsDownLeft(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }

        [Theory]
        [InlineData(8, new[] {17, 26, 35, 44, 53, 62 })]
        [InlineData(25, new[] { 34, 43, 52, 61 })]
        [InlineData(36, new[] { 45, 54, 63 })]
        [InlineData(55, new int[] { })]
        [InlineData(58, new int[] { })]
        public void ShouldListAllLocationsDownRightFromCurrent(int current, int[] expected)
        {
            //Arrange
            var locHelper = new LocationHelper();

            //Act 
            var result = locHelper.GetLocationsDownRight(current);

            //Assert
            Assert.Equal(expected.Length, result.Count());
            Assert.False(result.Except(expected).Any());
        }
    }
}
