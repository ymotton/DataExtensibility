using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class TableMappings
    {
        [DataMember]
        public List<TableMapping> Mappings { get; set; }

        public TableMappings()
        {
            Mappings = new List<TableMapping>();
        }

        public void Add(TableMapping mapping)
        {
            Mappings.Add(mapping);
        }
    }
}
