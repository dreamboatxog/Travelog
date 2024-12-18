using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travelog.DataAccess;
using Travelog.Core;
using Travelog.Application;
using Travelog.Core.Abstractions;
using Travelog.DataAccess.Repositories;
using Travelog.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TravelogDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(TravelogDbContext)));
    });


builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UserService>();
builder.Services.AddScoped<IPlacesRepository, PlacesRepository>();
builder.Services.AddScoped<IPlacesService, PlaceService>();
builder.Services.AddScoped<IPhotosService, PhotoService>();
builder.Services.AddAutoMapper(typeof(Mapper));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidIssuer = builder.Configuration["Jwt:Issuer"],
                  ValidAudience = builder.Configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
              };
          });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TravelogDbContext>(); 
    context.Database.Migrate();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();


app.Run();
