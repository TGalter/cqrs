using App;
using App.Interfaces;
using Dom.Interfaces;
using Infra;
using Infra.Context;
using Infra.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(App.Behaviors.AssemblyReference.Assembly));


builder.Services.AddAutoMapper(App.Behaviors.AssemblyReference.Assembly);


builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.Configure<ElasticSearchSettings>(builder.Configuration.GetSection("ElasticSearch"));
builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

