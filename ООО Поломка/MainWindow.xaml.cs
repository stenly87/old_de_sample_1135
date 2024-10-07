using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ООО_Поломка.Views;

namespace ООО_Поломка
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        static internal MainWindow instance;

        private Page currentPage;

        public Page CurrentPage { 
            get => currentPage;
            set
            {
                currentPage = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(CurrentPage)));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            DataContext = this;

            CurrentPage = new ClientPage();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}