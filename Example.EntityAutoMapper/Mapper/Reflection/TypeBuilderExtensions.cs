using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Example.EntityAutoMapper.Mapper.Reflection
{
    public static class TypeBuilderExtensions
    {
        public static void SetDebuggerDisplay(this TypeBuilder typeBuilder, Type type)
        {
            if (typeBuilder == null)
            {
                throw new ArgumentNullException(nameof(typeBuilder));
            }

            var debuggerDisplay = type.Name;
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(
                typeof(DebuggerDisplayAttribute).GetConstructor(new Type[] { typeof(string) }),
                new object[] { debuggerDisplay }));
        }

        public static PropertyBuilder DefineDefaultProperty(this TypeBuilder typeBuilder, PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var methodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
                                   MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

            var field = typeBuilder.DefineField("_" + property.Name, property.PropertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);

            var getMethod = typeBuilder.DefineMethod("get_" + property.Name, methodAttributes, property.PropertyType, Type.EmptyTypes);
            propertyBuilder.SetGetMethod(getMethod);

            var setMethod = typeBuilder.DefineMethod("set_" + property.Name, methodAttributes, typeof(void), new Type[] { property.PropertyType });
            propertyBuilder.SetSetMethod(setMethod);

            var getterGen = getMethod.GetILGenerator();
            getterGen.Emit(OpCodes.Ldarg_0);
            getterGen.Emit(OpCodes.Ldfld, field);
            getterGen.Emit(OpCodes.Ret);

            var setterGen = setMethod.GetILGenerator();
            setterGen.Emit(OpCodes.Ldarg_0);
            setterGen.Emit(OpCodes.Ldarg_1);
            setterGen.Emit(OpCodes.Stfld, field);
            setterGen.Emit(OpCodes.Ret);

            return propertyBuilder;
        }
    }
}
