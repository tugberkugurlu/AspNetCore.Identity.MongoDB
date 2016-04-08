using System;
using System.Security.Claims;

namespace Dnx.Identity.MongoDB.Models
{
    public class MongoUserClaim : IEquatable<MongoUserClaim>, IEquatable<Claim>
    {
        public MongoUserClaim(Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        public MongoUserClaim(string claimType, string claimValue)
        {
            if (claimType == null)
            {
                throw new ArgumentNullException(nameof(claimType));
            }
            if (claimValue == null)
            {
                throw new ArgumentNullException(nameof(claimValue));
            }

            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        public string ClaimType { get; private set; }
        public string ClaimValue { get; private set; }

        public bool Equals(MongoUserClaim other)
        {
            return other.ClaimType.Equals(ClaimType, StringComparison.InvariantCultureIgnoreCase)
                && other.ClaimValue.Equals(ClaimValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Equals(Claim other)
        {
            return other.Type.Equals(ClaimType, StringComparison.InvariantCultureIgnoreCase)
                && other.Value.Equals(ClaimValue, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
