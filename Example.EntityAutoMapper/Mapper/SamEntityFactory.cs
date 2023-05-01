using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Example.EntityAutoMapper.Mapper
{
    public class SamEntityFactory
    {
        private DbContext _context;
        private Dictionary<Type, SamEntityInfo> _entities;

        public DbContext Context
        {
            get => _context;
            set => _context = value;
        }

        public SamEntityFactory(List<SamEntityInfo> entities)
        {
            _entities = entities.ToDictionary(a => a.EntityType, b => b);
        }

        public SamDbSet<Tentity> CreateSet<Tentity>()
        {
            if (_entities.TryGetValue(typeof(Tentity), out var entityInfo))
            {
                return (SamDbSet<Tentity>)entityInfo.SetInitializer(_context);
            }

            throw new InvalidOperationException($"Entity '{typeof(Tentity).FullName}' is unknown.");
        }

        public Tentity Create<Tentity>(Action<Tentity> onInitialize = null)
        {
            if (_entities.TryGetValue(typeof(Tentity), out var entityInfo))
            {
                var entity = (Tentity)entityInfo.EntityInitializer();
                if (onInitialize != null)
                {
                    onInitialize(entity);
                }
                return entity;
            }

            throw new InvalidOperationException($"Entity '{typeof(Tentity).FullName}' is unknown.");
        }

        public IEnumerable<SamEntityInfo> GetEntities()
        {
            return _entities.Values;
        }
    }
}
