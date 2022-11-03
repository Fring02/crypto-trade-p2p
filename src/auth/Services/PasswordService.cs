using System.Security.Cryptography;
using System.Text;
using AuthService.Models;

namespace AuthService.Services;

static class PasswordService
{
    public static bool VerifyPassword(string password, User user)
    {
        using var hmac = new HMACSHA256(user.Salt);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(password)).SequenceEqual(user.Hash);
    }

    public static void HashPassword(string password, User user)
    {
        using var hmac = new HMACSHA256();
        user.Salt = hmac.Key;
        user.Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}