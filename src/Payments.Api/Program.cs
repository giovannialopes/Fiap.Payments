using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Payments.Domain.Dependency;
using Payments.Infrastructure.Data;
using Payments.Infrastructure.Dependency;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payments", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization",
        Description = "Insira o token JWT no formato: Bearer {seu token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração Service Bus
builder.Services.AddSingleton(sp => {
    var cfg = sp.GetRequiredService<IConfiguration>();
    var client = new ServiceBusClient(cfg["ServiceBus:ConnectionString"]);
    return client.CreateProcessor(cfg["ServiceBus:Queue"] ?? "payments");
});


// Injeta e inicia o Worker
builder.Services.AddHostedService<PaymentsQueueWorker>();

//Banco de Dados
builder.Services.AddDbContext<DbPayments>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwt = builder.Configuration.GetSection("JwtSettings");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });



builder.Services.AddServices();
builder.Services.AddRepositories();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<DbPayments>();
    await dbContext.Database.EnsureCreatedAsync();
}

await app.RunAsync();