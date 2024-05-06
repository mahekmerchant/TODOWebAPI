using Serilog;
using TODOWebAPI.Data;
using TODOWebAPI.Repositories;

namespace TODOWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var logger = new LoggerConfiguration()
              .ReadFrom.Configuration(builder.Configuration)
              .Enrich.FromLogContext()
              .WriteTo.Debug()
              .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "TODOWebAPI.xml");
                c.IncludeXmlComments(filePath);
            });
            builder.Services.AddCors();
            builder.Services.AddScoped<ITodoRepository, TodoRepository>();
            builder.Services.AddDbContext<TodoDbContext>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }
            app.UseCors(
                 options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            logger.Information("Application started successfully.");
            app.Run();

        }
    }
}
