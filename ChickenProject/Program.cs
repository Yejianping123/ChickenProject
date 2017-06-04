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
            String processing = myApplication.buffer.getOneCell();
            OrderClass orderToProcess = Decoder.Decode(processing);
            OrderProcessing process = new OrderProcessing(orderToProcess, price);
            
        }

        public void PricingModel()
        {
            while (p < 10)
            {
                Thread.Sleep(500);
                double price = 0;
                Decimal fff = rng.Next(1,60);
                Decimal what = fff / 100;
                double model = (double)what;
                price = getPrice();
                //Console.WriteLine("Old Price: "+price);
                if(fff%2 == 0)
                {
                   // Console.WriteLine("Price increased to: " + price+"\n");
                    price = price + model;
                }
                else
                {
                    
                    price = price - model;
                    p++;
                   // Console.WriteLine("Price dropped to: " + price + "\nThis is price drop #: " + p + "\n");
                }
                Console.WriteLine("New Price: " + price);
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
       // public ChickenFarm chicken = new ChickenFarm();
        public double price;
        public bool needorder = false;
        public TimeSpan orderSent;
        public void retailerFunc()
        {
            ChickenFarm chicken = new ChickenFarm();
            price = chicken.getPrice();
            Console.WriteLine("Store{0} initial chicken Price: {1}", Thread.CurrentThread.Name,price);
            while(true)
            {
                if(needorder==true)
                {
                    OrderClass order = new OrderClass();
                    order.SenderId = Thread.CurrentThread.Name;
                    order.CardNo = 5000 + int.Parse(Thread.CurrentThread.Name);
                    order.Amount = 25;
                    String encodedorder = Encoder.Encode(order);
                    orderSent = DateTime.Now.TimeOfDay;
                    myApplication.buffer.setOneCell(encodedorder);
                    needorder = false;
                }
            }         
        }

        public void chickenOnSale(double p)
        {
            price = p;
            needorder = true;
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
           // Console.WriteLine("Encoded Order: " + encoded);
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
        //public static void confirmOrder(OrderClass orderObject, int unitPrice)
        public OrderProcessing(OrderClass orderObject, double unitPrice)
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


    public class MultiCellBuffer
    {
        int N;
        String[] cells;
        int loc = 0;
        public MultiCellBuffer()
        {
            N = 5;
            cells = new String[5];
            for(int i=0;i<N;i++)
            {
                cells[i] = "";
            }
            
        }
        public void setOneCell(String encodedOrder)
        {
            Console.WriteLine("You got inside setOneCell, LOC= " + loc);
            cells[loc] = encodedOrder;
            loc++;
        }

        public String getOneCell()
        {
            loc--;
            Console.WriteLine("You got inside getonecell, LOC= " + loc);
            return cells[loc];
        }
    }

    public class myApplication
    {
        public static MultiCellBuffer buffer = new MultiCellBuffer();
        static void Main(string[] args)
        {
            /* OrderClass order = new OrderClass();
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
             */
            ChickenFarm chicken = new ChickenFarm();
            Thread farmer = new Thread(new ThreadStart(chicken.PricingModel));
            farmer.Start();
            Retailer store = new Retailer();
            //Thread calc = new Thread(new ThreadStart(store.chicken.PricingModel));
            //calc.Start();
            ChickenFarm.priceCut += new priceCutEvent(store.chickenOnSale);
            Thread[] retailers = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                retailers[i] = new Thread(new ThreadStart(store.retailerFunc));
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
            }

            /*
            ChickenFarm chicken = new ChickenFarm();
            Thread farmer = new Thread(new ThreadStart(chicken.PricingModel));
            farmer.Start();
            Retailer chickenstore = new Retailer();
            ChickenFarm.priceCut += new priceCutEvent(chickenstore.chickenOnSale);
            Thread[] retailers = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                retailers[i] = new Thread(new ThreadStart(chickenstore.retailerFunc));
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
                while (!retailers[i].IsAlive) ;
            }
            */

            /*
             ChickenFarm chicken = new ChickenFarm();
             Thread farmer = new Thread(new ThreadStart(chicken.farmerFunc));
             farmer.Start();
             Retailer chickenstore = new Retailer();
             ChickenFarm.priceCut += new priceCutEvent(chickenstore.chickenOnSale);
             Thread[] retailers = new Thread[5];
             for (int i = 0; i < 5; i++)
             {
                 retailers[i] = new Thread(new ThreadStart(chickenstore.retailerFunc));
                 retailers[i].Start();
             }
             */
        }
    }
}
