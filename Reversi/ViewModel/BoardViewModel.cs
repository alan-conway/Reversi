using Prism.Events;
using Reversi.Engine;
using Reversi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.ViewModel
{
    public class BoardViewModel
    {
        public BoardViewModel(IEventAggregator eventAggregator)
        {
            Cells = new List<CellViewModel>();

            for (int cellId = 0; cellId < 64; cellId++)
            {
                Cells.Add(new CellViewModel(cellId, eventAggregator));
            }
        }
        
        public List<CellViewModel> Cells { get; }

        

    }
}
