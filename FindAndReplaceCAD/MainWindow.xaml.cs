using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CADApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: Add background mask
        // TODO: Apply same width to all selected boxes
        public ObservableCollection<ObjectInformation> Test { get; set; } = new ObservableCollection<ObjectInformation>(CADUtil.ReadCADItems());

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
                File.WriteAllLines(saveFileDialog.FileName, Test.Select(item => item.ToString()));
            }
        }

        public void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV File (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                Test.Clear(); // Clear active list of items
                ReadFileToList(openFileDialog.FileName, Test);
            }
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

        private void ReadFileToList(string filePath, IList<ObjectInformation> textFound)
        {
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] fields = line.Trim().Split(',');
                textFound.Add(new ObjectInformation(fields[0], fields[1].Substring(1, fields[1].Length - 2), fields[2].Substring(1, fields[1].Length - 2)));
            }
        }     
    }
}
