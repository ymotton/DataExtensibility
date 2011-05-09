using System.Collections.Generic;

namespace TypeEmitAdministrator
{
    public class ExtendedTable
    {
        public string BaseTableName { get; set; }

        public string TableName { get; set; }

        public List<ExtendedType> Types { get; private set; }

        public List<ExtendedProperty> ExtendedProperties { get; private set; }

        public ExtendedTable(string baseTableName, string tableName, IEnumerable<ExtendedType> types, IEnumerable<ExtendedProperty> extendedProperties)
        {
            BaseTableName = baseTableName;

            TableName = tableName;

            Types = new List<ExtendedType>(types);
            ExtendedProperties = new List<ExtendedProperty>(extendedProperties);
        }
    }
}
