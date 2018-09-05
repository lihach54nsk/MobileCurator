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
    class On_Sign_In : EventArgs
    {
        string Login;
        string Password;

        public string LogIn
        {
            get { return Login; }
            set { Login = value; }
        }

        public string PassWord
        {
            get { return Password; }
            set { Password = value; }
        }

        public On_Sign_In(string login, string password, CuratorEngine curatorEngine) : base() //вход для куратора
        {
            curatorEngine.LoginCurator(login, password);
        }
    }

    class Dialog_Sign_In : DialogFragment
    {
        EditText Login;
        EditText Password;
        Button SignIN;

        public event EventHandler<On_Sign_In> OnSignIn;

        private CuratorEngine curatorEngine = null;

        public Dialog_Sign_In(CuratorEngine engine)
        {
            this.curatorEngine = engine;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) //форма входа
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DialogSignIn, container, false);

            Login = view.FindViewById<EditText>(Resource.Id.Login);
            Password = view.FindViewById<EditText>(Resource.Id.Password);
            SignIN = view.FindViewById<Button>(Resource.Id.SignIN);
            SignIN.Click += SignInClick;

            return view;
        }

        public void SignInClick(object sender, EventArgs e) //нажатие кнопки входа
        {
            OnSignIn.Invoke(this, new On_Sign_In(Login.Text, Password.Text, curatorEngine));
            this.Dismiss();
        }
    }
}