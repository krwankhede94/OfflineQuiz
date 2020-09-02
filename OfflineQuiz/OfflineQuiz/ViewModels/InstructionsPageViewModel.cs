using System;
using System.Windows.Input;

namespace OfflineQuiz.ViewModels
{
    public class InstructionsPageViewModel
    {
        public static event EventHandler NextPageRequested;

        public ICommand StartTestCommand { get; set; }

        public InstructionsPageViewModel()
        {
            StartTestCommand = new RelayCommand(StartTest);
        }

        private void StartTest()
        {
            if (NextPageRequested != null)
                NextPageRequested(this, EventArgs.Empty);
        }
    }
}