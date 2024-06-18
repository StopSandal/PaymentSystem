using Microsoft.EntityFrameworkCore;
using PaymentSystem.DataLayer.EF;

const string CONNECTION_STRING = "PaymentSystemConnection";
const string MIGRATION_ASSEMBLY = "PaymentSystem.DataLayer";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PaymentSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(CONNECTION_STRING)
    , x => x.MigrationsAssembly(MIGRATION_ASSEMBLY)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
