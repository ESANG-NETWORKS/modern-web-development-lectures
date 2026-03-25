using EsSecrets.Day2.Data;
using EsSecrets.Day2.Middleware;
using EsSecrets.Day2.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// AddDbContext는 기본적으로 scoped lifetime을 사용한다.
// Day 1에서 설명한 DI/lifetime이 여기서 DbContext와 자연스럽게 연결된다.
// EnableSensitiveDataLogging + LogTo를 함께 쓰면 파라미터 값이 포함된 SQL이 콘솔에 출력된다.
builder.Services.AddDbContext<SecretsDbContext>(options =>
{
    options.UseSqlite("Data Source=essecrets-day2.db");
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddScoped<ISecretService, SecretService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

// 앱 시작 시 SQLite DB 생성 + seed 데이터 적재
using (var scope = app.Services.CreateScope())
{
    SecretsDbContext dbContext = scope.ServiceProvider.GetRequiredService<SecretsDbContext>();
    await SeedData.InitializeAsync(dbContext);
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestAuditMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
