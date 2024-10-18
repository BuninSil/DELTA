using System.Windows;

namespace DELTA
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заказ оформлен!");
            var cart = DataContext as Window1.Cart;
            cart.Items.Clear();  // Очищаем корзину после оформления заказа
        }
    }
}
