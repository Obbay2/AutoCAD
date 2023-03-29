using System.Windows;

namespace CADApp
{
    /// <summary>
    /// Interaction logic for DetailsDialog.xaml
    /// </summary>
    public partial class DetailsDialog : Window
    {
        public DetailsDialog(ObjectInformation obj)
        {
            this.DataContext = obj;
            InitializeComponent();
        }
    }
}
