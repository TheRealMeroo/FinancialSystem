using BuildingBlocks.Infrastructure;
using Accounting.Infrastructure;
using Accounting.Application;
using BuildingBlocks.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddBuildingBlocksInfrustructure(builder.Configuration);
builder.Services.AddBuildingBlocksApplication();
builder.Services.AddAccountingInfrastructure(builder.Configuration);
builder.Services.AddAccountingApplication();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();