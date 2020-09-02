using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Configuration;

namespace OfflineQuiz.ViewModels
{
    public class ResultsPageViewModel : NotifyPropertyChangeBase
    {
        public static event EventHandler NextPageRequested;

        private string _examDateText;
        private string _scoreText;
        private string _greetingText;
        private string _messageText;
        private Brush _messageTextColor;

        #region Properties

        public string ExamDateText
        {
            get => _examDateText;
            set
            {
                _examDateText = value;
                OnPropertyChanged("ExamDateText");
            }
        }

        public string GreetingText
        {
            get => _greetingText;
            set
            {
                _greetingText = value;
                OnPropertyChanged("GreetingText");
            }
        }

        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        public string ScoreText
        {
            get => _scoreText;
            set
            {
                _scoreText = value;
                OnPropertyChanged("ScoreText");
            }
        }

        public Brush MessageTextColor
        {
            get => _messageTextColor;
            set
            {
                _messageTextColor = value;
                OnPropertyChanged("MessageTextColor");
            }
        }

        public static int CorrectAnswerCount { get; set; }

        public static string UserName => string.Format("User name: {0}", UserDetailsPageViewModel.UserName);

        #endregion


        public ICommand ExitCommand { get; set; }

        public ResultsPageViewModel()
        {
            ExitCommand = new RelayCommand(Exit);
            InitializeResultsPage();
        }

        private void InitializeResultsPage()
        {
            ExamDateText = string.Format("Date: {0}", DateTime.Now.ToString("d"));
            var marksPerQuestion = int.Parse(ConfigurationManager.AppSettings["MarksPerQuestion"]);
            var passingMarks = int.Parse(ConfigurationManager.AppSettings["PassingMarks"]);
            var noOfQuestions = int.Parse(ConfigurationManager.AppSettings["NoOfQuestions"]);

            var score = CorrectAnswerCount * marksPerQuestion;
            var maxScore = marksPerQuestion * noOfQuestions;
            if (score >= passingMarks)
            {
                GreetingText = "Congratulations!";
                MessageText = "You have passed the quiz.";
                MessageTextColor = Brushes.Green;
            }
            else
            {
                GreetingText = "Sorry!";
                MessageText = "You have failed the quiz.";
                MessageTextColor = Brushes.Crimson;
            }

            ScoreText = string.Format("Score: {0}/{1}", score, maxScore);
        }

        private void Exit()
        {
            if (NextPageRequested != null)
                NextPageRequested(this, EventArgs.Empty);
        }
    }
}