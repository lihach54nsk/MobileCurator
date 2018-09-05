using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinSandbox.Engine
{
    [Serializable]
    public class FAQ : IComparable<FAQ>
    {
        public enum Target : byte { Group, All }
        Target target;

        string question, answer;

        public string Answer => answer;
        public Target Target1 => target;
        public string Question => question;

        public FAQ(string question, string answer, Target target = Target.Group)
        {
            this.question = question;
            this.answer = answer;
            this.target = target;
        }

        public int CompareTo(FAQ other)
        {
            return Question.CompareTo(other.Question);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Question: " + Question);
            stringBuilder.AppendLine("Answer: " + answer);
            return stringBuilder.ToString();
        }
    }


    [Serializable]
    class FAQs
    {
        List<FAQ> listFAQ;

        public List<FAQ> ListFAQ => listFAQ;

        public FAQs()
        {
            listFAQ = new List<FAQ>();
        }

        /// <summary>
        /// Добавляет вопрос-ответ в базу
        /// </summary>
        /// <param name="faq"></param>
        public void AddFAQ(FAQ faq)
        {
            //if (listFAQ == null) listFAQ = new List<FAQ>();
            listFAQ.Add(faq);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (FAQ a in listFAQ) stringBuilder.Append(a);
            return stringBuilder.ToString();
        }
    }
}