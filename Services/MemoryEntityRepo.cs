using System.Collections.Concurrent;
using unistream_t2.Interfaces;
using unistream_t2.Models;

namespace unistream_t2.Services
{
    // Наша реализация репозитория - в виде хранилища в памяти для тестовых целей этой работы, но в продакшене будем использовать что-то сетевое и распределенное 
    public class MemoryEntityRepo : IEntityRepo
    {
        private readonly ConcurrentDictionary<Guid, Entity> _dic = new();

        public Task<Entity> GetAsync(Guid id)
        {
            if (_dic.TryGetValue(id, out Entity entity)) return Task.FromResult(entity);
            return Task.FromResult<Entity>(null);
        }

        public Task<bool> InsertAsync(Entity entity)
        {
            return Task.FromResult(_dic.TryAdd(entity.Id, entity));
        }
    }
}