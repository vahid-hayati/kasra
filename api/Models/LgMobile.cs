using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models;

public record BuyPhone(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)] string? Id,
    [MinLength(3), MaxLength(20)] string PhoneModel,
    [Range(1000000, 90000000)] double Price,
    [MinLength(3), MaxLength(25)] string Software,
    bool HasHeadPhones,
    bool Support5G
);