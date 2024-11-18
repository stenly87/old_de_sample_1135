using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        int countPageRecords;
        private string currentPageRecordsCount;

        public List<Gender> Genders { get; set; }
        public Gender SelectedGender
        {
            get => selectedGender;
            set
            {
                selectedGender = value;
                currentPageIndex = 0;
                LoadClients();
            }
        }

        public List<string> PageRecords { get; set; }
        public string CurrentPageRecordsCount
        {
            get => currentPageRecordsCount;
            set
            {
                currentPageIndex = 0;
                currentPageRecordsCount = value;
                LoadClients();
            }
        }
        public ObservableCollection<Client> Clients
        {
            get => clients;
            set
            {
                clients = value;
                Signal();
            }
        }
        ModelContext modelContext;
        public Client SelectedClient
        {
            get => selectedClient;
            set
            {
                selectedClient = value;
                Signal();
            }
        }

        int all = 0;
        int show = 0;
        public string CountRecords
        {
            get => $"Отображено записей: {show} из {all}";
        }

        public int SortingIndex
        {
            get => sortingIndex;
            set
            {
                sortingIndex = value;
                LoadClients();
            }
        }

        public string Search
        {
            get => search;
            set
            {
                currentPageIndex = 0;
                search = value;
                LoadClients();
            }
        }


        public ClientPage()
        {
            InitializeComponent();
            modelContext = new ModelContext();
            modelContext.Services.ToList();
            try
            {
                Genders = modelContext.Genders.ToList();
                Genders.Insert(0, new Gender { Code = null, Name = "Все" });
                selectedGender = Genders[0];
                all = modelContext.Clients.Count();
                PageRecords = new List<string>(new string[] { "10", "50", "200", "все" });
                CurrentPageRecordsCount = PageRecords.First();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при получении данных с сервера");
            }
            DataContext = this;
        }

        int currentPageIndex = 0;
        private ObservableCollection<Client> clients;
        private Gender selectedGender;
        private string search;
        private int sortingIndex = 0;
        private Client selectedClient;

        private void LoadClients()
        {
            try
            {
                all = modelContext.Clients.Count();

                int skip = 0;
                if (!int.TryParse(CurrentPageRecordsCount, out int take))
                {
                    take = int.MaxValue;
                    skip = 0;
                }
                else
                    skip = take;
                countPageRecords = take;

                var temp =
                    modelContext.
                    Clients.
                    Include(s => s.ClientServices).
                    Include(s => s.Tags).
                    Where(s => string.IsNullOrEmpty(search) ||
                        (s.FirstName.Contains(search) ||
                        s.LastName.Contains(search) ||
                        s.Patronymic.Contains(search) ||
                        s.Email.Contains(search) ||
                        s.Phone.Contains(search))).
                    Where(s => s.GenderCode == selectedGender.Code ||
                        selectedGender.Code == null).ToList();

                if (sortingIndex == 1)
                    temp = temp.OrderBy(s => s.LastName).ToList();
                else if (sortingIndex == 2)
                    temp = temp.OrderByDescending(s => s.LastVisit).ToList();
                else if (sortingIndex == 3)
                    temp = temp.OrderByDescending(s => s.CountVisit).ToList();

                Clients = new ObservableCollection<Client>(temp.Skip(currentPageIndex * skip).
                    Take(take).
                    ToList());
                show = Clients.Count();

                Signal(nameof(CountRecords));

                if (show == 0)
                    MessageBox.Show("По заданным критериям клиенты не найдены");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при получении данных с сервера");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void Signal([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private void PreviousPage(object sender, RoutedEventArgs e)
        {
            currentPageIndex--;
            if (currentPageIndex < 0)
                currentPageIndex = 0;
            LoadClients();
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            if (countPageRecords == int.MaxValue)
                return;
            try
            {
                var all = modelContext.Clients.Where(s => string.IsNullOrEmpty(search) ||
                        (s.FirstName.Contains(search) ||
                        s.LastName.Contains(search) ||
                        s.Patronymic.Contains(search) ||
                        s.Email.Contains(search) ||
                        s.Phone.Contains(search)))
                    .Where(s => s.GenderCode == selectedGender.Code ||
                        selectedGender.Code == null).Count();
                double totalPages = all / (double)countPageRecords;
                currentPageIndex++;
                int correction = (totalPages - (int)totalPages) == 0 ? 1 : 0;
                if (currentPageIndex >= totalPages)
                    currentPageIndex = (int)totalPages - correction;
                if (currentPageIndex < 0)
                    currentPageIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при получении данных с сервера");
                return;
            }
            LoadClients();

            // SOLID
            // SRP - класс (или функция) должен быть ответственнен только за одно действие
            // у класса не должно быть более одной причины для изменения
            // SRP очень часто нарушается, поскольку ему тяжело следовать

        }

        private void AddClient(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.CurrentPage = new EditClient();
        }

        private void EditClient(object sender, RoutedEventArgs e)
        {
            if (SelectedClient == null)
            {
                MessageBox.Show("Не выбран клиент");
                return;
            }
            MainWindow.instance.CurrentPage = new EditClient(SelectedClient);
        }

        private void RemoveClient(object sender, RoutedEventArgs e)
        {
            if (SelectedClient == null)
            {
                MessageBox.Show("Не выбран клиент");
                return;
            }

            if (selectedClient.ClientServices.Count > 0)
            {
                MessageBox.Show("Ценный клиент. Удаление запрещено!");
                return;
            }

            try
            {
                var model = new ModelContext();
                model.Clients.Remove(SelectedClient);
                model.SaveChanges();
                Clients.Remove(SelectedClient);
                SelectedClient = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при получении данных с сервера");
            }
        }

        private void ViewClientService(object sender, RoutedEventArgs e)
        {
            if (SelectedClient == null)
                return;
            MainWindow.instance.CurrentPage = new ClientServicePage(SelectedClient);
        }
    }
}
