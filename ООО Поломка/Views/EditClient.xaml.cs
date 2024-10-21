using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace ООО_Поломка.Views
{
    /// <summary>
    /// Логика взаимодействия для EditClient.xaml
    /// </summary>
    public partial class EditClient : Page
    {
        public Visibility IsEditClient { get; set; } = Visibility.Collapsed;
        public EditClient()
        {
            InitializeComponent();
            SelectedClient = new Client{ Birthday= DateOnly.FromDateTime( DateTime.Now)};
            DataContext = this;
        }

        public EditClient(Client selectedClient)
        {
            IsEditClient = Visibility.Visible;
            SelectedClient = selectedClient;
            DataContext = this;
        }

        public Client SelectedClient { get; set; }
    }
}
