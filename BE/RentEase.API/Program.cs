using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RentEase.API.Data;
using RentEase.API.Mappings;
using RentEase.API.Repositories;
using RentEase.API.Services;


var builder = WebApplication.CreateBuilder(args);

// Add database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
// Add repositories
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

// Add services to the container.
builder.Services.AddScoped<IPropertyService, PropertyService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreatePropertyRequestValidator>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(PropertyMappingProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();