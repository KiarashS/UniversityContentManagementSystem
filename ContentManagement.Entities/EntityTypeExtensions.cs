using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    public static class EntityTypeExtensions
    {
        public static ModelBuilder UseEntities(this ModelBuilder modelBuilder)
        {
            IEnumerable<Type> entityTypes;
            var asm = Assembly.GetExecutingAssembly();

            entityTypes = asm.GetExportedTypes()
                .Where(x => !x.IsInterface
                    && !x.IsAbstract
                    && x.IsClass
                    && (x.GetInterfaces().Any(y => (y == typeof(IEntityType))))).ToList();

            var entityMethod = typeof(ModelBuilder).GetMethods().First(m => m.Name == "Entity" && m.IsGenericMethodDefinition);
            foreach (dynamic entity in entityTypes)
            {
                entityMethod.MakeGenericMethod(entity).Invoke(modelBuilder, new object[] { });
            }
            
            return modelBuilder;
        }
    }
}
