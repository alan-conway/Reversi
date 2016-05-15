using Moq;
using Prism.Events;
using Reversi.Engine;
using Reversi.Events;
using Reversi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.UI.Tests.ViewModel
{
    public class BoardViewModelTests
    {
        [Fact]
        public void ShouldInitialiseWithBlankCells()
        {
            //Arrange 
            var cellSelectedEvent = new CellSelectedEvent();
            var mockEventAggregator = new Mock<IEventAggregator>();
            mockEventAggregator.Setup(ea => ea.GetEvent<CellSelectedEvent>())
                .Returns(cellSelectedEvent);

            //Act
            var viewModel = new BoardViewModel(mockEventAggregator.Object);

            //Assert
            Assert.Equal(64, viewModel.Cells.Count);
            Assert.True(viewModel.Cells.All(c => c.Piece == Piece.None));
        }
    }
}
