using BookStoreCatalog_API;
using BookStoreCatalog_API.Data;
using BookStoreCatalog_API.Repository;
using BookStoreCatalog_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson();

//--- Add DbContext Service InMemory option
builder.Services.AddDbContext<ApplicationDBContextInMem>(options => options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("BookDbInMem")));

//--- Add DbContext Service SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BookDb")));

builder.Services.AddAutoMapper(typeof(MappingConfig));


builder.Services.AddScoped<IBookRepo, BookModelRepo>();

builder.Services.AddScoped<IIssueRepo, IssueModelRepo>();

builder.Services.AddScoped<IUserRepo, UserModelRepo>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
