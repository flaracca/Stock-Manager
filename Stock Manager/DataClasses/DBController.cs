using Stock_Manager.Classes;
using Stock_Manager.Interfaces;
using System;
using System.Collections.Generic;
using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Stock_Manager.DataClasses
{
    public class DbController
    {
        static object locker = new object();
        private readonly HttpManager httpManager;



        SQLiteConnection database;
        public DbController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Opzioni>();


            httpManager = new HttpManager();

            // salvo l'opzione della connessione se non è già salvata

            Opzioni connessione = GetOpzione("connessione");

            if (connessione == null)  
            {
                connessione = new Opzioni();
                connessione.Nome = "connessione";
                connessione.Valore = "locale";
                SaveOpzione(connessione);
                Constants.RootUrl = Constants.ConnessioneLocale;

            } else
            {
                Constants.RootUrl = (connessione.Valore == "locale") ? Constants.ConnessioneLocale : Constants.ConnessioneHttp;
            }
            Constants.MainUrl = Constants.RootUrl + "api/";
        }

        public Opzioni GetOpzione(string nome)
        {
            Opzioni opz = new Opzioni();

            try
            {
                opz = database.Table<Opzioni>().Where(i => i.Nome == nome).FirstOrDefault();

            } catch
            {
                opz.Id = 0;
            }
            return opz;

        }

        public int SaveOpzione(Opzioni opzione)
        {
            if (opzione.Id != 0)
            {
                return database.Update(opzione);
            } else
            {
                return database.Insert(opzione);
            }
        }

        
    }
}
