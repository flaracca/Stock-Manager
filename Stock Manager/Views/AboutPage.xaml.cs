using Stock_Manager.Classes;
using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stock_Manager.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();

            

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Opzioni opz = App.DbController.GetOpzione("connessione");

                if (opz.Valore == "locale")
                {
                    radioLocale.IsChecked = true;
                    radioInternet.IsChecked = false;
                }
                else
                {
                    radioInternet.IsChecked = true;
                    radioLocale.IsChecked = false;
                }

               
            });

            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.CurrentPage = nameof(AboutPage);
            aggiornaIconaConnessione();
        }

        private void aggiornaIconaConnessione()
        {
            ToolbarItem item = Constants.GetConnectionIcon();
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(item);
        }
        private void radioLocale_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (radioLocale.IsChecked == true)
            {
                Constants.RootUrl = Constants.ConnessioneLocale;
                Constants.MainUrl = Constants.RootUrl + "api/";
                Opzioni opz = App.DbController.GetOpzione("connessione");

                opz.Valore = "locale";

                App.DbController.SaveOpzione(opz);

                aggiornaIconaConnessione();
            }
            
        }

        private void radioInternet_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (radioInternet.IsChecked == true)
            {
                Constants.RootUrl = Constants.ConnessioneHttp;
                Constants.MainUrl = Constants.RootUrl + "api/";
                Opzioni opz = App.DbController.GetOpzione("connessione");

                opz.Valore = "internet";

                App.DbController.SaveOpzione(opz);

                aggiornaIconaConnessione();
            }
            
        }
    }
}