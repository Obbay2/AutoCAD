using Autodesk.AutoCAD.DatabaseServices;
using System;
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
    public partial class MainWindow : Window
    {
        // TODO: Add background mask
        // TODO: Apply same width to all selected boxes
        private ObservableCollection<ObjectInformation> Test { get; set; } = new ObservableCollection<ObjectInformation>(CADUtil.ReadCADItems());

        public ICollectionView Texts { get; set; } 
        public string FilterText { get; set; } = "";
        public bool ShowText { get; set; } = true;
        public bool ShowMText { get; set; } = true;
        public bool ShowMLeader { get; set; } = true;
        public bool ShowDimension { get; set; } = true;

        public MainWindow()
        {     
            InitializeComponent();
            Texts = CollectionViewSource.GetDefaultView(Test);
            Texts.Filter = FindFilter;
            DataContext = this;
        }

        public void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            CADUtil.WriteCADItems(Test);
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

        private bool FindFilter(object item)
        {
            bool shouldShow = true;
            ObjectInformation objInfo = item as ObjectInformation;

            if (objInfo.Type == typeof(MText))
            {
                shouldShow &= ShowMText;
            }

            if (objInfo.Type == typeof(MLeader))
            {
                shouldShow &= ShowMLeader;
            }

            if (objInfo.Type == typeof(DBText))
            {
                shouldShow &= ShowText;
            }

            if (objInfo.Type == typeof(Dimension))
            {
                shouldShow &= ShowDimension;
            }

            return shouldShow &= objInfo.OriginalText.Contains(FilterText);
        }

        private void findButton_Click(object sender, RoutedEventArgs e)
        {
            Texts.Refresh();
        }
    }
}
