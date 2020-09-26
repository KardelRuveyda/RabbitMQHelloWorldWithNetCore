using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQPublisher
{
    class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            //Rabbitmqye ulaşmak için

            factory.Uri = new Uri("amqp://guest:guest@localhost:5672/%2F");
            //factory.HostName = "localhost";


            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //durable false yapılırsa, restart yapılırsa mesaj gider.
                    //True yapılırsa fiziksel liste yazar.
                    //false : memoryde dursun daha hızlı yazma imkanı.
                    channel.QueueDeclare("task_queue", false, false, false, null);

                    string Message = GetMessage(args);

                    for (int i = 1; i < 11; i++)
                    {

                        var bodyByte = Encoding.UTF8.GetBytes($"{Message} - {i}");

                        //Mesaja sağlama alma.
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //mesajlar byte dizisi olmalı
                        //instance çökse bile mesaj duruyor olacak. bu sayede proporties gönderildi.
                        channel.BasicPublish("", routingKey: "task_queue",properties, body: bodyByte);
                        Console.WriteLine($"Mesajınız Gönderilmiştir:{Message}-{i}");

                    }



                }

                Console.WriteLine("Çıkış yapmak için tıklayınız.");
                Console.ReadLine();



            }
        }

         
        private static string GetMessage(string[] args)
        {
           return args[0].ToString();
        }
    }
}
