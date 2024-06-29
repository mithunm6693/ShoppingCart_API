
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShoppingCart.Data;

namespace ShoppingCart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //add athentication 

            // add athurization

            // add db context

            builder.Services.AddDbContext<StoreContext>(options =>
         {
             options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
         });

            // configure identity api end points

            builder.Services.AddCors();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // map the indentity end points

            app.UseCors(options =>
            {
                options.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000", "http://localhost:3005");
            });


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                context.Database.Migrate();
                DbInitializer.Initilize(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error on data migration");
            }

            app.Run();
        }
    }
}
