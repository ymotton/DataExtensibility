using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Models
{
    public abstract class BusinessObject<T> : IObjectWithChangeTracker, INotifyPropertyChanged
        where T : class, new()
    {
        public static T Create()
        {
            return (T)Activator.CreateInstance(TableMappingManager.GetExtendedTypeOrDefault(typeof(T)));
        }

        protected TProperty GetProperty<TProperty>(string propertyName, TProperty propertyField)
        {
            return propertyField;
        }
        protected void SetProperty<TProperty>(string propertyName, ref TProperty propertyField, TProperty newValue)
        {
            if (EqualityComparer<TProperty>.Default.Equals(propertyField, newValue))
            {
                return;
            }

            ChangeTracker.RegisterOriginalValue(propertyName, propertyField);

            propertyField = newValue;

            RaisePropertyChanged(propertyName);
        }

        #region IObjectWithChangeTracker Members

        private ChangeTracker _changeTracker;
        public ChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ChangeTracker();
                }

                return _changeTracker;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
