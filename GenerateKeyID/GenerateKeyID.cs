using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GenerateKeyID
{
   static class GenerateKeyID
   {
      static Random ran = new Random();

      static int APPID = 415;

      public static string GetAuthKeyID(string inputString)
      {
         StringBuilder sb = new StringBuilder();

         foreach (byte b in GetAuthKey(inputString))
            sb.Append(b.ToString("X2"));

         return sb.ToString();
      }

      public static string GetUserKeyID(string UserName)
      {
         StringBuilder sb = new StringBuilder();

         foreach (byte b in GetUserHash(UserName))
            sb.Append(b.ToString("X2"));

         return sb.ToString();
      }

      private static byte[] GetAuthKey(string InputString)
      {
         string TimeAuthenication = DateTime.Now.ToString();
         
         HashAlgorithm algorithm = SHA256.Create();

         return algorithm.ComputeHash(Encoding.UTF8.GetBytes(InputString + APPID + TimeAuthenication + ran.Next(1,1000).ToString()));
      }

      private static byte[] GetUserHash(string InputString)
      {
         HashAlgorithm algorithm = SHA256.Create();

         return algorithm.ComputeHash(Encoding.UTF8.GetBytes(InputString));
      }
   }
}
