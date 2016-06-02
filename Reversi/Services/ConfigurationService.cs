using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using System.Configuration;
using Reversi.Engine.Core;
using Reversi.Properties;

namespace Reversi.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private Settings _settings;

        public ConfigurationService()
        {
            _settings = Settings.Default;
        }

        public IGameOptions GameOptions
        {
            get { return LoadGameOptions(); }
            set { SaveGameOptions(value); }
        }

        private IGameOptions LoadGameOptions()
        {
            return new GameOptions()
            {
                UserPlaysAsBlack = _settings.UserPlaysAsBlack,
                StrategyName = _settings.StrategyName,
                StrategyLevel = _settings.StrategyLevel
            };            
        }

        private void SaveGameOptions(IGameOptions value)
        {
            _settings[nameof(IGameOptions.UserPlaysAsBlack)] = value.UserPlaysAsBlack;
            _settings[nameof(IGameOptions.StrategyName)] = value.StrategyName;
            _settings[nameof(IGameOptions.StrategyLevel)] = value.StrategyLevel;
            _settings.Save();
        }
    }
}
