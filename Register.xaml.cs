using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;

namespace phinance2
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(passwordBox.Password) && passwordBox.Password.Length > 0)
                textPassword.Visibility = Visibility.Collapsed;
            else
                textPassword.Visibility = Visibility.Visible;
        }

        private void TextPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Focus();
        }

        private void TxtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text) && txtEmail.Text.Length > 0)
                textEmail.Visibility = Visibility.Collapsed;
            else
                textEmail.Visibility = Visibility.Visible;
        }

        private void TextEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }


        private void TxtFirstName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFirstName.Text) && txtFirstName.Text.Length > 0)
                textFirstName.Visibility = Visibility.Collapsed;
            else
                textFirstName.Visibility = Visibility.Visible;
        }

        private void TextFirstName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtFirstName.Focus();
        }

        private void TxtLastName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLastName.Text) && txtLastName.Text.Length > 0)
                textLastName.Visibility = Visibility.Collapsed;
            else
                textLastName.Visibility = Visibility.Visible;
        }

        private void TextLastName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtLastName.Focus();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Get user input
            string userEmail = txtEmail.Text;
            string userPassword = HashPassword(passwordBox.Password); // Hash the password
            string userFirstName = txtFirstName.Text;
            string userLastName = txtLastName.Text;

            // Establish a database connection (you need to provide the connection string)
            string connectionString = "server=localhost;user=Phil;password=Sieunhan123;database=phinance_wpf_database;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Check if the email already exists
                string checkEmailQuery = "SELECT COUNT(*) FROM User WHERE email = @Email";

                using (MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, connection))
                {
                    checkEmailCmd.Parameters.AddWithValue("@Email", userEmail);

                    int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                    if (emailCount > 0)
                    {
                        MessageBox.Show("Email already taken. Please choose a different email.");
                        return; // Exit the method without inserting the user
                    }
                }

                // Define an SQL query to insert data into the user table
                string insertQuery = "INSERT INTO User (email, password, first_Name, last_Name) VALUES (@Email, @Password, @FirstName, @LastName)";

                using (MySqlCommand insertUserCmd = new MySqlCommand(insertQuery, connection))
                {
                    // Add parameters to prevent SQL injection
                    insertUserCmd.Parameters.AddWithValue("@Email", userEmail);
                    insertUserCmd.Parameters.AddWithValue("@Password", userPassword);
                    insertUserCmd.Parameters.AddWithValue("@FirstName", userFirstName);
                    insertUserCmd.Parameters.AddWithValue("@LastName", userLastName);

                    // Execute the query
                    int rowsAffected = insertUserCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Registration successful!");
                        // Optionally, you can redirect the user to a login page or perform other actions.
                    }
                    else
                    {
                        MessageBox.Show("Registration failed.");
                    }

                    // Retrieve the ID of the newly inserted user
                    long userId = insertUserCmd.LastInsertedId;

                    // Define an SQL query to insert a new account for the user
                    string insertAccountQuery = "INSERT INTO account_balance (user_id, usd) VALUES (@UserId, 10000)";

                    using (MySqlCommand insertAccountCmd = new MySqlCommand(insertAccountQuery, connection))
                    {
                        // Add the user ID parameter
                        insertAccountCmd.Parameters.AddWithValue("@UserId", userId);

                        // Execute the account insert query
                        int accountRowsAffected = insertAccountCmd.ExecuteNonQuery();

                        if (accountRowsAffected > 0)
                        {
                            Login login = new Login();
                            login.Show();
                            this.Close();
                            // Optionally, you can redirect the user to a login page or perform other actions.
                        }
                        else
                        {
                            MessageBox.Show("Account creation failed.");
                        }
                    }
                }
            }
        }

        private string HashPassword(string password)
        {
            // You should use a secure password hashing library like BCrypt
            // For example: return BCrypt.Net.BCrypt.HashPassword(password);
            // This is just a simplified example.
            return password;
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
