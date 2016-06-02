using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Reversi.Engine.Core;
using Reversi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.UI.Tests.Services
{
    public class ConfigurationServiceTests
    {
        public ConfigurationServiceTests()
        {
            Properties.Settings.Default.Reset();
        }

        [Fact]
        public void ShouldLoadDataThatWasSaved()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var gameOptions = fixture.Create<GameOptions>();
            var configService = new ConfigurationService();

            //Act
            configService.GameOptions = gameOptions;
            var loadedSaved = configService.GameOptions;

            //Assert
            Assert.Equal(gameOptions.UserPlaysAsBlack, loadedSaved.UserPlaysAsBlack);
            Assert.Equal(gameOptions.StrategyName, loadedSaved.StrategyName);
            Assert.Equal(gameOptions.StrategyLevel, loadedSaved.StrategyLevel);

        }
    }
}
