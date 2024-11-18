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
    /// Логика взаимодействия для ClientServicePage.xaml
    /// </summary>
    public partial class ClientServicePage : Page
    {
        public Client SelectedClient { get; }

        public ClientServicePage(DB.Client selectedClient)
        {
            InitializeComponent();
            SelectedClient = selectedClient;
            DataContext = this;
        }

        
    }
}
