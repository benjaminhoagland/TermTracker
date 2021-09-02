using System;
using System.IO;
using TermTracker.Services;
using TermTracker.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using TermTracker.DB;

namespace TermTracker
{
    public partial class App : Application
    {

        public App()
        {

            var database = new SQLiteConnection(Constants.DatabasePath);
            try
            {
                if(Data.NotInitialized(database))
                {
                    Data.CreateTables(database);
                    Data.Populate(database);
                }
            }
            catch
            {
                Data.CreateTables(database);
                Data.Populate(database);
            }
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new NavigationPage(new TermPage());
            
            
        }




        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }

}
