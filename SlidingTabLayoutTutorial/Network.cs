using System;
using System.IO;

using XamarinSandbox.Engine.Authorization;
namespace XamarinSandbox.Engine
{
    /// <summary>
    /// Класс для работы с сетью и кэшем устройства. На данный момент является заглушкой и получает данные из локального хранилища
    /// </summary>
    public class Network
    {
        //Директория внутреннего кэша приложения
        string localeCacheDir;
        //Путь к папке с загружаемыми в кэш данными
        string downloadedDataPath;

        //Имя папки с иерархией новостей
        const string newsFolderName = "News";
        //Имя папки с хэшами пользователей
        const string userDataFolderName = "LogIn";
        //Имя папки для вопросов и ответов
        const string faqDirectory = "FAQ";
        //Название файла, содержащего упорядоченный набор папок
        const string newsBlocksFileName = "BlockList.bl";
        //Название файла с базой данных пользователей системы
        const string usersDataFile = "UsersData.bl";
        //Имя словаря имён пользователей
        const string userNamesDictonary = "NamesDictonary.bl";
        //Имя файла с общим списком вопросов/ответов
        const string faqForAllFile = "FaqForAll";

        //Номер следующего блока данных для считывания
        uint readingNewsNumber;

        //Количество новостей в одном блоке
        readonly uint newsInBlockCount;
        //Упорядоченный массив с названиями папок блоков данных
        string[] newsBlocks;

        UserNames userNames;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Network(string downloadedDataPath, string localeCacheDir, uint newsInBlockCount)
        {
            this.downloadedDataPath = downloadedDataPath;
            this.localeCacheDir = localeCacheDir;
            this.newsInBlockCount = newsInBlockCount;
            InitDataStruct();
        }

