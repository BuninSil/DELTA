using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace DELTA
{
    public partial class OrderManagementWindow : Window
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True";

        public OrderManagementWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    o.order_id,
                    o.user_id,
                    u.username,
                    o.order_date,
                    o.status,
                    o.address,
                    p.name AS product_name,
                    op.quantity
                FROM Orders o
                JOIN Users u ON o.user_id = u.user_id
                JOIN OrderProducts op ON o.order_id = op.order_id
                JOIN Products p ON op.product_id = p.product_id";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    var orders = new System.Collections.ObjectModel.ObservableCollection<Order>();

                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            order_id = reader.GetInt32(0),
                            user_id = reader.GetInt32(1),
                            username = reader.GetString(2),  // Добавлено поле username
                            order_date = reader.GetDateTime(3),
                            status = reader.GetString(4),
                            address = reader.GetString(5),
                            product_name = reader.GetString(6), // Добавлено поле product_name
                            quantity = reader.GetInt32(7) // Добавлено поле quantity
                        });
                    }

                    OrdersDataGrid.ItemsSource = orders;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}");
                }
            }
        }


        private void UpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                string newStatus = ((ComboBoxItem)StatusComboBox.SelectedItem)?.Content.ToString();

                if (newStatus != null)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "UPDATE Orders SET status = @status WHERE order_id = @orderId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@status", newStatus);
                            command.Parameters.AddWithValue("@orderId", selectedOrder.order_id);
                            command.ExecuteNonQuery();

                            MessageBox.Show("Статус заказа успешно изменен!");
                            LoadOrders(); // Обновляем список заказов
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка изменения статуса: {ex.Message}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите новый статус.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заказ для изменения статуса.");
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли заказ
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот заказ?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            // Удаление связанных записей из OrderProducts
                            string deleteOrderProductsQuery = "DELETE FROM OrderProducts WHERE order_id = @orderId";
                            using (SqlCommand command = new SqlCommand(deleteOrderProductsQuery, connection))
                            {
                                command.Parameters.AddWithValue("@orderId", selectedOrder.order_id);
                                command.ExecuteNonQuery();
                            }

                            // Удаление заказа
                            string deleteOrderQuery = "DELETE FROM Orders WHERE order_id = @orderId";
                            using (SqlCommand command = new SqlCommand(deleteOrderQuery, connection))
                            {
                                command.Parameters.AddWithValue("@orderId", selectedOrder.order_id);
                                command.ExecuteNonQuery();
                            }

                            // Удаление успешно завершено
                            MessageBox.Show("Заказ успешно удален.");
                            LoadOrders(); // Обновление списка заказов
                        }
                        catch (SqlException sqlEx)
                        {
                            // Обработка ошибок SQL
                            MessageBox.Show($"Ошибка при удалении заказа: {sqlEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            // Обработка других ошибок
                            MessageBox.Show($"Произошла ошибка: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заказ для удаления.");
            }
        }


        public class Order
        {
            public int order_id { get; set; }
            public int user_id { get; set; }
            public string username { get; set; } // Новое поле для имени пользователя
            public DateTime order_date { get; set; }
            public string status { get; set; }
            public string address { get; set; }
            public string product_name { get; set; } // Новое поле для названия товара
            public int quantity { get; set; } // Новое поле для количества товара
        }

    }
}
