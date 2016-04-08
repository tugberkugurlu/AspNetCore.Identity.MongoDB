using Dnx.Identity.MongoDB.Models;
using Microsoft.AspNet.Identity;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Threading;

namespace Dnx.Identity.MongoDB
{
    internal static class MongoConfig
    {
        private static bool _initialized = false;
        private static object _initializationLock = new object();
        private static object _initializationTarget;

        public static void EnsureConfigured()
        {
            EnsureConfiguredImpl();
        }

        private static void EnsureConfiguredImpl()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                Configure();
                return null;
            });
        }

        private static void Configure()
        {
            RegisterConventions();

            BsonClassMap.RegisterClassMap<MongoIdentityUser>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Id));
                cm.MapCreator(user => new MongoIdentityUser(user.UserName, user.Email));
            });

            BsonClassMap.RegisterClassMap<MongoUserClaim>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(c => new MongoUserClaim(c.ClaimType, c.ClaimValue));
            });

            BsonClassMap.RegisterClassMap<MongoUserEmail>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(cr => new MongoUserEmail(cr.Value));
            });

            BsonClassMap.RegisterClassMap<MongoUserPhoneNumber>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(cr => new MongoUserPhoneNumber(cr.Value));
            });

            BsonClassMap.RegisterClassMap<MongoUserLogin>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(l => new MongoUserLogin(new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)));
            });
        }

        private static void RegisterConventions()
        {
            var pack = new ConventionPack
            {
                new IgnoreIfNullConvention(false),
                new CamelCaseElementNameConvention(),
            };

            ConventionRegistry.Register("Dnx.Identity.MongoDB", pack, IsConventionApplicable);
        }

        private static bool IsConventionApplicable(Type type)
        {
            return type == typeof(MongoIdentityUser)
                || type == typeof(MongoUserClaim)
                || type == typeof(MongoUserContactRecord)
                || type == typeof(MongoUserEmail)
                || type == typeof(MongoUserLogin)
                || type == typeof(MongoUserPhoneNumber)
                || type == typeof(ConfirmationOccurrence)
                || type == typeof(FutureOccurrence)
                || type == typeof(Occurrence);
        }
    }
}