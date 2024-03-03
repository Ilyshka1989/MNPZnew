namespace MNPZ.DAL.Models
{
    public class Rate
    {
        public int Id { get; set; }
        public decimal CurInAmount { get; set; }
        public decimal CurOutAmount { get; set; }
        public Currency CurIn { get; set; }
        public Currency CurOut { get; set; }
    }
}
