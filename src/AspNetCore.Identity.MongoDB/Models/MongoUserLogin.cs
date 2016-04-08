using Microsoft.AspNet.Identity;
using System;

namespace Dnx.Identity.MongoDB.Models
{
    public class MongoUserLogin : IEquatable<MongoUserLogin>, IEquatable<UserLoginInfo>
    {
        public MongoUserLogin(UserLoginInfo loginInfo)
        {
            if (loginInfo == null)
            {
                throw new ArgumentNullException(nameof(loginInfo));
            }

            LoginProvider = loginInfo.LoginProvider;
            ProviderKey = loginInfo.ProviderKey;
            ProviderDisplayName = loginInfo.ProviderDisplayName;
        }

        public string LoginProvider { get; private set; }
        public string ProviderKey { get; private set; }
        public string ProviderDisplayName { get; private set; }

        public bool Equals(MongoUserLogin other)
        {
            return other.LoginProvider.Equals(LoginProvider, StringComparison.InvariantCultureIgnoreCase)
                && other.ProviderKey.Equals(ProviderKey, StringComparison.InvariantCulture);
        }

        public bool Equals(UserLoginInfo other)
        {
            return other.LoginProvider.Equals(LoginProvider, StringComparison.InvariantCultureIgnoreCase)
                && other.ProviderKey.Equals(ProviderKey, StringComparison.InvariantCulture);
        }
    }
}
