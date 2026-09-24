using System;
using System.Collections.Generic;

namespace Assignment08
{
    // =========================================================================
    // SECTION 01: BOOK LIBRARY SYSTEM
    // =========================================================================

    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }

        public Book(string _ISBN, string _Title, string[] _Authors, DateTime _PublicationDate, decimal _Price)
        {
            ISBN = _ISBN;
            Title = _Title;
            Authors = _Authors;
            PublicationDate = _PublicationDate;
            Price = _Price;
        }

        public override string ToString()
        {
            string authorsStr = Authors != null ? string.Join(", ", Authors) : "No authors";
            return $"ISBN: {ISBN}, Title: {Title}, Authors: {authorsStr}, Date: {PublicationDate.ToShortDateString()}, Price: {Price:C}";
        }
    }

    public class BookFunctions
    {
        public static string GetTitle(Book B)
        {
            if (B == null) return "No Book";
            return B.Title;
        }

        public static string GetAuthors(Book B)
        {
            if (B == null || B.Authors == null) return "No Authors";
            return string.Join(", ", B.Authors);
        }

        public static string GetPrice(Book B)
        {
            if (B == null) return "0";
            return B.Price.ToString("C");
        }
    }

    // User-Defined Delegate for Section 01
    public delegate string BookDelegate(Book B);

    public class LibraryEngine
    {
        // Parameterized with User-Defined Delegate
        public static void ProcessBooks(List<Book> bList, BookDelegate fPtr)
        {
            if (bList == null || fPtr == null) return;
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }

        // Parameterized with Built-in Delegate Func<Book, string>
        public static void ProcessBooks(List<Book> bList, Func<Book, string> fPtr)
        {
            if (bList == null || fPtr == null) return;
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }
    }


    // =========================================================================
    // SECTION 02: ORDER PROCESSING SYSTEM & EVENTS
    // =========================================================================

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Order(int id, string customerName, decimal price, int quantity)
        {
            Id = id;
            CustomerName = customerName;
            Price = price;
            Quantity = quantity;
        }
    }

    // Part 1: User-Defined Delegate
    public delegate decimal PriceCalculator(Order order);

    public class OrderService
    {
        // Part 5: Event Declaration
        public event Action<Order> OrderProcessed;

        // Part 1: Calculate price using custom delegate
        public decimal CalculateOrderPrice(Order order, PriceCalculator calculator)
        {
            return calculator(order);
        }

        // Part 2 & Bonus Challenge: Calculate price using Func<> strategy
        public decimal CalculateOrderPrice(Order order, Func<Order, decimal> calculator)
        {
            return calculator(order);
        }

        // Part 3: Validate order using Predicate<>
        public bool ValidateOrder(Order order, Predicate<Order> validationRule)
        {
            return validationRule(order);
        }

        // Part 4: Process order with Action<>
        public void ProcessOrder(Order order, Action<Order> action)
        {
            action(order);
        }

        // Part 5 & 6: Process order and raise event
        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\n[OrderService] Processing Order #{order.Id} for {order.CustomerName}...");
            
            // Raise the event if there are subscribers
            OrderProcessed?.Invoke(order);
        }
    }


    // =========================================================================
    // MAIN PROGRAM (DEMONSTRATION & ANSWERS)
    // =========================================================================

    internal class Program
    {
        static void Main(string[] args)
        {
            // -------------------------------------------------------------------------
            // SECTION 01 DEMO
            // -------------------------------------------------------------------------
            Console.WriteLine("=========================================================================");
            Console.WriteLine("                         SECTION 01: BOOK LIBRARY                        ");
            Console.WriteLine("=========================================================================");

            List<Book> books = new List<Book>
            {
                new Book("123-ABC", "C# Basics", new string[] { "John Doe", "Jane Smith" }, new DateTime(2022, 5, 10), 45.50m),
                new Book("456-DEF", "ASP.NET Core", new string[] { "Alex Green" }, new DateTime(2023, 8, 15), 60.00m)
            };

            Console.WriteLine("\n--- Case 1: User-Defined Delegate (GetTitle) ---");
            BookDelegate titleDel = BookFunctions.GetTitle;
            LibraryEngine.ProcessBooks(books, titleDel);

            Console.WriteLine("\n--- Case 2: Built-in Delegate Func (GetPrice) ---");
            Func<Book, string> priceFunc = BookFunctions.GetPrice;
            LibraryEngine.ProcessBooks(books, priceFunc);

            Console.WriteLine("\n--- Case 3: Anonymous Method (GetISBN) ---");
            Func<Book, string> isbnFunc = delegate (Book B)
            {
                return B != null ? B.ISBN : "No ISBN";
            };
            LibraryEngine.ProcessBooks(books, isbnFunc);

            Console.WriteLine("\n--- Case 4: Lambda Expression (GetPublicationDate) ---");
            LibraryEngine.ProcessBooks(books, B => B.PublicationDate.ToShortDateString());


            // -------------------------------------------------------------------------
            // SECTION 02 DEMO
            // -------------------------------------------------------------------------
            Console.WriteLine("\n\n=========================================================================");
            Console.WriteLine("                    SECTION 02: ORDER PROCESSING SYSTEM                  ");
            Console.WriteLine("=========================================================================");

            Order myOrder = new Order(101, "Ahmed Saeed", 100.00m, 3); // Total base = 300
            OrderService service = new OrderService();

            // --- Part 1: User-Defined Delegate ---
            Console.WriteLine("\n--- Part 1: User-Defined Delegate ---");
            PriceCalculator totalCalc = order => order.Price * order.Quantity;
            PriceCalculator discountCalc = order => (order.Price * order.Quantity) - 20;

            Console.WriteLine("Total Price: " + service.CalculateOrderPrice(myOrder, totalCalc));
            Console.WriteLine("Price with Discount: " + service.CalculateOrderPrice(myOrder, discountCalc));


            // --- Part 2 & Bonus Challenge: Func<> Pricing Strategies ---
            Console.WriteLine("\n--- Part 2 & Bonus: Func<> Pricing Strategies ---");
            Func<Order, decimal> normalPrice = x => x.Price * x.Quantity;
            Func<Order, decimal> discount10 = x => (x.Price * x.Quantity) * 0.90m;
            Func<Order, decimal> discount20 = x => (x.Price * x.Quantity) * 0.80m;
            Func<Order, decimal> vipDiscount = x => (x.Price * x.Quantity) * 0.70m;

            Console.WriteLine("Normal Price: " + service.CalculateOrderPrice(myOrder, normalPrice));
            Console.WriteLine("10% Discount Price: " + service.CalculateOrderPrice(myOrder, discount10));
            Console.WriteLine("20% Discount Price: " + service.CalculateOrderPrice(myOrder, discount20));
            Console.WriteLine("VIP Discount Price: " + service.CalculateOrderPrice(myOrder, vipDiscount));


            // --- Part 3: Predicate<> Validations ---
            Console.WriteLine("\n--- Part 3: Predicate<> Order Validation ---");
            bool isQuantityValid = service.ValidateOrder(myOrder, o => o.Quantity > 0);
            bool isPriceValid = service.ValidateOrder(myOrder, o => o.Price > 0);
            bool hasCustomerName = service.ValidateOrder(myOrder, o => !string.IsNullOrEmpty(o.CustomerName));

            Console.WriteLine("Is Quantity > 0? " + isQuantityValid);
            Console.WriteLine("Is Price > 0? " + isPriceValid);
            Console.WriteLine("Has Customer Name? " + hasCustomerName);


            // --- Part 4: Action<> Behaviors ---
            Console.WriteLine("\n--- Part 4: Action<> Process Actions ---");
            Action<Order> printInfo = o => Console.WriteLine($"[Info Action] Order ID: {o.Id}, Customer: {o.CustomerName}");
            Action<Order> sendConfirmation = o => Console.WriteLine($"[Email Action] Confirmation email sent to {o.CustomerName}.");
            Action<Order> writeAudit = o => Console.WriteLine($"[Audit Action] Audit log saved for Order ID: {o.Id}");

            service.ProcessOrder(myOrder, printInfo);
            service.ProcessOrder(myOrder, sendConfirmation);
            service.ProcessOrder(myOrder, writeAudit);


            // --- Part 5 & 6: Events, Subscriptions & Unsubscribing ---
            Console.WriteLine("\n--- Part 5 & 6: Events (Multicast, Subscribe & Unsubscribe) ---");
            
            // Subscribing Handlers
            service.OrderProcessed += Handler1_PrintMessage;
            service.OrderProcessed += Handler2_SendNotification;
            service.OrderProcessed += Handler3_WriteAuditLog;

            Console.WriteLine("-> Triggering Event with 3 Subscribed Handlers:");
            service.ProcessOrder(myOrder);

            // Unsubscribing Handler1
            Console.WriteLine("\n-> Unsubscribing Handler1 (Print Message)...");
            service.OrderProcessed -= Handler1_PrintMessage;

            Console.WriteLine("-> Triggering Event again (Handler1 should not execute):");
            service.ProcessOrder(myOrder);


            Console.WriteLine("\n=========================================================================");
            Console.WriteLine("End of Code Output. Scroll down in source code to read Q&A answers.");
            Console.WriteLine("=========================================================================");

            Console.ReadLine();
        }

        // Event Handlers for Part 5 & 6
        static void Handler1_PrintMessage(Order order)
        {
            Console.WriteLine($"[Handler 1] Order #{order.Id} completed successfully.");
        }

        static void Handler2_SendNotification(Order order)
        {
            Console.WriteLine($"[Handler 2] Notification sent for customer {order.CustomerName}.");
        }

        static void Handler3_WriteAuditLog(Order order)
        {
            Console.WriteLine($"[Handler 3] Log entry written to database for Order #{order.Id}.");
        }
    }
}


