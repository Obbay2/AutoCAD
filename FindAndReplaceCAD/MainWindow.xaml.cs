using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CADApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // TODO: Apply same width to all selected boxes
        private ObservableCollection<ObjectInformation> Test { get; set; } = new ObservableCollection<ObjectInformation>();

        public ICollectionView Texts { get; set; }

        private bool _showText = true;
        public bool ShowText { get { return _showText; } set { _showText = value; Texts?.Refresh(); } }

        private bool _showMText = true;
        public bool ShowMText { get { return _showMText; } set { _showMText = value; Texts?.Refresh(); } }

        private bool _showMLeader = true;
        public bool ShowMLeader { get { return _showMLeader; } set { _showMLeader = value; Texts?.Refresh(); } }

        private bool _showDimension = true;
        public bool ShowDimension { get { return _showDimension; } set { _showDimension = value; Texts?.Refresh(); } }

        private string _visibility = System.Windows.Visibility.Collapsed.ToString();
        public string HasAnySelection
        {
            get
            {
                return _visibility;
            }
            set
            {
                if (value != _visibility)
                {
                    _visibility = value;
                    NotifyPropertyChanged(nameof(HasAnySelection));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            Texts = CollectionViewSource.GetDefaultView(Test);
            Texts.Filter = FindFilter;
            refreshData();

            DataContext = this;
            InitializeComponent();
        }

        public void btnMask_Click(object sender, RoutedEventArgs e)
        {
            foreach (ObjectInformation item in Test.Where(item => item.IsSelected && item.CanBeMasked))
            {
                item.NewMask = true;
            }
        }

        public void btnUnmask_Click(object sender, RoutedEventArgs e)
        {
            foreach (ObjectInformation item in Test.Where(item => item.IsSelected && item.CanBeMasked))
            {
                item.NewMask = false;
            }
        }

        public void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            
            if (messageBoxResult == MessageBoxResult.Yes) {
                CADUtil.WriteCADItems(Test.Where(item => item.IsSelected));
            }
        }

        public void btnStrip_Click(object sender, RoutedEventArgs e)
        {
            foreach(ObjectInformation item in Test.Where(item => item.IsSelected))
            {
                item.NewText = item.OriginalText.Replace("\n", " ");
            }
        }

        private void btnJumpTo_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ObjectInformation info = button.DataContext as ObjectInformation;
            CADUtil.MoveViewPort(info.Id);
        }

        public void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                refreshData();
            }
        }

        private bool FindFilter(object item)
        {
            bool shouldShow = true;
            ObjectInformation objInfo = item as ObjectInformation;

            if (objInfo.Type == typeof(MText))
            {
                return ShowMText;
            }

            if (objInfo.Type == typeof(MLeader))
            {
                return ShowMLeader;
            }

            if (objInfo.Type == typeof(DBText))
            {
                return ShowText;
            }

            if (objInfo.Type == typeof(Dimension))
            {
                return ShowDimension;
            }

            return shouldShow;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            HasAnySelection = Test.Count(item => item.IsSelected) > 1 ? 
                System.Windows.Visibility.Visible.ToString() : System.Windows.Visibility.Collapsed.ToString();
        }

        private void refreshData()
        {
            Test.Clear();
            foreach (var item in CADUtil.ReadCADItems())
            {
                Test.Add(item);
            }
        }
    }
}
