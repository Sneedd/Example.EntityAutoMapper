using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Example.EntityAutoMapper.Mapper.ObjectModel;
using Example.EntityAutoMapper.Mapper.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Example.EntityAutoMapper.Mapper
{
    public class SamEntityGenerator
    {
        public List<SamEntityInfo> Generate()
        {
            var entityTypes = FindEntities();

            List<SamEntityInfo> entities = new List<SamEntityInfo>();
            AssemblyName assemblyName = new AssemblyName
            {
                Name = "Example.EntityAutoMapper.Generated"
            };
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder module = assembly.DefineDynamicModule("default");

            foreach (var entityType in entityTypes)
            {
                TypeBuilder type = module.DefineType("SamEntity" + entityType.Name, TypeAttributes.Public, null, new Type[] { entityType });
                type.SetDebuggerDisplay(entityType);

                foreach (var entityProperty in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var propertyAttributes = entityProperty.GetCustomAttributes();
                    var keyAttribute = propertyAttributes.OfType<SamKeyAttribute>().FirstOrDefault();
                    if (keyAttribute != null)
                    {
                        var propertyBuilder = type.DefineDefaultProperty(entityProperty);
                        propertyBuilder.SetCustomAttribute(typeof(KeyAttribute));
                    }

                    var propertyAttribute = propertyAttributes.OfType<SamPropertyAttribute>().FirstOrDefault();
                    if (propertyAttribute != null)
                    {
                        var propertyBuilder = type.DefineDefaultProperty(entityProperty);
                        propertyBuilder.SetCustomAttribute(typeof(ColumnAttribute));
                    }
                }

                var concreteType = type.CreateType();
                entities.Add(new SamEntityInfo 
                { 
                    EntityType = entityType, 
                    ConcreteType = concreteType, 
                    SetInitializer = this.CreateSetInitializer(entityType, concreteType),
                    EntityInitializer = this.CreateEntityInitializer(concreteType)
                });
            }
            return entities;
        }

        private Func<DbContext, object> CreateSetInitializer(Type entityType, Type concreteType)
        {
            var contextArg = Expression.Parameter(typeof(DbContext), "context");

            var dbContextSetMethod = typeof(DbContext).GetMethod("Set", 1, new Type[] { }).MakeGenericMethod(concreteType);
            var dbSetType = typeof(DbSet<>).MakeGenericType(concreteType);

            var samDbSetContainerType = typeof(SamDbSetContainer<,>).MakeGenericType(entityType, concreteType);
            var samDbSetContainerCtor = samDbSetContainerType.GetConstructor(new Type[] { dbSetType });

            var isamDbSetContainerType = typeof(ISamDbSetContainer<>).MakeGenericType(entityType);

            var samDbSetType = typeof(SamDbSet<>).MakeGenericType(entityType);
            var samDbSetCtor = samDbSetType.GetConstructor(new Type[] { isamDbSetContainerType });

            var body = Expression.New(samDbSetCtor, Expression.New(samDbSetContainerCtor, Expression.Call(contextArg, dbContextSetMethod)));
            var lambda = Expression.Lambda<Func<DbContext, object>>(body, contextArg);
            return lambda.Compile();
        }

        private Func<object> CreateEntityInitializer(Type concreteType)
        {
            var entityCtor = concreteType.GetConstructor(new Type[] {  });
            var body = Expression.New(entityCtor);
            var lambda = Expression.Lambda<Func<object>>(body);
            return lambda.Compile();
        }

        private List<Type> FindEntities()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(a => a.GetCustomAttribute<SamEntityAttribute>() != null)
                .ToList();
        }
    }
}
