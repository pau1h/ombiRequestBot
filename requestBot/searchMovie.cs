using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.General;
using TMDbLib.Client;

namespace requestBot
{
    class searchMovie
    {
        public static async Task<SearchContainer<SearchMovie>> getResultsAsync(String mediaName, TMDbClient client)
        {
            SearchContainer<SearchMovie> results = await client.SearchMovieAsync(mediaName);

            return results;
         }
            
    }
}

