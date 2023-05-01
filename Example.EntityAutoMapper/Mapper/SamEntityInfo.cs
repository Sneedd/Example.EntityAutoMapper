using Microsoft.EntityFrameworkCore;

namespace Example.EntityAutoMapper.Mapper
{
    public class SamEntityInfo
    {
        public Type ConcreteType { get; internal set; }
        public Type EntityType { get; internal set; }
        public Func<DbContext, object> SetInitializer { get; internal set; }
        public Func<object> EntityInitializer { get; internal set; }
    }
}