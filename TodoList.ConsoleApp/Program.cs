using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.ConsoleApp
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string baseUrl = "https://localhost:49159/";
        private static readonly UnicodeEncoding jsonEncoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true);


        static async Task Main(string[] args)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpResponseMessage responseMessage;

            await BulkPostRecords();

            responseMessage = await MakeGetRequest();
            //responseMessage = await MakePostRequest();
            //responseMessage = await MakePutRequest();
            //responseMessage = await MakeDeleteRequest();

            Console.Write(await responseMessage.Content.ReadAsStringAsync());
        }


        #region Get Requests

        private static async Task<HttpResponseMessage> MakeGetRequest()
        {
            return await client.GetAsync($"{baseUrl}api/TodoItems");
        }

        private static async Task<HttpResponseMessage> MakeGetByIdRequest(int id)
        {
            return await client.GetAsync($"{baseUrl}api/TodoItems/{id}");
        }

        #endregion

        #region Post Requests

        private static async Task<HttpResponseMessage> MakePostRequest()
        {
            Console.WriteLine("What is the name of your task?");
            string taskName = Console.ReadLine();

            TodoItem item = new TodoItem() { Name = taskName };

            var contentValue = JsonConvert.SerializeObject(item);
            var stringContent = new StringContent(contentValue, jsonEncoding, "application/json");

            var response = await client.PostAsync($"{baseUrl}api/TodoItems", stringContent);

            return response;
        }

        #endregion

        #region Put Requests

        private static async Task<HttpResponseMessage> MakePutRequest()
        {
            Console.WriteLine("What is the id of the task you wish to update?");
            int taskId;
            int.TryParse(Console.ReadLine(), out taskId);
            var serializedItem = await (await MakeGetByIdRequest(taskId)).Content.ReadAsStringAsync();

            var item = JsonConvert.DeserializeObject<TodoItem>(serializedItem);

            Console.WriteLine("Is the task complete? ('true' or 'false')");
            bool isComplete;
            bool.TryParse(Console.ReadLine(), out isComplete);

            item.IsComplete = isComplete;

            var contentValue = JsonConvert.SerializeObject(item);
            var stringContent = new StringContent(contentValue, jsonEncoding, "application/json");

            var response = await client.PutAsync($"{baseUrl}api/TodoItems/{taskId}", stringContent);

            return response;
        }

        #endregion

        #region Delete Requests

        private static async Task<HttpResponseMessage> MakeDeleteRequest()
        {
            Console.WriteLine("What is the id of the task you wish to delete?");
            int taskId;
            int.TryParse(Console.ReadLine(), out taskId);

            var response = await client.DeleteAsync($"{baseUrl}api/TodoItems/{taskId}");

            return response;
        }

        #endregion

        private static async Task BulkPostRecords()
        {
            for(int i = 0; i < 10; i++)
            {
                TodoItem item = new TodoItem() { Name = $"task number {i}", IsComplete = false };
                var contentValue = JsonConvert.SerializeObject(item);
                var stringContent = new StringContent(contentValue, jsonEncoding, "application/json");

                var response = await client.PostAsync($"{baseUrl}api/TodoItems", stringContent);
            }
        }

        private class TodoItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public bool IsComplete { get; set; }
        }
    }
}
