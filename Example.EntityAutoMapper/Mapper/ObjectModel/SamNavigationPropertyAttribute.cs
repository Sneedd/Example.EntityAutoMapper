using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.EntityAutoMapper.Mapper.ObjectModel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SamNavigationPropertyAttribute : Attribute
    {
        public SamNavigationPropertyAttribute(Type type, string foreignKeyPropertyName)
        {
        }
    }
}
