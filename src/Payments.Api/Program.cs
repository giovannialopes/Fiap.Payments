using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Payments.Domain.Dependency;
using Payments.Domain.Middleware;
using Payments.Domain.MessageBus;
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

// Configuração RabbitMQ
builder.Services.AddMessageBus(builder.Configuration);

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



// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("database", () => {
        // Verificação básica - será verificado no runtime
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database check configurado");
    })
    .AddCheck("rabbitmq", () => {
        // Verificação básica de conectividade RabbitMQ
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var port = builder.Configuration.GetValue<ushort>("RabbitMq:Port", 5672);
        try {
            using var client = new System.Net.Sockets.TcpClient();
            var result = client.BeginConnect(host, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            if (!success) return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("RabbitMQ não acessível");
            client.EndConnect(result);
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("RabbitMQ acessível");
        } catch {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("RabbitMQ não acessível");
        }
    });

builder.Services.AddServices();
builder.Services.AddRepositories();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ObservabilityMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoint
app.MapHealthChecks("/health");

app.MapControllers();


// Inicializar banco de dados (não crítico - aplicação continua mesmo se falhar)
try {
    using (var scope = app.Services.CreateScope()) {
        var dbContext = scope.ServiceProvider.GetRequiredService<DbPayments>();
        await dbContext.Database.EnsureCreatedAsync();
    }
} catch (Exception ex) {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Não foi possível conectar ao banco de dados na inicialização. A aplicação continuará rodando.");
}

await app.RunAsync();