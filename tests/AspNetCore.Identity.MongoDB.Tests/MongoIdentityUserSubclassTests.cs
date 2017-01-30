using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDB.Models;
using AspNetCore.Identity.MongoDB.Tests.Common;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace AspNetCore.Identity.MongoDB.Tests
{
    public class MongoIdentityUserSubclassTests
    {
        [Fact]
        public async Task MongoIdentityUser_CanBeSavedAndRetrieved_WhenItBecomesTheSubclass()
        {
            var username = TestUtils.RandomString(10);
            var countryName = TestUtils.RandomString(10);
            var loginProvider = TestUtils.RandomString(5);
            var providerKey = TestUtils.RandomString(5);
            var displayName = TestUtils.RandomString(5);
            var user = new MyIdentityUser(username);
            user.AddClaim(new Claim(ClaimTypes.Country, countryName));
            user.AddLogin(new MongoUserLogin(new UserLoginInfo(loginProvider, providerKey, displayName)));

            using (var dbProvider = MongoDbServerTestUtils.CreateDatabase())
            {
                var store = new MongoUserStore<MyIdentityUser>(dbProvider.Database);

                // ACT, ASSERT
                var result = await store.CreateAsync(user, CancellationToken.None);
                Assert.True(result.Succeeded);

                // ACT, ASSERT
                var retrievedUser = await store.FindByIdAsync(user.Id, CancellationToken.None);
                Assert.NotNull(retrievedUser);
                Assert.Equal(username, retrievedUser.UserName);
            }
        }

        public class MyIdentityUser : MongoIdentityUser
        {
            public MyIdentityUser(string userName) : base(userName)
            {
            }

            public string MyCustomThing { get; set; }
        }
    }
}