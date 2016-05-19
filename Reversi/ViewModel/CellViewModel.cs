using Prism.Commands;
using Prism.Events;
using Reversi.Engine;
using Reversi.Engine.Core;
using Reversi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reversi.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        private bool _isSelected;
        private IEventAggregator _eventAggregator;
        private Piece _piece;
        private bool _isValidMove;

        public CellViewModel(int cellId, IEventAggregator eventAggregator)
        {
            Id = cellId;
            _eventAggregator = eventAggregator;
            _piece = Piece.None;
            _isValidMove = false;
            CellSelected = new DelegateCommand(OnExecuteCellSelected, CanExecuteCellSelected);
            eventAggregator.GetEvent<CellSelectedEvent>().Subscribe(OnCellSelectedEvent);
        }
                
        public int Id { get; }

        public bool IsValidMove
        {
            get { return _isValidMove; }
            set
            {
                if (_isValidMove != value)
                {
                    _isValidMove = value;
                    Notify();
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    Notify();
                }
            }
        }

        public Piece Piece
        {
            get { return _piece; }
            set
            {
                if (_piece != value)
                {
                    _piece = value;
                    Notify();
                }
            }
        }

        public ICommand CellSelected { get; }

        private void OnExecuteCellSelected()
        {
            if (IsValidMove)
            {
                _eventAggregator.GetEvent<CellSelectedEvent>().Publish(Id);
            }
        }

        private bool CanExecuteCellSelected()
        {
            return true;            
        }

        private void OnCellSelectedEvent(int cellId)
        {
            IsSelected = (Id == cellId);
            IsValidMove = false;
        }

    }
}
