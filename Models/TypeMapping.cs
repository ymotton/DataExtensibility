using System.Runtime.Serialization;
using System;

namespace Models
{
    [DataContract]
    public class TypeMapping
    {
        [DataMember]
        public string BaseType { get; set; }

        [DataMember]
        public string BaseAssembly { get; set; }

        [DataMember]
        public string ExtendedType { get; set; }

        [DataMember]
        public string ExtendedAssembly { get; set; }

        public TypeMapping(Type baseType, Type extendedType)
        {
            BaseType = baseType.FullName;
            BaseAssembly = baseType.Assembly.FullName;
            ExtendedType = extendedType.FullName;
            ExtendedAssembly = extendedType.Assembly.FullName;
        }
    }
}
