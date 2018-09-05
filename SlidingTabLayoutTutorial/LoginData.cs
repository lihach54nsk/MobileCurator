using System;
using System.Collections.Generic;

namespace XamarinSandbox.Engine
{
    //Класс-обёртка для массива данных о пользователях
    [Serializable]
    class LoginData
    {
        private List<HashData> hashData;
        //Конструктор по умолчанию
        public LoginData() => hashData = new List<HashData>();

        /// <summary>
        /// Проверяет наличие пользователя по его логину и паролю. Возвращает кортеж из имени файла данных пользователя и его токена
        /// </summary>
        public Tuple<string, byte[]> IsDataExist(string login, byte[] passwordHash)
        {
            Tuple<string, byte[]> tuple=null;
            for (int i = 0; i < hashData.Count; i++)
            {
                tuple = hashData[i].CheckData(login, passwordHash);
                if (tuple!=null) break;
            }
            return tuple;
        }

        /// <summary>
        /// Проверяет, существует ли пользователь с таким токеном
        /// </summary>
        public string IsDataExist(byte[] token)
        {
            string fileName = null;
            for (int i = 0; i < hashData.Count; i++)
            {
                fileName = hashData[i].CheckData(token);
                if (fileName != null) break;
            }
            return fileName;
        }

        /// <summary>
        /// Добавляет данные для входа нового пользователя
        /// </summary>
        public void AddUser(string login, byte[] passwordHash, string fileName)
        {
            byte[] token;
            token = GetToken();
            hashData.Add(new HashData(login, passwordHash, token, fileName));
        }

        /// <summary>
        /// Определяет, существует ли в системе пользователь с таким логином
        /// </summary>
        public bool IsUserExist(string login)
        {
            for (int i = 0; i < hashData.Count; i++)
                if (hashData[i].CheckLogin(login)) return true;
            return false;
        }

        /// <summary>
        /// Проверяет наличие в базе такого токена
        /// </summary>
        bool CheckToken(byte[] token)
        {
            for (int i = 0; i < hashData.Count; i++)
                if (hashData[i].CheckToken(token))
                    return false;
            return true;
        }

        /// <summary>
        /// Получить случайно сгенерированный токен, который ещё не задействован в системе
        /// </summary>
        byte[] GetToken()
        {
            Random random = new Random();
            byte[] token;
            do
            {
                byte[] str = new byte[100];
                random.NextBytes(str);
                token = System.Security.Cryptography.SHA256.Create().ComputeHash(str);
            } while (!CheckToken(token));
            return token;
        }
    }

    //Данные об одном пользователе
    [Serializable]
    struct HashData
    {
        string login;
        byte[] passwordHash;
        byte[] token;
        string userDataFileName;

        public HashData(string login, byte[] passwordHash, byte[] token, string userDataFileName)
        {
            this.login = login;
            this.passwordHash = passwordHash;
            this.userDataFileName = userDataFileName;
            this.token = token;
        }

        /// <summary>
        /// Проверяет, соответствуют ли логин и хэш пароля этому объекту. 
        /// Если да - возвращает ссылку на файл с данными пользователя, в ином случае null
        /// </summary>
        public Tuple<string, byte[]> CheckData(string login, byte[] passwordHash)
        {
            if (this.login == login && EqualHash(this.passwordHash, passwordHash))
            {
                return Tuple.Create(userDataFileName,this.token);
            }
            return null;
        }

        /// <summary>
        /// Проверяет, соответствует ли логин этому объекту
        /// </summary>
        public bool CheckLogin(string login) => (this.login == login);

        /// <summary>
        /// Проверяет, соответствует ли токен этому объекту
        /// </summary>
        public bool CheckToken(byte[] token) => EqualHash(this.token, token);

        /// <summary>
        /// Проверяет, соответствует ли токен этому объекту. 
        /// Если да - возвращает ссылку на файл с данными пользователя, в ином случае null
        /// </summary>
        public string CheckData(byte[] token)
        {
            if (CheckToken(token))
                return userDataFileName;
            return null;
        }

        /// <summary>
        /// Сравнивает 2 хэша. Вовращает true, если они совпадают
        /// </summary>
        private static bool EqualHash(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;
            for (int i = 0; i < hash1.Length; i++)
                if (hash1[i] != hash2[i]) return false;
            return true;
        }
    }

    [Serializable]
    class UserNames
    {
        Dictionary<uint, Tuple<string, string>> dictionary = new Dictionary<uint, Tuple<string, string>>();

        /// <summary>
        /// Добавить имя пользователя в базу. Возвращает id, который ему присвоен
        /// </summary>
        public uint AddName(string firtsname, string lastname)
        {
            uint id = GetID();
            dictionary.Add(GetID(), Tuple.Create(firtsname, lastname));
            return id;
        }

        /// <summary>
        /// Сгенерировать id
        /// </summary>
        /// <returns></returns>
        uint GetID()
        {
            uint id;
            for (id = 0; dictionary.ContainsKey(id); id++) ;
            return id;
        }

        /// <summary>
        /// Индексатор класса
        /// </summary>
        public Tuple<string, string> this[uint index]
        {
            get => dictionary[index];
            set => dictionary[index] = value;
        }
    }
}