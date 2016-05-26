using Ploeh.AutoFixture.Xunit2;
using Reversi.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reversi.Engine.Tests.Core
{
    public class GameOptionsTests
    {
        [Theory, AutoData]
        public void ShouldCreateDeepCopyOnClone(GameOptions options1)
        {
            //Arrange - injected

            //Act
            var options2 = options1.Clone();
            options1.UserPlaysAsBlack = !options1.UserPlaysAsBlack;

            //Assert
            Assert.NotEqual(options1.UserPlaysAsBlack, options2.UserPlaysAsBlack);
        }
    }
}
