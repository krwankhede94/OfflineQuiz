using System;
using System.Windows;

namespace OfflineQuiz.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangeBase
    {
        private object _currentViewModel;
        private string _titleBarText;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }

        public string TitleBarText
        {
            get => _titleBarText;
            set
            {
                _titleBarText = value;
                OnPropertyChanged("TitleBarText");
            }
        }

        public MainWindowViewModel()
        {
            var userDetailsPageViewModel = new UserDetailsPageViewModel();
            UserDetailsPageViewModel.NextPageRequested += UserDetailsPageViewModel_NextPageRequested;
            CurrentViewModel = userDetailsPageViewModel;
            TitleBarText = "Welcome";
        }

        private void UserDetailsPageViewModel_NextPageRequested(object sender, EventArgs e)
        {
            UserDetailsPageViewModel.NextPageRequested -= UserDetailsPageViewModel_NextPageRequested;

            var instructionsPageViewModel = new InstructionsPageViewModel();
            InstructionsPageViewModel.NextPageRequested += InstructionsPageViewModel_NextPageRequested;
            CurrentViewModel = instructionsPageViewModel;
            TitleBarText = "Instructions";
        }

        private void InstructionsPageViewModel_NextPageRequested(object sender, EventArgs e)
        {
            InstructionsPageViewModel.NextPageRequested -= InstructionsPageViewModel_NextPageRequested;

            var testAreaPageViewModel = new TestAreaPageViewModel();
            TestAreaPageViewModel.NextPageRequested += TestAreaPageViewModel_NextPageRequested;
            CurrentViewModel = testAreaPageViewModel;
            TitleBarText = "Quiz started!";
        }

        private void TestAreaPageViewModel_NextPageRequested(object sender, EventArgs e)
        {
            TestAreaPageViewModel.NextPageRequested -= TestAreaPageViewModel_NextPageRequested;

            ResultsPageViewModel.CorrectAnswerCount = TestAreaPageViewModel.CorrectAnswerCount;
            var resultsPageViewModel = new ResultsPageViewModel();
            ResultsPageViewModel.NextPageRequested += ResultsPageViewModel_NextPageRequested;
            CurrentViewModel = resultsPageViewModel;
            TitleBarText = "Result";
        }

        private static void ResultsPageViewModel_NextPageRequested(object sender, EventArgs e)
        {
            ResultsPageViewModel.NextPageRequested -= ResultsPageViewModel_NextPageRequested;
            Application.Current.Shutdown();
        }
    }
}