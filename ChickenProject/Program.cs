using System;
using System.Threading;

namespace ChickenProject
{
    public delegate void priceCutEvent(Int32 pr);

    public class ChickenFarm
    {
        static Random rng = new Random();
        public static event priceCutEvent priceCut;
        private static Int32 chickenPrice = 10;

        public Int32 getPrice()
        {
            return chickenPrice;
        }

        public static void changePrice(Int32 price)
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
                    Int32 p = chicken.getPrice();
                    Console.WriteLine("Store {0} has everyday low price: ${1} each", Thread.CurrentThread.Name, p);
                }
            }

            public void chickenOnSale(Int32 p)
            {
                //order chickens from chicken farm - send order into queue
                Console.WriteLine("Store {0} chickens are on sale: as low as ${1} each", Thread.CurrentThread.Name, p);
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
            Console.WriteLine("Name: "+senderId+"\nCard: "+cardNo+"\nAmount: "+amount);
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

        public class myApplication
        {
            static void Main(string[] args)
            {
            OrderClass order = new OrderClass();
            order.SenderId = "Jesse";
            order.CardNo = 41234;
            order.Amount = 10094;
            order.toString();
            String encoded = Encoder.Encode(order);
            OrderClass decodedOrder = Decoder.Decode(encoded);
            decodedOrder.toString();
                /*ChickenFarm chicken = new ChickenFarm();
                Thread farmer = new Thread(new ThreadStart(chicken.farmerFunc));
                farmer.Start();
                Retailer chickenstore = new Retailer();
                ChickenFarm.priceCut += new priceCutEvent(chickenstore.chickenOnSale);
                Thread[] retailers = new Thread[3];
                for (int i = 0; i < 3; i++)
                {
                    retailers[i] = new Thread(new ThreadStart(chickenstore.retailerFunc));
                    retailers[i].Start();
                }*/
            }
        }
}
