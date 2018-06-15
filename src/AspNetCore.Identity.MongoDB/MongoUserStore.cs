using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;
using AspNetCore.Identity.MongoDB.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Conventions;
using EnsureThat;

namespace AspNetCore.Identity.MongoDB
{
    public class MongoUserStore<TUser> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>
        where TUser : MongoIdentityUser
    {
        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        private static bool _initialized = false;

        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        private static object _initializationLock = new object();

        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        private static object _initializationTarget;

        private readonly IMongoCollection<TUser> _usersCollection;

        static MongoUserStore()
        {
            MongoConfig.EnsureConfigured();
        }

        public MongoUserStore(IMongoDatabase database)
            : this(database, Constants.DefaultCollectionName)
        {
        }

        public MongoUserStore(IMongoDatabase database, string userCollectionName)
        {
            Ensure.That(database, nameof(database)).IsNotNull();
            Ensure.That(userCollectionName, nameof(userCollectionName)).IsNotNullOrWhiteSpace();

            _usersCollection = database.GetCollection<TUser>(userCollectionName);

            EnsureIndicesCreatedAsync().GetAwaiter().GetResult();
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            cancellationToken.ThrowIfCancellationRequested();

            await _usersCollection.InsertOneAsync(user, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            cancellationToken.ThrowIfCancellationRequested();

            user.Delete();

            var query = Builders<TUser>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<TUser>.Update.Set(u => u.DeletedOn, user.DeletedOn);

            await _usersCollection.UpdateOneAsync(query, update, cancellationToken: cancellationToken).ConfigureAwait(false);

            return IdentityResult.Success;
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            Ensure.That(userId, nameof(userId)).IsNotNullOrWhiteSpace();

            cancellationToken.ThrowIfCancellationRequested();

            var query = Builders<TUser>.Filter.And(
                Builders<TUser>.Filter.Eq(u => u.Id, userId),
                Builders<TUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            return _usersCollection.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            Ensure.That(normalizedUserName, nameof(normalizedUserName)).IsNotNullOrWhiteSpace();

            cancellationToken.ThrowIfCancellationRequested();

            var query = Builders<TUser>.Filter.And(
                Builders<TUser>.Filter.Eq(u => u.NormalizedUserName, normalizedUserName),
                Builders<TUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            return _usersCollection.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(normalizedName, nameof(normalizedName)).IsNotNullOrWhiteSpace();

            user.SetNormalizedUserName(normalizedName);

            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("Changing the username is not supported.");
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            var query = Builders<TUser>.Filter.And(
                Builders<TUser>.Filter.Eq(u => u.Id, user.Id),
                Builders<TUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            var replaceResult = await _usersCollection.ReplaceOneAsync(query, user, new UpdateOptions { IsUpsert = false }).ConfigureAwait(false);

            return replaceResult.IsModifiedCountAvailable && replaceResult.ModifiedCount == 1
                ? IdentityResult.Success
                : IdentityResult.Failed();
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(login, nameof(login)).IsNotNull();

            // NOTE: Not the best way to ensure uniquness.
            if (user.Logins.Any(x => x.Equals(login)))
            {
                throw new InvalidOperationException("Login already exists.");
            }

            user.AddLogin(new MongoUserLogin(login));

            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(loginProvider, nameof(loginProvider)).IsNotNullOrWhiteSpace();
            Ensure.That(providerKey, nameof(providerKey)).IsNotNullOrWhiteSpace();

            var login = new UserLoginInfo(loginProvider, providerKey, string.Empty);
            var loginToRemove = user.Logins.FirstOrDefault(x => x.Equals(login));

            if (loginToRemove != null)
            {
                user.RemoveLogin(loginToRemove);
            }

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            var logins = user.Logins.Select(login =>
                new UserLoginInfo(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName));

            return Task.FromResult<IList<UserLoginInfo>>(logins.ToList());
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            Ensure.That(loginProvider, nameof(loginProvider)).IsNotNullOrWhiteSpace();
            Ensure.That(providerKey, nameof(providerKey)).IsNotNullOrWhiteSpace();

            var notDeletedQuery = Builders<TUser>.Filter.Eq(u => u.DeletedOn, null);
            var loginQuery = Builders<TUser>.Filter.ElemMatch(usr => usr.Logins,
                Builders<MongoUserLogin>.Filter.And(
                    Builders<MongoUserLogin>.Filter.Eq(lg => lg.LoginProvider, loginProvider),
                    Builders<MongoUserLogin>.Filter.Eq(lg => lg.ProviderKey, providerKey)
                )
            );

            var query = Builders<TUser>.Filter.And(notDeletedQuery, loginQuery);

            return _usersCollection.Find(query).FirstOrDefaultAsync();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            var claims = user.Claims.Select(clm => new Claim(clm.ClaimType, clm.ClaimValue)).ToList();

            return Task.FromResult<IList<Claim>>(claims);
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(claims, nameof(claims)).IsNotNull();

            foreach (var claim in claims)
            {
                user.AddClaim(claim);
            }

            return Task.FromResult(0);
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(claim, nameof(claim)).IsNotNull();
            Ensure.That(newClaim, nameof(newClaim)).IsNotNull();

            user.RemoveClaim(new MongoUserClaim(claim));
            user.AddClaim(newClaim);

            return Task.FromResult(0);
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(claims, nameof(claims)).IsNotNull();

            foreach (var claim in claims)
            {
                user.RemoveClaim(new MongoUserClaim(claim));
            }

            return Task.FromResult(0);
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            Ensure.That(claim, nameof(claim)).IsNotNull();

            var notDeletedQuery = Builders<TUser>.Filter.Eq(u => u.DeletedOn, null);
            var claimQuery = Builders<TUser>.Filter.ElemMatch(usr => usr.Claims,
                Builders<MongoUserClaim>.Filter.And(
                    Builders<MongoUserClaim>.Filter.Eq(c => c.ClaimType, claim.Type),
                    Builders<MongoUserClaim>.Filter.Eq(c => c.ClaimValue, claim.Value)
                )
            );

            var query = Builders<TUser>.Filter.And(notDeletedQuery, claimQuery);
            var users = await _usersCollection.Find(query).ToListAsync().ConfigureAwait(false);

            return users;
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            user.SetPasswordHash(passwordHash);

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(stamp, nameof(stamp)).IsNotNullOrWhiteSpace();

            user.SetSecurityStamp(stamp);

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            if (enabled)
            {
                user.EnableTwoFactorAuthentication();
            }
            else
            {
                user.DisableTwoFactorAuthentication();
            }

            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(email, nameof(email)).IsNotNullOrWhiteSpace();

            user.SetEmail(email);

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            var email = (user.Email != null) ? user.Email.Value : null;

            return Task.FromResult(email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            if (user.Email == null)
            {
                throw new InvalidOperationException("Cannot get the confirmation status of the e-mail since the user doesn't have an e-mail.");
            }

            return Task.FromResult(user.Email.IsConfirmed());
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            if (user.Email == null)
            {
                throw new InvalidOperationException("Cannot set the confirmation status of the e-mail because user doesn't have an e-mail.");
            }

            if (confirmed)
            {
                user.Email.SetConfirmed();
            }
            else
            {
                user.Email.SetUnconfirmed();
            }

            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            Ensure.That(normalizedEmail, nameof(normalizedEmail)).IsNotNullOrWhiteSpace();

            var query = Builders<TUser>.Filter.And(
                Builders<TUser>.Filter.Eq(u => u.Email.NormalizedValue, normalizedEmail),
                Builders<TUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            return _usersCollection.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            var normalizedEmail = (user.Email != null) ? user.Email.NormalizedValue : null;

            return Task.FromResult(normalizedEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            // This method can be called even if user doesn't have an e-mail.
            // Act cool in this case and gracefully handle.
            // More info: https://github.com/aspnet/Identity/issues/645

            if (normalizedEmail != null && user.Email != null)
            {
                user.Email.SetNormalizedEmail(normalizedEmail);
            }

            return Task.FromResult(0);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            var lockoutEndDate = user.LockoutEndDate != null
                ? new DateTimeOffset(user.LockoutEndDate.Instant)
                : default(DateTimeOffset?);

            return Task.FromResult(lockoutEndDate);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            if (lockoutEnd != null)
            {
                user.LockUntil(lockoutEnd.Value.UtcDateTime);
            }

            return Task.FromResult(0);
        }

        public async Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            var filter = Builders<TUser>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<TUser>.Update.Inc(usr => usr.AccessFailedCount, 1);
            var findOneAndUpdateOptions = new FindOneAndUpdateOptions<TUser, int>
            {
                ReturnDocument = ReturnDocument.After,
                Projection = Builders<TUser>.Projection.Expression(usr => usr.AccessFailedCount)
            };

            var newCount = await _usersCollection
                .FindOneAndUpdateAsync<int>(filter, update, findOneAndUpdateOptions)
                .ConfigureAwait(false);

            user.SetAccessFailedCount(newCount);

            return newCount;
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            user.ResetAccessFailedCount();

            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.IsLockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            if (enabled)
            {
                user.EnableLockout();
            }
            else
            {
                user.DisableLockout();
            }

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();
            Ensure.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrWhiteSpace();

            user.SetPhoneNumber(phoneNumber);

            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            return Task.FromResult(user.PhoneNumber?.Value);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            if (user.PhoneNumber == null)
            {
                throw new InvalidOperationException("Cannot get the confirmation status of the phone number since the user doesn't have a phone number.");
            }

            return Task.FromResult(user.PhoneNumber.IsConfirmed());
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            Ensure.That(user, nameof(user)).IsNotNull();

            if (user.PhoneNumber == null)
            {
                throw new InvalidOperationException("Cannot set the confirmation status of the phone number since the user doesn't have a phone number.");
            }

            user.PhoneNumber.SetConfirmed();

            return Task.FromResult(0);
        }

        public void Dispose()
        {
        }

        private async Task EnsureIndicesCreatedAsync()
        {
            var obj = LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                return EnsureIndicesCreatedImplAsync();
            });

            if (obj != null)
            {
                var taskToAwait = (Task)obj;
                await taskToAwait.ConfigureAwait(false);
            }
        }

        private async Task EnsureIndicesCreatedImplAsync()
        {
            var indexNames = new
            {
                UniqueEmail = "identity_email_unique",
                Login = "identity_logins_loginProvider_providerKey"
            };

            var pack = ConventionRegistry.Lookup(typeof(CamelCaseElementNameConvention));

            var emailKeyBuilder = Builders<TUser>.IndexKeys.Ascending(user => user.Email.Value);
            var loginKeyBuilder = Builders<TUser>.IndexKeys.Ascending("logins.loginProvider").Ascending("logins.providerKey");

            var tasks = new[]
            {
                _usersCollection.Indexes.CreateOneAsync(emailKeyBuilder, new CreateIndexOptions { Unique = true, Name = indexNames.UniqueEmail }),
                _usersCollection.Indexes.CreateOneAsync(loginKeyBuilder, new CreateIndexOptions { Name = indexNames.Login })
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
