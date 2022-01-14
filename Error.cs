using System;

namespace Error
{
    class SoftError
    {
        public SoftError(string str)
        {
            Console.WriteLine("Error: "+str);
            this.errorStr = str;
        }
        public string errorStr;
    }
}