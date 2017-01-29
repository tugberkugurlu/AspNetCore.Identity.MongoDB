using System;
using System.Linq;
using MongoDB.Bson;

namespace AspNetCore.Identity.MongoDB.Tests.Common
{
    internal static class TestUtils
    {
        private static readonly Random Random = new Random();

        /// <remarks>
        /// See http://stackoverflow.com/a/1344242/463785.
        /// </remarks>
        public static string RandomString(int length) => 
            new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());

        public static string NewId() => ObjectId.GenerateNewId().ToString();
    }
}