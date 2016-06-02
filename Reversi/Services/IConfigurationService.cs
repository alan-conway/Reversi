using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services
{
    /// <summary>
    /// Manages loading from and saving data to config
    /// </summary>
    public interface IConfigurationService
    {
        IGameOptions GameOptions { get; set; }
    }
}
