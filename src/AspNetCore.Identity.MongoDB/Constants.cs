using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AspNetCore.Identity.MongoDB.Tests")]
namespace AspNetCore.Identity.MongoDB
{
    internal static class Constants
    {
        public const string DefaultCollectionName = "users";
    }
}