        /// <summary>
        /// Инициализация структуры хранения данных
        /// </summary>
        void InitDataStruct()
        {
            string pathUserNamesDictonary = Path.Combine(downloadedDataPath, userDataFolderName, userNamesDictonary);
            if (File.Exists(pathUserNamesDictonary)) userNames = DataManager.DeserializeData<UserNames>(pathUserNamesDictonary);
            else userNames = new UserNames();
            //Инициализация директории
            void InitDirectory(string folderName)
            {
                string path = Path.Combine(downloadedDataPath, folderName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            InitDirectory(newsFolderName);
            InitDirectory(userDataFolderName);
            InitDirectory(faqDirectory);

            RefreshNewsBlocks();
        }

        /// <summary>
        /// Загружает в память файлы новостей и возвращает массив путей к ним
        /// </summary>
        /// <param name="path">Путь к блоку новостей</param>
        /// <param name="group">Имя группы, к которой относится пользователь</param>
        /// <returns></returns>
        string[] LoadNewsCacheData(string path, string group)
        {
            //Получаем список файлов, находящихся в папке для симуляции сервера
            string[] sumLoadedCache;
            string[] loadedCacheFiles = Directory.GetFiles(path);
            string groupPath = Path.Combine(path, group);
            if (Directory.Exists(groupPath))
            {
                string[] loadedCacheFiles1 = Directory.GetFiles(groupPath);
                sumLoadedCache = new string[loadedCacheFiles.Length + loadedCacheFiles1.Length];
                loadedCacheFiles.CopyTo(sumLoadedCache, 0);
                loadedCacheFiles1.CopyTo(sumLoadedCache, loadedCacheFiles.Length);
            }
            else
            {
                sumLoadedCache = loadedCacheFiles;
            }
            string destFileName;
            for (int i = 0; i < sumLoadedCache.Length; i++)
            {
                destFileName = Path.Combine(localeCacheDir, Path.GetFileName(sumLoadedCache[i]));
                if (!File.Exists(destFileName))
                    File.Copy(sumLoadedCache[i], destFileName);
                sumLoadedCache[i] = destFileName;
            }
            return sumLoadedCache;
        }

        /// <summary>
        /// Загружает следующий блок новостей. Возвращает массив путей к ним
        /// </summary>
        public string[] LoadNextBlock(User user)
        {
            if (newsBlocks.Length <= readingNewsNumber || newsBlocks.Length == 0)
                return null;
            string pathNextBlock = newsBlocks[readingNewsNumber];
            readingNewsNumber++;
            if (!Directory.Exists(pathNextBlock))
                return LoadNextBlock(user);
            return LoadNewsCacheData(pathNextBlock, user.Group);
        }

        /// <summary>
        /// Добавляет в структуру новый блок данных
        /// </summary>
        /// <param name="path">Путь к директории с блоками новостей</param>
        /// <returns></returns>
        string AddNewsBlock(string path)
        {
            string newPath = Path.Combine(path, Guid.NewGuid().ToString());
            Directory.CreateDirectory(newPath);
            int length = newsBlocks.Length + 1;
            Array.Resize(ref newsBlocks, length);
            newsBlocks[length - 1] = newPath;
            File.WriteAllLines(Path.Combine(path, newsBlocksFileName), newsBlocks);
            return newPath;
        }

        /// <summary>
        /// Обновить данные в списке блоков новостей
        /// </summary>
        public void RefreshNewsBlocks()
        {
            string path = Path.Combine(downloadedDataPath, newsFolderName, newsBlocksFileName);
            if (File.Exists(path))
                newsBlocks = File.ReadAllLines(Path.Combine(downloadedDataPath, newsFolderName, newsBlocksFileName));
            else newsBlocks = new string[0];
            readingNewsNumber = 0;
        }

        /// <summary>
        /// Делегат метода подтверждения пользователя
        /// </summary>
        public delegate void ApplyUser(Curator user, byte[] token);

        /// <summary>
        /// Проверка корректности логина и пароля
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="passwordHash">Пароль</param>
        /// <param name="apply">Делегируемый метод подтверждения пользователя</param>
        public bool CheckLogin(string login, byte[] passwordHash, ApplyUser apply)
        {
            //Объект, представляющий массив пользовательских данных для входа
            LoginData hashes;
            //Имя файла с данными о пользователе и его токен
            Tuple<string, byte[]> userData;
            //Путь к папке пользовательских данных
            string path = Path.Combine(downloadedDataPath, userDataFolderName);
            //Путь к файлу пользовательских данных
            string pathUsersDataFile = Path.Combine(path, usersDataFile);
            if (File.Exists(pathUsersDataFile))
                hashes = DataManager.DeserializeData<LoginData>(pathUsersDataFile);
            else
                return false;

            //Осуществляем проверку наличия данного пользователя
            userData = hashes.IsDataExist(login, passwordHash);
            if (userData == null)
                return false;
            else
            {
                apply(DataManager.DeserializeData<Authorization.Curator>(Path.Combine(path, userData.Item1)), userData.Item2);
                return true;
            }

        }

        /// <summary>
        /// Добавление нового пользователя в систему
        /// </summary>
        public void AddUser(Curator user, string firstname, string lastname, string login, byte[] passwordHash)
        {
            LoginData hashes;
            //Путь к папке пользовательских данных
            string path = Path.Combine(downloadedDataPath, userDataFolderName);
            //Путь к файлу пользовательских данных
            string pathUsersDataFile = Path.Combine(path, usersDataFile);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!File.Exists(pathUsersDataFile))
            {
                hashes = new LoginData();
            }
            else
            {
                hashes = DataManager.DeserializeData<LoginData>(pathUsersDataFile);
            }
            if (hashes.IsUserExist(login)) return;
            user.ID = userNames.AddName(firstname, lastname);
            DataManager.SerializeData(userNames, Path.Combine(path, userNamesDictonary));
            path = Path.Combine(path, Guid.NewGuid().ToString());
            DataManager.SerializeData(user, path);
            hashes.AddUser(login, passwordHash, Path.GetFileName(path));
            DataManager.SerializeData(hashes, pathUsersDataFile);

        }

        /// <summary>
        /// Получение имени пользователя из базы по id. Если пользователь не существует возвращет null
        /// </summary>
        public string GetUserNameByID(uint id)
        {
            Tuple<string, string> tuple = userNames[id];
            return String.Format("{0} {1}", tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// Найти количество файлов в блоке новостей
        /// </summary>
        int GetCountFilesInDirectory(string directory)
        {
            int count = Directory.GetFiles(directory).Length;
            foreach (string a in Directory.GetDirectories(directory))
            {
                count += Directory.GetFiles(a).Length;
            }
            return count;
        }

        /// <summary>
        /// Добавление нового вопроса-ответа в базу
        /// </summary>
        /// <param name="token">Токен добавляющего пользователя</param>
        /// <param name="faq">Объект вопроса-ответа</param>
        public bool SendFAQ(byte[] token, FAQ faq)
        {
            string path = Path.Combine(downloadedDataPath, userDataFolderName);
            LoginData loginData = DataManager.DeserializeData<LoginData>(Path.Combine(path, usersDataFile));
            string userFile = loginData.IsDataExist(token);
            if (userFile != null)
            {
                Curator curator = DataManager.DeserializeData<Authorization.Curator>(Path.Combine(path, userFile));
                if ((curator.AccessLevel1 != Curator.AccessLevel.admin && curator.AccessLevel1 != Curator.AccessLevel.high) && ( faq.Target1 == FAQ.Target.All))
                    return false;
                switch (faq.Target1)
                {
                    case FAQ.Target.All: SendDataFAQ(faq); break;
                    case FAQ.Target.Group: SendDataFAQ(faq, curator.Group); break;
                }
            }
            else throw new Exception("Login is incorrect");
            return true;
        }

        /// <summary>
        /// Размещение вопроса-ответа на внешнем накопителе устройства
        /// </summary>
        private void SendDataFAQ(FAQ faq, string filename = faqForAllFile)
        {
            FAQs faqs;
            string path = Path.Combine(downloadedDataPath, faqDirectory, filename+".faq");
            if (File.Exists(path)) faqs = DataManager.DeserializeData<FAQs>(path);
            else faqs = new FAQs();
            faqs.AddFAQ(faq);
            DataManager.SerializeData(faqs, path);
        }

        /// <summary>
        /// Загрузить вопросы-отвееты из базы для указанного пользователя
        /// </summary>
        /// <returns>Пути к файлам с вопросами-ответами во внутреннем кэше</returns>
        public string[] LoadFAQ(User user)
        {
            string[] returningPaths = new string[2];
            string path = Path.Combine(downloadedDataPath, faqDirectory);


            void LoadFile(string fileName, ref string returningPath)
            {
                string filePath = Path.Combine(path, fileName + ".faq");
                if (!File.Exists(filePath)) return;
                returningPath = Path.Combine(localeCacheDir, fileName + ".faq");
                bool isFileExist = File.Exists(returningPath);
                if (!isFileExist || (isFileExist && (File.GetCreationTimeUtc(filePath) > File.GetCreationTimeUtc(returningPath))))
                    File.Copy(filePath, returningPath, true);
            }

            LoadFile(faqForAllFile, ref returningPaths[0]);
            LoadFile(user.Group, ref returningPaths[1]);
            return returningPaths;
        }

        /// <summary>
        /// Осуществляет отправку новости. Если отправка успешна, то возвращает true
        /// </summary>
        public bool SendNews(byte[] token, News news)
        {
            LoginData loginData = DataManager.DeserializeData<LoginData>(Path.Combine(downloadedDataPath, userDataFolderName, usersDataFile));
            string userFile = loginData.IsDataExist(token);
            if (userFile != null)
            {
                Curator curator = DataManager.DeserializeData<Authorization.Curator>(Path.Combine(downloadedDataPath, userDataFolderName, userFile));
                if ((curator.AccessLevel1 != Curator.AccessLevel.admin && curator.AccessLevel1 != Curator.AccessLevel.high) && (news.IsHighPriority || news.Target1 == News.Target.All))
                    return false;
                news.SignNews(curator.ID, DateTime.Now);
                switch (news.Target1)
                {
                    case News.Target.All: SendNewsData(news);
                        break;
                    case News.Target.Group: SendNewsData(news, curator.Group);
                        break;
                }

                /*if(news.IsHighPriority)
                {
                    //TODO: Добавить код для новости с высоким приоритетом
                    throw new NotImplementedException();
                }*/
            }
            else throw new Exception("Login is incorrect");
            return true;
        }

        /// <summary>
        /// Размещение новости в блоке на внешнем накопителе устройства
        /// </summary>
        void SendNewsData(News news)
        {
            //Директория, содержащая все блоки
            string directory = Path.Combine(downloadedDataPath, newsFolderName);
            string blockDirectory;
            //Директория конкретного блока
            if (newsBlocks.Length != 0)
            {
                blockDirectory = Path.Combine(directory, newsBlocks[newsBlocks.Length - 1]);
                if (GetCountFilesInDirectory(blockDirectory) >= newsInBlockCount)
                    blockDirectory = AddNewsBlock(directory);
            }
            else
            {
                blockDirectory = AddNewsBlock(directory);
            }
            DataManager.SerializeData(news, Path.Combine(blockDirectory, Guid.NewGuid().ToString()));
        }

        /// <summary>
        /// Размещает новость в блоке, согласно указанной группе
        /// </summary>
        /// <param name="news">Объект новости</param>
        /// <param name="group">Имя группы</param>
        void SendNewsData(News news, string group)
        {
            string directory = Path.Combine(downloadedDataPath, newsFolderName);
            string blockDirectory;
            //Директория конкретного блока
            if (newsBlocks.Length != 0)
            {
                blockDirectory = Path.Combine(directory, newsBlocks[newsBlocks.Length - 1]);
                if (GetCountFilesInDirectory(blockDirectory) >= newsInBlockCount)
                    blockDirectory = AddNewsBlock(directory);
            }
            else
            {
                blockDirectory = AddNewsBlock(directory);
            }
            directory = Path.Combine(blockDirectory, group);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            DataManager.SerializeData(news, Path.Combine(directory, Guid.NewGuid().ToString()));
        }
    }
}