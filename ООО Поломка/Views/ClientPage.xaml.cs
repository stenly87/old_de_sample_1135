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
        public Gender SelectedGender {
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
        public Client SelectedClient { get; set; }

        int all = 0;
        int show = 0;
        public string CountRecords {
            get => $"Отображено записей: {show} из {all}";
        }

        public string Search {
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
            Genders = modelContext.Genders.ToList();
            Genders.Insert(0, new Gender { Code = null, Name = "Все" });
            selectedGender = Genders[0];
            all = modelContext.Clients.Count();
            PageRecords = new List<string>(new string[] { "10", "50", "200", "все" });
            CurrentPageRecordsCount = PageRecords.First();
            DataContext = this;
        }

        int currentPageIndex = 0;
        private ObservableCollection<Client> clients;
        private Gender selectedGender;
        private string search;

        private void LoadClients()
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

            Clients = new ObservableCollection<Client>(
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
                    selectedGender.Code == null).
                Skip(currentPageIndex * skip).
                Take(take).
                ToList());
            show = Clients.Count();
            Signal(nameof(CountRecords));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void Signal([CallerMemberName]string prop = null) =>
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
            LoadClients();

            // SOLID
            // SRP - класс (или функция) должен быть ответственнен только за одно действие
            // у класса не должно быть более одной причины для изменения
            // SRP очень часто нарушается, поскольку ему тяжело следовать

        }
    }
}
