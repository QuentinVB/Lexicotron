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
using System.IO;
using System.Net;

namespace Lexicotron.BabelAPI
{
    public class BabelAPICore
    {
        readonly string _apikey;

        HttpClient client ;
        DALAPIAdapter _dal;

        public string Apikey => _apikey;

        internal DALAPIAdapter Database { get => _dal; set => _dal = value; }

        public BabelAPICore(string apikey)
        {
            _apikey =apikey;
            _dal = new DALAPIAdapter(new Database.LocalWordDB());
            
            //client.DefaultRequestHeaders.Accept.Add(new );

            var handler = new HttpClientHandler();
            
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //TODO : url forge (using string builder + param)
        }
        //IEnumerable<IWord> words
        public void RetrieveWordSense(int limit)
        {
            int requestsAvailable = Database.RequestsAvailable();

            List<DbWord> wordswithoutsynset = Database.GetWordsWithoutSynset(Math.Min(requestsAvailable,limit)).ToList();

            List<string> allRequests = new List<string>();

            foreach (DbWord word in wordswithoutsynset)
            {
                if (word.SynsetId != null) throw new InvalidDataException();
                string url = $"https://babelnet.io/v5/getSenses?lemma={word.Word}&searchLang=FR&key={Apikey}";
                //var data = await GetBabelSenseAsync(url);

                allRequests.Add(url);
            }

            var results = ResolveRequests(allRequests).Result.ToList();

            Console.WriteLine(results.Count());

            //TODO : insert retrieved synset into database

        }
        
        private async Task<IEnumerable<List<BabelSense>>> ResolveRequests(List<string> urls)
        {
            List<Task<List<BabelSense>>> listOfTasks = new List<Task<List<BabelSense>>>();
            try
            {
                foreach (string url in urls)
                {
                    listOfTasks.Add(GetBabelSenseAsync(url));
                }
                return await Task.WhenAll(listOfTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        
        public async Task<List<BabelSense>> GetBabelSenseAsync(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                //using newton soft
                //Project m = JsonConvert.DeserializeObject<Project>(await response.Content.ReadAsStringAsync());
                var babelsense = response.Content.ReadAsAsync<List<BabelSense>>().Result;
                return babelsense;
            }
            throw new TimeoutException();

        }
    }
}
