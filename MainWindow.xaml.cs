using KonyvtarAsztali;
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

namespace KonyvtarAsztaliWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Statisztika service;
        public MainWindow()
        {
            InitializeComponent();
            Read();
        }

        private void Read()
        {
            try
            {
                this.service = new Statisztika();
                service.Feltoltes();
                dataGridBooks.ItemsSource = service.Konyvek;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.Close();
            }
        }

        private void buttonTorles_Click(object sender, RoutedEventArgs e)
        {
            Konyv selected = dataGridBooks.SelectedItem as Konyv;
            if (selected == null)
            {
                MessageBox.Show("Törléshez előbb válasszon ki egy könyvet!");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Biztosan törölni szeretné a kiválasztott könyvet?",
                "Törlés", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (this.service.Torles(selected.Id))
                {
                    MessageBox.Show("Sikeres törlés!");
                }
                else
                {
                    MessageBox.Show("Sikertelen törlés!");
                }
                Read();
            }
        }
    }
}
