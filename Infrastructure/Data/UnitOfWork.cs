using Core.Interfaces;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _db;        
    

        public UnitOfWork(StoreContext db)
        {
            _db = db;
            Class = new ClassRepository(_db);

        }
         public IClassRepository Class { get; private set; }

    

        public void Dispose()
        {
             _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}