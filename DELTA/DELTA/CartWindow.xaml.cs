using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;


namespace DELTA
{
    public partial class CartWindow : Window
    {
        private int userId;

        public CartWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;

           
        }



        // Метод для получения адреса
        public void SetAddress(string address)
        {
            MessageBox.Show($"Выбранный адрес: {address}");
            // Здесь можно сохранить адрес в базу данных или использовать дальше в логике
        }
    

   

        private void GetAddressFromMap_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Адрес выбран: " + AddressTextBox.Text);
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            var cart = DataContext as Window1.Cart;
            string deliveryAddress = AddressTextBox.Text;

            if (cart != null && cart.Items.Any())
            {
                if (string.IsNullOrEmpty(deliveryAddress))
                {
                    MessageBox.Show("Пожалуйста, введите адрес доставки.");
                    return;
                }

                try
                {
                    SaveOrderToDatabase(cart, deliveryAddress); // Передаем адрес доставки
                    MessageBox.Show("Заказ успешно оформлен!");
                    cart.Items.Clear();  // Очищаем корзину после оформления заказа
                    AddressTextBox.Text = "";
                    this.Close();
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

        private void SaveOrderToDatabase(Window1.Cart cart, string deliveryAddress)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Вставляем запись в таблицу Orders с адресом доставки
                        string insertOrderQuery = "INSERT INTO Orders (user_id, order_date, status, address) OUTPUT INSERTED.order_id VALUES (@UserId, GETDATE(), 'Создан', @DeliveryAddress)";
                        SqlCommand orderCommand = new SqlCommand(insertOrderQuery, connection, transaction);
                        orderCommand.Parameters.AddWithValue("@UserId", userId);
                        orderCommand.Parameters.AddWithValue("@DeliveryAddress", deliveryAddress);

                        int orderId = (int)orderCommand.ExecuteScalar();

                        foreach (var item in cart.Items)
                        {
                            string insertOrderProductQuery = "INSERT INTO OrderProducts (order_id, product_id, quantity) VALUES (@OrderId, @ProductId, @Quantity)";
                            SqlCommand orderProductCommand = new SqlCommand(insertOrderProductQuery, connection, transaction);
                            orderProductCommand.Parameters.AddWithValue("@OrderId", orderId);
                            orderProductCommand.Parameters.AddWithValue("@ProductId", GetProductIdByName(item.Name, connection, transaction));
                            orderProductCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            orderProductCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int GetProductIdByName(string productName, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT product_id FROM Products WHERE Name = @ProductName";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@ProductName", productName);
            return (int)command.ExecuteScalar();
        }
    }

    [System.Runtime.InteropServices.ComVisible(true)]
    public class ScriptInterface
    {
        private CartWindow _window;

        public ScriptInterface(CartWindow window)
        {
            _window = window;
        }

        public void SetAddress(string address)
        {
            _window.SetAddress(address);
        }
    }
}
