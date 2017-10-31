using System;

namespace AspNetCore.Identity.MongoDB.Models
{
    public class MongoUserToken 
    {
        public string LoginProvider { get; set; }
        public string TokenName { get;  set; }
        public string TokenValue { get;  set; }
    }
}