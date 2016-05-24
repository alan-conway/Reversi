using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services
{
    public class DelayProvider : IDelayProvider
    {
        public async Task Delay(int millis)
        {
            await Task.Delay(millis);
        }
    }
}
