using AutoMapper;
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
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddFluentValidators()
    .AddMappingProfiles()
    .RegisterDatabaseServices(builder.Configuration)
    .RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger()
    .UseSwaggerUI()
    .UseHttpsRedirection();

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
            var respBody = validationException.Errors
                .Select(e => new { Propriedade = e.PropertyName, Mensagem = e.ErrorMessage, ValorRecebido = e.AttemptedValue });

            await context.Response.WriteAsJsonAsync(respBody);
        }
        else if (e is AccountNotFoundException accountNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            var respBody = new
            {
                Mensagem = $"Conta de ID {accountNotFoundException.AccountId} não foi encontrada",
            };

            await context.Response.WriteAsJsonAsync(respBody);
        }
        else if (e is InvalidTransactionException invalidTransactionException)
        {
            context.Response.StatusCode = 422;
            var respBody = new
            {
                Mensagem = "A transação é invalida"
            };

            await context.Response.WriteAsJsonAsync(respBody);
        }
        else
        {
            throw;
        }
    }
});

app.MapPost("/clientes/{id:int}/trasacoes", async (
    [FromRoute(Name = "id")] int accountId,
    [FromBody] InTransaction transaction,
    [FromServices] IValidator<InTransaction> validator,
    [FromServices] IMapper mapper,
    [FromServices] IDataService dataService) =>
{
    validator.ValidateAndThrow(transaction);
    var tranDto = mapper.Map<TransactionDto>(transaction);
    var tranResult = await dataService.RegisterTransactionForAccount(accountId, tranDto);
    return TypedResults.Ok(mapper.Map<OutTransactionResult>(tranResult));
})
.WithName("register_transaction")
.WithOpenApi();

app.MapGet("/clientes/{id:int}/extrato", async (
    [FromRoute(Name = "id")] int accountId,
    [FromServices] IMapper mapper,
    [FromServices] IDataService dataService
) =>
{
    var statement = await dataService.GetAccountStatement(accountId);
    return TypedResults.Ok(mapper.Map<OutAccountStatement>(statement));
})
.WithName("get_account_statement")
.WithOpenApi();

app.Run();
