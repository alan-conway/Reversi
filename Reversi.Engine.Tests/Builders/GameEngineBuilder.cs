using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Tests.Builders
{
    public class GameEngineBuilder
    {
        private IGameContext _context;
        private ICaptureHelper _captureHelper;
        private IValidMoveFinder _validMoveFinder;
        private IMoveStrategy _moveStrategy;
        private IGameStatusExaminer _statusExaminer;

        public GameEngineBuilder()
        {
            //assign default values
            var mockLocationHelper = new Mock<ILocationHelper>();
            mockLocationHelper.Setup(lh => lh.GetLocationsGroups(It.IsAny<int>()))
                .Returns(new[] { new int[] { 0, 1 } });

            var mockCaptureHelper = new Mock<ICaptureHelper>();
            _captureHelper = mockCaptureHelper.Object;

            var mockValidMoveFinder = new Mock<IValidMoveFinder>();
            _validMoveFinder = mockValidMoveFinder.Object;

            var mockMoveStrategy = new Mock<IMoveStrategy>();
            _moveStrategy = mockMoveStrategy.Object;

            var mockStatusExaminer = new Mock<IGameStatusExaminer>();
            _statusExaminer = mockStatusExaminer.Object;

            var mockContext = new Mock<IGameContext>();
            _context = mockContext.Object;
        }

        public GameEngineBuilder SetContext(IGameContext context)
        {
            _context = context;
            return this;
        }

        public GameEngineBuilder SetCaptureHelper(ICaptureHelper captureHelper)
        {
            _captureHelper = captureHelper;
            return this;
        }

        public GameEngineBuilder SetValidMoveFinder(IValidMoveFinder validMoveFinder)
        {
            _validMoveFinder = validMoveFinder;
            return this;
        }

        public GameEngineBuilder SetStatusExaminer(IGameStatusExaminer statusExaminer)
        {
            _statusExaminer = statusExaminer;
            return this;
        }

        public GameEngineBuilder SetMoveStrategy(IMoveStrategy moveStrategy)
        {
            _moveStrategy = moveStrategy;
            return this;
        }

        public IGameEngine Build()
        {
            return new GameEngine(_context, _captureHelper, 
                _validMoveFinder, _moveStrategy, _statusExaminer);
        }


    }
}
