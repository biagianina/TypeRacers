using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace TypeRacers.ViewModel
{
    interface IViewModel
    {
        IEnumerable<Inline> Inlines { get; }

        bool IsValid { get; set; }

        string Progress { get; }

        int CurrentWordLength { get; }

        bool AllTextTyped { get; set; }

        string TextToType { get; }

        string CurrentInputText { get; set; }
        bool TypingAlert { get; set; }

        string InputBackgroundColor { get; }

        void CheckUserInput(string value);

        void ReportProgress();

        void HighlightText();


    }
}
