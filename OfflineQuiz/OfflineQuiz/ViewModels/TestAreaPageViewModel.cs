using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using OfflineQuiz.Models;

namespace OfflineQuiz.ViewModels
{
    public class TestAreaPageViewModel : NotifyPropertyChangeBase
    {
        private static readonly int TotalQuestions = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NoOfQuestions"]);
        private readonly TimeSpan _quizDuration = TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["TestDuration"]);


        private int _progressBarValue;
        private string _timerText;
        private string _errorText;
        private string _currentQuestion;
        private System.Timers.Timer _timer;
        private TimeSpan _remainingTime;
        private int _currentQuestionNumber;
        private char _selectedOption = char.MinValue;
        private string _optionA, _optionB, _optionC, _optionD;
        private bool _optionA_Checked, _optionB_Checked, _optionC_Checked, _optionD_Checked;
        private List<Question> _questionList;
        private readonly char[] _answerSheet = new char[TotalQuestions];
        private bool _quizFinished = false;


        public static event EventHandler NextPageRequested;
        public static int CorrectAnswerCount = 0;


        #region Commands
        public ICommand EndTestCommand { get; set; }
        public ICommand PreviousQuestionCommand { get; set; }
        public ICommand SubmitCommand { get; set; }
        public ICommand NextQuestionCommand { get; set; }
        public ICommand RadioButtonCheckChanged { get; set; }
        public ICommand PageLoaded { get; set; }
        #endregion

        #region Properties
        public int TotalNumberOfQuestions => TotalQuestions;

