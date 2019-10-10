using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace WPF_MySQL_Demo
{
    /// <summary>
    /// Interaction logic for LocationAdd.xaml
    /// </summary>
    public partial class LocationEdit : Window
    {
        private string connStr = "server=localhost;user=nmt_demo_user;database=nmt_demo;port=3306;password=Password1";

        private MySqlConnection connection;

        private int globalLocationId = 0;

        public LocationEdit(int locationId)
        {
            globalLocationId = locationId;

            InitializeComponent();

            GetConnection();

            txtNameError.Text = txtLocationError.Text = string.Empty;

            FindLocationById();
        }

        //public LocationEdit()
        //{
        //    InitializeComponent();

        //    GetConnection();

        //    txtNameError.Text = txtLocationError.Text = string.Empty;
        //}

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
                if (EditLocationInDB(sName, sCode))
                {
                    MessageBox.Show("Country Eddited Successfully!");
                    BtnCancel_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Update Failed!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
            //else if (FindLocationInDB(sName))
            //{
            //    txtNameError.Text = "Name already exists in Database";
            //}
        }

        private void ValidateCode(string sCode)
        {
            if (sCode.Length != 2)
            {
                txtLocationError.Text = "Invalid Location Code (exactly 2 letter required)";
            }
            //else if (FindCodeInDB(sCode))
            //{
            //    txtLocationError.Text = "Code already exists in Database";
            //}
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

        private bool EditLocationInDB(string sName, string sCode)
        {
            string sql =    "update locations set location_name = '" + sName +"', two_letter_code ='" + sCode + "' "+
                            "where id = " + globalLocationId.ToString();

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

        private void FindLocationById()
        {
            string sql = "select * from locations where id = " + globalLocationId.ToString() ;

            using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
            {
                MySqlDataReader data = cmdSel.ExecuteReader();

                data.Read();
                txtFullName.Text = data.GetString(1);
                txtLocationCode.Text = data.GetString(2);
            }
        }
    }
}
