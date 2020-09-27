using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQConsumer
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

                    channel.ExchangeDeclare("logs", ExchangeType.Fanout, durable: true);

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");
                    
                    
                    //Bana bir tane mesaj geldi prefetchCount, bunu hallettikten sonra bana tekrar mesaj gönder
                    //O nedenle 1 veriyorum. 2 tane işlesin düşünülürse; false yerine true denilebilirdi
                    //1 yerine 10 derseniz ve true derseniz.
                    //Bu instance den 5 tane oluşturursanız. 
                    //Bu 5 tane instance toplam 10 tan mesaj alabilir tek seferde(true) dersem. 
                    //False dersem iki kol da ayrı ayrı 10 tane mesaj alabilir 
                    
                    channel.BasicQos(prefetchSize:0,prefetchCount:1, false);

                    Console.WriteLine("Logları bekliyorum..");

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queueName, autoAck:false, consumer);
                    consumer.Received += (model, ea) =>
                    {   
                        
                        var log = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine("Log alındı : " + log);

                        int time = int.Parse(GetMessage(args));
                        Thread.Sleep(time);
                        Console.WriteLine("Loglama bitti...");

                        channel.BasicAck(ea.DeliveryTag, false);
                        //Mesaj başarıyla işlendi. Kuyruktan silebilirsin anlamına gelir. 
                        // Mesajlar sırayla dağıtıacak birer birer dağıtılacak. 
                    };

                    Console.WriteLine("Çıkış yapmak için tıklayınız.");
                    Console.ReadLine();

                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }

    }
}
