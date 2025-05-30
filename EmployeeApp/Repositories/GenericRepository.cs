using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeApp.Repositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly Dictionary<int, T> _entities = new Dictionary<int, T>();
        private int _nextId = 1;

        public async Task AddAsync(T entity)
        {
            await Task.Run(() => 
            {
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null && idProperty.CanWrite)
                {
                    idProperty.SetValue(entity, _nextId, null);
                    _entities.Add(_nextId, entity);
                    _nextId++;
                }
                else
                {
                    throw new InvalidOperationException("Entity must have a writable Id property");
                }
            });
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await Task.Run(() => _entities.TryGetValue(id, out var entity) ? entity : null);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Task.Run(() => _entities.Values.ToList());
        }

        public async Task UpdateAsync(T entity)
        {
            await Task.Run(() =>
            {
                var idProperty = entity.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    int id = (int)idProperty.GetValue(entity);
                    if (_entities.ContainsKey(id))
                    {
                        _entities[id] = entity;
                    }
                }
            });
        }

        public async Task DeleteAsync(int id)
        {
            await Task.Run(() => _entities.Remove(id));
        }
    }
}