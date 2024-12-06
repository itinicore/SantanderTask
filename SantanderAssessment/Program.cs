
using Microsoft.Extensions.DependencyInjection;
using SantanderAssessment.BackgroundServices;
using SantanderAssessment.Data;

namespace SantanderAssessment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IHackerNewsObserver, HackerNewsObserver>();
            builder.Services.AddHostedService<HackerNewsBackgroundService>();
            builder.Services.AddSingleton<HackerNewsBackgroundService>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<HackerNewsObserver>());

            builder.Services.AddSingleton<IStoriesStateService, StoriesStateService>();
            builder.Services.AddTransient<IStoriesRepository, StoriesRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
