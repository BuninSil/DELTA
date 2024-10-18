using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace DELTA
{
    public partial class OrdersWindow : Window
    {
        public ObservableCollection<Order> Orders { get; set; }

        private int userId;

        public OrdersWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadOrders();
        }

        public class Order
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public string Status { get; set; }
            public ObservableCollection<OrderProduct> Products { get; set; }
        }

        public class OrderProduct
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
        }

        private void LoadOrders()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=delta;Integrated Security=True;";
            string query = "SELECT o.order_id, o.order_date, o.status, op.product_id, p.name, op.quantity " +
                           "FROM Orders o " +
                           "JOIN OrderProducts op ON o.order_id = op.order_id " +
                           "JOIN Products p ON op.product_id = p.product_id " +
                           "WHERE o.user_id = @UserId";

            Orders = new ObservableCollection<Order>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Order currentOrder = null;
                        while (reader.Read())
                        {
                            int orderId = (int)reader["order_id"];
                            if (currentOrder == null || currentOrder.OrderId != orderId)
                            {
                                currentOrder = new Order
                                {
                                    OrderId = orderId,
                                    OrderDate = (DateTime)reader["order_date"],
                                    Status = reader["status"].ToString(),
                                    Products = new ObservableCollection<OrderProduct>()
                                };
                                Orders.Add(currentOrder);
                            }

                            currentOrder.Products.Add(new OrderProduct
                            {
                                Name = reader["name"].ToString(),
                                Quantity = (int)reader["quantity"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказов: " + ex.Message);
            }

            DataContext = this;
        }
    }
}
