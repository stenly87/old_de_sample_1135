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
using ООО_Поломка.DB;
using ООО_Поломка.Views;

namespace ООО_Поломка
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        Stack<Page> history = new Stack<Page>();
        static internal MainWindow instance;

        private Page currentPage;
        private Visibility buttonHistoryVisibility;

        public Page CurrentPage 
        { 
            get => currentPage;
            set
            {
                if (value != null)
                {
                    if (currentPage != null)
                        history.Push(currentPage);
                }
                currentPage = value;

                ButtonHistoryVisibility = value is ClientPage ?
                       Visibility.Collapsed :
                       Visibility.Visible;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(CurrentPage)));
            }
        }

        public Visibility ButtonHistoryVisibility
        {
            get => buttonHistoryVisibility;
            set
            {
                buttonHistoryVisibility = value;
                PropertyChanged?.Invoke(this,
                   new PropertyChangedEventArgs(nameof(ButtonHistoryVisibility)));
            }
        }

        public List<Client> ClientsBirthdayMonth { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            var modelContext = new ModelContext();
            int month = DateTime.Now.Month;
            try
            {
                ClientsBirthdayMonth = modelContext.Clients.Where(
                    s => s.Birthday.Value.Month == month).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при получении данных с сервера");
            }

            DataContext = this;
            CurrentPage = new ClientPage();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void BackHistory(object sender, RoutedEventArgs e)
        {
            if (history.Count == 1)
                CurrentPage = history.Peek();
            else
                CurrentPage = history.Pop();
        }
    }
}