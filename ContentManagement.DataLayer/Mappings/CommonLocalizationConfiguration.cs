using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    abstract class CommonLocalizationConfiguration
    {
        public void ConfigureBase<TEntity>(EntityTypeBuilder<TEntity> builder)
            where TEntity : CommonLocalizationEntry
        {
            
        }
    }
}
