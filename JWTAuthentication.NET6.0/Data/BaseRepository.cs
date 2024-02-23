using JWTAuthentication.NET6._0.Auth;

namespace JWTAuthentication.NET6._0.Data
{
    public abstract class BaseRepository : IBaseRepository
    {
        private readonly ApplicationDbContext _dataDb;
        public BaseRepository(ApplicationDbContext dataDB)
        {
            _dataDb = dataDB;
        }

        public ApplicationDbContext DataDB { get; }

        public void SaveChanges()
        {
            _dataDb.SaveChanges();
        }
    }
}
