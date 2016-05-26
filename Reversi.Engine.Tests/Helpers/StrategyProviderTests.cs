using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit2;
using Reversi.Engine.Core;
using Reversi.Engine.Helpers;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Helpers
{
    public class StrategyProviderTests
    {
        [Theory, AutoData]
        public void ShouldMakeAllStrategyInfosAvailable(IEnumerable<string> names)
        {
            //Arrange - injected
            var allStrategies = new List<IMoveStrategy>();
            var allInfo = new List<StrategyInfo>();
            foreach(var name in names)
            {
                var mockStrategy = new Mock<IMoveStrategy>();
                var info = new StrategyInfo(name, true, 1);
                mockStrategy.Setup(s => s.StrategyInfo).Returns(info);
                allStrategies.Add(mockStrategy.Object);
                allInfo.Add(info);
            }
            var provider = new StrategyProvider(allStrategies);

            //Act
            var strategyInfos = provider.GetStrategyInfoCollection();

            //Assert
            Assert.Equal(allInfo, strategyInfos);
        }

        [Theory, AutoData]
        public void ShouldReturnCorrectStrategy(IEnumerable<string> names)
        {
            //Arrange - injected
            var allStrategies = new List<IMoveStrategy>();
            foreach (var name in names)
            {
                var mockStrategy = new Mock<IMoveStrategy>();
                var info = new StrategyInfo(name, true, 1);
                mockStrategy.Setup(s => s.StrategyInfo).Returns(info);
                allStrategies.Add(mockStrategy.Object);
            }

            //Act
            var provider = new StrategyProvider(allStrategies);

            //Assert
            foreach(var strategy in allStrategies)
            {
                Assert.Equal(strategy, provider.GetStrategy(strategy.StrategyInfo.Name));
            }            
        }
    }
}
