using System;
using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace CoreDdd.Infrastructure.Conventions
{
    public class CustomForeignKeyConvention : ForeignKeyConvention
    {
        protected override string GetKeyName(Member property, Type type)
        {
            return type.Name + "Id";
        }
    }
}