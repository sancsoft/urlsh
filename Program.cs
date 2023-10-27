using Microsoft.EntityFrameworkCore;
using UrlSh.Data;
using UrlSh.Data.Models;
using Sqids;
using MySql.Data.MySqlClient;
using UrlSh;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionStringBuilder = new MySqlConnectionStringBuilder(builder.Configuration.GetConnectionString("UrlSh"));

var dbName = builder.Configuration.GetValue<string>("DB_NAME");
if (!String.IsNullOrEmpty(dbName))
{
    connectionStringBuilder.Database = dbName;
}

var dbHost = builder.Configuration.GetValue<string>("DB_HOST");
if(!String.IsNullOrEmpty(dbHost))
{
    connectionStringBuilder.Server = dbHost;
}

var dbPort = builder.Configuration.GetValue<uint?>("DB_PORT");
if(dbPort.HasValue)
{
    connectionStringBuilder.Port = dbPort.Value;
}

var dbUser = builder.Configuration.GetValue<string>("DB_USER");
if (!String.IsNullOrEmpty(dbUser))
{
    connectionStringBuilder.UserID = dbUser;
}

var dbPassword = builder.Configuration.GetValue<string>("DB_PASSWORD");
if (!String.IsNullOrEmpty(dbPassword))
{
    connectionStringBuilder.Password = dbPassword;
}

var connectionString = connectionStringBuilder.ConnectionString;
if(String.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Unable to create connection string.");
}

builder.Services.AddDbContext<UrlShContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddTransient<SqidsService>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/{code}", async (HttpContext context, string code, UrlShContext urlShContext, CancellationToken ct) =>
{
    var redirect = await urlShContext.Redirects.SingleOrDefaultAsync(t => t.Code == code, ct);
    if(redirect == null)
    {
        return Results.NotFound();
    }

    var redirectLog = new RedirectLog()
    {
        RedirectId = redirect.Id,
        IPAddress = context.Connection.RemoteIpAddress?.ToString(),
        Referer = context.Request.Headers["Referer"],
        UserAgent = context.Request.Headers["User-Agent"]
    };

    urlShContext.RedirectsLogs.Add(redirectLog);

    await urlShContext.SaveChangesAsync(ct);

    return Results.Redirect(redirect.Url.AbsoluteUri, false, false);
})
.WithName("Redirect");

await using var scope = app.Services.CreateAsyncScope();
await using var context = scope.ServiceProvider.GetRequiredService<UrlShContext>();
{
    app.Logger.LogInformation("Migrating database to latest version...");
    await context.Database.MigrateAsync();
}

app.Run();
