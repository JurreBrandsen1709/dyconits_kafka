using Confluent.Kafka;
using System;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Timers;
using System.IO;
using Dyconit.Producer;
using Dyconit.Overlord;

class Producer {
    static void Main(string[] args)
    {
        // configure bootstrap.servers in text
        var configuration = new ProducerConfig
        {
            BootstrapServers = "localhost:19091,localhost:19092, localhost:19093",
            // StatisticsIntervalMs = 2000,
        };

        // var adminClient = new DyconitOverlord("127.0.0.1:19092", 2000);

        const string topic = "input_topicc";

        using (var producer = new ProducerBuilder<Null, String>(configuration).Build())
        {
            Console.WriteLine("Press Ctrl+C to quit.");

            var numProduced = 0;
            Random rnd = new Random();
            const int numMessages = 5000;

            // Set up a timer to send 5 messages every second
            var timer = new Timer(1000); // 1000 milliseconds = 1 second
            timer.Elapsed += (sender, e) => {
                for (int i = 0; i < 5; i++) {

                    // create a random length payload string
                    var payload = new string('x', rnd.Next(1, 1000));

                    var message = new DyconitMessage<Null, string>
                    {
                        Value = payload,
                        Weight = 3
                    };

                    var deliveryReport = producer.ProduceAsync(topic, message).GetAwaiter().GetResult();
                    Console.WriteLine($"T: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - {numProduced}");
                    numProduced += 1;
                }
            };
            timer.Start();

            // Wait until all messages have been sent
            while (numProduced < numMessages) {
                // You can do something else while waiting here
            }

            timer.Stop();

            producer.Flush(TimeSpan.FromSeconds(10));
            Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
        }
    }
}
