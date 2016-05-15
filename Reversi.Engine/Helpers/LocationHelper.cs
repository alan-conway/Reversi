using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Helpers
{
    /// <summary>
    /// Identifies squares on the board that form part of a straight line
    /// from the location given
    /// </summary>
    public class LocationHelper : ILocationHelper
    {
        public IEnumerable<IEnumerable<int>> GetLocationsGroups(int location)
        {
            return new[] {
                GetLocationsUp(location),
                GetLocationsDown(location),
                GetLocationsLeft(location),
                GetLocationsRight(location),
                GetLocationsUpLeft(location),
                GetLocationsUpRight(location),
                GetLocationsDownLeft(location),
                GetLocationsDownRight(location)
            };
        }

        public IEnumerable<int> GetLocationsUp(int moveLocation)
        {
            int numSteps = moveLocation / 8;
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current -= 8;
                yield return current;
            }
        }

        public IEnumerable<int> GetLocationsDown(int moveLocation)
        {
            int numSteps = (63 - moveLocation) / 8;
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current += 8;
                yield return current;
            }
        }

        public IEnumerable<int> GetLocationsLeft(int moveLocation)
        {
            int numSteps = moveLocation % 8;
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current--;
                yield return current;
            }
        }

        public IEnumerable<int> GetLocationsRight(int moveLocation)
        {
            int numSteps = 8 - (moveLocation % 8) - 1;
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current++;
                yield return current;
            }
        }

        public IEnumerable<int> GetLocationsUpLeft(int moveLocation)
        {
            int numSteps = Math.Min(moveLocation / 8, moveLocation % 8);
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current -= 9;
                yield return current;
            }
        }

        public IEnumerable<int> GetLocationsUpRight(int moveLocation)
        {
            int numSteps = Math.Min(moveLocation / 8, 7 - (moveLocation % 8));
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current -= 7;
                yield return current;
            }
        }

        public IEnumerable<int> GetLocationsDownLeft(int moveLocation)
        {
            int numSteps = Math.Min(7 - (moveLocation / 8), moveLocation % 8);
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current += 7;
                yield return current;
            }
        }

        public IEnumerable<int> GetLocationsDownRight(int moveLocation)
        {
            int numSteps = Math.Min(7 - (moveLocation / 8), 7 - (moveLocation % 8));
            int current = moveLocation;

            for (int i = 0; i < numSteps; i++)
            {
                current += 9;
                yield return current;
            }
        }
    }
}
