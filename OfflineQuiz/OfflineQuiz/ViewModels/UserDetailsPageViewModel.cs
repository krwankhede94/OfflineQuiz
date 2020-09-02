using System;
using System.Windows.Input;

namespace OfflineQuiz.ViewModels
{
    public class UserDetailsPageViewModel:NotifyPropertyChangeBase
    {
        private string _errorText;
        public static event EventHandler NextPageRequested;

        public ICommand SubmitUserDetailsCommand { get; set; }

        public static string UserName { get; set; }

        public string Email { get; set; }

        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value; 
                OnPropertyChanged("ErrorText");
            }
        }

        public UserDetailsPageViewModel()
        {
            SubmitUserDetailsCommand = new RelayCommand(SubmitUserDetails);
        }


        private void SubmitUserDetails()
        {
            if (string.IsNullOrEmpty(UserName) ||string.IsNullOrEmpty(Email))
            {
                ErrorText = "Please fill all the details.";
                return;
            }

            if (NextPageRequested != null)
                NextPageRequested(this, EventArgs.Empty);
        }
    }
}
