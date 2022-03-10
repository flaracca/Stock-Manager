using Honeywell.AIDC.CrossPlatform;
using Newtonsoft.Json;
using Stock_Manager.Classes;
using Stock_Manager.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace Stock_Manager.Views
{
    public partial class NewItemPage : ContentPage
    {

        //BarcodeReader newItemBarcodeReader = new BarcodeReader();
        CaricoScaricoViewModel viewModel;
        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();

            MessagingCenter.Subscribe<CaricoScaricoViewModel, string>("Attenzione", "Inserimento Nuovo Articolo", async (sender, arg) =>
            {

                try
                {
                    await DisplayAlert("", arg, "OK");
                }
                catch (Exception ex)
                {

                }


            });

            MessagingCenter.Subscribe<CaricoScaricoViewModel, string>("Attenzione", "RimuoviPagina", async (sender, arg) =>
            {

                Device.BeginInvokeOnMainThread(async () =>
                            //await Shell.Current.GoToAsync("..")                            
                            await Navigation.PopModalAsync()
                        ); ;


            });

            App.barcodeReader.BarcodeDataReady += NewItemBarcodeReader_BarcodeDataReady;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ToolbarItem item = Constants.GetConnectionIcon();
            
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(item);

            App.CurrentPage = nameof(NewItemPage);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        
        
        private void NewItemBarcodeReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {

            if (App.CurrentPage == nameof(NewItemPage))
            {
                if (skuInterno.IsFocused)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        skuInterno.Text = e.Data;
                    });
                }

                if (skuFornitore.IsFocused)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        skuFornitore.Text = e.Data;
                    });
                }

                

                if (contieneSkuFornitore.IsFocused)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        //contieneSkuFornitore.Text = e.Data;
                        searchSKU(e.Data);
                    });
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
                    this.IsBusy.IsVisible = true;
                });

                StockArticolo a = new StockArticolo();

                var RestURL = Constants.MainUrl + "Stock/SearchSku/" + skuToSend;

                

                try
                {
                    string webResponse = await App.RestService.PostResponse(RestURL);



                    esito = JsonConvert.DeserializeObject<Esito>(webResponse);

                    a = esito.dynamic;

                    if (a.ArticoloId == 0) // articolo non esiste
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {

                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await DisplayAlert("Attenzione Recupero SKU", skuToSend + " non è presente nel database. Inserire prima questo prodotto e poi la confezione.", "OK");
                            });


                        });
                    }
                    else
                    {

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            contieneSkuFornitore.Text = skuToSend;
                        });

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                   
                }
                


                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.IsBusy.IsVisible = false;


                });
                App.contatoreBarcode = 0;
            }
        }
    }
}