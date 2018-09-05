using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.App.FragmentManager;
using Android.Support.V4.View;
using XamarinSandbox;
using XamarinSandbox.Engine;

namespace SlidingTabLayoutTutorial
{
    public class SlidingTabsFragment : Fragment
    {
        private SlidingTabScrollView mSlidingTabScrollView;
        private ViewPager mViewPager;

        public static CuratorEngine curatorEngine = null;

        public SlidingTabsFragment(CuratorEngine engine)
        {
            curatorEngine = engine;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_sample, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            mSlidingTabScrollView = view.FindViewById<SlidingTabScrollView>(Resource.Id.sliding_tabs);
            mViewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);
            mViewPager.Adapter = new SamplePagerAdapter();

            mSlidingTabScrollView.ViewPager = mViewPager;
        }

        public class SamplePagerAdapter : PagerAdapter
        {
            List<string> items = new List<string>();

            public SamplePagerAdapter() : base() //констуктор меню
            {
                items.Add("Главные новости");                
                items.Add("Личный кабинет");
                items.Add("FAQ");
            }
            
            public override int Count //количество Layout-oв
            {
                get { return items.Count; }
            }

            public override bool IsViewFromObject(View view, Java.Lang.Object obj)
            {
                return view == obj;
            }

            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position) //что отображается на Layout-e
            {
                View view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.pager_item, container, false); // Layout для news и FAQ
                View view1 = LayoutInflater.From(container.Context).Inflate(Resource.Layout.autorization_page, container, false); // Layout для кнопок авторизации
                int pos = position + 1;

                CuratorEngine curatorEngine = SlidingTabsFragment.curatorEngine;

                News news1 = new News("Новость 1" + "\n", "Строка 1" + "\n");
                News news2 = new News("Новость 2" + "\n", "Строка 2" + "\n");
                News news3 = new News("Новость 3" + "\n", "Строка 3" + "\n");

                curatorEngine.SendNews(news1);
                curatorEngine.SendNews(news2);
                curatorEngine.SendNews(news3);

                curatorEngine.LoadNextNews();

                List<News> list = new List<News> { news1, news2, news3 };

                if (pos == 1) //1-ый Layout(News)
                {
                    container.AddView(view);

                    TextView txtTitle = view.FindViewById<TextView>(Resource.Id.item_subtitle);
                    TextView txtDate = view.FindViewById<TextView>(Resource.Id.textViewDate);
                    TextView txtMain = view.FindViewById<TextView>(Resource.Id.textViewMainText);

                    foreach (News a in list)
                    {
                        txtTitle.Text += a.Title.ToString();
                        txtDate.Text += a.DateTime.ToString();
                        txtMain.Text += a.MainText.ToString();
                    }

                    return view;
                }
                else
                { 
                    if (pos == 2) //2-ый Layout(LK)
                    {
                        //кнопки для входа
                        container.AddView(view1);

                        Button SignIn = view1.FindViewById<Button>(Resource.Id.buttonSignIn);
                        Button Register = view1.FindViewById<Button>(Resource.Id.buttonRegister);

                        SignIn.Click += (object sender, EventArgs args) =>
                        {
                            FragmentManager fragmentManager = null;
                            FragmentTransaction transaction = fragmentManager.BeginTransaction();
                            Dialog_Sign_In dialog_Sign_In = new Dialog_Sign_In(curatorEngine);
                            dialog_Sign_In.Show(transaction,"Platyoj proshol");
                        };

                        Register.Click += (object sender, EventArgs args) =>
                        {
                            FragmentManager fragmentManager = null;
                            FragmentTransaction transaction = fragmentManager.BeginTransaction();
                            Dialog_Register dialog_Register = new Dialog_Register(curatorEngine);
                            dialog_Register.Show(transaction, "Platyoj proshol");
                        };

                        return view1;
                    }
                    else //3-ый Layout(FAQ)
                    {
                        container.AddView(view);

                        TextView txtTitle = view.FindViewById<TextView>(Resource.Id.item_subtitle);
                        TextView txtDate = view.FindViewById<TextView>(Resource.Id.textViewDate);
                        TextView txtMain = view.FindViewById<TextView>(Resource.Id.textViewMainText);

                        txtTitle.Text = news2.Title.ToString();
                        txtDate.Text = news2.DateTime.ToString();
                        txtMain.Text = news2.MainText.ToString();

                        return view;
                    }
                }
            }           

            public string GetHeaderTitle (int position) //номер Layout-a
            {
                return items[position];
            }

            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj) //уничтожение объекта
            {
                container.RemoveView((View)obj);
            }
        }
    }
}