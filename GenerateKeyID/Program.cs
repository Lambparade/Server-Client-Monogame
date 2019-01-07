using System;

namespace GenerateKeyID
{
   class Program
   {
      static void Main(string[] args)
      {
         string KeyID = GenerateKeyID.GetKeyID("test");
         string UserKeyID = GenerateKeyID.GetUserKeyID("test");

         Console.WriteLine(KeyID);
         Console.WriteLine(UserKeyID);

         Console.ReadKey();
      }
   }
}
