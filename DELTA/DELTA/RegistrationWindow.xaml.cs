using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows;
using System;

namespace DELTA
{
    public partial class RegistrationWindow : Window
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True";

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        // Метод для регистрации нового пользователя
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string password = Password.Password;
            string confirmPassword = ConfirmPassword.Password;

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            // Получаем выбранную роль через Tag
            ComboBoxItem selectedRoleItem = (ComboBoxItem)RoleComboBox.SelectedItem;
            string role = selectedRoleItem.Tag.ToString(); // Здесь получаем role как "user", "manager" или "admin"

            RegisterUser(username, password, role);
        }

        private void RegisterUser(string username, string password, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL запрос для добавления пользователя в БД
                    string query = "INSERT INTO Users (username, password, role) VALUES (@username, @password, @role)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@role", role);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Регистрация успешна!");
                    MainWindow win2 = new MainWindow();
                    win2.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка регистрации: {ex.Message}");
                }
            }
        }


        private void TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();  // Закрыть окно регистрации
        }
    }
}
