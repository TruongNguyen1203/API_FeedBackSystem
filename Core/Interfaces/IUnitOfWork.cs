using System;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
         
    
         void Save();
    }
}