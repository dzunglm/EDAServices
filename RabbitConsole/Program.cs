using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitConsole.Data;
using RabbitConsole;
using Newtonsoft.Json.Linq;
using System.Text;
//Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
var factory = new ConnectionFactory
{
    HostName = "localhost"
};
//Create the RabbitMQ connection using connection factory details as i mentioned above
var connection = factory.CreateConnection();
//Here we create channel with session and model
using var channel = connection.CreateModel();
//declare the queue after mentioning name and a few property related to that
//channel.QueueDeclare("learner.postenrollment", exclusive: false);
//Set Event object whic
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message received: {message}");
    var data = JObject.Parse(message);
    var type = eventArgs.RoutingKey;
    if (type == "learner.add")
    {
        var dbContext = new SchoolContext();
        
        dbContext.Enrollments.Add(new Enrollment()
        {
            LearnerId = data["id"].Value<int>(),
            CourseId = 4,
            Grade = 0
        });
        dbContext.SaveChanges();
    }
};
//read the message
channel.BasicConsume(queue: "learner.postenrollment", autoAck: true, consumer: consumer);
Console.ReadKey();