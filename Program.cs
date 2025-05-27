using générationdétiquettes.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BarcodeDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BarcodeDb;Trusted_Connection=True;"));

// 🔥 Ajout CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("http://localhost:5062");


var app = builder.Build();



    app.UseSwagger();
    app.UseSwaggerUI();
   app.UseHttpsRedirection();

// 🔥 Activer CORS
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
