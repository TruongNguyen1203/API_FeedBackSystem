using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IClassRepository 
    {
        Task<IEnumerable<Class>> GetClasses();
        Task<Class> GetClassById(int id);
        Task<Class> AddClass(Class @class);
        Task<Class> UpdateClass(Class @class);
        Task<Class> DeleteClass(int Id);

    }
}