using Newtonsoft.Json;
using Stock_Manager.Classes;
using Stock_Manager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Stock_Manager.ViewModels
{
    public partial class CaricoScaricoViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }

        #region tastiera
        /*
         * qtaDecrease
         * qtaIncrease
         * qta1
         * qtaDelete
         * qtaReset
         * qtaDelete
         * qtaReset
         */

        Command _qtaDecrease;
        Command _qtaIncrease;
        Command _qtaDelete;
        Command _qtaReset;

        Command _qta1;
        Command _qta2;
        Command _qta3;
        Command _qta4;
        Command _qta5;
        Command _qta6;
        Command _qta7;
        Command _qta8;
        Command _qta9;
        Command _qta0;

        public Command qtaDecrease { get { return _qtaDecrease ?? (_qtaDecrease = new Command(cmdBtnQtaDecrease)); } }
        void cmdBtnQtaDecrease() { 
            if (!string.IsNullOrEmpty(qta)) { 
                intQta = Convert.ToInt32(qta); 
                //intQta = (intQta > 0) ? intQta-- : intQta; 
                if (intQta > 1)
                {
                    intQta--;
                    qta = intQta.ToString();
                } else
                {
                    qta = string.Empty;
                }
                
            } 
        }
        public Command qtaIncrease { get { return _qtaIncrease ?? (_qtaIncrease = new Command(cmdBtnQtaIncrease)); } }
        void cmdBtnQtaIncrease() { if (!string.IsNullOrEmpty(qta)) { intQta = Convert.ToInt32(qta); intQta++; qta = intQta.ToString(); } else { qta = "1"; } }
        public Command qtaDelete { get { return _qtaDelete ?? (_qtaDelete = new Command(cmdBtnQtaDelete)); } }
        void cmdBtnQtaDelete() { if (qta.Length > 1) { qta = qta.Remove(qta.Length - 1); } else { qta = string.Empty; } }
        public Command qtaReset { get { return _qtaReset ?? (_qtaReset = new Command(cmdBtnQtaReset)); } }
        void cmdBtnQtaReset() { qta = string.Empty; }

        public Command qta1 { get { return _qta1 ?? (_qta1 = new Command(cmdBtnQta1)); } }
        void cmdBtnQta1() { qta = (string.IsNullOrEmpty(qta)) ? "1" : qta + "1"; }
        public Command qta2 { get { return _qta2 ?? (_qta2 = new Command(cmdBtnQta2)); } }
        void cmdBtnQta2() { qta = (string.IsNullOrEmpty(qta)) ? "2" : qta + "2"; }
        public Command qta3 { get { return _qta3 ?? (_qta3 = new Command(cmdBtnQta3)); } }
        void cmdBtnQta3() { qta = (string.IsNullOrEmpty(qta)) ? "3" : qta + "3"; }
        public Command qta4 { get { return _qta4 ?? (_qta4 = new Command(cmdBtnQta4)); } }
        void cmdBtnQta4() { qta = (string.IsNullOrEmpty(qta)) ? "4" : qta + "4"; }
        public Command qta5 { get { return _qta5 ?? (_qta5 = new Command(cmdBtnQta5)); } }
        void cmdBtnQta5() { qta = (string.IsNullOrEmpty(qta)) ? "5" : qta + "5"; }
        public Command qta6 { get { return _qta6 ?? (_qta6 = new Command(cmdBtnQta6)); } }
        void cmdBtnQta6() { qta = (string.IsNullOrEmpty(qta)) ? "6" : qta + "6"; }
        public Command qta7 { get { return _qta7 ?? (_qta7 = new Command(cmdBtnQta7)); } }
        void cmdBtnQta7() { qta = (string.IsNullOrEmpty(qta)) ? "7" : qta + "7"; }
        public Command qta8 { get { return _qta8 ?? (_qta8 = new Command(cmdBtnQta8)); } }
        void cmdBtnQta8() { qta = (string.IsNullOrEmpty(qta)) ? "8" : qta + "8"; }
        public Command qta9 { get { return _qta9 ?? (_qta9 = new Command(cmdBtnQta9)); } }
        void cmdBtnQta9() { qta = (string.IsNullOrEmpty(qta)) ? "9" : qta + "9"; }
        public Command qta0 { get { return _qta0 ?? (_qta0 = new Command(cmdBtnQta0)); } }
        void cmdBtnQta0() { qta = (string.IsNullOrEmpty(qta)) ? string.Empty : qta + "0"; }

        #endregion tastiera

        #region carico scarico
        /*
         * carico
         * scarico
         */

        Command _carico;
        Command _scarico;

        public Command carico { get { return _carico ?? (_carico = new Command(cmdBtnCarico)); } }
        public void cmdBtnCarico() { caricoScarico(1, SelectedUser.codOper); }

        public Command scarico { get { return _scarico ?? (_scarico = new Command(cmdBtnScarico)); } }
        public void cmdBtnScarico() { caricoScarico(-1, SelectedUser.codOper); }

        public async void caricoScarico(int moltiplicatore, string operatore)
        {
            await caricoScaricoAsync(moltiplicatore, operatore);
        }
        private async Task<Esito> caricoScaricoAsync(int moltiplicatore, string operatore)
        {


            IsBusy = true;

            StockMovimento sm = new StockMovimento();

            

            Esito esito = new Esito();
            string esitoMsg = string.Empty;

            int tmpQta;

            try
            {
                tmpQta = Convert.ToInt32(qta);
            }
            catch
            {
                tmpQta = 0;
            }

            if (tmpQta > 0)
            {
                try
                {
                    sm.ArticoloId = Constants.tmpArticoloId;
                    sm.Qta = Convert.ToInt32(qta) * moltiplicatore;
                    sm.Data = DateTime.Today;
                    sm.Operatore = operatore;


                    var RestURL = Constants.MainUrl + "Stock/Movimento/Add/";

                    var postContent = JsonConvert.SerializeObject(sm);
                    var jsonContent = new StringContent(postContent, Encoding.UTF8, "application/json");

                    string webResponse = await App.RestService.PostResponseWithData(RestURL, jsonContent);

                    esito = JsonConvert.DeserializeObject<Esito>(webResponse);

                    Constants.tmpArticoloId = 0;
                }
                catch (Exception ex)
                {
                    esito.Success = false;
                    esito.Message = ex.Message;
                }
            } else
            {
                esito.Success = false;
                esito.Message = "Imposta correttamente la quantità";
            }
           

            if (esito.Success == true)
            {
                resetFields();
                Device.BeginInvokeOnMainThread(() =>
                           MessagingCenter.Send(this, "resetFields", string.Empty)
                        );
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                           MessagingCenter.Send(this, "Carico Scarico", esito.Message)
                        );
            }

            IsBusy = false;
            return esito;
        }
        
        #endregion carico scarico

        #region search reset
        Command _btnResetSku;
        Command _btnSearchSku;

        public Command resetSKU
        {
            get
            {
                return _btnResetSku ?? (_btnResetSku = new Command(cmdBtnResetSku));
            }
        }

        public Command searchSKU
        {
            get
            {
                return _btnSearchSku ?? (_btnSearchSku = new Command(cmdBtnSearchSku));
            }
        }

        void cmdBtnResetSku()
        {
            sku = string.Empty;
            descrizione = string.Empty;
            qtaAttuale = string.Empty;
            Constants.tmpArticoloId = 0;
        }

        void cmdBtnSearchSku()
        {
            cercaSku(sku);
        }
        #endregion search reset

        public int articoloId;

        private ObservableCollection<User> _UserPicker;
        public ObservableCollection<User> UserPicker
        {
            get { return _UserPicker; }
            set { _UserPicker = value; OnPropertyChanged(); }
        }

        private User _SelectedUser;
        public User SelectedUser
        {
            get
            {
                if (_SelectedUser == null)
                {
                    _SelectedUser = new User();
                    _SelectedUser.codOper = string.Empty;
                }
                return _SelectedUser;
            }
            set
            {
                _SelectedUser = value;
                OnPropertyChanged(nameof(_SelectedUser));
            }
        }

        private Color _scannerStatus;
        public Color scannerStatus
        {
            get { return _scannerStatus; }
            set
            {
                _scannerStatus = value;
                OnPropertyChanged();

            }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                OnPropertyChanged();

            }
        }

        private string skuToSearch;

        private string _sku;
        public string sku
        {
            get { return _sku; }
            set
            {
                _sku = value;
                OnPropertyChanged();
                skuToSearch = _sku;
            }
        }

        

        private string _descrizione;
        public string descrizione
        {
            get { return _descrizione; }
            set
            {
                _descrizione = value;
                OnPropertyChanged();

            }
        }

        private string _qtaAttuale;
        public string qtaAttuale
        {
            get { return _qtaAttuale; }
            set
            {
                _qtaAttuale = value;
                OnPropertyChanged();

            }
        }

        private int intQta;

        private string _qta;
        public string qta
        {
            get { return _qta; }
            set
            {
                _qta = value;
                OnPropertyChanged();

            }
        }

       
        public CaricoScaricoViewModel(INavigation navigation) { 
            Navigation = navigation;
            scannerStatus = Color.Gray;
            UserPicker = new ObservableCollection<User>();
            foreach (User u in Constants.ListaPersonale) { 
                UserPicker.Add(u); 
            }
        }

        public void updateScannerStatus(string colore)
        {
            scannerStatus = (colore == "open") ? Color.Green : Color.Red;
        }
        //public CaricoScaricoViewModel(){}

        public void Test()
        {
            Device.BeginInvokeOnMainThread(() =>
                               MessagingCenter.Send(this, "Carico Scarico", sku)
                            );
        }

        public Esito cercaSku(string sku)
        {
            skuToSearch = sku;


            Esito sa = cercaSkuAsync().Result;

            return sa;
        }

        private async Task<Esito> cercaSkuAsync()
        {
            

            IsBusy = true;
            Esito esito = new Esito();

            StockArticolo a = new StockArticolo();

            var RestURL = Constants.MainUrl + "Stock/SearchSku/" + skuToSearch;

            articoloId = 0;
            descrizione = string.Empty;
            sku = string.Empty;
            qtaAttuale = string.Empty;
            qta = string.Empty;

            try
            {
                string webResponse = await App.RestService.PostResponse(RestURL);

                

                esito = JsonConvert.DeserializeObject<Esito>(webResponse);
                
                a = esito.dynamic;

                if (a.ArticoloId == 0) // articolo non esiste
                {
                    
                } else
                {
                    
                    Constants.tmpArticoloId = a.ArticoloId;
                    try
                    {
                        descrizione = a.Descrizione;
                        sku = string.Empty;
                        qtaAttuale = a.QtaAttuale.ToString();
                        qta = string.Empty;

                        
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("CaricoScaricoVM.cs -> cercaSkuAsync: " + ex.Message);
                    }
                    
                    //Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(this, "SetFocus", "hiddenSku") );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Device.BeginInvokeOnMainThread(() =>
                               MessagingCenter.Send(this, "Carico Scarico", ex.Message)
                            );
            }
            
            IsBusy = false;
            return esito;
        }

        private void resetFields()
        {
            articoloId = 0;

            Device.BeginInvokeOnMainThread(() =>
            {
                descrizione = string.Empty;
                sku = string.Empty;
                qtaAttuale = string.Empty;
                qta = string.Empty;

            });
            
        }

        
    }
}
