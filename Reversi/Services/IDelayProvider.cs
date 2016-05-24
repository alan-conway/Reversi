using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services
{
    public interface IDelayProvider
    {
        Task Delay(int millis);
    }
}
