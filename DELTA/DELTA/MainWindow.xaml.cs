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
            var (userId, role) = AuthenticateUser(username, password);

            if (userId != -1)
            {
                MessageBox.Show("Авторизация успешна!");

                // Проверка роли и открытие соответствующего окна
                switch (role)
                {
                    case "admin":
                        AdminWindow adminWindow = new AdminWindow(userId);
                        adminWindow.Show();
                        break;
                    case "manager":
                        ManagerWindow managerWindow = new ManagerWindow(userId);
                        managerWindow.Show();
                        break;
                    case "user":
                        Window1 userWindow = new Window1(userId);
                        userWindow.Show();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль.");
                        break;
                }

                this.Close(); // Закрываем текущее окно
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }

        // Метод для проверки авторизации и получения роли
        private (int, string) AuthenticateUser(string username, string password)
        {
            int userId = -1;
            string role = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL запрос для получения ID пользователя и его роли
                    string query = "SELECT user_id, role FROM Users WHERE username = @username AND password = @password";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    // Выполнение команды и чтение результата
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        userId = Convert.ToInt32(reader["user_id"]);
                        role = reader["role"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
                }
            }

            return (userId, role); // Возвращаем и userId, и роль
        }

        private void Label_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RegistrationWindow win2 = new RegistrationWindow();
            win2.Show();
            this.Close();
        }
    }
}
