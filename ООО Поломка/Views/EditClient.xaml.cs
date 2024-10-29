using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class EditClient : Page, INotifyPropertyChanged
    {
        private byte[] foto;

        public bool Man { get; set; }
        public bool Woman { get; set; }
        public byte[] Foto
        {
            get => foto;
            set
            {
                foto = value;
                Signal();
            }
        }
        string fotoPath;

        public List<Gender> Genders { get; set; }

        public Visibility IsEditClient { get; set; } = Visibility.Collapsed;
        public EditClient()
        {
            InitializeComponent();
            LoadData();
            MakeArrayChars();
            SelectedClient = new Client { Birthday = DateOnly.FromDateTime(DateTime.Now) };
            DataContext = this;

        }

        private void MakeArrayChars()
        {
            var temp = new List<int>();
            temp.Add(32);
            temp.Add(45);
            temp.Add(1105);
            temp.Add(1025);
            temp.AddRange(Enumerable.Range(1040, 63));
            arrayChars = temp.ToArray();
            temp.Clear();
            temp.Add(45);
            temp.Add(40);
            temp.Add(41);
            temp.Add(43);
            temp.AddRange(Enumerable.Range(48, 10));
            arrayNumbers = temp.ToArray();
        }

        int[] arrayChars;
        int[] arrayNumbers;

        public event PropertyChangedEventHandler? PropertyChanged;
        void Signal([CallerMemberName] string prop = null) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private void LoadData()
        {
            var modelContext = new ModelContext();
            Genders = modelContext.Genders.ToList();
        }

        public EditClient(Client selectedClient)
        {
            InitializeComponent();
            LoadData();
            MakeArrayChars();
            IsEditClient = Visibility.Visible;
            SelectedClient = selectedClient;
            if (selectedClient.GenderCodeNavigation.Name == "Мужской")
                Man = true;
            else
                Woman = true;

            if (!string.IsNullOrEmpty(selectedClient.PhotoPath))
            {
                if (File.Exists(selectedClient.PhotoPath))
                {
                    fotoPath = selectedClient.PhotoPath;
                    Foto = File.ReadAllBytes(selectedClient.PhotoPath);
                }
            }
            DataContext = this;
        }

        public Client SelectedClient { get; set; }

        private void SelectFoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image|*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    MessageBox.Show("Выберите файл");
                    return;
                }
                if (new FileInfo(openFileDialog.FileName).Length > 2 * 1024 * 1024)
                {
                    MessageBox.Show("Файл превышает 2МБ");
                    return;
                }
                fotoPath = openFileDialog.FileName;
                Foto = File.ReadAllBytes(fotoPath);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var modelContext = new ModelContext();
            if (CheckFIOLengthIsBad(SelectedClient))
            {
                MessageBox.Show("ФИО клиента должны быть длиной менее 50 символов");
                return;
            }

            if (CheckFIOCharsIsBad(SelectedClient))
            {
                MessageBox.Show("ФИО клиента должны состоять из букв и символов пробела или дефиса");
                return;
            }

            if (CheckEmailIsBad(SelectedClient))
            {
                MessageBox.Show("Email клиента некорректный");
                return;
            }

            if (CheckPhoneIsBad(SelectedClient))
            {
                MessageBox.Show("Телефон клиента может содержать только цифры и символы: + -  ( ) и пробел");
                return;
            }

            if (!Man && !Woman)
            {
                MessageBox.Show("Небинарные личности не приветствуются");
                return;
            }
            if (Man)
            {
                SelectedClient.GenderCodeNavigation = Genders.First(s => s.Name == "Мужской");
                SelectedClient.GenderCode = SelectedClient.GenderCodeNavigation.Code;
            }
            else
            {
                SelectedClient.GenderCodeNavigation = Genders.First(s => s.Name == "Женский");
                SelectedClient.GenderCode = SelectedClient.GenderCodeNavigation.Code;
            }
            SelectedClient.PhotoPath = fotoPath;

            if (SelectedClient.Id == 0)
                modelContext.Clients.Add(SelectedClient);
            else
            {
                var original = modelContext.Clients.Find(SelectedClient.Id);
                modelContext.Entry(original).CurrentValues.SetValues(SelectedClient);
            }
            modelContext.SaveChanges();
            MainWindow.instance.CurrentPage = new ClientPage();
        }

        private bool CheckPhoneIsBad(Client selectedClient)
        {
            foreach (char c in selectedClient.Phone)
                if (!arrayNumbers.Contains(c))
                    return true;
            return false;
        }

        private bool CheckEmailIsBad(Client selectedClient)
        {
            return false;
        }

        private bool CheckFIOCharsIsBad(Client selectedClient)
        {
            return CheckCharsBad(selectedClient.FirstName) ||
                CheckCharsBad(selectedClient.LastName) ||
                CheckCharsBad(selectedClient.Patronymic);
        }

        private bool CheckCharsBad(string firstName)
        {
            foreach (char c in firstName)
            {
                if (!arrayChars.Contains(c))
                    return true;
            }
            return false;
        }

        private bool CheckFIOLengthIsBad(Client selectedClient)
        {
            return (selectedClient.FirstName.Length > 50 ||
                selectedClient.LastName.Length > 50 ||
                selectedClient.Patronymic.Length > 50);
        }
    }
}
