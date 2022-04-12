using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var rewrite = new RewriteOptions()
            .AddRewrite("(?i)test(.*)", "Test", skipRemainingRules: true)
            .AddRewrite("(?i)getimages(.*)", "GetImages", skipRemainingRules: true);

app.UseRewriter(rewrite);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
