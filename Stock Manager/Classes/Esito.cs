using System;
using System.Collections.Generic;
using System.Text;

namespace Stock_Manager.Classes
{
    public class Esito
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int IntValue { get; set; }
        public StockArticolo dynamic { get; set; }

        public Esito() { }
    }
}
