using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMqPublisher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Mina!");
            Console.WriteLine("Connecting...");

            var factory = new ConnectionFactory
            {
                HostName = "54.235.226.35",
                Port = 5673,
                UserName = "admin",
                Password = "admin"
                //UserName = "localhost"
            };

            using var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "ProfileCreatedQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            Console.WriteLine("Press any key to execute the action. Press ESC to exit.");

            
            int No;
            

            while (true)
            {
                Console.Write("Enter starting user number: ");

                while (!int.TryParse(Console.ReadLine(), out No))
                {
                    Console.Write("Invalid input. Please enter a valid integer: ");
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Exiting...");
                    break;
                }

                Console.WriteLine($"Adding User No. {No}...");
                await CreateProfile(No, channel);
            }


        }

        static async Task CreateProfile(int uNo, IChannel channel)
        {
            var user = new
            {
                UserId = $"u{uNo}"
            };

            var message = JsonSerializer.Serialize(user);

            var bin = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "ProfileCreatedQueue",
                mandatory: true,
                basicProperties: new BasicProperties
                {
                    Persistent = true
                },
                body: bin);
        }
    }
}
