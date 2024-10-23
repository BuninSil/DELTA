using System;
using System.Windows;

namespace DELTA
{
    public partial class ProductEditor : Window
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; }

        public ProductEditor(string productName = "", decimal price = 0, int stock = 0, string imagePath = "")
        {
            InitializeComponent();

            // Предзаполняем поля, если редактируем товар
            ProductNameTextBox.Text = productName;
            PriceTextBox.Text = price.ToString();
            StockTextBox.Text = stock.ToString();
            ImagePathTextBox.Text = imagePath;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверки на корректность данных
                if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                {
                    throw new Exception("Название товара не может быть пустым.");
                }

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    throw new Exception("Некорректная цена. Введите корректное число.");
                }

                if (!int.TryParse(StockTextBox.Text, out int stock))
                {
                    throw new Exception("Некорректное количество на складе. Введите целое число.");
                }

                // Записываем данные обратно в свойства
                ProductName = ProductNameTextBox.Text;
                Price = price;
                Stock = stock;
                ImagePath = ImagePathTextBox.Text;

                // Закрываем окно и возвращаем результат
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
