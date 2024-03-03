using System.Configuration;

namespace MNPZ.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
    }
}
