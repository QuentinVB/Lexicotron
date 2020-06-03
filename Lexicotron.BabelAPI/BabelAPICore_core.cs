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
            client.Timeout = TimeSpan.FromMinutes(5);
            //TODO : url forge (using string builder + param)
        }
       
    }
}
