using Reversi.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Reversi.Converters
{
    public class PieceToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return BrushForNone;
            }
            switch((Piece)value)
            {
                case Piece.Black: return BrushForBlack;
                case Piece.White: return BrushForWhite;
                default: return BrushForNone;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Brush BrushForBlack { get; set; } = Brushes.Black;
        public Brush BrushForWhite{ get; set; } = Brushes.White;
        public Brush BrushForNone { get; set; } = Brushes.Transparent;
    }
}
