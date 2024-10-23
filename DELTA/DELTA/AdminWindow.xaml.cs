using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace DELTA
{
    public partial class AdminWindow : Window
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True";
        private int userId;
        public AdminWindow(int userId)
        {
            InitializeComponent();
            LoadUsers();  // Загрузка данных пользователей при открытии окна
            this.userId = userId;
        }

        // Класс для представления пользователя
        public class User
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
            public string Contact { get; set; }
        }

        // Метод для загрузки списка пользователей из базы данных
        private void LoadUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT user_id, username, role,contact FROM Users";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Username = reader["username"].ToString(),
                            Role = reader["role"].ToString(),
                            Contact = reader["contact"].ToString()
                        });
                    }

                    UsersDataGrid.ItemsSource = users;  // Привязка данных к DataGrid
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных пользователей: {ex.Message}");
                }
            }
        }

        // Метод для удаления пользователя
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.Username}?",
                                                          "Подтверждение удаления",
                                                          MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            string query = "DELETE FROM Users WHERE user_id = @userId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@userId", selectedUser.UserId);
                            command.ExecuteNonQuery();

                            MessageBox.Show("Пользователь удален.");
                            LoadUsers();  // Обновляем список после удаления
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.");
            }
        }

        // Метод для редактирования пользователя
        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                //Открываем окно редактирования пользователя
                EditUserWindow editWindow = new EditUserWindow(selectedUser.UserId);
                editWindow.ShowDialog();

                // Обновляем список после редактирования
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow manager = new ManagerWindow(userId);
            manager.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
