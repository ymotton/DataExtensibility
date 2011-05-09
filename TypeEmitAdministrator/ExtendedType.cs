using System;

namespace TypeEmitAdministrator
{
    public class ExtendedType
    {
        public Type BaseType { get; private set; }
        
        public ExtendedType(Type baseType)
        {
            BaseType = baseType;
        }
    }
}
