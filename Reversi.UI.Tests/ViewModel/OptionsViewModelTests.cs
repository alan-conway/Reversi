using Moq;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Reversi.UI.Tests.Extensions;
using Prism.Commands;
using Reversi.Services.MessageDialogs;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Reversi.UI.Tests.ViewModel
{
    public class OptionsViewModelTests
    {
        [Theory]
        [InlineAutoData(true)]
        [InlineAutoData(false)]
        public void ShouldInitialiseCorrectlyFromGameOptions(bool playAsBlack, 
            string strategyName, int strategyLevel)
        {
            //Arrange
            var gameOptions = new GameOptions()
            {
                UserPlaysAsBlack = playAsBlack,
                StrategyName = strategyName,
                StrategyLevel = strategyLevel
            };
            var strategies = new[] { new StrategyInfo(strategyName, false, 2) };

            //Act
            var optionsViewModel = new OptionsViewModel(gameOptions, strategies);

            //Assert
            Assert.Equal(playAsBlack, optionsViewModel.UserStartsNewGames);
            Assert.Equal(strategyName, optionsViewModel.SelectedAlgorithm);
            Assert.Equal(strategyLevel, optionsViewModel.SelectedLevel);
        }

        [Theory, AutoData]
        public void ShouldInitialiseCorrectlyFromStrategies(GameOptions gameOptions, 
            IEnumerable<StrategyInfo> strategies)
        {
            //Arrange - injected

            //Act
            var optionsViewModel = new OptionsViewModel(gameOptions, strategies);

            //Assert
            Assert.True(strategies.Count() > 2); // just checking that we're testing something
            Assert.Equal(strategies.Select(x => x.Name), optionsViewModel.AlgorithmChoices);
        }

        [Theory]
        [InlineAutoData(true)]
        [InlineAutoData(false)]
        public void ShouldReflectChangesCorrectlyInGameOptions(bool playAsBlack,
            string strategyName, int strategyLevel)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var gameOptions = new GameOptions()
            {
                UserPlaysAsBlack = playAsBlack,
                StrategyName = strategyName,
                StrategyLevel = strategyLevel
            };
            fixture.Inject<IGameOptions>(gameOptions);

            var optionsViewModel = fixture.Create<OptionsViewModel>();
            // changing the values
            var newPlayAsBlack = !playAsBlack;
            var newName = fixture.Create<string>();
            var newLevel = strategyLevel+1; 
            optionsViewModel.UserStartsNewGames = newPlayAsBlack;
            optionsViewModel.SelectedAlgorithm = newName;
            optionsViewModel.SelectedLevel = newLevel;

            //Act
            var newGameOptions = optionsViewModel.ToGameOptions();

            //Assert
            Assert.Equal(newPlayAsBlack, newGameOptions.UserPlaysAsBlack);
            Assert.Equal(newName, newGameOptions.StrategyName);
            Assert.Equal(newLevel, newGameOptions.StrategyLevel);
        }

        [Theory, AutoData]
        public void ShouldReturnUpdatedObjectWhenChangesAreSaved(GameOptions gameOptions)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameOptions>(gameOptions);

            var optionsViewModel = fixture.Create<OptionsViewModel>();
            // changing the values
            var newPlayAsBlack = !gameOptions.UserPlaysAsBlack;
            var newName = fixture.Create<string>();
            var newLevel = gameOptions.StrategyLevel + 1;
            optionsViewModel.UserStartsNewGames = newPlayAsBlack;
            optionsViewModel.SelectedAlgorithm = newName;
            optionsViewModel.SelectedLevel = newLevel;

            var mockWindow = fixture.Freeze<Mock<IDialogWindow>>();
            var command = (DelegateCommand<IDialogWindow>)optionsViewModel.SaveOptionsCommand;

            //Act
            command.Execute(mockWindow.Object);
            var newGameOptions = optionsViewModel.ToGameOptions();

            //Assert
            Assert.Equal(newPlayAsBlack, newGameOptions.UserPlaysAsBlack);
            Assert.Equal(newName, newGameOptions.StrategyName);
            Assert.Equal(newLevel, newGameOptions.StrategyLevel);
        }

        [Theory]
        [InlineData(true, DialogChoice.Ok)]
        [InlineData(false, DialogChoice.Cancel)]
        public void ShouldRecordCorrectChoiceWhenClosingWindow(
            bool saveChanges, DialogChoice choice)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameOptions>(new GameOptions() { UserPlaysAsBlack = true });

            var optionsViewModel = fixture.Create<OptionsViewModel>();
            optionsViewModel.UserStartsNewGames = false; // changing the value

            var mockWindow = fixture.Freeze<Mock<IDialogWindow>>();
            var command = (DelegateCommand<IDialogWindow>)(saveChanges ?
                optionsViewModel.SaveOptionsCommand :
                optionsViewModel.UndoChangesCommand);

            //Act
            command.Execute(mockWindow.Object);

            //Assert
            Assert.Equal(choice, optionsViewModel.GetDialogChoice());
        }

        [Theory, AutoData]
        public void ShouldFirePropertyChangedEventWhenSettingUserPlaysBlack(GameOptions gameOptions,
            IEnumerable<StrategyInfo> strategies)
        {
            //Arrange
            bool eventFired = false;
            var optionsViewModel = new OptionsViewModel(gameOptions, strategies);
            optionsViewModel.OnNotify(
                nameof(OptionsViewModel.UserStartsNewGames),
                () => eventFired = true);

            //Act
            optionsViewModel.UserStartsNewGames = !optionsViewModel.UserStartsNewGames;

            //Assert
            Assert.True(eventFired); 
        }

        public void ShouldFirePropertyChangedEventWhenSettingSelectedAlgorithm(GameOptions gameOptions,
            IEnumerable<StrategyInfo> strategies)
        {
            //Arrange
            bool eventFired = false;
            var optionsViewModel = new OptionsViewModel(gameOptions, strategies);
            optionsViewModel.OnNotify(
                nameof(OptionsViewModel.SelectedAlgorithm),
                () => eventFired = true);

            //Act
            optionsViewModel.SelectedAlgorithm = "newValue";

            //Assert
            Assert.True(eventFired);
        }

        public void ShouldFirePropertyChangedEventWhenSettingSelectedLevel(GameOptions gameOptions,
            IEnumerable<StrategyInfo> strategies)
        {
            //Arrange
            bool eventFired = false;
            var optionsViewModel = new OptionsViewModel(gameOptions, strategies);
            optionsViewModel.OnNotify(
                nameof(OptionsViewModel.SelectedLevel),
                () => eventFired = true);

            //Act
            optionsViewModel.SelectedLevel++;

            //Assert
            Assert.True(eventFired);
        }

        

    }
}
