namespace Dnx.Identity.MongoDB.Models
{
    public class MongoUserPhoneNumber : MongoUserContactRecord
    {
        public MongoUserPhoneNumber(string phoneNumber) : base(phoneNumber)
        {
        }
    }
}
