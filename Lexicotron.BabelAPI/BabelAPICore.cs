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
using Lexicotron.Database;

namespace Lexicotron.BabelAPI
{
    public class BabelAPICore
    {
        readonly string _apikey;

        HttpClient client ;
        DALAPIAdapter _dal;

        public string Apikey => _apikey;

        internal DALAPIAdapter Database { get => _dal; set => _dal = value; }

        public BabelAPICore(string apikey, LocalWordDB database)
        {
            _apikey =apikey;
            _dal = new DALAPIAdapter(database);
            
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
        public List<(BabelLog, List<BabelSense>)> RetrieveWordSense(int limit)
        {
            int requestsAvailable = Database.RequestsAvailable();

            List<DbWord> wordswithoutsynset = Database.GetWordsWithoutSynset(Math.Min(requestsAvailable,limit)).ToList();

            List<(BabelLog, List<BabelSense>)> results = ResolveRequests(wordswithoutsynset).Result.ToList();

            return results;
        }
        
        private async Task<IEnumerable<(BabelLog, List<BabelSense>)>> ResolveRequests(List<DbWord> words)
        {
            List<Task<(BabelLog, List<BabelSense>)>> listOfTasks = new List<Task<(BabelLog, List<BabelSense>)>>();
            try
            {
                foreach (DbWord word in words)
                {
                    listOfTasks.Add(GetBabelSenseAsync(word));
                }
                return await Task.WhenAll(listOfTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        
        public async Task<(BabelLog, List<BabelSense>)> GetBabelSenseAsync(DbWord word)
        {
            HttpResponseMessage response = await client.GetAsync($"https://babelnet.io/v5/getSenses?lemma={word.Word}&searchLang=FR&key={Apikey}");
            if (response.IsSuccessStatusCode)
            {
                //using newton soft
                //Project m = JsonConvert.DeserializeObject<Project>(await response.Content.ReadAsStringAsync());
                var babelsense = response.Content.ReadAsAsync<List<BabelSense>>().Result;

                BabelLog logentry = new BabelLog() {RequestedSynset = word.Word, JsonReturned = response.Content.ReadAsStringAsync().Result };

                return (logentry, babelsense);
            }
            throw new TimeoutException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordsenses"></param>
        /// <returns>tuple with the db word to update and the list of db words to add</returns>
        public HashSet<DbWord> ParseBabelSenseToDbWord(List<(BabelLog, List<BabelSense>)> wordsenses)
        {

            HashSet<DbWord> dbWords = new HashSet<DbWord>();
            foreach (var item in wordsenses)
            {
                DbWord requestedInitialy = new DbWord()
                {
                    Word = item.Item1.RequestedSynset,
                };

                dbWords.Add(requestedInitialy);

                //log results
                Database.LogBabelRequest(item.Item1.RequestedSynset, item.Item1.JsonReturned);

                //var wordsense = item.Item2.Where(x => x.Properties.FullLemma.ToLower() == item.Item1.RequestedSynset.ToLower()).FirstOrDefault();

                //store sense as dbword               
                foreach(BabelSense babelSense in item.Item2)
                {
                    if (babelSense.Properties.FullLemma.Contains("_")) continue;
                    else if (babelSense.Properties.Language != "FR") continue;
                    else if(
                        babelSense.Properties.Pos.Contains("NOUN")
                        || babelSense.Properties.Pos.Contains("VER")
                        || babelSense.Properties.Pos.Contains("ADJ")
                        )
                    {
                        if (requestedInitialy.SynsetId == null && babelSense.Properties.FullLemma.ToLower() == requestedInitialy.Word)
                        {
                            requestedInitialy.SynsetId = babelSense.Properties.SynsetId.Id;
                            requestedInitialy.SenseId = babelSense.Properties.IdSense;
                        }
                        else
                        {
                            dbWords.Add(new DbWord()
                            {
                                Word = babelSense.Properties.FullLemma.ToLower(),
                                SynsetId = babelSense.Properties.SynsetId.Id,
                                SenseId = babelSense.Properties.IdSense
                            });
                        }
                    }
                    
                    
                }
            }
            return dbWords;
        }
    }
}
