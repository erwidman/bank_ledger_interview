/*
 * Author : Eric Richard Widmann
 * Date   : 1/18/2019
 * Description :
 *      Provides symmetric AES encryption for passwords via a 256bit RSA key.     
 *      Pretty weak as a security tactic, key is stored in client program and 
 *      is all that is needed to decrypt any password stored in db. Would be slightly
 *      better if the program used a web service to access the DB to prevent key
 *      distribution. This should suffice for the exercise however.
 */
using System;
using ArtisanCode.SimpleAesEncryption;
namespace Ledger.Security
{
   
    public static class Encryptor
    {
        private static RijndaelMessageEncryptor _encrypt = new RijndaelMessageEncryptor();
        private static RijndaelMessageDecryptor _decrypt = new RijndaelMessageDecryptor();
        /*
        * Description:
        *      Encrypts an input string and returns cyphertext
        * Params:
        *      string input -
        *           The string to be encrypted.
        * Return:
        *       A string representing the cypertext of encrypted input.
        */
        public static string Encrypt(string input)
        {
            return _encrypt.Encrypt(input);
        }
        /*
        * Description:
        *      Decrypts and input cyphertext.
        * Params:
        *      string input-
        *           The cyphertext to be decrypted.
        * Return:
        *       The decrypted cyphertext.
        *
        *
        */
        public static  string Decrypt(string input)
        {
            return _decrypt.Decrypt(input);
        }

    }
}
