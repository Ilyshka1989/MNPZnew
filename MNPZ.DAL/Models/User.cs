namespace MNPZ.DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsOperator { get; set; } = false;
    }
}
