using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Models;

namespace Data
{
    public static class EntityTypeConfigurationExtensions
    {
        /// <summary>
        /// TPH Customers -> CustomerExtensions
        /// http://weblogs.asp.net/manavi/archive/2010/12/28/inheritance-mapping-strategies-with-entity-framework-code-first-ctp5-part-2-table-per-type-tpt.aspx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelBuilder"></param>
        public static void Register<T>(this DbModelBuilder modelBuilder)
            where T : class
        {
            Type baseType = typeof (T);

            TableMapping tableMapping = TableMappingManager.GetTableMapping(baseType);
            if (tableMapping == null)
            {
                return;
            }

            Type extendedType = TableMappingManager.GetExtendedTypeOrDefault(baseType);

            var configuration = Entity(modelBuilder, extendedType);

            ToTable(configuration, tableMapping.TableName);
        }

        private static object Entity(DbModelBuilder modelBuilder, Type extendedType)
        {
            MethodInfo entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
            MethodInfo typeEntityMethod = entityMethod.MakeGenericMethod(new[] { extendedType });
            return typeEntityMethod.Invoke(modelBuilder, null);
        }
        private static void ToTable(object configuration, string tableName)
        {
            MethodInfo toTableMethod = configuration.GetType().GetMethods().First(mi => mi.Name == "ToTable" && !mi.IsGenericMethod);
            toTableMethod.Invoke(configuration, new[] { tableName });
        }
        private static void HasKey(object configuration, Type entityType, Type propertyType, string propertyName)
        {
            InvokeGenericMemberAccessor(configuration, entityType, "HasKey", propertyType, propertyName);
        }
        private static void Ignore(object configuration, Type entityType, Type propertyType, string propertyName)
        {
            InvokeGenericMemberAccessor(configuration, entityType, "Ignore", propertyType, propertyName);
        }
        private static void InvokeGenericMemberAccessor(object target, Type entityType, string methodName, Type propertyType, string propertyName)
        {
            ParameterExpression argumentExpression = Expression.Parameter(entityType, "x");
            PropertyInfo pi = entityType.GetProperty(propertyName);
            Expression propertyExpression = Expression.Property(argumentExpression, pi);

            InvokeGenericmethod(target, propertyType, methodName, Expression.Lambda(propertyExpression, argumentExpression));
        }
        private static void InvokeGenericmethod(object target, Type genericType, string methodName, params object[] arguments)
        {
            MethodInfo ignoreMethod = target.GetType().GetMethod(methodName).MakeGenericMethod(genericType);
            ignoreMethod.Invoke(target, arguments);
        }
    }
}
