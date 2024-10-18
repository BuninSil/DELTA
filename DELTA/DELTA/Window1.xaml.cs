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

        public Window1()
        {
            InitializeComponent();
            ShoppingCart = new Cart();
            LoadProducts();
        }

        public class Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Stock { get; set; }
            public string ImagePath { get; set; }
            public int Quantity { get; set; }  // Количество товара в корзине
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

            public decimal TotalPrice
            {
                get
                {
                    return Items.Sum(item => item.Price);
                }
            }
        }

        private void LoadProducts()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True;";
            string query = "SELECT Name, Price, stock, ImagePath FROM Products";

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
                                Name = reader["Name"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
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
            var cartWindow = new CartWindow { DataContext = ShoppingCart };
            cartWindow.Show();
        }
    }
}
