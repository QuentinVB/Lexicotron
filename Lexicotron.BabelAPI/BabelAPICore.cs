using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lexicotron.BabelAPI.Models;
using Lexicotron.Database.Models;

namespace Lexicotron.BabelAPI
{
    public class BabelAPICore
    {
        readonly string _apikey;

        HttpClient client = new HttpClient();
        DALAPIAdapter _dal;

        public string Apikey => _apikey;

        internal DALAPIAdapter Database { get => _dal; set => _dal = value; }

        public BabelAPICore(string apikey)
        {
            _apikey =apikey;
            //TODO : url forge (using string builder + param)
        }

        public void RetrieveWordSense(IEnumerable<IWord> words)
        {
            int requestsAvailable = Database.RequestsAvailable();

            Database.GetWordsWithoutSynset(requestsAvailable);
        }
        /*
        public async Task RunAsync()
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
        public void ShowProject(Project project)
        {
            Console.WriteLine($"Title: {project.Title}\nId: " +
                $"{project.Id}");
        }

        public async Task<Project> GetProjectAsync(string path)
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
        }*/
    }
}
