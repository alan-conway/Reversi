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

namespace Reversi.UI.Tests.ViewModel
{
    public class OptionsViewModelTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldInitialiseOptionsCorrectlyFromEngine(bool userPlaysBlack)
        {
            //Arrange
            var mockEngine = new Mock<IGameEngine>();
            var options = new GameOptions() { UserPlaysAsBlack = userPlaysBlack };
            mockEngine.Setup(e => e.GameOptions).Returns(options);

            //Act
            var optionsViewModel = new OptionsViewModel(mockEngine.Object);

            //Assert
            Assert.Equal(userPlaysBlack, optionsViewModel.UserPlaysAsBlack);
        }

        [Fact]
        public void ShouldNotifyPropertyChangedWhenUpdatingPlayerColour()
        {
            //Arrange
            var mockEngine = new Mock<IGameEngine>();
            var options = new GameOptions() { UserPlaysAsBlack = true };
            mockEngine.Setup(e => e.GameOptions).Returns(options);
            var optionsViewModel = new OptionsViewModel(mockEngine.Object);
            bool eventWasFired = false;
            optionsViewModel.OnNotify(
                nameof(OptionsViewModel.UserPlaysAsBlack), 
                () => eventWasFired = true);

            //Act
            optionsViewModel.UserPlaysAsBlack = false; // i.e changing the value

            //Assert
            Assert.True(eventWasFired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedWhenUpdatingPlayerColour()
        {
            //Arrange
            var mockEngine = new Mock<IGameEngine>();
            var options = new GameOptions() { UserPlaysAsBlack = true };
            mockEngine.Setup(e => e.GameOptions).Returns(options);
            var optionsViewModel = new OptionsViewModel(mockEngine.Object);
            bool saveEventRaised = false;
            bool undoEventRaised = false;

            optionsViewModel.SaveOptionsCommand
                .CanExecuteChanged += (e, s) => saveEventRaised = true;
            optionsViewModel.UndoChangesCommand
                .CanExecuteChanged += (e, s) => undoEventRaised = true;

            //Act
            optionsViewModel.UserPlaysAsBlack = false; // i.e changing the value

            //Assert
            Assert.True(saveEventRaised);
            Assert.True(undoEventRaised);
        }

        [Fact]
        public void ShouldNotifyPropertyChangedWhenUndoingUpdatedPlayerColour()
        {
            //Arrange
            var mockEngine = new Mock<IGameEngine>();
            var options = new GameOptions() { UserPlaysAsBlack = true };
            mockEngine.Setup(e => e.GameOptions).Returns(options);
            var optionsViewModel = new OptionsViewModel(mockEngine.Object);
            optionsViewModel.UserPlaysAsBlack = false; // i.e changing the value

            bool eventWasFired = false;
            optionsViewModel.OnNotify(
                nameof(OptionsViewModel.UserPlaysAsBlack),
                () => eventWasFired = true);

            //Act
            optionsViewModel.UndoChangesCommand.Execute(null);

            //Assert
            Assert.True(eventWasFired);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldRaiseCanExecuteChangedUndoingOrSavingChanges(bool eventIsSave)
        {
            //Arrange
            var mockEngine = new Mock<IGameEngine>();
            var options = new GameOptions() { UserPlaysAsBlack = true };
            mockEngine.Setup(e => e.GameOptions).Returns(options);
            var optionsViewModel = new OptionsViewModel(mockEngine.Object);
            bool eventRaised = false;
            DelegateCommand command = (DelegateCommand)(eventIsSave ?
                optionsViewModel.SaveOptionsCommand :
                optionsViewModel.UndoChangesCommand);

            command.CanExecuteChanged += (e, s) => eventRaised = true;

            //Act
            command.Execute(); 

            //Assert
            Assert.True(eventRaised);
        }

        [Fact]
        public void ShouldSaveChangesWhenSaveCommandExecuted()
        {
            //Arrange
            var mockEngine = new Mock<IGameEngine>();
            var options = new GameOptions() { UserPlaysAsBlack = true };
            mockEngine.Setup(e => e.GameOptions).Returns(options);
            var optionsViewModel = new OptionsViewModel(mockEngine.Object);
            optionsViewModel.UserPlaysAsBlack = false; // i.e changing the value

            //Act
            optionsViewModel.SaveOptionsCommand.Execute(null);

            //Assert
            mockEngine.VerifySet(e => 
                e.GameOptions = It.Is<IGameOptions>(opt => opt.UserPlaysAsBlack == false));
        }

        [Fact]
        public void ShouldResetChangesWhenUndoCommandExecuted()
        {
            //Arrange
            var mockEngine = new Mock<IGameEngine>();
            var options = new GameOptions() { UserPlaysAsBlack = true };
            mockEngine.Setup(e => e.GameOptions).Returns(options);
            var optionsViewModel = new OptionsViewModel(mockEngine.Object);
            optionsViewModel.UserPlaysAsBlack = false; // i.e changing the value

            //Act
            optionsViewModel.UndoChangesCommand.Execute(null);

            //Assert
            mockEngine.VerifySet(e =>
                e.GameOptions = It.Is<IGameOptions>(opt => opt.UserPlaysAsBlack == false),Times.Never);
            Assert.True(optionsViewModel.UserPlaysAsBlack);
        }

        
    }
}
