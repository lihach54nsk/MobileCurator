using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinSandbox.Engine.Authorization;
using XamarinSandbox.Engine;

namespace SlidingTabLayoutTutorial
{
    class Register : EventArgs
    {
        Curator curator;
        string firstname;
        string lastname;
        string login;
        string password;

        public string LogIn
        {
            get { return login; }
            set { login = value; }
        }

        public string FirstName
        {
            get { return firstname; }
            set { firstname = value; }
        }

        public string LastName
        {
            get { return lastname; }
            set { lastname = value; }
        }

        public string PassWord
        {
            get { return password; }
            set { password = value; }
        }

        public Register(string firstname, string lastname, string login, string password, CuratorEngine curatorEngine)
        {
            curatorEngine.Registy(firstname, lastname, "АВТ-613", "АВТФ", 0, login, password);
        }
    }

        class Dialog_Register : DialogFragment
        {
            EditText FirstName;
            EditText LastName;
            EditText Login;
            EditText Password;
            Button Registry;

            public event EventHandler<Register> OnRegistry;

            private CuratorEngine curatorEngine = null;

            public Dialog_Register(CuratorEngine engine)
            {
                this.curatorEngine = engine;
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) //форма входа
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                var view = inflater.Inflate(Resource.Layout.DialogRegistry, container, false);

                FirstName = view.FindViewById<EditText>(Resource.Id.FirstName);
                LastName = view.FindViewById<EditText>(Resource.Id.LastName);
                Login = view.FindViewById<EditText>(Resource.Id.LoginReg);
                Password = view.FindViewById<EditText>(Resource.Id.PasswordReg);
                Registry = view.FindViewById<Button>(Resource.Id.Registry);

                Registry.Click += RegistryClick;

                return view;
            }

            public void RegistryClick(object sender, EventArgs e)
            {
                OnRegistry.Invoke(this, new Register(FirstName.Text, LastName.Text, Login.Text, Password.Text, curatorEngine));
                this.Dismiss();
            }
        }
}