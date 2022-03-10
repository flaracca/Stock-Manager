using SQLite;

namespace Stock_Manager.Classes
{
    public class Opzioni
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Valore { get; set; }
    }
}
