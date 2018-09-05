using System;
using System.Text;

namespace XamarinSandbox.Engine
{
    [Serializable]
    public  class News:IComparable<News>
    {
        public enum Target : byte { Group, All }
        //Назначение (группе или всем)
        private Target target1;
        //Приоритет новости
        private bool isHighPriority;
        //Заголовок новости
        string title;
        //Текст новости
        string mainText;
        //Дата публикации новости
        DateTime dateTime;
        //Ссылка на прикреплённое изображение
        string imageRef;
        //ID создателя публикации
        uint creatorID;
        //Имя создателя новости
        [NonSerialized] string creatorName = null;


        //Инкапсуляция полей
        public string Title { get => title; }
        public string MainText { get => mainText; }
        public DateTime DateTime { get => dateTime; }
        public string ImageRef { get => imageRef; }
        internal Target Target1 { get => target1; }
        public bool IsHighPriority { get => isHighPriority; }
        public string CreatorName { get => creatorName; }

        public News(string title, string mainText, bool isHighPriority = false, Target target = Target.Group, string imageRef = null)
        {
            this.title = title;
            this.mainText = mainText;
            this.imageRef = imageRef;
            this.isHighPriority = isHighPriority;
            this.target1 = target;
        }

        /// <summary>
        /// Переопределение метода преобразования объекта в строку
        /// </summary>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Title: " + title);
            stringBuilder.AppendLine("Text: " + MainText);
            stringBuilder.AppendLine("CreatorName: " + CreatorName);
            stringBuilder.AppendLine("Date: " + dateTime);
            stringBuilder.AppendLine("Image reference: " + imageRef);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Реализация метода сравнения новости с другой новостью
        /// </summary>
        public int CompareTo(News other) => this.dateTime.CompareTo(other.dateTime);

        /// <summary>
        /// Осуществляет "подписывание" новости при публикации. Устанавливает дату публикации и id автора
        /// </summary>
        public void SignNews(uint creatorID, DateTime dateTime)
        {
            this.creatorID = creatorID;
            this.dateTime = dateTime;
        }

        /// <summary>
        /// Задаёт имя автора публикации, исходя из значения поля id
        /// </summary>
        public void SetCreatorName(Network network)
        {
            creatorName = network.GetUserNameByID(creatorID);
        }
    }
}
