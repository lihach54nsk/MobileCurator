using XamarinSandbox.Engine.Authorization;
using System.Collections.Generic;

namespace XamarinSandbox.Engine
{
    public  class CuratorEngine
    {
        Authentication authorization;
        Network network;
        DataManager dataManager;

        public CuratorEngine(string downloadedDataPath, string localeCacheDir, string localeFilesDir, uint newsInBlockCount)
        {
            network = new Network(downloadedDataPath, localeCacheDir, newsInBlockCount);
            dataManager = new DataManager(localeFilesDir, localeCacheDir, network);
            authorization = new Authentication(localeFilesDir, network);
        }

        /// <summary>
        /// Сохранить настройки приложения перед его закрытием
        /// </summary>
        public void SaveData()
        {
            authorization.SaveUser();
        }

        /// <summary>
        /// Загрузка следующего блока новостей
        /// </summary>
        public bool LoadNextNews() => dataManager.LoadNewsData(authorization.SignedUser);

        /// <summary>
        /// Обновить список новостей
        /// </summary>
        public void RefreshNewsList() => dataManager.RefreshNewsList(authorization.SignedUser);

        /// <summary>
        /// Опубликовать новость от имени текущего пользователя
        /// </summary>
        /// <param name="news">Объект отправляемой новости</param>
        public bool SendNews(News news) => dataManager.SendNews(news, authorization.SignedUser as Curator);

        /// <summary>
        /// Публикация вопроса-ответа он имени текущего пользователя
        /// </summary>
        /// <param name="faq">Объект вопроса-ответа</param>
        public bool SendFAQ(FAQ faq) => dataManager.SendFAQ(faq, authorization.SignedUser as Curator);

        /// <summary>
        /// Обновить список вопросов-ответов на устройстве
        /// </summary>
        public void RefreshFAQ() => dataManager.RefreshFAQ(authorization.SignedUser);

        /// <summary>
        /// Загрузка списка вопросов-ответов на локальном устройстве
        /// </summary>
        public void LoadLocaleFAQ() => dataManager.LoadLocleFAQ();

        /// <summary>
        /// Вход в систему пользователя в режиме обычного пользователя
        /// </summary>
        /// <param name="faculty">Имя факультета</param>
        /// <param name="group">Имя группы</param>
        public void LoginDefault(string faculty, string group) => authorization.LoginDefault(faculty,group);

        /// <summary>
        /// Вход в систему от имени куратора
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Успех авторизации</returns>
        public bool LoginCurator(string login, string password) => authorization.LoginCurator(login, password);

        /// <summary>
        /// Доступ к списку новостей в режиме "только для чтения"
        /// </summary>
        internal IReadOnlyList<News> NewsList => dataManager.NewsList;

        /// <summary>
        /// Доступ к списку вопросов-ответов на локальном устройстве
        /// </summary>
        internal IReadOnlyList<FAQ> FAQList => dataManager.ListFAQ;


        /// <summary>
        /// Являетя ли текущий пользователь куратором
        /// </summary>
        internal bool IsCuarator => authorization.IsCurator;

        /// <summary>
        /// Авторизация выполнена
        /// </summary>
        internal bool IsAuthorized => authorization.IsAuthorized;

        /// <summary>
        /// [TEST ONLY] Добавление в систему нового пользователя
        /// </summary>
        public void Registy(string firstname, string lastname, string group, string faculty, Curator.AccessLevel accessLevel, string login, string password) => authorization.AddUser(new Curator(group, faculty, accessLevel), firstname, lastname, login, password);
    }
}