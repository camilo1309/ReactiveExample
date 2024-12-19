using System.Reactive.Linq;

class Order
{
    public string Product { get; }
    public int Quantity { get; }
    public double Price { get; }

    public Order(string product, int quantity, double price)
    {
        Product = product;
        Quantity = quantity;
        Price = price;
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Order> orders =
        [
            new("Product A", 2, 50.0),
            new("Product B", 1, 30.0),
            new("Product A", 3, 50.0),
            new("Product C", 1, 20.0),
            new("Product D", 5, 10.0)
        ];

        var promotions = new List<string> { "Product A", "Product C" };

        var ordersObservable = orders.ToObservable();

        var filteredOrders = ordersObservable
            .Where(order => promotions.Contains(order.Product));

        var orderTotals = filteredOrders
            .Select(order => new { order.Product, Total = order.Quantity * order.Price });

        var notifications = orderTotals
            .SelectMany(order =>
                Observable.Return($"Notification: Processed order for {order.Product} with total {order.Total:C}"));

        var mergedStream = ordersObservable
            .Select(order => $"Order received: {order.Product}")
            .Merge(notifications);

        var zippedStream = ordersObservable
            .Zip(Observable.Range(1, orders.Count), (order, id) =>
                $"Order ID: {id}, Product: {order.Product}, Quantity: {order.Quantity}, Price: {order.Price:C}");




        Console.WriteLine("Filtered Orders:");
        filteredOrders.Subscribe(order =>
            Console.WriteLine($"Product: {order.Product}, Quantity: {order.Quantity}, Price: {order.Price:C}"));

        Console.WriteLine("\nOrder Totals:");
        orderTotals.Subscribe(order =>
            Console.WriteLine($"Product: {order.Product}, Total: {order.Total:C}"));

        Console.WriteLine("\nMerged Stream:");
        mergedStream.Subscribe(Console.WriteLine);

        Console.WriteLine("\nZipped Stream:");
        zippedStream.Subscribe(Console.WriteLine);
    }
}
