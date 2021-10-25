using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessBomberman.LocalApp
{
    class FunctionCaller
    {
        private static int n = 100;

        static void Main(string[] args)
        {
            var staticMove = RunStaticMoveAsync();
            staticMove.Wait();

            var cosmosDBMove = RunCosmosDBMoveAsync();
            cosmosDBMove.Wait();
        }

        private static async Task RunStaticMoveAsync()
        {
            var client = new HttpClient();

            var tasks = new List<Task<string>>();
            for (int i = 0; i < n; i++)
            {
                async Task<string> func()
                {
                    var response = await client.GetAsync("https://serverlessbomberman.azurewebsites.net/api/staticmove?dist=1");
                    return await response.Content.ReadAsStringAsync();
                }

                tasks.Add(func());
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            watch.Stop();

            var postResponses = new List<string>();
            foreach (var t in tasks)
            {
                string postResponse = await t; //t.Result would be okay too.
                postResponses.Add(postResponse);
            }
            postResponses.ForEach(p => Debug.Write(p + "  "));

            Debug.WriteLine("\nTime Async (Static) = " + watch.ElapsedMilliseconds / n + "\n\n");
        }

        private static async Task RunCosmosDBMoveAsync()
        {
            var client = new HttpClient();

            var tasks = new List<Task<string>>();
            for (int i = 0; i < n; i++)
            {
                async Task<string> func()
                {
                    var response = await client.GetAsync("https://serverlessbomberman.azurewebsites.net/api/CosmosDBMove?id=testId&dist=1");
                    return await response.Content.ReadAsStringAsync();
                }

                tasks.Add(func());
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            watch.Stop();

            var postResponses = new List<string>();
            foreach (var t in tasks)
            {
                string postResponse = await t; //t.Result would be okay too.
                postResponses.Add(postResponse);
            }
            postResponses.ForEach(p => Debug.Write(p + "  "));
            Debug.WriteLine("\nTime Async (CosmosDB) = " + watch.ElapsedMilliseconds / n + "\n\n");
        }

    }
}
