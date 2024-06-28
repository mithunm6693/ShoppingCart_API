
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
            builder.Services.AddSwaggerGen(options =>
            {

                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Demo",
                    Version = "v1"

                });

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Scheme = "bearer",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Description = "Enter Token Here.."

                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
            new OpenApiSecurityScheme{

            Reference = new OpenApiReference
            {
               Type=ReferenceType.SecurityScheme,
                Id="Bearer"
            }

            },

            []
            }

    });


            });

            //add athentication 

            // add athurization

            // add db context

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // configure identity api end points



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // map the indentity end points


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
