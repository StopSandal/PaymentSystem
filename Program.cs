using Microsoft.EntityFrameworkCore;
using PaymentSystem.DataLayer.EF;
using PaymentSystem.Helpers;

public partial class Program
{
    private static void Main(string[] args)
    {
        const string CONNECTION_STRING = "PaymentSystemConnection";
        const string MIGRATION_ASSEMBLY = "PaymentSystem.DataLayer";

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<PaymentSystemContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString(CONNECTION_STRING)
                , x => x.MigrationsAssembly(MIGRATION_ASSEMBLY)));

        builder.Services.RegisterServices();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddControllers();
        builder.Services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddConsole();
            config.AddDebug();
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var basePath = AppContext.BaseDirectory;

            var xmlPath = Path.Combine(basePath, "PaymentSystem.xml");
            options.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        // migrate DB
        app.ApplyMigrations<PaymentSystemContext>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}