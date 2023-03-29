using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

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

        private string _editedAttributes = "Edited 0 attributes on 0 records";
        public string EditedAttributes
        {
            get
            {
                return _editedAttributes;
            }
            set
            {
                if (value != _editedAttributes)
                {
                    _editedAttributes = value;
                    NotifyPropertyChanged(nameof(EditedAttributes));
                }
            }
        }

        public string CharacterEncodingURL
        {
            get
            {
                string currentYear = DateTime.Now.Year.ToString();
                return $"https://help.autodesk.com/view/ACD/{currentYear}/ENU/?guid=GUID-7D8BB40F-5C4E-4AE5-BD75-9ED7112E5967";
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
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Execute Confirmation", MessageBoxButton.YesNo);
            
            if (messageBoxResult == MessageBoxResult.Yes) {
                CADUtil.WriteCADItems(Test.Where(item => item.IsSelected));
            }
        }

        public void btnStrip_Click(object sender, RoutedEventArgs e)
        {
            foreach(ObjectInformation item in Test.Where(item => item.IsSelected))
            {
                item.NewText = item.OriginalText.Replace(@"\P", " ");
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
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure? All edits made so far will be discarded. Latest changes will retrieved from drawing file.", "Refresh Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                refreshData();
            }
        }

        public void revertButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure? All edits in selected rows will be discarded.", "Revert Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                revertSelectedData();
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

        private void revertSelectedData()
        {
            foreach (var item in Test.Where(item => item.IsSelected))
            {
                item.NewText = item.OriginalText;
                item.NewMask = item.OriginalMask;
            }
        }

        private void refreshData()
        {
            Test.Clear();
            foreach (var item in CADUtil.ReadCADItems())
            {
                item.EditableAttributeChanged += DataGrid_AttributesChanged;
                Test.Add(item);
            }
        }

        private void DataGrid_AttributesChanged(object sender, PropertyChangedEventArgs e)
        {
            int recordsChanged = Test.Count(item => item.AttributesChanged() > 0);
            int attributesChanged = Test.Sum(item => item.AttributesChanged());
            EditedAttributes = $"Edited {attributesChanged} attributes on {recordsChanged} records";
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            ObjectInformation objInfo = (ObjectInformation) ((Button)sender).DataContext;
            new DetailsDialog(objInfo).ShowDialog();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
