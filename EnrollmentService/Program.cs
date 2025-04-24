using EnrollmentService.Data;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SchoolContext>(options => options
.UseSqlServer(builder.Configuration.GetConnectionString("SchoolContext")));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.Run();

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
