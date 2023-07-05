using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HorusAPI.Entities
{
    /// <summary>
    /// Entity that represents a user
    /// </summary>
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id {get; set;}

        [BsonElement("Email")]
        public string? Email { get; set;}

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Password")]
        public string? Password { get; set; }

        [BsonElement("Salt")]
        public string? Salt { get; set; }

        [BsonElement("Role")]
        public Role? Role { get; set; }
    }

    public enum Role
    {
        [BsonRepresentation(BsonType.String)]
        Admin,
        [BsonRepresentation(BsonType.String)]
        User,
        [BsonRepresentation(BsonType.String)]
        Developer,
    }
}
