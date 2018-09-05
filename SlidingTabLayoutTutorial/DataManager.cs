using System;
using System.Collections.Generic;
using System.IO;



namespace XamarinSandbox.Engine
{
    class DataManager
    {
        //Файл, содержащий пути к файлам с вопросами/ответами
        const string faqsFileName = "faqingPaths.bl";
        //Имя директории для данных DataManager
        const string dataManagerDirName = "dataManagerDirectory";

        //Объект, симулирующий доступ к сети
        Network network;
        //Директория локального кэша
        string localeCacheDir;
        //Директория локальных данных приложения
        string localeFileDir;

        //Список новостей
        List<News>  listNews;
        //Список вопросов/ответов
        List<FAQ> listFAQ;

        internal IReadOnlyList<News> NewsList { get => listNews; }
        internal IReadOnlyList<FAQ> ListFAQ { get => listFAQ; }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="localeFileDir">Директория данных приложения</param>
        /// <param name="localeCacheDir">Директория кэша приложения</param>
        public DataManager(string localeFileDir, string localeCacheDir, Network network)
        {
            this.localeFileDir = localeFileDir;
            this.localeCacheDir = localeCacheDir;
            this.network = network;
            listNews = new List<News>();
            listFAQ = new List<FAQ>();
        }


        /// <summary>
        /// Загружает в список новостей следующий блок
        /// </summary>
        /// <param name="user">Данные о текщем пользователе</param>
        /// <returns>Выполнена ли загрузка?</returns>
        public bool LoadNewsData(Authorization.User user)
        {
            if (user == null) throw new Exception("Signed user error!");

            string[] newsRef = network.LoadNextBlock(user);
            if (newsRef == null) return false;

            News[] listNewsPool = new News[newsRef.Length];

            for (int i = 0; i < newsRef.Length; i++)
            {
                listNewsPool[i] = DeserializeData<News>(newsRef[i]);
                listNewsPool[i].SetCreatorName(network);
            }
            Array.Sort(listNewsPool);
            listNews.AddRange(listNewsPool);
            return true;
        }

        /// <summary>
        /// Обновить список новостей на устройстве
        /// </summary>
        /// <param name="user">Данные о текщем пользователе</param>
        public void RefreshNewsList(Authorization.User user)
        {
            if (user == null) throw new Exception("Signed user error!");
            network.RefreshNewsBlocks();
            listNews = new List<News>();
            LoadNewsData(user);
        }


        /// <summary>
        /// Отправка новости на сервер
        /// </summary>
        public bool SendNews(News news, Authorization.Curator curator)
        {
            if (curator == null) throw new Exception("Signed curator error!");
            return curator.SendNews(network, news);
        }
        
        
        /// <summary>
        /// Отправка вопроса-ответа на сервер
        /// </summary>
        public bool SendFAQ(FAQ faq, Authorization.Curator curator)
        {
            if (curator == null) throw new Exception("Signed curator error!");
            return curator.SendFAQ(network, faq);
        }

        /// <summary>
        /// Обновить кэш списка вопросов-ответов
        /// </summary>
        /// <param name="user">Данные о текщем пользователе</param>
        public void RefreshFAQ(Authorization.User user)
        {
            if (user == null) throw new Exception("Signed user error!");
            string[] faqFilesPath = network.LoadFAQ(user);
            string path = Path.Combine(localeCacheDir, dataManagerDirName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, faqsFileName);
            File.WriteAllLines(path, faqFilesPath);
            LoadLocleFAQ();
        }


        /// <summary>
        /// Загрузить в память список кэш вопросов-ответов
        /// </summary>
        public void LoadLocleFAQ()
        {
            string path = Path.Combine(localeCacheDir, dataManagerDirName, faqsFileName);
            if (!File.Exists(path)) { listFAQ = new List<FAQ>(); return; }
            string[] faqFilesPath = File.ReadAllLines(path);
            listFAQ = new List<FAQ>();
            foreach (string a in faqFilesPath)
                if (File.Exists(a))
                    listFAQ.AddRange(DeserializeData<FAQs>(a).ListFAQ);
            listFAQ.Sort();
        }


        /////////////////////
        //Статические члены//
        /////////////////////

        static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        /// <summary>
        /// Сериализация объектов
        /// </summary>
        /// <typeparam name="TDataType">Тип данных сериализуемого объекта</typeparam>
        /// <param name="obj">Сериализуемый объект</param>
        /// <param name="filePath">Путь к файлу, в который производится сериализация</param>
        public static void SerializeData<TDataType>(TDataType obj, string filePath)
        {
            Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        /// <summary>
        /// Десериализация объектов
        /// </summary>
        /// <typeparam name="TDataType">Тип данных десериализуемого объекта</typeparam>
        /// <param name="filePath">Путь к файлу, из которого производится десериализация</param>
        /// <returns></returns>
        public static TDataType DeserializeData<TDataType>(string filePath)
        {
            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            TDataType pool = (TDataType) formatter.Deserialize(stream);
            stream.Close();
            return pool;
        }
    }
}