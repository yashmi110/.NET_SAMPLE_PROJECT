using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Repositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly Dictionary<int, T> _entities = new Dictionary<int, T>();
        private int _nextId = 1;

        public void Add(T entity)
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
        }

        public T GetById(int id)
        {
            return _entities.TryGetValue(id, out var entity) ? entity : null;
        }

        public List<T> GetAll()
        {
            return _entities.Values.ToList();
        }

        public void Update(T entity)
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
        }

        public void Delete(int id)
        {
            _entities.Remove(id);
        }
    }
}
