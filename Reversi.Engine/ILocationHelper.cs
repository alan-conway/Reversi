using System.Collections.Generic;

namespace Reversi.Engine
{
    public interface ILocationHelper
    {
        /// <summary>
        /// Returns the collection of all direction-specific collections.
        /// Each such collection contains the index of any square on the board that
        ///  is found when moving in that direction from the specified location
        /// </summary>
        /// <remarks>
        /// Up/Down/Left/Right refers to a board in this layout:
        /// 00, 01, 02, ..
        /// 08, 09, 10, ..
        /// 16, 17, 18, ..
        /// eg, from position 09, UpLeft is 00, DownRight is 18, Left is 08. etc.
        /// </remarks>
        IEnumerable<IEnumerable<int>> GetLocationsGroups(int location);

        IEnumerable<int> GetLocationsDown(int moveLocation);
        IEnumerable<int> GetLocationsDownLeft(int moveLocation);
        IEnumerable<int> GetLocationsDownRight(int moveLocation);
        IEnumerable<int> GetLocationsLeft(int moveLocation);
        IEnumerable<int> GetLocationsRight(int moveLocation);
        IEnumerable<int> GetLocationsUp(int moveLocation);
        IEnumerable<int> GetLocationsUpLeft(int moveLocation);
        IEnumerable<int> GetLocationsUpRight(int moveLocation);
    }
}