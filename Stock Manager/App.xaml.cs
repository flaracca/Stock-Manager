using Stock_Manager.Classes;
using Stock_Manager.DataClasses;
using Stock_Manager.Views;
using System;
using Xamarin.Forms;
using Honeywell.AIDC.CrossPlatform;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Stock_Manager
{
    public partial class App : Application
    {
        static RestService restService;
        static DbController dbController;
        public static BarcodeReader barcodeReader;
        public static int contatoreBarcode;
        public static string CurrentPage;
        public App()
        {
            InitializeComponent();

            dbController = new DbController();

            Constants.GetListaUtenti();

            InizializzaScanner();

            contatoreBarcode = 0;
            MainPage = new AppShell();
        }

        

        public async Task InizializzaScanner()
        {
            barcodeReader = new BarcodeReader();

            try
            {

                BarcodeReader.Result result = await barcodeReader.OpenAsync();

                if (result.Code == BarcodeReader.Result.Codes.SUCCESS ||
                    result.Code == BarcodeReader.Result.Codes.READER_ALREADY_OPENED)
                {
                    Dictionary<string, object> settings = new Dictionary<string, object>();

                    //settings.Add(mBarcodeReader.SettingKeys.Code39Enabled, true);
                    //settings.Add(mBarcodeReader.SettingKeys.Code39CheckDigitMode, mBarcodeReader.SettingValues.Code39CheckDigitMode_NoCheck);

                    settings.Add(barcodeReader.SettingKeys.Ean13Enabled, true);
                    await barcodeReader.SetAsync(settings);



                    if (barcodeReader.IsReaderOpened)
                    {
                        Debug.WriteLine("******************  IsReaderOpened: OK ****************** ", "App");

                        BarcodeReaderBase.Result brbr = await barcodeReader.EnableAsync(true); // enable the scanner

                        if (brbr.Code == BarcodeReaderBase.Result.Codes.SUCCESS)
                        {
                            Debug.WriteLine("****************** MBarcodeReader_BarcodeDataReady ****************** ", "App");

                        }
                    }
                    else
                    {

                        Debug.WriteLine("****************** barcode IsReaderOpened: KO ****************** ", "App");
                    }


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("****************** " + ex.Message + " ***************** ", "App");
            }
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

        public static RestService RestService
        {
            get
            {
                if (restService == null)
                {
                    restService = new RestService();
                }
                return restService;
            }
        }

        public static DbController DbController
        {
            get
            {
                if (dbController == null)
                {
                    dbController = new DbController();
                }
                return dbController;
            }
        }

        public static bool IsPageAtTop(ContentPage contentPage, Type typeOfPage)
        {
            //var stack = contentPage.Navigation.NavigationStack;
            var stack = contentPage.Navigation.ModalStack;
            return (stack[stack.Count - 1].GetType() == typeOfPage);
        }
    }
}
