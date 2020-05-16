using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace APIRequestsTest
{
    public class Project
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();

        }

        static void ShowProject(Project project)
        {
            Console.WriteLine($"Title: {project.Title}\nId: " +
                $"{project.Id}");
        }

        static async Task<Project> GetProjectAsync(string path)
        {
            Project product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                //using newton soft
                //Project m = JsonConvert.DeserializeObject<Project>(await response.Content.ReadAsStringAsync());

                product = await response.Content.ReadAsAsync<Project>();
            }
            return product;
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localdev.les-planetes2kentin.fr/api/project/1");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );

            try
            {
                Project project;
                // Get the product
                project = await GetProjectAsync("http://localdev.les-planetes2kentin.fr/api/project/1");
                ShowProject(project);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
