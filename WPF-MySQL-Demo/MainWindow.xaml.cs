using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_MySQL_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ScanStatusKeysInBackground();

            FillTable();
        }

        public void CheckStatusKeys()
        {
            NumLockStatus.Foreground = Keyboard.IsKeyToggled(Key.NumLock) ? Brushes.Red : Brushes.Gray;
            CapsLockStatus.Foreground = Keyboard.IsKeyToggled(Key.CapsLock) ? Brushes.Red : Brushes.Gray;
            ScrollLockStatus.Foreground = Keyboard.IsKeyToggled(Key.Scroll) ? Brushes.Red : Brushes.Gray;
        }

        public async Task ScanStatusKeysInBackground()
        {
            while (true)
            {
                CheckStatusKeys();
                await Task.Delay(100);
            }
        }

        public void FillTable(string searchTerm = "")
        {
            string connStr = "server=localhost;" +
                             "user=nmt_demo_user;" +
                             "database=nmt_demo;" +
                             "port=3306;" +
                             "password=Password1";

            try
            {
                MessageTextBlock.Text = "Connecting to MySql ...";

                string sql = "select * from locations";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    sql += " where location_name like '%" + searchTerm.Trim() + "%'";
                }

                MessageTextBox.Text = sql;

                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    connection.Open();
                    MessageTextBlock.Text = "Processing ...";

                    using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                        da.Fill(dt);
                        LocationDataGrid.DataContext = dt;
                    }

                    connection.Close();
                }
                MessageTextBlock.Text = "Ready";
            }
            catch (Exception e)
            {
                MessageTextBlock.Text = "Error: " + e.Message;
            }
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillTable(FilterTextBox.Text);
        }

        private void BtnAddCountry_Click(object sender, RoutedEventArgs e)
        {
            LocationAdd locationAdd = new LocationAdd();
            locationAdd.ShowDialog();
            FillTable(FilterTextBox.Text);
        }
    }
}
