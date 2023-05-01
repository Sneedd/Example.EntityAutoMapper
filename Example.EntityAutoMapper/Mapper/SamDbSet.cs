using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Example.EntityAutoMapper.Mapper
{
    public class SamDbSet<Tentity> : IQueryable<Tentity>
    {
        private ISamDbSetContainer<Tentity> _container;

        public SamDbSet(ISamDbSetContainer<Tentity> container)
        {
            _container = container;
        }

        Type IQueryable.ElementType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => typeof(Tentity);
        }

        Expression IQueryable.Expression
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _container.Expression;
        }

        IQueryProvider IQueryable.Provider
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _container.Provider;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Tentity> GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Tentity entity)
        {
            _container.Add(entity);
        }
    }
}