using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.ContextRepository
{
    public class Repositorio<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _api;

        public Repositorio(DbContext api)
        {
            _api = api;
        }



        public async Task<ICollection<T>> GetAllAsync()
        {
            var aux = new List<T>();
                       
            return await _api.Set<T>().AsNoTracking().ToListAsync();          
                           
            // se tiver mts registros devo adicionar um limite de consulta. mas.. e se eu quiser mais do q o limite ? (:
        }

        public async Task<T> GetIdAsync(int id)
        {
            return await _api.Set<T>().FindAsync(id);            
        }


        public T Create(T entity)
        {
            _api.Set<T>().Add(entity);
            return entity;                    
        }

        public T Update(T entity)
        {
            
            _api.Set<T>().Update(entity);
            return entity;
        }

        public T Delete(T entity)
        {
            _api.Set<T>().Remove(entity);
            return entity;
        }        
    }
}
