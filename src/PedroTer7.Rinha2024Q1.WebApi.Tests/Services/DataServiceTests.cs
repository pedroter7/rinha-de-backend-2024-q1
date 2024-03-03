using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PedroTer7.Rinha2024Q1.Database.Enums;
using PedroTer7.Rinha2024Q1.Database.Dtos;
using PedroTer7.Rinha2024Q1.Database.Services;
using PedroTer7.Rinha2024Q1.Tests.Common.Fixtures;
using PedroTer7.Rinha2024Q1.WebApi.Exceptions;
using PedroTer7.Rinha2024Q1.WebApi.Services;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;
using AutoMapper;

namespace PedroTer7.Rinha2024Q1.WebApi.Tests;

public class DataServiceTests : IClassFixture<RandomValuesFixture>, IClassFixture<DtosFixture>
{
    private readonly RandomValuesFixture _randomValuesFixture;
    private readonly DtosFixture _dtosFixture;
    private readonly Fixture _autoFixture = new();

    public DataServiceTests(RandomValuesFixture randomValuesFixture, DtosFixture dtosFixture)
    {
        _randomValuesFixture = randomValuesFixture;
        _autoFixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        _dtosFixture = dtosFixture;
    }


    [Fact(DisplayName = $"{nameof(DataService.GetAccountStatement)} throws as expected when account is not found")]
    [Trait(nameof(DataService), nameof(DataService.GetAccountStatement))]
    public async Task GetAccountStatement_ThrowsAsExpected_WhenAccountIsNotFound()
    {
        // Arrange
        var accountId = _randomValuesFixture.RandomAccountId;
        var dataAccessServiceMock = _autoFixture.Freeze<Mock<IDataAccessService>>();
        dataAccessServiceMock
            .Setup(x => x.CallGetAccountStatementProcedure(It.IsAny<int>()))
            .Returns(Task.FromResult(new GetAccountStatementProcedureResultDto(DataBaseProcedureResultCodeEnum.INVALID_ACCOUNT, 0, 0, DateTime.UtcNow, [])));

        var service = new DataService(dataAccessServiceMock.Object, _autoFixture.Create<IMapper>());

        // Act
        // Assert
        await Assert.ThrowsAsync<AccountNotFoundException>(() => service.GetAccountStatement(accountId));
    }

