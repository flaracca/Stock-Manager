
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stock_Manager.Classes
{
    public class Constants
    {
        public static string appVersion = "Ver. 1.0.0";
        public static string appVersionDate = "03.11.2021";
        public static int appVersionInt = 1;

        public static string RootUrl;
        public static string ConnessioneHttp = "http://***.***.***.***:8008/"; // Http connection
        public static string ConnessioneLocale = "http://***.***.***.***/"; // LAN connection

        public static string MainUrl = RootUrl + "api/";


        public static string PwdUrl = "******";
        public static string ApiKey = "******";

        public static string tmpSku = string.Empty;

        public static int tmpArticoloId = 0;

        public static List<User> ListaPersonale = new List<User>();

        public static ToolbarItem GetConnectionIcon()
        {
            //string icon = (Constants.RootUrl == Constants.ConnessioneLocale) ? "lan.png" : "internet.png";
            Opzioni opz = App.DbController.GetOpzione("connessione");

            string icon = (opz.Valore == "locale") ? "lan.png" : "internet.png";

            ToolbarItem item = new ToolbarItem
            {
                IconImageSource = ImageSource.FromFile(icon),
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };

            return item;
        }

        public static void GetListaUtenti()
        {
            User u;
            u = new User(); u.codOper = "AA"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "AG"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "CM"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "DB"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "EM"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "FC"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "FL"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "GC"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "LV"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "MG"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "MR"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "OM"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "RO"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "SM"; ListaPersonale.Add(u);
            u = new User(); u.codOper = "WM"; ListaPersonale.Add(u);

        }

        public static async void GetListaUtentiAsync()
        {
            var RestURL = Constants.MainUrl + "User/ListAll/";

            List<User> a = new List<User>();
            try
            {
                string webResponse = await App.RestService.PostResponse(RestURL);



                a = JsonConvert.DeserializeObject<List<User>>(webResponse);

                if (a.Count > 0) // articolo non esiste
                {
                    ListaPersonale.Clear();

                    foreach(User u in a)
                    {
                        ListaPersonale.Add(u);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Constants->GetListaUtentiAsync: " + ex.Message);
            }

        }
    }
}
