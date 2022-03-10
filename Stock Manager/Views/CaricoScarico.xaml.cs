using Honeywell.AIDC.CrossPlatform;
using Stock_Manager.Classes;
using Stock_Manager.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace Stock_Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CaricoScarico : ContentPage
    {
        CaricoScaricoViewModel viewModel;
        BarcodeReader mBarcodeReader;
        public CaricoScarico()
        {
            InitializeComponent();
            BindingContext = new CaricoScaricoViewModel(Navigation);
            viewModel = new CaricoScaricoViewModel(Navigation);
            //BindingContext = new CaricoScaricoViewModel();

            MessagingCenter.Subscribe<CaricoScaricoViewModel, string>("Attenzione", "Carico Scarico", async (sender, arg) =>
            {

                try
                {
                    await DisplayAlert("", arg, "OK");
                }
                catch (Exception ex)
                {
                    
                }


            });

            MessagingCenter.Subscribe<CaricoScaricoViewModel, string>("resetFields", "resetFields", async (sender, arg) =>
            {

                sku.Text = string.Empty;
                labelDescrizione.Text = string.Empty;
                labelQtaAttuale.Text = string.Empty;

            });

            App.barcodeReader.BarcodeDataReady += MBarcodeReader_BarcodeDataReady;
            App.contatoreBarcode = 0;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ToolbarItem item = Constants.GetConnectionIcon();
            this.ToolbarItems.Clear();
            this.ToolbarItems.Add(item);

            App.CurrentPage = nameof(CaricoScarico);
        }



        
          //lo scanner memorizza ogni scansione, così che alla terza scansione MBarcodeReader_BarcodeDataReady viene chiamato 3 volte (!).


        private void MBarcodeReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            Debug.WriteLine("****************** BarcodeDataReady(" + e.Data + ": " + App.contatoreBarcode + ") ****************** ", "CaricoScarico");
            if (App.CurrentPage == nameof(CaricoScarico)) { 
            if (App.contatoreBarcode == 0)
            {
                App.contatoreBarcode++;
                if (!sku.IsFocused)
                {
                    try
                    {
                        searchSKU(e.Data);
                        

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);

                    }
                    //throw new NotImplementedException();
                }

                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        sku.Text = e.Data;
                        
                    }); 
                    
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

                esito = this.viewModel.cercaSku(skuToSend);
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
                    
                    
                });
                App.contatoreBarcode = 0;
            }
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
                    labelDescrizione.Text = sa.Descrizione;
                    labelQtaAttuale.Text = sa.QtaAttualeTesto;
                }

                
            });
           
        }


        #region scan entry

        #endregion scan entry



        private void Eraser_Clicked(object sender, EventArgs e)
        {
            sku.Text = string.Empty;
            labelDescrizione.Text = string.Empty;
            labelQtaAttuale.Text = string.Empty;

        }

        private void btnCarico_Clicked(object sender, EventArgs e)
        {
            string u = User.SelectedItem.ToString();
            this.viewModel.caricoScarico(1, u);
        }

        private void btnScarico_Clicked(object sender, EventArgs e)
        {
            string u = User.SelectedItem.ToString();
            this.viewModel.caricoScarico(-1, u);
        }


    }
}