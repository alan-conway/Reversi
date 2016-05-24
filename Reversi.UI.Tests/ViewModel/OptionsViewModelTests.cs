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

namespace Reversi.UI.Tests.ViewModel
{
    public class OptionsViewModelTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldInitialiseCorrectlyFromGameOptions(bool playAsBlack)
        {
            //Arrange
            var gameOptions = new GameOptions() { UserPlaysAsBlack = playAsBlack };

            //Act
            var optionsViewModel = new OptionsViewModel(gameOptions);

            //Assert
            Assert.Equal(playAsBlack, optionsViewModel.UserPlaysAsBlack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldReflectChangesCorrectlyInGameOptions(bool playAsBlack)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameOptions>(new GameOptions() { UserPlaysAsBlack = !playAsBlack });

            var optionsViewModel = fixture.Create<OptionsViewModel>();
            optionsViewModel.UserPlaysAsBlack = playAsBlack; // changing the value

            //Act
            var newGameOptions = optionsViewModel.ToGameOptions();

            //Assert
            Assert.Equal(playAsBlack, newGameOptions.UserPlaysAsBlack);
        }

        [Fact]
        public void ShouldReturnUpdatedObjectWhenChangesAreSaved()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameOptions>(new GameOptions() { UserPlaysAsBlack = true });

            var optionsViewModel = fixture.Create<OptionsViewModel>();
            optionsViewModel.UserPlaysAsBlack = false; // changing the value

            var mockWindow = fixture.Freeze<Mock<IDialogWindow>>();
            var command = (DelegateCommand<IDialogWindow>)optionsViewModel.SaveOptionsCommand;

            //Act
            command.Execute(mockWindow.Object);

            //Assert
            Assert.False(optionsViewModel.ToGameOptions().UserPlaysAsBlack);
        }

        [Theory]
        [InlineData(true, DialogChoice.Ok)]
        [InlineData(false, DialogChoice.Cancel)]
        public void ShouldRecordCorrecyChoiceWhenClosingWindow(
            bool saveChanges, DialogChoice choice)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameOptions>(new GameOptions() { UserPlaysAsBlack = true });

            var optionsViewModel = fixture.Create<OptionsViewModel>();
            optionsViewModel.UserPlaysAsBlack = false; // changing the value

            var mockWindow = fixture.Freeze<Mock<IDialogWindow>>();
            var command = (DelegateCommand<IDialogWindow>)(saveChanges ?
                optionsViewModel.SaveOptionsCommand :
                optionsViewModel.UndoChangesCommand);

            //Act
            command.Execute(mockWindow.Object);

            //Assert
            Assert.Equal(choice, optionsViewModel.GetDialogChoice());
        }
        
    }
}
