using ExchangeMarket.Model;
using ExchangeMarket.Model.Exceptions;
using ExchangeMarket.Services;
using ExchangeMarket.UnitTests.Mocks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeMarket.UnitTests
{
    public class ExchangeServiceTest
    {
        private readonly Mock<ILogger<ExchangeService>> _loggerExchangeService;

        public ExchangeServiceTest()
        {
            _loggerExchangeService = new Mock<ILogger<ExchangeService>>();
        }

        [Fact]
        public async Task GetExchangeRateByISOCodeAsync_ARS_ThrowInvalidCurrencyException()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);

            //Act
            Func<Task> invalidResult = async () => { await exchangeService.GetExchangeRateByISOCodeAsync("ARS"); };

            //Assert
            await invalidResult.Should().ThrowAsync<InvalidCurrencyException>();
        }

        /// <summary>
        /// The currency would allow to use a negative number, but thats not possible because its already validated on the Model
        /// </summary>
        #region BuyCurrency
        [Fact]
        public async Task BuyCurrency_ARS_ThrowInvalidCurrencyException()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = 100, Currency = "ARS", UserId = "Mocked" };

            //Act
            Func<Task> invalidResult = async () => { await exchangeService.BuyCurrency(BuyOrder); };

            //Assert
            await invalidResult.Should().ThrowAsync<InvalidCurrencyException>();
        }
        
        [Fact]
        public async Task BuyCurrency_USD_ShouldSucceedAndThenThrow()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = 20000, Currency = "USD", UserId = "Mocked" };
            var BuyOrder2 = new BuyCurrencyOrder() { Amount = 1.01m, Currency = "USD", UserId = "Mocked" };

            //Act
            Func<Task> successfulBuy = async () => { await exchangeService.BuyCurrency(BuyOrder); };
            Func<Task> exception = async () => { await exchangeService.BuyCurrency(BuyOrder2); };

            //Assert
            await successfulBuy.Should().NotThrowAsync();
            await exception.Should().ThrowAsync<MonthlyLimitException>();
        }

        [Fact]
        public async Task BuyCurrency_BRL_ShouldSucceedAndThenThrow()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = 7500, Currency = "BRL", UserId = "Mocked" };
            var BuyOrder2 = new BuyCurrencyOrder() { Amount = 1.01m, Currency = "BRL", UserId = "Mocked" };

            //Act
            Func<Task> successfulBuy = async () => { await exchangeService.BuyCurrency(BuyOrder); };
            Func<Task> exception = async () => { await exchangeService.BuyCurrency(BuyOrder2); };

            //Assert
            await successfulBuy.Should().NotThrowAsync();
            await exception.Should().ThrowAsync<MonthlyLimitException>();

            decimal brlAmountForMocked = await mockedRepository.GetCurrentSpentAmountByUser("Mocked", "BRL");
            brlAmountForMocked.Should().Be(300m);
        }

        [Fact]
        public async Task BuyCurrency_BRLARS_ShouldSucceed()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = 7500, Currency = "BRL", UserId = "Mocked" };
            var BuyOrder2 = new BuyCurrencyOrder() { Amount = 20000, Currency = "USD", UserId = "Mocked" };

            //Act
            Func<Task> successfulBuy = async () => { await exchangeService.BuyCurrency(BuyOrder); };
            Func<Task> successfulBuy2 = async () => { await exchangeService.BuyCurrency(BuyOrder2); };


            //Assert
            await successfulBuy.Should().NotThrowAsync();
            await successfulBuy2.Should().NotThrowAsync();

            decimal usdAmountForMocked = await mockedRepository.GetCurrentSpentAmountByUser("Mocked", "USD");
            decimal brlAmountForMocked = await mockedRepository.GetCurrentSpentAmountByUser("Mocked", "BRL");
            usdAmountForMocked.Should().Be(200m);
            brlAmountForMocked.Should().Be(300m);

        }

        [Fact]
        public async Task BuyCurrency_BRLARSEachTwiceChangingDate_ShouldSucceed()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = 7500, Currency = "BRL", UserId = "Mocked" };
            var BuyOrder2 = new BuyCurrencyOrder() { Amount = 20000, Currency = "USD", UserId = "Mocked" };
            //Act
            await exchangeService.BuyCurrency(BuyOrder);
            await exchangeService.BuyCurrency(BuyOrder2);

            //We set the transactions from the month before
            mockedRepository.TransactionsUser["Mocked"].ForEach(x => x.Date.AddDays(-31));

            //And then we try as if in this month the value is 0
            Func<Task> successfulBuy = async () => { await exchangeService.BuyCurrency(BuyOrder); };
            Func<Task> successfulBuy2 = async () => { await exchangeService.BuyCurrency(BuyOrder2); };

            //Assert
            decimal usdAmountForMocked = await mockedRepository.GetCurrentSpentAmountByUser("Mocked", "USD");
            decimal brlAmountForMocked = await mockedRepository.GetCurrentSpentAmountByUser("Mocked", "BRL");
            usdAmountForMocked.Should().Be(200m);
            brlAmountForMocked.Should().Be(300m);

        }
        #endregion

        #region Model Validations

        [Fact]
        public void Validation_NegativeNumber_CausesModelInvalid()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = -100, Currency = "USD", UserId = "Mocked" };

            var validationContext = new ValidationContext(BuyOrder, null, null);
            var validationResults = new List<ValidationResult>();

            ////Act
            Validator.TryValidateObject(BuyOrder, validationContext, validationResults, true);

            //Assert
            validationResults.Count.Should().Be(1);
        }

        [Fact]
        public void Validation_DangerousUser_CausesModelInvalid()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = 100, Currency = "USD", UserId = "User;--; DELETE *"};

            var validationContext = new ValidationContext(BuyOrder, null, null);
            var validationResults = new List<ValidationResult>();

            ////Act
            Validator.TryValidateObject(BuyOrder, validationContext, validationResults, true);

            //Assert
            validationResults.Count.Should().Be(1);
        }

        [Fact]
        public void Validation_InvalidISOAmountChars_CausesModelInvalid()
        {
            //Arrange
            var mockedRepository = new MockedExchangeRepository();
            var exchangeService = new ExchangeService(_loggerExchangeService.Object, mockedRepository);
            var BuyOrder = new BuyCurrencyOrder() { Amount = 100, Currency = "USDD", UserId = "User" };

            var validationContext = new ValidationContext(BuyOrder, null, null);
            var validationResults = new List<ValidationResult>();

            ////Act
            Validator.TryValidateObject(BuyOrder, validationContext, validationResults, true);

            //Assert
            validationResults.Count.Should().Be(1);
        }
        #endregion
    }
}
