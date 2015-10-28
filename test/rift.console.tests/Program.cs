using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace rift.console.tests
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();

            var response = client.GetAsync("http://localhost:9000/api/head/").Result;
            Console.WriteLine(response);

            Console.ReadKey();
        }
    }
}
