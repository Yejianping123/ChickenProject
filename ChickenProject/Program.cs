using System;
using System.Threading;

namespace ChickenProject
{
    public delegate void priceCutEvent(double pr);
    public delegate void orderCompletedEvent();
    public class ChickenFarm
    {
        static Random rng = new Random();
        public static event priceCutEvent priceCut;
        private static double chickenPrice = 10;
        private int p = 0;
        public double getPrice()
        {
            return chickenPrice;
        }

        public static void changePrice(double price)
        {
            if (price < chickenPrice)
            {
                if (priceCut != null)
                {
                    priceCut(price);

                }
            }
            chickenPrice = price;
        }

        public void PricingModel()
        {
            while (p < 10)
            {
                double price = 0;
                Decimal fff = rng.Next(1,60);
                Decimal what = fff / 100;
                double model = (double)what;
                price = getPrice();
                if(fff%2 == 0)
                {
                    price = price + model;
                }
                else
                {
                    price = price - model;
                    p++;
                }
                changePrice(price);
            }
        }
        public void farmerFunc()
        {
            for (Int32 i = 0; i < 50; i++)
            {
                Thread.Sleep(500);
                //Take order from the queue of the orders
                //Decide the price based on the orders
                Int32 p = rng.Next(5, 10);
                //Console.WriteLine("New Price is {0}", p);
                ChickenFarm.changePrice(p);
            }
        }
    }

    public class Retailer
    {
        public void retailerFunc()
        {
            ChickenFarm chicken = new ChickenFarm();
            for (Int32 i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                double p = chicken.getPrice();
                Console.WriteLine("Store{0} has everyday low price: ${1} each", Thread.CurrentThread.ManagedThreadId, p);
            }
        }

        public void chickenOnSale(Int32 p)
        {
            //order chickens from chicken farm - send order into queue
            Console.WriteLine("Store{0} chickens are on sale: as low as ${1} each", Thread.CurrentThread.ManagedThreadId, p);
        }
    }

    public class OrderClass
    {
        private String senderId;
        private int cardNo;
        private int amount;

        public String SenderId
        {
            get
            {
                return senderId;
            }
            set
            {
                senderId = value;
            }
        }

        public int CardNo
        {
            get
            {
                return cardNo;
            }
            set
            {
                cardNo = value;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
            }
        }

        public void toString()
        {
            Console.WriteLine("Name: " + senderId + "\nCard: " + cardNo + "\nAmount: " + amount);
        }
    }

    public class Encoder
    {
        public static String Encode(OrderClass orderObject)
        {
            String encoded = orderObject.SenderId + "/" + orderObject.CardNo + "/" + orderObject.Amount;
            Console.WriteLine("Encoded string: " + encoded);
            return encoded;
        }
    }

    public class Decoder
    {
        public static OrderClass Decode(String encoded)
        {
            var parts = encoded.Split('/');
            String senderId = parts[0];
            int cardNo = int.Parse(parts[1]);
            int amount = int.Parse(parts[2]);
            OrderClass orderObject = new OrderClass();
            orderObject.SenderId = senderId;
            orderObject.CardNo = cardNo;
            orderObject.Amount = amount;
            return orderObject;
        }
    }

    public class OrderProcessing
    {
        public static void confirmOrder(OrderClass orderObject, int unitPrice)
        {
            double tax = .05;
            double shippingHandling;
            if(orderObject.CardNo>=5000 && orderObject.CardNo<=7000)
            {
                int NoOfChickens = orderObject.Amount;
                double total = 0;
                total = NoOfChickens * unitPrice;
                if (NoOfChickens < 50)
                {
                    shippingHandling = 100;
                }
                else
                {
                    shippingHandling = 50;
                }
                total = total + (total * tax) + shippingHandling;

                Console.WriteLine("Order from {0}\n\tAmount: {1}\n\tCredit Card Number: {2}\n\tChicken Price: {3}\n\tOrder Cost: {4}",
orderObject.SenderId, orderObject.Amount, orderObject.CardNo, unitPrice, total);
            }
        }
    }

    public class myApplication
    {
        static void Main(string[] args)
        {
            OrderClass order = new OrderClass();
            order.SenderId = "Jesse";
            order.CardNo = 5155;
            order.Amount = 10094;
            String encoded = Encoder.Encode(order);
            OrderClass decodedOrder = Decoder.Decode(encoded);         
            OrderProcessing.confirmOrder(decodedOrder, 7);
            ChickenFarm chicken = new ChickenFarm();
            while(true)
            {
                Thread.Sleep(500);
                chicken.PricingModel();
            }
           /* ChickenFarm chicken = new ChickenFarm();
            Thread farmer = new Thread(new ThreadStart(chicken.farmerFunc));
            farmer.Start();
            Retailer chickenstore = new Retailer();
            ChickenFarm.priceCut += new priceCutEvent(chickenstore.chickenOnSale);
            Thread[] retailers = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                retailers[i] = new Thread(new ThreadStart(chickenstore.retailerFunc));
                retailers[i].Start();
            }*/
        }
    }
}
