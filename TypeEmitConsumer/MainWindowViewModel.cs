using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Data;
using Models.DataExtensibilityModel;

namespace TypeEmitConsumer
{
    public class MainWindowViewModel : NotifyableObject
    {
        #region Fields

        private DataExtensibilityContext _context;
        private Repository<Customer> _repository;
        private string _propertyPath;
        private IMainWindow _mainWindow;

        #endregion

        #region Properties

        private List<Customer> _extendedCustomers;
        public List<Customer> ExtendedCustomers
        {
            get { return _extendedCustomers; }
            set { _extendedCustomers = value; RaisePropertyChanged("ExtendedCustomers"); }
        }

        private Customer _extendedCustomer;
        public Customer ExtendedCustomer
        {
            get { return _extendedCustomer; }
            set { _extendedCustomer = value; RaisePropertyChanged("ExtendedCustomer"); }
        }

        private IEnumerable<PropertyInfo> _properties;
        public IEnumerable<PropertyInfo> Properties
        {
            get { return _properties; }
            set 
            {
                SelectedProperty = null;
                _properties = value; RaisePropertyChanged("Properties");
            }
        }

        private PropertyInfo _selectedProperty;
        public PropertyInfo SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                _selectedProperty = value;

                if (_selectedProperty != null)
                {
                    _propertyPath += ((_propertyPath != null) ? "." : "") + _selectedProperty.Name;
                    Properties = _selectedProperty.PropertyType.GetProperties().ToList();
                }

                RaisePropertyChanged("SelectedProperty");
            }
        }

        public string ColumnName { get; set; }

        public ICommand GetCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand AddColumnCommand { get; set; }

        #endregion

        #region Constructor

        public MainWindowViewModel(IMainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            GetCommand = new RelayCommand(Get);
            SaveCommand = new RelayCommand(Save);
            RefreshCommand = new RelayCommand(Refresh);
            AddColumnCommand = new RelayCommand(AddColumn);
        }

        #endregion

        #region Methods

        private void Get()
        {
            SetProperties();

            _context = new DataExtensibilityContext();
            _repository = new Repository<Customer>(_context);

            ExtendedCustomers = _repository.GetAll().ToList();
        }
        private void Save()
        {
            _repository.UpdateBatch(ExtendedCustomers);
            _mainWindow.ShowMessage(_context.SaveChanges().ToString());
        }
        private void Refresh()
        {
            ExtendedCustomers = new List<Customer>();
        }
        private void AddColumn()
        {
            _mainWindow.AddColumnToGrid(_propertyPath);

            SetProperties();            
        }
        private void SetProperties()
        {
            _propertyPath = null;
            Properties = Customer.Create().GetType().GetProperties().ToList();
        }
        
        #endregion
    }
}
