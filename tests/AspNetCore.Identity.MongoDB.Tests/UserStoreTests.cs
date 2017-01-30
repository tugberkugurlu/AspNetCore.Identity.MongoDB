using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDB.Tests.Common;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Xunit;

namespace AspNetCore.Identity.MongoDB.Tests
{
    public class UserStoreTests
    {
        [Fact]
        public async Task CreateAsync_ShouldCreateUser()
        {
            // ARRANGE
            using (var dbProvider = MongoDbServerTestUtils.CreateDatabase())
            {
                var userStore = new MongoUserStore<MongoIdentityUser>(dbProvider.Database) as IUserStore<MongoIdentityUser>;
                var user = new MongoIdentityUser(TestUtils.RandomString(10));

                // ACT
                await userStore.CreateAsync(user, CancellationToken.None);

                // ASSERT
                var collection = dbProvider.Database.GetDefaultCollection();
                var filter = Builders<MongoIdentityUser>.Filter.Eq(x => x.Id, user.Id);
                var retrievedUser = await collection.Find(filter).FirstOrDefaultAsync();

                Assert.NotNull(retrievedUser);
                Assert.Equal(user.UserName, retrievedUser.UserName);
                Assert.Equal(user.NormalizedUserName, retrievedUser.NormalizedUserName);
            }
        }
    }
}