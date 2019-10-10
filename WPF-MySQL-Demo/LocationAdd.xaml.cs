using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace WPF_MySQL_Demo
{
    /// <summary>
    /// Interaction logic for LocationAdd.xaml
    /// </summary>
    public partial class LocationAdd : Window
    {
        private string connStr = "server=localhost;user=nmt_demo_user;database=nmt_demo;port=3306;password=Password1";

        private MySqlConnection connection;

        public LocationAdd()
        {
            InitializeComponent();

            GetConnection();

            txtNameError.Text = txtLocationError.Text = string.Empty;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            CloseConnection();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            txtNameError.Text = txtLocationError.Text = string.Empty;

            string sName = txtFullName.Text.Trim();
            string sCode = txtLocationCode.Text.Trim();

            ValidateName(sName);
            ValidateCode(sCode);

            if (txtLocationError.Text.Length + txtNameError.Text.Length == 0)
            {
                if (AddLocationToDB(sName, sCode))
                {
                    MessageBox.Show("Country Added Successfully!");
                    BtnCancel_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Insert Failed!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TxtFullName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtFullName.Text = string.Empty;
        }

        private void TxtLocationCode_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLocationCode.Text = string.Empty;
        }

        private void GetConnection()
        {
            connection = new MySqlConnection(connStr);
            connection.Open();
        }

        private void CloseConnection()
        {
            connection.Close();
        }

        private void ValidateName(string sName)
        {
            if (sName.Length < 4 || sName.Length > 128)
            {
                txtNameError.Text = "Invalid Name (min 4, max 128 chars)";
            }
            else if (FindLocationInDB(sName))
            {
                txtNameError.Text = "Name already exists in Database";
            }
        }

        private void ValidateCode(string sCode)
        {
            if (sCode.Length != 2)
            {
                txtLocationError.Text = "Invalid Location Code (exactly 2 letter required)";
            }
            else if (FindCodeInDB(sCode))
            {
                txtLocationError.Text = "Code already exists in Database";
            }
        }

        private bool FindLocationInDB(string sName)
        {
            string sql = "select 1 from locations where location_name = '" + sName + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
            {
                if (cmdSel.ExecuteScalar() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool FindCodeInDB(string sCode)
        {
            string sql = "select 1 from locations where two_letter_code = '" + sCode + "'";

            using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
            {
                if (cmdSel.ExecuteScalar() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AddLocationToDB(string sName, string sCode)
        {
            string sql = "Insert into locations (location_name, two_letter_code) select '" + sName + "', '" + sCode + "' ";

            try
            {
                using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
                {
                    cmdSel.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
