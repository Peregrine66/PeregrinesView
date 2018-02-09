using System.Windows;
using System.Windows.Media;

namespace Peregrine.WPF.View.Controls
{
    public class perViewBase : Window
    {
        public perViewBase()
        {
            UseLayoutRounding = true;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
        }
    }
}