        public int ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }

        public string TimerText
        {
            get => _timerText;
            set
            {
                _timerText = value;
                OnPropertyChanged("TimerText");
            }
        }

        public string ErrorText
        {
            get => _errorText;
            set
            {
                _errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }

        public string CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                OnPropertyChanged("CurrentQuestion");
            }
        }

        public string Option_A
        {
            get => _optionA;
            set
            {
                _optionA = value;
                OnPropertyChanged("Option_A");
            }
        }

        public string Option_B
        {
            get => _optionB;
            set
            {
                _optionB = value;
                OnPropertyChanged("Option_B");
            }
        }

        public string Option_C
        {
            get => _optionC;
            set
            {
                _optionC = value;
                OnPropertyChanged("Option_C");
            }
        }

        public string Option_D
        {
            get => _optionD;
            set
            {
                _optionD = value;
                OnPropertyChanged("Option_D");
            }
        }

        public bool Option_A_Checked
        {
            get => _optionA_Checked;
            set
            {
                _optionA_Checked = value;
                OnPropertyChanged("Option_A_Checked");
            }
        }

        public bool Option_B_Checked
        {
            get => _optionB_Checked;
            set
            {
                _optionB_Checked = value;
                OnPropertyChanged("Option_B_Checked");
            }
        }

        public bool Option_C_Checked
        {
            get => _optionC_Checked;
            set
            {
                _optionC_Checked = value;
                OnPropertyChanged("Option_C_Checked");
            }
        }

        public bool Option_D_Checked
        {
            get => _optionD_Checked;
            set
            {
                _optionD_Checked = value;
                OnPropertyChanged("Option_D_Checked");
            }
        }
        #endregion


        public TestAreaPageViewModel()
        {
            PageLoaded = new RelayCommand(OnPageLoaded);
            EndTestCommand = new RelayCommand(ExecuteEndTest);
            RadioButtonCheckChanged = new ParameterizedRelayCommand(OnRadioButtonCheckChanged);
            PreviousQuestionCommand = new RelayCommand(NavigatePreviousQuestion, CanNavigatePrevious);
            SubmitCommand = new RelayCommand(Submit, CanSubmit);
            NextQuestionCommand = new RelayCommand(NavigateNextQuestion, CanNavigateNext);

            TimerText = _remainingTime.ToString(@"hh\:mm\:ss");
            _remainingTime = _quizDuration;
            _currentQuestionNumber = 0;
            ProgressBarValue = 0;
        }

        private void OnPageLoaded()
        {
            StartTimer();
            LoadQuestions();
        }

        private void StartTimer()
        {
            _timer = new System.Timers.Timer { Interval = 1000, AutoReset = true };
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(new TimeSpan(0, 0, 1));
            TimerText = _remainingTime.ToString(@"hh\:mm\:ss");

            if (_remainingTime == TimeSpan.Zero)
                Dispatcher.CurrentDispatcher.Invoke(EndTest);
        }

        private void EndTest()
        {
            if (_quizFinished) return;

            DisposeTimer();
            ComputeResult();
            if (NextPageRequested != null)
                NextPageRequested(this, EventArgs.Empty);

            _quizFinished = true;
        }

        private void DisposeTimer()
        {
            if (_timer != null && _timer.Enabled)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            _timer = null;
        }

        private void ComputeResult()
        {
            for (var i = 0; i < TotalQuestions; i++)
                if (_questionList[i].CorrectAnswer == _answerSheet[i])
                    CorrectAnswerCount++;
        }

        private void LoadQuestions()
        {
            var questionPicker = new QuestionPicker();
            _questionList = QuestionPicker.GetQuestions(TotalQuestions);
            _currentQuestionNumber = 0;
            UpdateQuestion();
        }

        private void UpdateQuestion()
        {
            CurrentQuestion = string.Format("Question {0}: {1}", _currentQuestionNumber + 1,
                _questionList[_currentQuestionNumber].QuestionToAsk);
            Option_A = _questionList[_currentQuestionNumber].OptionA;
            Option_B = _questionList[_currentQuestionNumber].OptionB;
            Option_C = _questionList[_currentQuestionNumber].OptionC;
            Option_D = _questionList[_currentQuestionNumber].OptionD;
        }

        private void ExecuteEndTest()
        {
            var msg = _answerSheet.Any(x => x == char.MinValue)
                ? "You have skipped few questions. Do you really want to end quiz?"
                : "Do you really want to end quiz?";

            var messageBoxResult = MessageBox.Show(msg, "Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                EndTest();
        }

        private void OnRadioButtonCheckChanged(object param)
        {
            _selectedOption = char.Parse((string)param);
            CommandManager.InvalidateRequerySuggested();

            ErrorText = CanSubmit() ? "Please click on Submit to record your answer." : string.Empty;
        }

        private bool CanSubmit()
        {
            return _selectedOption != _answerSheet[_currentQuestionNumber];
        }

        private void NavigatePreviousQuestion()
        {
            _currentQuestionNumber--;
            UpdateQuestion();
            ClearSelection();
            LoadPreviousAnswer();
            CommandManager.InvalidateRequerySuggested();
        }

        private void ClearSelection()
        {
            Option_A_Checked = Option_B_Checked = Option_C_Checked = Option_D_Checked = false;
            _selectedOption = char.MinValue;
            ErrorText = string.Empty;
        }

        private void LoadPreviousAnswer()
        {
            if (_answerSheet[_currentQuestionNumber] == char.MinValue) return;

            switch (_answerSheet[_currentQuestionNumber])
            {
                case 'A':
                    Option_A_Checked = true;
                    _selectedOption = 'A';
                    break;
                case 'B':
                    Option_B_Checked = true;
                    _selectedOption = 'B';
                    break;
                case 'C':
                    Option_C_Checked = true;
                    _selectedOption = 'C';
                    break;
                case 'D':
                    Option_D_Checked = true;
                    _selectedOption = 'D';
                    break;
            }
        }

        private bool CanNavigatePrevious()
        {
            return _currentQuestionNumber > 0;
        }

        private void Submit()
        {
            ErrorText = string.Empty;

            if (_selectedOption == _answerSheet[_currentQuestionNumber])
                return;

            if (_answerSheet[_currentQuestionNumber] == char.MinValue)
                ProgressBarValue++;

            _answerSheet[_currentQuestionNumber] = _selectedOption;
        }

        private void NavigateNextQuestion()
        {
            _currentQuestionNumber++;
            UpdateQuestion();
            ClearSelection();
            LoadPreviousAnswer();
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanNavigateNext()
        {
            return _currentQuestionNumber < TotalQuestions - 1;
        }
    }
}