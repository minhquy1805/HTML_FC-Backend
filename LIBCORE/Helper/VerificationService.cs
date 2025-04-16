

using System;

namespace LIBCORE.Helper
{
    public class VerificationService
    {
        public string GenerateVerificationCode(int length = 6)
        {
            var randomNumber = new Random();
            string code = "";
            for (int i = 0; i < length; i++)
            {
                code += randomNumber.Next(0, 10); // random số từ 0 đến 9
            }
            return code;
        }
    }
}
