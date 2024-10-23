using System;
using System.Data.SqlClient;
using System.Windows;

namespace DELTA
{
    public partial class ManagerWindow : Window
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True";

        public ManagerWindow(int userId)
        {
            InitializeComponent();
            LoadProducts(); // Загружаем список товаров при инициализации окна
        }

        // Метод для загрузки списка товаров из базы данных
        private void LoadProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Products";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    var products = new System.Collections.ObjectModel.ObservableCollection<Product>();

                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            product_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            price = reader.GetDecimal(2),
                            stock = reader.GetInt32(3),
                            imagepath = reader.IsDBNull(4) ? null : reader.GetString(4)
                        });
                    }

                    ProductsDataGrid.ItemsSource = products;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                }
            }
        }

        // Метод для добавления товара
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductEditor editor = new ProductEditor();
            if (editor.ShowDialog() == true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "INSERT INTO Products (name, price, stock, imagepath) VALUES (@name, @price, @stock, @imagepath)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@name", editor.ProductName);
                        command.Parameters.AddWithValue("@price", editor.Price);
                        command.Parameters.AddWithValue("@stock", editor.Stock);
                        command.Parameters.AddWithValue("@imagepath", string.IsNullOrWhiteSpace(editor.ImagePath) ? (object)DBNull.Value : editor.ImagePath);
                        command.ExecuteNonQuery();
                        LoadProducts(); // Обновляем таблицу после добавления товара
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка добавления товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        // Метод для редактирования товара
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                // Открываем окно ProductEditor с текущими данными
                ProductEditor editor = new ProductEditor(
                    selectedProduct.name,
                    selectedProduct.price,
                    selectedProduct.stock,
                    selectedProduct.imagepath
                );

                if (editor.ShowDialog() == true)
                {
                    // Проверяем, если данные были сохранены
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "UPDATE Products SET name = @name, price = @price, stock = @stock, imagepath = @imagepath WHERE product_id = @product_id";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@product_id", selectedProduct.product_id);
                            command.Parameters.AddWithValue("@name", editor.ProductName);
                            command.Parameters.AddWithValue("@price", editor.Price);
                            command.Parameters.AddWithValue("@stock", editor.Stock);
                            command.Parameters.AddWithValue("@imagepath", string.IsNullOrWhiteSpace(editor.ImagePath) ? (object)DBNull.Value : editor.ImagePath);
                            command.ExecuteNonQuery();
                            LoadProducts(); // Обновляем таблицу после редактирования товара
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка редактирования товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Метод для удаления товара
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM Products WHERE product_id = @product_id";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@product_id", selectedProduct.product_id);
                        command.ExecuteNonQuery();
                        LoadProducts(); // Обновляем таблицу после удаления товара
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления товара: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OrderManagementWindow order = new OrderManagementWindow();
            order.Show();
        }
    }

    // Класс для описания товара
    public class Product
    {
        public int product_id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int stock { get; set; }
        public string imagepath { get; set; }
    }
}
