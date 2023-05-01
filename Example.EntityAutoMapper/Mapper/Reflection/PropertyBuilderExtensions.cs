using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Example.EntityAutoMapper.Mapper.Reflection
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder SetCustomAttribute(this PropertyBuilder propertyBuilder, Type attributeType)
        {
            if (propertyBuilder == null)
            {
                throw new ArgumentNullException(nameof(propertyBuilder));
            }
            if (attributeType == null)
            {
                throw new ArgumentNullException(nameof(attributeType));
            }
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new ArgumentException("The specified type is not an attribute.", nameof(attributeType));
            }

            ConstructorInfo attributeConstructor = attributeType.GetConstructor(Type.EmptyTypes);
            if (attributeConstructor == null)
            {
                throw new ArgumentException("The specified type does not have a parameterless constructor.", nameof(attributeType));
            }

            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(attributeConstructor, new object[] { });
            propertyBuilder.SetCustomAttribute(attributeBuilder);
            return propertyBuilder;
        }
    }
}
