using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ConsoleHelpers
    {
        public static int RequestInt(string ErrorMessage)
        {
            bool flag = false;
            int result = 0;
            do
            {
                string read = Console.ReadLine();
                flag = int.TryParse(read, out result);
                if (!flag)
                {
                    Console.WriteLine(ErrorMessage);
                }
            } while (!flag);
            return result;
        }
        public static string RequestNonEmptyText(string ErrorMessage)
        { 
            bool flag = false;
            string read = "";
            do 
            {
                read = Console.ReadLine();
                flag = (read != String.Empty);
                if (!flag)
                { 
                    Console.WriteLine(ErrorMessage);
                }
            }while (!flag);
            return read;
        }
    }
}
