using Core.Entities.Identity;

namespace Infrastructure.Data
{
    public class AuthenticationRepository:Repository<AppUser>
    {
        private readonly StoreContext _context;
        public AuthenticationRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
        
    }
}