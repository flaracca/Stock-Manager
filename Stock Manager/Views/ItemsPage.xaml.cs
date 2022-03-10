using Newtonsoft.Json;
using Stock_Manager.Classes;
using Stock_Manager.ViewModels;
using Stock_Manager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Honeywell.AIDC.CrossPlatform;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace Stock_Manager.Views
{
    public partial class ItemsPage : ContentPage
    {
        CaricoScaricoViewModel viewModel;
        BarcodeReader mBarcodeReaderMultiple = new BarcodeReader();

       
        public ObservableCollection<ListArticolo> ScannedArticoli { get; set; }
        public ItemsPage()
        {
            InitializeComponent();

            

            BindingContext = this;

            //creo i dati del picker User

            foreach (User u in Constants.ListaPersonale)
            {
                this.User.Items.Add(u.codOper.Trim());
            }
            App.contatoreBarcode = 0;
            App.barcodeReader.BarcodeDataReady += MultiBarcodeReaderMultiple_BarcodeDataReady;
            ScannedArticoli = new ObservableCollection<ListArticolo>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ToolbarItem item = Constants.GetConnectionIcon();
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(item);

            App.CurrentPage = nameof(ItemsPage);
        }

        #region Scanner

        
        private void MultiBarcodeReaderMultiple_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            Debug.WriteLine("****************** BarcodeDataReady(" + e.Data + ": " + App.contatoreBarcode + ") ****************** ", "Multi");

            if (App.CurrentPage == nameof(ItemsPage)) {
                if (App.contatoreBarcode == 0)
            {
                App.contatoreBarcode++;

                try
                {
                    searchSKU(e.Data);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            }
        }
        
        private async Task searchSKU(string skuToSend)
        {
            Esito esito = new Esito();

            if (!string.IsNullOrEmpty(skuToSend))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.IsBusy = true;
                });

                Task<Esito> tsa = cercaSkuAsync(skuToSend);
                esito = tsa.Result;
                StockArticolo sa = esito.dynamic;

                if (esito.Success)
                {
                    if (sa.ArticoloId == 0)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            Constants.tmpSku = skuToSend;

                            var route = $"{nameof(NewItemPage)}";
                            await Shell.Current.GoToAsync("NewItemPage");


                        });
                    }
                    else if (sa.ArticoloId > 0)
                    {

                        UpdateUI_scandata(sa);
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Attenzione Recupero SKU", esito.Message, "OK");
                        });
                    }
                } else
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Errore Recupero SKU", esito.Message, "OK");
                    });
                }
                

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.IsBusy = false;
                    App.contatoreBarcode = 0;
                });

            }
        }

        private async Task<Esito> cercaSkuAsync(string skuToSearch)
        {

            Esito a = new Esito();

            var RestURL = Constants.MainUrl + "Stock/SearchSku/" + skuToSearch;

                
            try
            {
                string webResponse = await App.RestService.PostResponse(RestURL);



                a = JsonConvert.DeserializeObject<Esito>(webResponse);

                if (a.dynamic.ArticoloId == 0) // articolo non esiste
                {
                    // apro la pagina per l'inserimento in anagrafica

                    try
                    {
                        Constants.tmpSku = skuToSearch;

                        var newPage = new NavigationPage(new NewItemPage());

                        //await Navigation.PushModalAsync(newPage);

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            
            return a;
        }
        /// <summary>
        /// This method updates any object on the user interface
        /// </summary>
        private void UpdateUI_scandata(StockArticolo sa)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (sa.ArticoloId == 0)
                {

                }
                else
                {
                    ListArticolo listArticolo = new ListArticolo();
                    if( ScannedArticoli.Count() > 0)
                    {
                        List<ListArticolo> tmpListArticolo = new List<ListArticolo>();
                        foreach (ListArticolo lisArt in ScannedArticoli)
                        {
                            tmpListArticolo.Add(lisArt);

                        }

                        //verifico se esiste già il prodotto nella lista
                        ScannedArticoli.Clear();
                        bool isNew = true;
                        foreach (ListArticolo lisArt in tmpListArticolo)
                        {
                            if (lisArt.StockArticolo.ArticoloId == sa.ArticoloId)
                            {
                                lisArt.Qta++;
                                ScannedArticoli.Insert(0, lisArt);
                                isNew = false;
                            } else
                            {
                                ScannedArticoli.Add(lisArt);
                            }
                        }

                        if (isNew)
                        {
                            listArticolo.StockArticolo = sa;
                            listArticolo.Id = 1;
                            listArticolo.Qta = 1;
                            

                            ScannedArticoli.Insert(0, listArticolo);
                        }
                    } else
                    {
                        // primo articolo scansionato
                        listArticolo.StockArticolo = sa;
                        listArticolo.Id = 1;
                        listArticolo.Qta = 1;

                        ScannedArticoli.Insert(0, listArticolo);
                    }
                    

                    
                    
                    cwlArticoli.ItemsSource = ScannedArticoli;
                }

            });

        }

        #endregion Scanner

        
        void cwlArticoliSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionData(e.PreviousSelection, e.CurrentSelection);
        }
        void UpdateSelectionData(IEnumerable<object> previousSelectedArticolo, IEnumerable<object> currentSelectedArticolo)
        {
            var selected = currentSelectedArticolo.FirstOrDefault() as ListArticolo;

            Debug.WriteLine("************ UpdateSelectionData", selected.StockArticolo.Descrizione);
            
        }
        

        void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            Console.WriteLine($"\tSwipeStarted: Direction - {e.SwipeDirection}");
        }

        void OnSwipeChanging(object sender, SwipeChangingEventArgs e)
        {
            Console.WriteLine($"\tSwipeChanging: Direction - {e.SwipeDirection}, Offset - {e.Offset}");
        }

        void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            Console.WriteLine($"\tSwipEnded: Direction - {e.SwipeDirection}");
        }

        void OnCloseRequested(object sender, EventArgs e)
        {
            Console.WriteLine("\tCloseRequested.");
        }

        

        

        async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            
            SwipeItem item = sender as SwipeItem;
            ListArticolo tmpListArticolo = item.BindingContext as ListArticolo;
            ScannedArticoli.Remove(tmpListArticolo);
            
        }

        async void OnAddSwipeItemInvoked(object sender, EventArgs e)
        {

            SwipeItem item = sender as SwipeItem;
            ListArticolo tmpLisArt = item.BindingContext as ListArticolo;

            List<ListArticolo> tmpListArticolo = new List<ListArticolo>();
            foreach (ListArticolo lisArt in ScannedArticoli)
            {
                tmpListArticolo.Add(lisArt);

            }

            //verifico se esiste già il prodotto nella lista
            ScannedArticoli.Clear();
            bool isNew = true;
            foreach (ListArticolo lisArt in tmpListArticolo)
            {
                if (lisArt.StockArticolo.ArticoloId == tmpLisArt.StockArticolo.ArticoloId)
                {
                    lisArt.Qta++;
                    ScannedArticoli.Add(lisArt);
                    
                }
                else
                {
                    ScannedArticoli.Add(lisArt);
                }
            }



        }

        async void OnRemoveSwipeItemInvoked(object sender, EventArgs e)
        {

            SwipeItem item = sender as SwipeItem;
            ListArticolo tmpLisArt = item.BindingContext as ListArticolo;

            int tmpQta = tmpLisArt.Qta;

            if (tmpQta-- > 0)
            {
                

                List<ListArticolo> tmpListArticolo = new List<ListArticolo>();
                foreach (ListArticolo lisArt in ScannedArticoli)
                {
                    tmpListArticolo.Add(lisArt);

                }

                //verifico se esiste già il prodotto nella lista
                ScannedArticoli.Clear();
                bool isNew = true;
                foreach (ListArticolo lisArt in tmpListArticolo)
                {
                    if (lisArt.StockArticolo.ArticoloId == tmpLisArt.StockArticolo.ArticoloId)
                    {
                        lisArt.Qta--;
                        ScannedArticoli.Add(lisArt);

                    }
                    else
                    {
                        ScannedArticoli.Add(lisArt);
                    }
                }

            }

            if (tmpQta-- == 0)
            {
                ScannedArticoli.Remove(tmpLisArt);
            }

            

        }

        private async void btnAzzera_Clicked(object sender, EventArgs e)
        {
            
            var action = await DisplayAlert("Azzeramento", "Confermi di azzerare la lista?", "Si", "No");
            if (action)
            {
                ScannedArticoli.Clear();
            }
        }

        

        private async void btnScarico_Clicked(object sender, EventArgs e)
        {

            var action = await DisplayAlert("Scarico", "Confermi di scaricare la lista?", "Si", "No");
            if (action)
            {
                ObservableCollection<ListArticolo> tmpScannedArticoli = new ObservableCollection<ListArticolo>();

                foreach (ListArticolo la in ScannedArticoli)
                {
                    tmpScannedArticoli.Add(la);
                }

                foreach (ListArticolo la in tmpScannedArticoli)
                {
                    Esito esito = await caricoScaricoAsync(-1, la);

                    if (esito.Success)
                    {
                        ScannedArticoli.Remove(la);
                    }
                }
            }

            
        }

        private async void btnCarico_Clicked(object sender, EventArgs e)
        {

            var action = await DisplayAlert("Carico", "Confermi di caricare la lista?", "Si", "No");
            if (action)
            {
                ObservableCollection<ListArticolo> tmpScannedArticoli = new ObservableCollection<ListArticolo>();

                foreach (ListArticolo la in ScannedArticoli)
                {
                    tmpScannedArticoli.Add(la);
                }

                foreach (ListArticolo la in tmpScannedArticoli)
                {
                    Esito esito = await caricoScaricoAsync(1, la);

                    if (esito.Success)
                    {
                        ScannedArticoli.Remove(la);
                    }
                }
            }
        }

        private async Task<Esito> caricoScaricoAsync(int moltiplicatore, ListArticolo lisArt)
        {
            StockMovimento sm = new StockMovimento();

            sm.ArticoloId = lisArt.StockArticolo.ArticoloId;
            sm.Qta = lisArt.Qta * moltiplicatore;
            sm.Data = DateTime.Today;
            try
            {
                sm.Operatore = User.SelectedItem.ToString();
            } catch{
                sm.Operatore = string.Empty;
            }
            


            var RestURL = Constants.MainUrl + "Stock/Movimento/Add/";

            var postContent = JsonConvert.SerializeObject(sm);
            var jsonContent = new StringContent(postContent, Encoding.UTF8, "application/json");

            Esito esito = new Esito();
            string esitoMsg = string.Empty;

            try
            {

                string webResponse = await App.RestService.PostResponseWithData(RestURL, jsonContent);

                esito = JsonConvert.DeserializeObject<Esito>(webResponse);
                
            }
            catch (Exception ex)
            {
                esito.Success = false;
                esito.Message = ex.Message;
            }

            
            return esito;
        }
    }
}