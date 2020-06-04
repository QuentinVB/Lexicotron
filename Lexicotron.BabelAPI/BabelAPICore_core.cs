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
    /// <summary>
    /// The main class to call babel api
    /// </summary>
    public partial class BabelAPICore
    {
        readonly string _apikey;
        HttpClient client ;
        DALAPIAdapter _dal;

        /// <summary>
        /// The string representation of the user's babel api key
        /// </summary>
        public string Apikey => _apikey;
        /// <summary>
        /// proxy adapter for the database
        /// </summary>
        internal DALAPIAdapter Database { get => _dal; set => _dal = value; }
        /// <summary>
        /// Create a new instance of the api with a given database DAL and the babel API key
        /// </summary>
        /// <param name="apikey">The babel API Key</param>
        /// <param name="database">DAL for the </param>
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
            client.Timeout = TimeSpan.FromMinutes(5);
            //TODO : url forge ? (using string builder + param)
        }
       
    }
}
