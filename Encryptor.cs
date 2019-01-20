using System;
using ArtisanCode.SimpleAesEncryption;
namespace Ledger.Security
{
   
    public static class Encryptor
    {
        private static RijndaelMessageEncryptor _encrypt = new RijndaelMessageEncryptor();
        private static RijndaelMessageDecryptor _decrypt = new RijndaelMessageDecryptor();
   
        public static string Encrypt(string input)
        {
            return _encrypt.Encrypt(input);
        }
        public static  string Decrypt(string input)
        {
            return _decrypt.Decrypt(input);
        }

    }
}