    [Fact(DisplayName = $"{nameof(DataService.GetAccountStatement)} returns account statement correctly")]
    [Trait(nameof(DataService), nameof(DataService.GetAccountStatement))]
    public async Task GetAccountStatement_ReturnsAccountStatementCorrectly()
    {
        // Arrange
        var accountId = _randomValuesFixture.RandomAccountId;
        var balance = _randomValuesFixture.RandomPositiveBalance;
        var limit = _randomValuesFixture.RandomValidLimit;
        var timestamp = _randomValuesFixture.RandomPastUtcTime;
        var transactionsHistory = _dtosFixture.GetRandomTransactionLogModels(_randomValuesFixture.RandomInt(0, 10), accountId);
        var transactionHistoryExpectedResult = transactionsHistory
            .Select(t => new TransactionLogDto(t.Amount, t.Type.ToString(), t.Description, t.Timestamp_utc))
            .ToList();

        var procedureResultDto = new GetAccountStatementProcedureResultDto(DataBaseProcedureResultCodeEnum.SUCCESS, balance, limit, timestamp, transactionsHistory);
        var dataAccessServiceMock = _autoFixture.Freeze<Mock<IDataAccessService>>();
        dataAccessServiceMock
            .Setup(x => x.CallGetAccountStatementProcedure(It.IsAny<int>()))
            .Returns(Task.FromResult(procedureResultDto));

        var mapperMock = _autoFixture.Freeze<Mock<IMapper>>();
        mapperMock
            .Setup(m => m.Map<AccountStatementDto>(It.Is<GetAccountStatementProcedureResultDto>(d => d.Equals(procedureResultDto))))
            .Returns(new AccountStatementDto(balance, limit, timestamp, transactionHistoryExpectedResult));

        var service = new DataService(dataAccessServiceMock.Object, mapperMock.Object);

        // Act
        var resultStatement = await service.GetAccountStatement(accountId);

        // Assert
        Assert.Equal(balance, resultStatement.Balance);
        Assert.Equal(limit, resultStatement.Limit);
        Assert.Equal(timestamp, resultStatement.TimeStamp);
        Assert.Equal(transactionHistoryExpectedResult, resultStatement.Transactions);
        dataAccessServiceMock
            .Verify(s => s.CallTransactionProcedure(It.IsAny<int>(), It.IsAny<char>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        dataAccessServiceMock
            .Verify(s => s.CallGetAccountStatementProcedure(It.Is<int>(i => i != accountId)), Times.Never);
    }

    [Fact(DisplayName = $"{nameof(DataService.RegisterTransactionForAccount)} throws as expected when account is not found")]
    [Trait(nameof(DataService), nameof(DataService.RegisterTransactionForAccount))]
    public async Task RegisterTransactionForAccount_ThrowsAsExpected_WhenAccountIsNotFound()
    {
        // Arrange
        var accountId = _randomValuesFixture.RandomAccountId;
        var type = (char)_randomValuesFixture.RandomInt(99, 100);
        var value = type == 'c' ? _randomValuesFixture.RandomTransactionPositiveAmount : _randomValuesFixture.RandomTransactionNegativeAmount;
        var transactionDto = new TransactionDto(value, type.ToString(), _randomValuesFixture.RandomDescription);
        var dataAccessServiceMock = _autoFixture.Freeze<Mock<IDataAccessService>>();
        dataAccessServiceMock
            .Setup(x => x.CallTransactionProcedure(It.IsAny<int>(), It.IsAny<char>(), It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.FromResult(new TransactionProcedureResultDto(DataBaseProcedureResultCodeEnum.INVALID_ACCOUNT, 0, 0)));

        var service = new DataService(dataAccessServiceMock.Object, _autoFixture.Create<IMapper>());

        // Act
        // Assert
        await Assert.ThrowsAsync<AccountNotFoundException>(() => service.RegisterTransactionForAccount(accountId, transactionDto));
    }

    [Fact(DisplayName = $"{nameof(DataService.RegisterTransactionForAccount)} throws as expected when transaction is invalid")]
    [Trait(nameof(DataService), nameof(DataService.RegisterTransactionForAccount))]
    public async Task RegisterTransactionForAccount_ThrowsAsExpected_WhenAccountHasNoFundsFor_d_Operation()
    {
        // Arrange
        var accountId = _randomValuesFixture.RandomAccountId;
        var type = (char)_randomValuesFixture.RandomInt(99, 100);
        var value = type == 'c' ? _randomValuesFixture.RandomTransactionPositiveAmount : _randomValuesFixture.RandomTransactionNegativeAmount;
        var transactionDto = new TransactionDto(value, type.ToString(), _randomValuesFixture.RandomDescription);
        var dataAccessServiceMock = _autoFixture.Freeze<Mock<IDataAccessService>>();
        dataAccessServiceMock
            .Setup(x => x.CallTransactionProcedure(It.IsAny<int>(), It.IsAny<char>(), It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.FromResult(new TransactionProcedureResultDto(DataBaseProcedureResultCodeEnum.INVALID_TRANSACTION, 0, 0)));

        var service = new DataService(dataAccessServiceMock.Object, _autoFixture.Create<IMapper>());

        // Act
        // Assert
        await Assert.ThrowsAsync<InvalidTransactionException>(() => service.RegisterTransactionForAccount(accountId, transactionDto));
    }

    [Fact(DisplayName = $"{nameof(DataService.RegisterTransactionForAccount)} throws as expected when transaction arguments are invalid")]
    [Trait(nameof(DataService), nameof(DataService.RegisterTransactionForAccount))]
    public async Task RegisterTransactionForAccount_ThrowsAsExpected_WhenTransactionArgumentsAreInvalid()
    {
        // Arrange
        var accountId = _randomValuesFixture.RandomAccountId;
        var type = (char)_randomValuesFixture.RandomInt(99, 100);
        var value = type == 'c' ? _randomValuesFixture.RandomTransactionPositiveAmount : _randomValuesFixture.RandomTransactionNegativeAmount;
        var transactionDto = new TransactionDto(value, type.ToString(), _randomValuesFixture.RandomDescription);
        var dataAccessServiceMock = _autoFixture.Freeze<Mock<IDataAccessService>>();
        dataAccessServiceMock
            .Setup(x => x.CallTransactionProcedure(It.IsAny<int>(), It.IsAny<char>(), It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.FromResult(new TransactionProcedureResultDto(DataBaseProcedureResultCodeEnum.INVALID_OPERATION_ARGUMENTS, 0, 0)));

        var service = new DataService(dataAccessServiceMock.Object, _autoFixture.Create<IMapper>());

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.RegisterTransactionForAccount(accountId, transactionDto));
    }

    [Fact(DisplayName = $"{nameof(DataService.RegisterTransactionForAccount)} returns as expected")]
    [Trait(nameof(DataService), nameof(DataService.RegisterTransactionForAccount))]
    public async Task RegisterTransactionForAccount_ReturnsAsExpected()
    {
        // Arrange
        var accountId = _randomValuesFixture.RandomAccountId;
        var type = (char)_randomValuesFixture.RandomInt(99, 100);
        var value = _randomValuesFixture.RandomTransactionPositiveAmount;
        var expectedValueArgument = type == 'c' ? value : (-1) * value;
        var description = _randomValuesFixture.RandomDescription;
        var transactionDto = new TransactionDto(value, type.ToString(), description);
        var newAccountBalance = _randomValuesFixture.RandomPositiveBalance;
        var accountLimit = _randomValuesFixture.RandomValidLimit;
        var procedureResultDto = new TransactionProcedureResultDto(DataBaseProcedureResultCodeEnum.SUCCESS, newAccountBalance, accountLimit);
        var expcetedTransactionResultDto = new TransactionResultDto(newAccountBalance, accountLimit);
        var dataAccessServiceMock = _autoFixture.Freeze<Mock<IDataAccessService>>();
        dataAccessServiceMock
            .Setup(x => x.CallTransactionProcedure(It.IsAny<int>(), It.IsAny<char>(), It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.FromResult(procedureResultDto));

        var mapperMock = _autoFixture.Freeze<Mock<IMapper>>();
        mapperMock
            .Setup(m => m.Map<TransactionResultDto>(It.Is<TransactionProcedureResultDto>(d => d.Equals(procedureResultDto))))
            .Returns(expcetedTransactionResultDto);

        var service = new DataService(dataAccessServiceMock.Object, mapperMock.Object);

        // Act
        var result = await service.RegisterTransactionForAccount(accountId, transactionDto);

        // Assert
        Assert.Equal(expcetedTransactionResultDto, result);
        dataAccessServiceMock
            .Verify(m => m.CallTransactionProcedure(It.Is<int>(i => i == accountId), It.Is<char>(c => c == type), It.Is<int>(v => v == expectedValueArgument), It.Is<string>(s => s.Equals(description))), Times.Once);
        dataAccessServiceMock
            .Verify(m => m.CallTransactionProcedure(It.Is<int>(i => i == accountId), It.Is<char>(c => c == type), It.Is<int>(v => v != expectedValueArgument), It.IsAny<string>()), Times.Never);
        dataAccessServiceMock
            .Verify(m => m.CallTransactionProcedure(It.Is<int>(i => i == accountId), It.Is<char>(c => c == type), It.Is<int>(v => v == expectedValueArgument), It.Is<string>(s => !s.Equals(description))), Times.Never);
        dataAccessServiceMock
            .Verify(m => m.CallTransactionProcedure(It.Is<int>(i => i == accountId), It.Is<char>(c => c != type), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}
