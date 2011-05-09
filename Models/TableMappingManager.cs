using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Models
{
    public static class TableMappingManager
    {
        private readonly static TableMappings TableMappings;
        private readonly static IDictionary<Type, TableMapping> BaseToTableMapping;
        private readonly static IDictionary<Type, Type> BaseToExtended;

        static TableMappingManager()
        {
            TableMappings = DeserializeMappings(Properties.Settings.Default.CustomModelAssembly);
            BaseToTableMapping = new ConcurrentDictionary<Type, TableMapping>(
                       from tableMapping in TableMappings.Mappings
                       from typeMapping in tableMapping.Mappings
                       select new KeyValuePair<Type, TableMapping>(Type.GetType(typeMapping.BaseType + ", " + typeMapping.BaseAssembly), tableMapping)
                );
            BaseToExtended = new ConcurrentDictionary<Type, Type>(
                       from tableMapping in TableMappings.Mappings
                       from typeMapping in tableMapping.Mappings
                       select new KeyValuePair<Type, Type>(Type.GetType(typeMapping.BaseType + ", " + typeMapping.BaseAssembly), Type.GetType(typeMapping.ExtendedType + ", " + typeMapping.ExtendedAssembly))
                );
        }

        public static Type GetExtendedTypeOrDefault(Type baseType)
        {
            Type extendedType;
            return BaseToExtended.TryGetValue(baseType, out extendedType) ? extendedType : baseType;
        }
        public static TableMapping GetTableMapping(Type baseType)
        {
            TableMapping tableMapping;
            return BaseToTableMapping.TryGetValue(baseType, out tableMapping) ? tableMapping : null;
        }

        private static TableMappings DeserializeMappings(string assemblyName)
        {
            using (var reader = XmlReader.Create(assemblyName + ".xml"))
            {
                var serializer = new DataContractSerializer(typeof(TableMappings));
                return (TableMappings)serializer.ReadObject(reader);
            }
        }
    }
}
