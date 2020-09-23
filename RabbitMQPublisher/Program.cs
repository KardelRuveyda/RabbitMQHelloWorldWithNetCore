using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQPublisher
{
    class Program
    {
        static void Main(string[] args)
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
                    channel.QueueDeclare("hello", false, false, false, null);

                    string Message = "Hello World";

                    var bodyByte = Encoding.UTF8.GetBytes(Message);

                    //mesajlar byte dizisi olmalı
                    channel.BasicPublish("", routingKey: "hello", null, body: bodyByte);
                    Console.WriteLine("Mesajınız Gönderilmiştir. ");

                }

                Console.WriteLine("Çıkış yapmak için tıklayınız.");
                Console.ReadLine();



            }
        }
    }
}
