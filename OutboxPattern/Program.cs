using Microsoft.EntityFrameworkCore;
using OutboxPattern.Infrastructure;
using MediatR;
using OutboxPattern.Infrastructure.Interceptors;
using OutboxPattern.Infrastructure.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ConertDomainEventsToOutboxMessagesInterceptor>();
builder.Services.AddDbContext<OrderingDbContext>((sp, optionsBuilder) =>
{ 
    var interceptor=sp.GetService<ConertDomainEventsToOutboxMessagesInterceptor>();
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).AddInterceptors(interceptor);
}
) ;
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddHostedService<ProccessOutboxMessageBackgroundService>();

builder.Services.AddCap(x =>
{
    x.UseSqlServer(options => {
        //SqlServerOptions
        options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.Schema = "cap";
    });
    //x.UseRabbitMQ("localhost:15672");
    x.UseRabbitMQ(o =>
    {
        o.HostName = "localhost";
        o.ConnectionFactoryOptions = options =>
        {
            //options.Port = 15672;
            options.UserName = "guest";
            options.Password = "guest";
        };
    });

});

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
