using System;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
         IClassRepository Class { get; }
         IEnrollmentRepository Enrollment {get;}
         void Save();
    }
}