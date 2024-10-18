using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows;
using System;

namespace DELTA
{
    public partial class MainWindow : Window
    {
        // Строка подключения к базе данных
        private string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод авторизации при нажатии кнопки
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем введенные данные
            string username = User.Text;  // TextBox для ввода логина
            string password = Pass.Password;  // PasswordBox для ввода пароля

            // Проверяем пользователя в базе данных
            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Авторизация успешна!");
                Window1 Win = new Window1();
                Win.Show();
                this.Close(); // Закрываем текущее окно
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }

        // Метод для проверки авторизации
        private bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL запрос для проверки логина и пароля
                    string query = "SELECT COUNT(1) FROM Users WHERE username = @username AND password = @password";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    // Выполнение команды и получение результата
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count == 1)
                    {
                        isAuthenticated = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
                }
            }

            return isAuthenticated;
        }
    }
}
