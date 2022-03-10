using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Manager.Classes
{
    public class StockArticolo
    {
        
        public int ArticoloId { get; set; }
        public string Descrizione { get; set; }
        public string SkuFornitore { get; set; }
        public string Sku { get; set; }
        public int GestionaleId { get; set; }
        public int QtaAttuale { get; set; }

        public string QtaAttualeTesto { get; set; }
        public string ContieneSku { get; set; }
        public string ContenutoInSku { get; set; }
        public int QtaContenuta { get; set; }

        public StockArticolo() { }

        
    }

   public class ListArticolo
    {
        public int Id { get; set; }
        public StockArticolo StockArticolo { get; set; }
        public int Qta { get; set; }
    }
}
