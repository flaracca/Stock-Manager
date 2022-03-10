using System;
using System.Collections.Generic;
using System.Text;

namespace Stock_Manager.Classes
{
    public class StockMovimento
    {
        public int Movimentoid { get; set; }
        public int ArticoloId { get; set; }
        public int Qta { get; set; }
        public DateTime Data { get; set; }
        public string Operatore { get; set; }

        public StockMovimento() {  }
    }
}