/*
=================================================================================
                      QUESTIONS AND ANSWERS (SECTION 02)
=================================================================================

Q1: What is the difference between PriceCalculator and Func<Order, decimal>?
Answer:
- PriceCalculator is a custom user-defined delegate that needs explicit declaration.
- Func<Order, decimal> is a built-in generic delegate provided by C#.
- They do the same job, but Func save us time because we don't need to define custom delegates.

---------------------------------------------------------------------------------

Q2: What is the difference between Action<Order> and Func<Order, decimal>?
Answer:
- Action<Order> takes an Order as parameter and returns 'void' (no return value).
- Func<Order, decimal> takes an Order as parameter and returns a 'decimal' value.

---------------------------------------------------------------------------------

Q3: Why does Predicate<T> return bool? What kind of problem is it designed to represent?
Answer:
- Predicate<T> always returns bool because it is used for testing conditions and rules.
- It is designed for searching, filtering, and checking if an object matches a condition (like validation).

---------------------------------------------------------------------------------

Q4: What is the difference between a delegate and an event?
Answer:
- A Delegate is a type that holds references to methods and can be called anywhere.
- An Event is a wrapper over a delegate. It restricts access so outside code can only subscribe (+=) or unsubscribe (-=), but cannot invoke it directly or overwrite it.

---------------------------------------------------------------------------------

Q5: Why can't external code normally invoke an event declared in another class?
Answer:
- Because events are designed using the publisher-subscriber pattern.
- Only the class that owns/declares the event (the publisher) can invoke it. This protects the event from being triggered accidentally or malicious modification by outside code.

---------------------------------------------------------------------------------

Q6: What happens when multiple handlers subscribe to the same event?
Answer:
- The event becomes a "Multicast Delegate".
- When the event is raised, all subscribed handler methods are called one by one in the order they were added.

---------------------------------------------------------------------------------

Q7: Explain what this code means: orderService.OrderProcessed += HandleOrderProcessed;
Answer:
- orderService: The object instance of the class that has the event.
- OrderProcessed: The event name inside OrderService.
- += : The operator used to attach/subscribe a new method handler to the event list.
- HandleOrderProcessed: The target method that will run when the event is fired.

---------------------------------------------------------------------------------

Q8 (Challenge): Explain the difference between Action<Order> and event Action<Order>?
Answer:
- If we use public Action<Order>, external classes can overwrite it completely using '=' (which erases other listeners) or call it directly.
- If we use 'event Action<Order>', external classes are forced to use only '+=' and '-='. They cannot reset the subscriber list or trigger the event themselves, making the code much safer.

=================================================================================
*/