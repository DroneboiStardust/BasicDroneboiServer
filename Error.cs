using System;

namespace Error
{
    class SoftError
    {
        public SoftError(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: "+str);
            Console.ResetColor();
            this.errorStr = str;
        }
        public string errorStr;
    }
}