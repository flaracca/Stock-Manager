using Stock_Manager.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Stock_Manager.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}