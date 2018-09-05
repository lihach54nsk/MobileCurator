using System;

namespace XamarinSandbox.Engine.Authorization
{
    [Serializable]
    public class User
    {
        private string faculty;
        private string group;
        public User(string group, string faculty)
        {
            this.faculty = faculty;
            this.group = group;
        }

        public string Group { get => group; }
        public string Faculty { get => faculty; }
    }

    [Serializable]
    public class Curator : User
    {
        public enum AccessLevel : byte { standart, high, admin };
        //Уровень доступа текущего пользователя
        readonly AccessLevel accessLevel;
        //Токен текущего пользователя. Нужен для подтверждения прав на сервере
        private byte[] token = null;
        //Идентификатор пользователя
        uint id;

        public AccessLevel AccessLevel1 { get => accessLevel; }
        public uint ID { get => id; set => id = value; }
        public byte[] Token { set => token = value; }

        public Curator(string group, string faculty, AccessLevel accessLevel): base(group, faculty)
        {
            //Set(firstname, lastname);
            this.accessLevel = accessLevel;
        }

        /// <summary>
        /// Отправка новости на сервер вместе с токеном
        /// </summary>
        public bool SendNews(Network network, News news) => network.SendNews(token, news);

        /// <summary>
        /// Отправка вопроса-ответа на сервер вместе с токеном
        /// </summary>
        public bool SendFAQ(Network network, FAQ faq) => network.SendFAQ(token, faq);
    }

    
}