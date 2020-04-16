using CsvHelper;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
        private ObservableCollection<DisplayableObjectInformation> Test { get; set; } = new ObservableCollection<DisplayableObjectInformation>(CADUtil.ReadCADItems());
        public string FilterText { get; set; } = "";

        public MainWindow()
        {     
            InitializeComponent();
            DataContext = this;
        }

        public void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV File (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(Test);
                }
            }
        }

        public void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV File (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                using (var reader = new StreamReader(openFileDialog.FileName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    //Test = new ObservableCollection<DisplayableObjectInformation>(csv.GetRecords<DisplayableObjectInformation>());
                }
            }
        }

        public void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            CADUtil.WriteCADItems((IList<ObjectInformation>) Test);
        }

        public void btnStrip_Click(object sender, RoutedEventArgs e)
        {
            foreach(DisplayableObjectInformation item in Test.Where(item => item.IsSelected))
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

        //private bool FindFilter(object item)
        //{
        //    DisplayableObjectInformation objInfo = item as DisplayableObjectInformation;

        //    if (FilterText.Equals(""))
        //    {
        //        return true;
        //    }
        //    return objInfo.OriginalText.Contains("");
        //}
    }
}
