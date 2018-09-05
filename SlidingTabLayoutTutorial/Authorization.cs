using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace XamarinSandbox.Engine.Authorization
{
    class Authentication
    {
        //Директория для файла пользовательских данных
        const string userDataDirectory = "SignedUser";
        const string userDataFilename = "Signed.user";
        readonly string localeFilesDir;

        //Класс для работы с сетью
        Network network;
        User signedUser = null;

        public bool IsAuthorized => (SignedUser != null);
        public bool IsCurator { get => signedUser.GetType() == typeof(Curator); }
        internal User SignedUser { get => signedUser; }

        public Authentication(string localeFilesDir, Network network)
        {
            this.network = network;
            this.localeFilesDir = localeFilesDir;
            LoadUser();
        }

        public void SaveUser()
        {
            string path = Path.Combine(localeFilesDir, userDataDirectory);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            DataManager.SerializeData(signedUser, Path.Combine(path, userDataFilename));
        }

        void LoadUser()
        {
            string path = Path.Combine(localeFilesDir, userDataDirectory, userDataFilename);
            if (File.Exists(path))
                signedUser = DataManager.DeserializeData<User>(path);
        }

        byte[] CalculateHash(string str)
        {
            byte[] input = Encoding.UTF8.GetBytes(str);
            return SHA256.Create().ComputeHash(input);
        }

        public void LoginDefault(string faculty, string group) { signedUser = new User(group, faculty); }
        //Получает на вход логин и пароль и проверяет их достоверность
        public bool LoginCurator(string login, string password) => network.CheckLogin(login, CalculateHash(password), (user, token) => { user.Token = token; signedUser = user; });

        /// <summary>
        /// [ТОЛЬКО ДЛЯ ТЕСТИРОВАНИЯ] Добавление в систему нового пользователя
        /// </summary>
        public void AddUser(Curator user, string firstname, string lastname, string login, string password) => network.AddUser(user, firstname, lastname, login, CalculateHash(password));
    }
}