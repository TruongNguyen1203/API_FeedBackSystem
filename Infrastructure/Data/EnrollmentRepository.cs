using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
         private readonly StoreContext _context;
        public EnrollmentRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
    }
}