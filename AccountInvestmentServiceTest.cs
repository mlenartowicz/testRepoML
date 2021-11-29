namespace Accelerator.Dashboard.Services.IntegrationTests
{
    using System.Linq;
    using Accelerator.Core.ServiceClients.Client;
    using Accelerator.Dashboard.Services.Contracts.DataContracts;
    using Accelerator.SIL.Sonata.IntegrationTest.DataAccess.DataBuilder;
    using Accelerator.SIL.Sonata.IntegrationTest.DataAccess.DataBuilder.Loaders;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AccountInvestmentServiceTest : DashboardIntegrationTest
    {
        [TestMethod]
        public void ShouldSaveExclusionList()
        {
            // arrange
            //// var accountCriteria = new GetAccounts()
            ////.Having(a => a.AccountHoldings.Any());

            var clientAccountsLoader = new GetAccounts().ForDirectClient().Having(a => a.AccountReference == "A10031907");
            var userTestData = new UserTestDataBuilder(new GetUserByLogin("100068966")).With(clientAccountsLoader).GetFirstValid();
            var account = userTestData.Accounts.First();

            //// var userTestData =
            //// new UserTestDataBuilder(new DirectAdvisedClientUserDataLoader())
            ////   .With(accountCriteria)
            ////   .GetFirstValid();

            if (userTestData.User == null)
            {
                throw new AssertInconclusiveException("Could not find proper user");
            }

            var accountWithHoldings = userTestData.Accounts.FirstOrDefault(a => a.AccountHoldings.Any());
            if (accountWithHoldings == null || accountWithHoldings.AccountHoldings == null || accountWithHoldings.AccountHoldings.Any() == false)
            {
                throw new AssertInconclusiveException("User does not have any AccountHoldings");
            }

            var accountId = account.AccountId;

            var exclusionList = accountWithHoldings.AccountHoldings.Select(a => new ExcludeAssetFromRebalanceModel()
            {
                ExcludeFromRebalance = a.ExcludeFromRebalance == "Yes",
                Isin = a.Asset.Isin 
            }).ToList();
                
            // act 
            // fails on SaveExcludedFromRebalance this.accountAccessGuard.ThrowIfDoesNotHaveAccessToAccount(accountId) - needs special user role or access to account
            SimpleServiceResponse<SimpleResult> response = this.AccountInvestmentClient.SaveExcludedFromRebalance(accountId, exclusionList);

            // assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response.HasError);
        }
    }
}