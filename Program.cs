using unistream_t2.Interfaces;
using unistream_t2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IEntityRepo, MemoryEntityRepo>(); // Добавляем свою реализацию (в виде хранилища в памяти) интерфейса репозитория как синглтон

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
