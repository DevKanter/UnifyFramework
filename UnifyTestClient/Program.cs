using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifyFramework.ClientSide;

namespace UnifyTestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new UnifyClient("127.0.0.1", 40550);
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
