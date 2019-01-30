using GalaSoft.MvvmLight;
using System.Collections.Generic;
using Peregrine.Library;

namespace FormattedTextBlockDemo
{
    public class MainViewModel: ViewModelBase
    {
        public MainViewModel()
        {
            var items = new List<perValueDisplayPair<int>>
            {
                1.CreateValueDisplayPair("An example of all available tags"),
                2.CreateValueDisplayPair("Implicit closing of tags"),
                3.CreateValueDisplayPair("Multiple nested tags of the same type"),
                4.CreateValueDisplayPair("More nested tags of the same type")
            };

            SampleTextItems = items;
        }

        public IEnumerable<perValueDisplayPair<int>> SampleTextItems { get; }

        private int _selectedSampleTextId;

        public int SelectedSampleTextId
        {
            get => _selectedSampleTextId;
            set
            {
                Set(nameof(SelectedSampleTextId), ref _selectedSampleTextId, value);

                switch (SelectedSampleTextId)
                {
                    case 1:
                        TheText = "Some text with <b>bold</b>, <i>italic</i>, <u>underline</u>, different <fg #FF0000>foreground</fg>, <bg=orange> background </bg>, <fs=32>font size</fs>, <ff=Courier New>font family</ff><lb>line break,<lb>How about some H<sub>2</sub>O or e = mc<sup>2</sup>.";
                        break;
                    case 2:
                        TheText = "Some <b><i><u><fg=green><fs=*1.75>big, bold, italic, underline and green text</b>, then back to normal again (note: no big, italic, underline or green text here, even those tags weren't explicitly closed). Click the checkbox to see the underline color functionality.";
                        break;
                    case 3:
                        TheText = "Plain Text. <b>Bold Text. <b>Still bold here. <b>Yet more bold.</b> What about here?</b> And here?</b> And here?";
                        break;
                    case 4:
                        TheText = "Default font <fs 30>30 <fs 50>50 <fs 70>70</fs> 50?<fs 70>70?</fs> 50?</fs> 30?</fs> default font?";
                        break;
                    default:
                        TheText = "";
                        break;
                }
            }
        }

        private string _theText;

        public string TheText
        {
            get => _theText;
            set => Set(nameof(TheText), ref _theText, value);
        }

        private bool _underlineUsesCurrentForegroundBrush;

        public bool UnderlineUsesCurrentForegroundBrush
        {
            get => _underlineUsesCurrentForegroundBrush;
            set => Set(nameof(UnderlineUsesCurrentForegroundBrush), ref _underlineUsesCurrentForegroundBrush, value);
        }
    }
}
