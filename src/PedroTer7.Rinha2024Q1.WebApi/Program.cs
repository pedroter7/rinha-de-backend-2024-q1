using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PedroTer7.Rinha2024Q1.Database;
using PedroTer7.Rinha2024Q1.WebApi;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;
using PedroTer7.Rinha2024Q1.WebApi.Exceptions;
using PedroTer7.Rinha2024Q1.WebApi.Mappers;
using PedroTer7.Rinha2024Q1.WebApi.Services;
using PedroTer7.Rinha2024Q1.WebApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddFluentValidators()
    .RegisterDatabaseServices(builder.Configuration)
    .RegisterServices()
    .ConfigureHttpJsonOptions(cfg =>
    {
        cfg.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception e)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        if (e is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return; ;
        }
        else if (e is AccountNotFoundException accountNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return; ;
        }
        else if (e is InvalidTransactionException invalidTransactionException)
        {
            context.Response.StatusCode = 422;
            return; ;
        }
        else
        {
            throw;
        }
    }
});

app.MapPost("/clientes/{id:int}/transacoes", async (
    [FromRoute(Name = "id")] int accountId,
    [FromBody] InTransaction transaction,
    [FromServices] IValidator<InTransaction> validator,
    [FromServices] IDataService dataService) =>
{
    validator.ValidateAndThrow(transaction);
    var tranDto = transaction.ToTransactionDto();
    var tranResult = await dataService.RegisterTransactionForAccount(accountId, tranDto);
    return TypedResults.Ok(tranResult.ToOutTransactionResult());
});

app.MapGet("/clientes/{id:int}/extrato", async (
    [FromRoute(Name = "id")] int accountId,
    [FromServices] IDataService dataService
) =>
{
    var statement = await dataService.GetAccountStatement(accountId);
    return TypedResults.Ok(statement.ToOutAccountStatement());
});

app.Run();
