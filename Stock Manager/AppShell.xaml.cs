using Stock_Manager.ViewModels;
using Stock_Manager.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stock_Manager
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(CaricoScarico), typeof(CaricoScarico));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
        }

    }
}
