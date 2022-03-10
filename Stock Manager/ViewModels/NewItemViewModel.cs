using Newtonsoft.Json;
using Stock_Manager.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Stock_Manager.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string _skuFornitore;
        private string _skuInterno;
        private string _descrizione;
        private string _contieneSkuFornitore;
        private string _contenutoQta;
        private bool _IsBusy;
        private bool _CancelIsVisible;
        private bool _stackContiene;

        public NewItemViewModel()
        {
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            GeneraBarcodeCommand = new Command(OnGeneraBarcodeCommand);
            
            BtnShowStackContiene = new Command(OnBtnShowStackContieneCommand);

            skuFornitore = Constants.tmpSku;

            CancelIsVisible = (string.IsNullOrEmpty(Constants.tmpSku)) ? false : true;

            stackContiene = false;
        }

        public bool CancelIsVisible
        {
            get { return _CancelIsVisible; }
            set
            {
                _CancelIsVisible = value;
                OnPropertyChanged();

            }
        }

        public bool stackContiene
        {
            get { return _stackContiene; }
            set
            {
                _stackContiene = value;
                OnPropertyChanged();

            }
        }

        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                OnPropertyChanged();

            }
        }
        public string skuInterno
        {
            get => _skuInterno;
            set => SetProperty(ref _skuInterno, value);
        }

        public string skuFornitore
        {
            get => _skuFornitore;
            set => SetProperty(ref _skuFornitore, value);
        }

        public string descrizione
        {
            get => _descrizione;
            set => SetProperty(ref _descrizione, value);
        }

        public string contieneSkuFornitore
        {
            get => _contieneSkuFornitore;
            set => SetProperty(ref _contieneSkuFornitore, value);
        }

        

        public string contenutoQta
        {
            get => _contenutoQta;
            set => SetProperty(ref _contenutoQta, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command GeneraBarcodeCommand { get; }

        
        public Command BtnShowStackContiene { get; }

        private async void OnCancel()
        {
            Constants.tmpSku = string.Empty;
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            bool procedi = true;

            if (string.IsNullOrEmpty(descrizione) || (string.IsNullOrEmpty(skuInterno) && string.IsNullOrEmpty(skuFornitore)))
            {
                try
                {
                    Device.BeginInvokeOnMainThread(() =>
                                MessagingCenter.Send(this, "Inserimento Nuovo Articolo", "La descrizione ed uno SKU sono necessari per salvare un nuovo prodotto.")
                             );
                    //await pageDialogService.DisplayAlert("Inserimento Commessa", esitoMsg, "OK");
                }
                catch (Exception ex_)
                {

                }

                procedi = false;
            }

            // contiene

            // sku valorizzato ma qtà mancanti
            if ((!string.IsNullOrEmpty(contieneSkuFornitore)) && (string.IsNullOrEmpty(contenutoQta)))
            {
                try
                {
                    Device.BeginInvokeOnMainThread(() =>
                                MessagingCenter.Send(this, "Inserimento Nuovo Articolo", "Lo SKU e la quantità contenuta sono necessari per salvare un nuovo prodotto.")
                             );
                    //await pageDialogService.DisplayAlert("Inserimento Commessa", esitoMsg, "OK");
                }
                catch (Exception ex_)
                {

                }

                procedi = false;
            }

            if (procedi)
            {
                StockArticolo newItem = new StockArticolo();

                newItem.Sku = (string.IsNullOrEmpty(skuInterno)) ? string.Empty : skuInterno;
                newItem.SkuFornitore = (string.IsNullOrEmpty(skuFornitore)) ? string.Empty : skuFornitore;
                newItem.ContieneSku = (string.IsNullOrEmpty(contieneSkuFornitore)) ? string.Empty : contieneSkuFornitore;
                newItem.QtaContenuta = (string.IsNullOrEmpty(contenutoQta)) ? 0 : Convert.ToInt32(contenutoQta);
                newItem.Descrizione = descrizione;
                newItem.GestionaleId = 0;


                Esito esito = await saveAsync(newItem);

                if (esito.Success == true)
                {
                    try
                    {
                        // This will pop the current page off the navigation stack                        
                        await Shell.Current.GoToAsync("..");

                        //Device.BeginInvokeOnMainThread(() =>  MessagingCenter.Send(this, "RimuoviPagina", string.Empty) );
                    }
                    catch
                    {

                    }
                   
                    
                   
                }
                else
                {
                    try
                    {
                        Device.BeginInvokeOnMainThread(() =>
                                    MessagingCenter.Send(this, "Inserimento Nuovo Articolo", esito.Message)
                                 );
                       
                    }
                    catch (Exception ex_)
                    {

                    }
                }
            }
            
            
            
        }

        
        private async Task<Esito> saveAsync(StockArticolo s)
        {


            IsBusy = true;

            StockArticolo a = new StockArticolo();

            var RestURL = Constants.MainUrl + "Stock/Add/";

            var postContent = JsonConvert.SerializeObject(s);
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

            IsBusy = false;
            return esito;
        }

        private async void OnGeneraBarcodeCommand()
        {


            Esito esito = await GeneraBarcodeAsync();

            if (esito.Success == true)
            {
                try
                {
                    skuInterno = esito.Message;
                    // This will pop the current page off the navigation stack                        
                    await Shell.Current.GoToAsync("..");

                    //Device.BeginInvokeOnMainThread(() =>  MessagingCenter.Send(this, "RimuoviPagina", string.Empty) );
                }
                catch
                {

                }



            }
            else
            {
                try
                {
                    Device.BeginInvokeOnMainThread(() =>
                                MessagingCenter.Send(this, "Generazione Nuovo Barcode", esito.Message)
                             );

                }
                catch (Exception ex_)
                {

                }
            }

        }

        
        private async Task<Esito> GeneraBarcodeAsync()
        {

            Esito esito = new Esito();

            IsBusy = true;

            StockArticolo a = new StockArticolo();

            var RestURL = Constants.MainUrl + "Stock/NewBarcode/";

            string webResponse = await App.RestService.PostResponse(RestURL);

            esito = JsonConvert.DeserializeObject<Esito>(webResponse);

            IsBusy = false;
            return esito;
        }

        private void OnBtnShowStackContieneCommand()
        {
            stackContiene = !stackContiene;
        }
    }
}
