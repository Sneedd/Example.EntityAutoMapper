using System.Linq.Expressions;

namespace Example.EntityAutoMapper.Mapper
{
    public interface ISamDbSetContainer<Tentity>
    {
        Type ElementType { get; }

        Expression Expression { get; }

        IQueryProvider Provider { get; }

        IEnumerator<Tentity> GetEnumerator();

        void Add(Tentity entity);
    }
}