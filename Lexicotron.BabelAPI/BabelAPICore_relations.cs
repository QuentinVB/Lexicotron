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
    public partial class BabelAPICore
    {
       
        public List<(BabelLog, DbWord, List<BabelEdge>)> RetrieveRelations(int limit)
        {
            int requestsAvailable = Database.RequestsAvailable();

            List<DbWord> wordswithoutrelation = Database.GetWordsWithoutRelation(Math.Min(requestsAvailable,limit)).ToList();

            List<(BabelLog, DbWord, List<BabelEdge>)> results = ResolveRelationsRequests(wordswithoutrelation).Result.ToList();

            return results;
        }
        
        private async Task<IEnumerable<(BabelLog, DbWord, List<BabelEdge>)>> ResolveRelationsRequests(List<DbWord> words)
        {
            List<Task<(BabelLog, DbWord, List<BabelEdge>)>> listOfTasks = new List<Task<(BabelLog, DbWord, List<BabelEdge>)>>();
            try
            {
                foreach (DbWord word in words)
                {
                    listOfTasks.Add(GetBabelRelationsAsync(word));
                }
                return await Task.WhenAll(listOfTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        
        public async Task<(BabelLog, DbWord, List<BabelEdge>)> GetBabelRelationsAsync(DbWord word)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"https://babelnet.io/v5/getOutgoingEdges?id={word.SynsetId}&key={Apikey}"
                );
                if (response.IsSuccessStatusCode)
                {
                    //using newton soft
                    //Project m = JsonConvert.DeserializeObject<Project>(await response.Content.ReadAsStringAsync());
                    var babelsense = response.Content.ReadAsAsync<List<BabelEdge>>().Result;

                    BabelLog logentry = new BabelLog() { RequestedSynset = word.SynsetId, JsonReturned = response.Content.ReadAsStringAsync().Result };

                    return (logentry, word, babelsense);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            
            throw new TimeoutException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordsenses"></param>
        /// <returns>tuple with the db word to update and the list of db words to add</returns>
        public HashSet<DbRelation> ParseBabelRelationsToDbRelation(List<(BabelLog, DbWord, List<BabelEdge>)> relations)
        {
            HashSet<DbRelation> dbRelations = new HashSet<DbRelation>();
            foreach (var item in relations)
            {
                //log results
                Database.LogBabelRequest(item.Item1.RequestedSynset, item.Item1.JsonReturned);

                if (item.Item3.Count == 0) continue;

                //store sense as dbword               
                foreach (BabelEdge babelEdge in item.Item3)
                {
                    if (!(babelEdge.Language == "FR" || babelEdge.Language == "EN")) continue;
                    //if (babelEdge.Pointer.RelationGroup == "OTHER") continue;
                    dbRelations.Add(new DbRelation()
                    {
                        WordSourceId = item.Item2.WordId,
                        RelationGroup= babelEdge.Pointer.RelationGroup.ToLower(),
                        TargetSynsetId = babelEdge.Target,
                        WordSource= item.Item2
                    });
                }
            }
            return dbRelations;
        }
    }
}
