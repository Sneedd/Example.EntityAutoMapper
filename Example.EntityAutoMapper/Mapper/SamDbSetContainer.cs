using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Example.EntityAutoMapper.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example.EntityAutoMapper.Mapper
{
    public class SamDbSetContainer<Tentity, TconcreteEntity> : ISamDbSetContainer<Tentity>, IQueryable<TconcreteEntity>
        where TconcreteEntity : class, Tentity
    {
        private DbSet<TconcreteEntity> _set;
        private IQueryable<TconcreteEntity> _queryableSet;

        public SamDbSetContainer(DbSet<TconcreteEntity> set)
        {
            _set = set;
            _queryableSet = set;
        }

        public Type ElementType
        {
            get => typeof(TconcreteEntity);
        }

        public Expression Expression
        {
            get => _queryableSet.Expression;
        }

        public IQueryProvider Provider
        {
            get => _queryableSet.Provider;
        }

        public IEnumerator<TconcreteEntity> GetEnumerator()
        {
            return _queryableSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryableSet.GetEnumerator();
        }

        Type ISamDbSetContainer<Tentity>.ElementType
        {
            get => typeof(Tentity);
        }

        Expression ISamDbSetContainer<Tentity>.Expression
        {
            get => _queryableSet.Expression;
        }

        IQueryProvider ISamDbSetContainer<Tentity>.Provider
        {
            get => _queryableSet.Provider;
        }

        IEnumerator<Tentity> ISamDbSetContainer<Tentity>.GetEnumerator()
        {
            return _queryableSet.GetEnumerator();
        }

        public void Add(Tentity entity)
        {
            MethodInfo addMethod = _set.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(TconcreteEntity) });

            addMethod.Invoke(_set, new object[] { entity });
        }
    }
}
