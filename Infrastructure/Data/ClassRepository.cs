using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ClassRepository : Repository<Class>, IClassRepository
    {
        private readonly StoreContext _context;
        public ClassRepository(StoreContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Class @class)
        {
            _context.Update(@class);
            // var classDb = _context.Classes.FirstOrDefault(x => x.ClassID == @class.ClassID);
            // if(classDb != null)
            // {
            //     classDb.ClassName = @class.ClassName;
            //     classDb.Capacity = @class.Capacity;
            //     classDb.EndTime = @class.EndTime;
            // }
        }
    }
}