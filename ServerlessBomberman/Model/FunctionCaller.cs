using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using ServerlessBomberman.Model;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessBomberman.LocalApp
{
    class FunctionCaller
    {
        private static int n = 1000;
        private static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            client.BaseAddress = new Uri("https://serverlessbomberman.azurewebsites.net/api");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            RunStaticMove();
        }

        private static async void RunStaticMove()
        {
            DateTime start = DateTime.Now;
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(CallStaticMoveAsync("/staticmove?dist=1"));
            }
            await Task.WhenAll();
            Console.WriteLine("\nTime = " + (DateTime.Now - start) / n);
        }

        private static async Task<String> CallStaticMoveAsync(string path)
        {
            String position = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                position = await response.Content.ReadAsStringAsync();
            }
            return position;
        }



    }
}
