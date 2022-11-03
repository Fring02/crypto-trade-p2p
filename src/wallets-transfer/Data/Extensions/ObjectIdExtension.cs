using MongoDB.Bson;

namespace Data.Extensions;

public static class ObjectIdExtension
{
    public static bool IsParsable(string s) => ObjectId.TryParse(s, out _);
}