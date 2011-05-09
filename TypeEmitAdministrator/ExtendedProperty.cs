using System;

namespace TypeEmitAdministrator
{
    public class ExtendedProperty
    {
        public Type PropertyType { get; private set; }
        public string PropertyName { get; private set; }

        public ExtendedProperty(Type propertyType, string propertyName)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
        }
    }
}
