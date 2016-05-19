using Moq;
using Prism.Events;
using Reversi.Events;
using Reversi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Reversi.UI.Tests.Extensions;
using Reversi.Engine;
using Reversi.Engine.Core;

namespace Reversi.UI.Tests.ViewModel
{
    public class CellViewModelTests
    {
        private Mock<IEventAggregator> _mockEventAggregator;
        private CellSelectedEvent _cellSelectedEvent;
        private CellViewModel _cellViewModel;

        public CellViewModelTests()
        {
            _cellSelectedEvent = new CellSelectedEvent();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockEventAggregator.Setup(ea => ea.GetEvent<CellSelectedEvent>())
                .Returns(_cellSelectedEvent);

            _cellViewModel = new CellViewModel(0, _mockEventAggregator.Object);
        }

        [Fact]
        public void ShouldNotBeSelectedInitially()
        {
            //Arrange - see constructor

            //Act - nothing to do

            //Assert
            Assert.False(_cellViewModel.IsSelected);
        }

        [Fact]
        public void ShouldBeSelectedWhenCellSelectedEventIsPublished()
        {
            //Arrange - see constructor

            //Act - no action required
            _cellSelectedEvent.Publish(_cellViewModel.Id);

            //Assert
            Assert.True(_cellViewModel.IsSelected);
        }

        [Fact]
        public void ShouldMakeRemainingMovesInvalidWhenCellSelectedEventIsPublished()
        {
            //Arrange - see constructor

            //Act - no action required
            _cellSelectedEvent.Publish(_cellViewModel.Id);

            //Assert
            Assert.False(_cellViewModel.IsValidMove);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 1)]
        public void ShouldPublishCellSelectedEventWhenSelected(bool isValidMove, int numExpectedEvents)
        {
            //Arrange
            int cellId = 123;
            var mockCellSelectedEvent = new Mock<CellSelectedEvent>();
            _mockEventAggregator.Setup(ea => ea.GetEvent<CellSelectedEvent>())
                .Returns(mockCellSelectedEvent.Object);
            var cellViewModel = new CellViewModel(cellId, _mockEventAggregator.Object);
            cellViewModel.IsValidMove = isValidMove;

            //Act
            cellViewModel.CellSelected.Execute(cellId);

            //Assert
            mockCellSelectedEvent.Verify(cce => cce.Publish(cellId), Times.Exactly(numExpectedEvents));
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventWhenIsValidIsChanged()
        {
            //Arrange
            bool fired = false;
            
            _cellViewModel.OnNotify(
                nameof(CellViewModel.IsValidMove),
                () => fired = true);

            //Act
            _cellViewModel.IsValidMove = true;

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventWhenIsSelectedIsChanged()
        {
            //Arrange
            bool fired = false;

            _cellViewModel.OnNotify(
                nameof(CellViewModel.IsSelected),
                () => fired = true);

            //Act
            _cellViewModel.IsSelected = true;

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventWhenPieceIsChanged()
        {
            //Arrange
            bool fired = false;

            _cellViewModel.OnNotify(
                nameof(CellViewModel.Piece),
                () => fired = true);

            //Act
            _cellViewModel.Piece = Piece.Black;

            //Assert
            Assert.True(fired);
        }
    }
}
