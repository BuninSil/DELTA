using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace DELTA
{
    public partial class CartWindow : Window
    {
        private int userId;

        public CartWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            // Здесь вы можете использовать userId для загрузки данных корзины и т.д.
        }
    

    // Предполагается, что user_id передается через контекст данных
    //public int UserId { get; set; }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            var cart = DataContext as Window1.Cart;
            if (cart != null && cart.Items.Any())
            {
                try
                {
                    SaveOrderToDatabase(cart);
                    MessageBox.Show("Заказ успешно оформлен!");
                    cart.Items.Clear();  // Очищаем корзину после оформления заказа
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Корзина пуста!");
            }
        }

        private void SaveOrderToDatabase(Window1.Cart cart)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Начинаем транзакцию, чтобы в случае ошибки откатить все изменения
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Вставляем запись в таблицу Orders
                        string insertOrderQuery = "INSERT INTO Orders (user_id, order_date, status) OUTPUT INSERTED.order_id VALUES (@UserId, GETDATE(), 'placed')";
                        SqlCommand orderCommand = new SqlCommand(insertOrderQuery, connection, transaction);
                        orderCommand.Parameters.AddWithValue("@UserId", userId);

                        int orderId = (int)orderCommand.ExecuteScalar(); // Получаем сгенерированный order_id

                        // Вставляем товары в таблицу OrderProducts
                        foreach (var item in cart.Items)
                        {
                            string insertOrderProductQuery = "INSERT INTO OrderProducts (order_id, product_id, quantity) VALUES (@OrderId, @ProductId, @Quantity)";
                            SqlCommand orderProductCommand = new SqlCommand(insertOrderProductQuery, connection, transaction);
                            orderProductCommand.Parameters.AddWithValue("@OrderId", orderId);
                            orderProductCommand.Parameters.AddWithValue("@ProductId", GetProductIdByName(item.Name, connection, transaction));  // Получаем product_id по имени
                            orderProductCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            orderProductCommand.ExecuteNonQuery();
                        }

                        // Подтверждаем транзакцию
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Откатываем транзакцию в случае ошибки
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Метод для получения product_id по имени продукта
        private int GetProductIdByName(string productName, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT product_id FROM Products WHERE Name = @ProductName";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@ProductName", productName);
            return (int)command.ExecuteScalar();
        }
    }
}
