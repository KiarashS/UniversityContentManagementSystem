using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    public static class EntityTypeConfigurationExtensions
    {
        private static readonly MethodInfo entityMethod = typeof(ModelBuilder).GetTypeInfo().GetMethods().Single(x => (x.Name == "Entity") && (x.IsGenericMethod == true) && (x.GetParameters().Length == 0));

        private static Type FindEntityType(Type type)
        {
            var interfaceType = type.GetInterfaces().First(x => (x.GetTypeInfo().IsGenericType == true) && (x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));
            return interfaceType.GetGenericArguments().First();
        }

        private static readonly Dictionary<Assembly, IEnumerable<Type>> typesPerAssembly = new Dictionary<Assembly, IEnumerable<Type>>();

        //public static ModelBuilder ApplyConfiguration<T>(this ModelBuilder modelBuilder, IEntityTypeConfiguration<T> configuration) where T : class
        //{
        //    var entityType = FindEntityType(configuration.GetType());

        //    dynamic entityTypeBuilder = entityMethod
        //        .MakeGenericMethod(entityType)
        //        .Invoke(modelBuilder, new object[0]);

        //    configuration.Configure(entityTypeBuilder);

        //    return modelBuilder;
        //}

        public static ModelBuilder UseEntityTypeConfiguration(this ModelBuilder modelBuilder)
        {
            IEnumerable<Type> configurationTypes;
            var asm = Assembly.GetExecutingAssembly();//.GetEntryAssembly();

            //if (typesPerAssembly.TryGetValue(asm, out configurationTypes))
            //{
            //    typesPerAssembly[asm] = configurationTypes = asm
            //        .GetExportedTypes()
            //        .Where(x => (x.GetTypeInfo().IsClass == true) && (x.GetTypeInfo().IsAbstract == false) && (x.GetInterfaces().Any(y => (y.GetTypeInfo().IsGenericType == true) && (y.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))));
            //}
            configurationTypes = asm.GetExportedTypes()
                .Where(x => !x.IsInterface && !x.IsAbstract && x.IsClass && (x.GetInterfaces().Any(y => (y.GetTypeInfo().IsGenericType) && (y.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))))).ToList();

            foreach (dynamic configuration in configurationTypes)
            {
                modelBuilder.ApplyConfiguration(Activator.CreateInstance(configuration));
            }

            return modelBuilder;
        }
    }
}
