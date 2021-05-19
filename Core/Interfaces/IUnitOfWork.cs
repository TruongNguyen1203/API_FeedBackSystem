using System;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
         IClassRepository Class { get; }
         void Save();
    }
}