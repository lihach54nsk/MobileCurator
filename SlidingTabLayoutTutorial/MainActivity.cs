using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XamarinSandbox.Engine.Authorization;
using XamarinSandbox.Engine;
using System.Collections.Generic;

namespace SlidingTabLayoutTutorial
{
    [Activity(Label = "Sliding Tab Layout", MainLauncher = true, Icon = "@drawable/xs")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            CuratorEngine curatorEngine = new CuratorEngine(ExternalCacheDir.AbsolutePath, CacheDir.AbsolutePath, ExternalCacheDir.AbsolutePath, 10);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SlidingTabsFragment fragment = new SlidingTabsFragment(curatorEngine);
            transaction.Replace(Resource.Id.sample_content_fragment, fragment);
            transaction.Commit();

            curatorEngine.Registy("Alexey", "Luzyanin", "AVT-613", "AVTF", Curator.AccessLevel.admin, "dota", "virtuspro");

            curatorEngine.LoginCurator("dota", "virtuspro");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.actionbar_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }       
    }
}