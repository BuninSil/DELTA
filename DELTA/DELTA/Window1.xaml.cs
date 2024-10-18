using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DELTA
{
    public partial class Window1 : Window
    {
        public ObservableCollection<Product> Products { get; set; }
        public Cart ShoppingCart { get; set; }
        
        private int userId;
        public Window1(int userId)
        {
            InitializeComponent();
            ShoppingCart = new Cart();
            this.userId = userId;
            
            LoadProducts();
        }

        public class Product
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Stock { get; set; }
            public string ImagePath { get; set; }
            public int Quantity { get; set; }
        }

        public class Cart
        {
            public ObservableCollection<Product> Items { get; private set; }

            public Cart()
            {
                Items = new ObservableCollection<Product>();
            }

            public void AddToCart(Product product)
            {
                var existingProduct = Items.FirstOrDefault(p => p.Name == product.Name);
                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {
                    product.Quantity = 1;
                    Items.Add(product);
                }
            }

            public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
        }

        private void LoadProducts()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True;";
            string query = "SELECT product_id, name, price, stock, ImagePath FROM Products";

            Products = new ObservableCollection<Product>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Products.Add(new Product
                            {
                                ProductId = (int)reader["product_id"],
                                Name = reader["name"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                Stock = reader["stock"].ToString(),
                                ImagePath = reader["ImagePath"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }

            DataContext = this;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Product;
            ShoppingCart.AddToCart(product);
            MessageBox.Show($"{product.Name} добавлен в корзину.");
        }

        private void OpenCart_Click(object sender, RoutedEventArgs e)
        {
            var cartWindow = new CartWindow(userId) { DataContext = ShoppingCart };
            cartWindow.Show();
        }

        private void PlaceOrder(int userId)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Создать новый заказ в таблице Orders
                        string insertOrderQuery = "INSERT INTO Orders (user_id, order_date, status) OUTPUT INSERTED.order_id VALUES (@UserId, GETDATE(), 'placed')";
                        SqlCommand insertOrderCommand = new SqlCommand(insertOrderQuery, connection, transaction);
                        insertOrderCommand.Parameters.AddWithValue("@UserId", userId);

                        int orderId = (int)insertOrderCommand.ExecuteScalar();

                        // 2. Добавить продукты в OrderProducts
                        foreach (var item in ShoppingCart.Items)
                        {
                            string insertOrderProductQuery = "INSERT INTO OrderProducts (order_id, product_id, quantity) VALUES (@OrderId, @ProductId, @Quantity)";
                            SqlCommand insertOrderProductCommand = new SqlCommand(insertOrderProductQuery, connection, transaction);
                            insertOrderProductCommand.Parameters.AddWithValue("@OrderId", orderId);
                            insertOrderProductCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                            insertOrderProductCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            insertOrderProductCommand.ExecuteNonQuery();
                        }

                        // 3. Подтвердить транзакцию
                        transaction.Commit();
                        MessageBox.Show("Заказ успешно оформлен!");
                    }
                    catch (Exception ex)
                    {
                        // В случае ошибки откатить транзакцию
                        transaction.Rollback();
                        MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
                    }
                }
            }
        }

        private void ShowOrders_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow(userId);
            ordersWindow.Show();
        }
    }
}
