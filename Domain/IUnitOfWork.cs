using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface IUnitOfWork : NPatterns.ObjectRelational.IUnitOfWork
    {
        NPatterns.ObjectRelational.IRepository<T> Repository<T>() where T : Entity;
    }
}
