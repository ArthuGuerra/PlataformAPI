using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.Interfaces
{
    public interface IRepository<T>  where T: class
    {
        public Task<ICollection<T>> GetAllAsync();
        public Task<T> GetIdAsync(int id);
        public T Create(T entity);
        public T Update(T entity);
        public T Delete(T entity);
    }
}
