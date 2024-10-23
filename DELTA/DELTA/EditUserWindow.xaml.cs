using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace DELTA
{
    public partial class EditUserWindow : Window
    {
        private int userId;  // ID пользователя для редактирования
        private string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True";

        public EditUserWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;

            // Загрузить данные пользователя при открытии окна
            LoadUserData();
        }

        // Метод для загрузки данных пользователя
        private void LoadUserData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL запрос для получения данных пользователя
                    string query = "SELECT username, role,contact FROM Users WHERE user_id = @userId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Устанавливаем данные в поля для редактирования
                        UsernameTextBox.Text = reader["username"].ToString();
                        string role = reader["role"].ToString();
                        ContactTextBox.Text = reader["contact"].ToString();

                        // Устанавливаем выбранную роль в ComboBox
                        foreach (ComboBoxItem item in RoleComboBox.Items)
                        {
                            if (item.Content.ToString().ToLower() == role.ToLower())
                            {
                                item.IsSelected = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных пользователя: {ex.Message}");
                }
            }
        }

        // Метод для сохранения изменений
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = UsernameTextBox.Text;
            string newContact = ContactTextBox.Text;
            string newRole = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL запрос для обновления данных пользователя
                    string query = "UPDATE Users SET username = @username, role = @role, contact = @contact WHERE user_id = @userId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", newUsername);
                    command.Parameters.AddWithValue("@role", newRole);
                    command.Parameters.AddWithValue("@contact", newContact);
                    command.Parameters.AddWithValue("@userId", userId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Данные пользователя успешно обновлены.");
                    this.Close();  // Закрываем окно после успешного обновления
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
                }
            }
        }
    }
}
