using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Example.EntityAutoMapper.Mapper
{
    public class SamDbContext : DbContext
    {
        private readonly SamEntityFactory _factory;

        public SamDbContext(SamEntityFactory factory)
        {
            _factory = factory;
            _factory.Context = this;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("sam", builder =>
            {
                builder.EnableNullChecks();
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in _factory.GetEntities())
            {
                modelBuilder.Entity(entity.ConcreteType);
            }
        }

        public new SamDbSet<TEntity> Set<TEntity>()
        {
            return _factory.CreateSet<TEntity>();
        }
    }
}
