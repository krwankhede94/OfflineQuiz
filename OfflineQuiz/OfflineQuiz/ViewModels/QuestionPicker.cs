using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using OfflineQuiz.Models;

namespace OfflineQuiz.ViewModels
{
    public class QuestionPicker
    {
        public static List<Question> GetQuestions(int count)
        {
            var allQuestions = GetAllQuestions();

            if (allQuestions == null || count > allQuestions.Count)
                throw new InvalidOperationException("Insufficient data in questions repository.");

            var randomQuestionNumbers = GetRandomQuestionNumbers(count, allQuestions.Count);

            return GetQuestionList(randomQuestionNumbers, allQuestions);
        }

        private static List<Question> GetQuestionList(int[] questionNumbers, XmlNodeList allQuestions)
        {
            var questions = new List<Question>();
            foreach (var questionNumber in questionNumbers)
            {
                var questionNode = allQuestions[questionNumber - 1];
                var question = new Question()
                {
                    QuestionToAsk = questionNode.SelectSingleNode("QuestionToAsk")?.InnerText,
                    OptionA = questionNode.SelectSingleNode("OptionA")?.InnerText,
                    OptionB = questionNode.SelectSingleNode("OptionB")?.InnerText,
                    OptionC = questionNode.SelectSingleNode("OptionC")?.InnerText,
                    OptionD = questionNode.SelectSingleNode("OptionD")?.InnerText,
                    CorrectAnswer =
                        char.Parse(questionNode.SelectSingleNode("CorrectAnswer")?.InnerText ?? string.Empty)
                };
                questions.Add(question);
            }

            return questions;
        }

        private static XmlNodeList GetAllQuestions()
        {
            var questionsData = new XmlDocument();
            questionsData.Load("Data\\QuestionData.xml");
            var allQuestions = questionsData.DocumentElement?.GetElementsByTagName("Question");
            return allQuestions;
        }

        private static int[] GetRandomQuestionNumbers(int count, int totalQuestions)
        {
            var questionNumbers = new int[count];
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                int number;
                do
                {
                    number = random.Next(1, totalQuestions+1);
                } while (questionNumbers.Contains(number));

                questionNumbers[i] = number;
            }

            return questionNumbers;
        }
    }
}
