using System.Collections.Generic;

namespace Models
{
    public class ChangeTracker
    {
        private IDictionary<string, object> _properties;

        public ChangeTracker()
        {
            _properties = new Dictionary<string, object>();
        }

        public void RegisterOriginalValue(string propertyName, object value)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                _properties.Add(propertyName, value);
            }
        }
    }
}
