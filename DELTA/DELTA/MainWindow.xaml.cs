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
            int userId = AuthenticateUser(username, password);
            if (userId != -1)
            {
                MessageBox.Show("Авторизация успешна!");
                // Передаем userId в окна корзины и заказов
                Window1 cartWindow = new Window1(userId);
                cartWindow.Show();
                this.Close(); // Закрываем текущее окно
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }


        // Метод для проверки авторизации
        private int AuthenticateUser(string username, string password)
        {
            int userId = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL запрос для получения ID пользователя
                    string query = "SELECT user_id FROM Users WHERE username = @username AND password = @password";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    // Выполнение команды и получение результата
                    userId = Convert.ToInt32(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
                }
            }

            return userId;
        }

    }
}
