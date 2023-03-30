using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CADApp
{
    /// <summary>
    /// Interaction logic for FindAndReplace.xaml
    /// </summary>
    public partial class FindAndReplace : UserControl
    {
        public event EventHandler<EventArgs.FindClickedArgs> FindChanged;

        public event EventHandler<EventArgs.ReplaceClickedArgs> ReplaceClicked;

        private string _findString;
        public string FindString
        {
            get
            {
                return _findString;
            }
            set
            {
                if (_findString != value) { 
                    _findString = value;
                    FindChanged?.Invoke(this, new EventArgs.FindClickedArgs() { FindText = FindString, IsRegex = IsRegex, IsCaseInsensitive = IsCaseInsensitive });
                }
            }
        }

        public string ReplaceString { get; set; }

        private bool _isRegex;
        public bool IsRegex {
            get
            { 
                return _isRegex;
            }
            set
            { 
                if (_isRegex != value) { 
                    _isRegex = value;
                    FindChanged?.Invoke(this, new EventArgs.FindClickedArgs() { FindText = FindString, IsRegex = IsRegex, IsCaseInsensitive = IsCaseInsensitive });
                }
            }
        }

        private bool _isCaseInsensitive;
        public bool IsCaseInsensitive {
            get
            {
                return _isCaseInsensitive;
            }
            set
            { 
                if (_isCaseInsensitive != value) { 
                    _isCaseInsensitive = value;
                    FindChanged?.Invoke(this, new EventArgs.FindClickedArgs() { FindText = FindString, IsRegex = IsRegex, IsCaseInsensitive = IsCaseInsensitive });
                }
            }
        }

        public FindAndReplace()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceClicked?.Invoke(this, new EventArgs.ReplaceClickedArgs() { FindText = FindString, ReplaceText = ReplaceString, IsRegex = IsRegex, IsCaseInsensitive = IsCaseInsensitive });
        }
    }
}
