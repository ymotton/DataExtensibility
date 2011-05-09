using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class TableMapping
    {
        [DataMember]
        public List<TypeMapping> Mappings { get; private set; }

        [DataMember]
        public string TableName { get; set; }

        public TableMapping(string tableName)
        {
            TableName = tableName;
            Mappings = new List<TypeMapping>();
        }

        public void Add(TypeMapping mapping)
        {
            Mappings.Add(mapping);
        }
    }
}
