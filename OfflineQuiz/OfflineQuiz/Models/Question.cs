using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineQuiz.Models
{
    public class Question
    {
        public string QuestionToAsk { get; set; }

        public string OptionA{ get; set; }
        public string OptionB{ get; set; }
        public string OptionC{ get; set; }
        public string OptionD{ get; set; }

        public char CorrectAnswer { get; set; }
    }
}
