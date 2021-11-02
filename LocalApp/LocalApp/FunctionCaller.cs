using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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

            var durableEntitiesMove = RunDurableEntitiesMoveAsync();
            durableEntitiesMove.Wait();

            var durableEntitiesWithOrchestrationMove = RunDurableEntitiesWithOrchestrationMoveAsync();
            durableEntitiesWithOrchestrationMove.Wait();

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

        private static async Task RunDurableEntitiesMoveAsync()
        {
            var client = new HttpClient();

            var tasks = new List<Task<string>>();
            for (int i = 0; i < n; i++)
            {
                async Task<string> func()
                {
                    var response = await client.GetAsync("https://serverlessbomberman.azurewebsites.net/api/move/testId/1?");
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

            Debug.WriteLine("\nTime Async (Durable Entities) = " + watch.ElapsedMilliseconds / n + "\n\n");
        }

        private static async Task RunDurableEntitiesWithOrchestrationMoveAsync()
        {
            var client = new HttpClient();

            var tasks = new List<Task<string>>();
            for (int i = 0; i < n; i++)
            {
                async Task<string> func()
                {
                    var response = await client.GetAsync("https://serverlessbomberman.azurewebsites.net/api/move2/testId/1?");
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

            Debug.WriteLine("\nTime Async (Durable Entities w/ Orchestration) = " + watch.ElapsedMilliseconds / n + "\n\n");
        }

    }
}
