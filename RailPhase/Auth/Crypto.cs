using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using CryptSharp.Utility;

namespace RailPhase
{
    public static class Crypto
    {
        public static byte[] RandomData(int bytes)
        {
            RandomNumberGenerator rng = new RNGCryptoServiceProvider();
            byte[] data = new byte[bytes];
            rng.GetBytes(data);

            return data;
        }

        public static string RandomBase64(int bytes = 16)
        {
            return Convert.ToBase64String(RandomData(bytes));
        }

        public static string RandomHex(int bytes = 16)
        {
            return BitConverter.ToString(RandomData(bytes)).Replace("-", "");
        }

        public static string GetSecretSaltBase64()
        {
            var saltBase64 = RailPhaseConfig.Default.SecretSalt;

            if (String.IsNullOrEmpty(saltBase64))
            {
                saltBase64 = RandomBase64(16);
                RailPhaseConfig.Default.SecretSalt = saltBase64;
                RailPhaseConfig.Default.Save();
            }

            return saltBase64;
        }

        public static byte[] GetSecretSalt()
        {
            var salt16Bytes = Convert.FromBase64String(GetSecretSaltBase64());
            return salt16Bytes;
        }

        const int BcryptDifficulty = 9;

        public static string HashPassword(string password)
        {
            var salt = GetSecretSalt();

            var hashedBytes = BlowfishCipher.BCrypt(Encoding.UTF8.GetBytes(password), salt, BcryptDifficulty);
            return Convert.ToBase64String(hashedBytes);
        }


    }
}
