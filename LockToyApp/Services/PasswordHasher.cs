
using LockToyApp.Helpers;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using ToyContracts;

namespace LockToyApp.Services
{
    public class PasswordHasher : IPasswordHasher
    {

        private Settings settings;

        public PasswordHasher(IOptionsSnapshot<Settings> settings)
        {
            this.settings = settings.Value;
        }


        public string Generate(string password)
        {
            var salt = settings.SaltString.ToByteArray();
            var pdkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);


            byte[] hash = pdkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}
