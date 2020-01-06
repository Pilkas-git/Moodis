using System;
using System.Security.Cryptography;

namespace apiMoodis.Encryption
{
    public class Crypto
    {
        static public string EncryptPassword(string password)
        {
            char[] array = password.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        static public bool ComparePasswords(string hashedPassword, string userInput)
        {
            if (EncryptPassword(userInput) == hashedPassword) return true;
            else return false;
        }
    }
}