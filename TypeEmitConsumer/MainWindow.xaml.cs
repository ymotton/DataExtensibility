using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TypeEmitConsumer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(this);
        }

        #endregion

        #region IMainWindow Members

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void AddColumnToGrid(string propertyPath)
        {
            var column = new DataGridTextColumn
            {
                Header = propertyPath,
                Binding = new Binding(propertyPath)
            };
            _dataGrid.Columns.Add(column);
        }

        #endregion
    }
}