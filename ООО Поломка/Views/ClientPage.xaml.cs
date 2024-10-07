using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<Client> Clients { get; set; }

        public Client SelectedClient { get; set; }
        public ClientPage()
        {
            InitializeComponent();

            LoadClients();
            DataContext = this;
        }

        private void LoadClients()
        {
            ModelContext modelContext = new ModelContext();
            Clients = new ObservableCollection<Client>(
                modelContext.Clients.Include(s => s.ClientServices).ToList());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
