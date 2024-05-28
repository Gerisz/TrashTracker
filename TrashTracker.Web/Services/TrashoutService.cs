using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Data;
using System.Text;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Services
{
    public class TrashoutService
    {
        private TrashTrackerDbContext _context;
        private IConfiguration _config;
        private ILogger<TrashoutService> _logger;

        public readonly HttpClient TrashoutClient;

        public TrashoutService(TrashTrackerDbContext context, IConfiguration config, ILogger<TrashoutService> logger, HttpClient trashoutClient)
        {
            _context = context;
            _config = config;
            _logger = logger;
            TrashoutClient = trashoutClient;
        }

        public async Task<String> GetTokenAsync()
        {
            // put email and password into a JSON format for body
            String body = $"{{\"email\":\"{LoginSecrets.Email}\"," +
                $"\"password\":\"{LoginSecrets.Password}\",\"returnSecureToken\":\"true\"}}";
            // send the url with the Google API key, wait for and store response
            var response = await TrashoutClient.PostAsync(
                "https://www.googleapis.com/identitytoolkit/v3/relyingparty/" +
                "verifyPassword?key=AIzaSyAyQZZmcJl4X9dJAndOmhoLkcK16KOQzcI",
                new StringContent(body, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            // and it's content
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            // get the idToken from the response's content's JSON
            var data = Serializer.Deserialize<TokenFromGoogle>(responseContent);
            // and return it
            return data.IdToken;
        }

        /// <summary>
        /// Get the list of trash since the last update from Trashout API
        /// and filter them by countries in <see cref="Country"/>.
        /// </summary>
        /// <param name="token">Token for TrashOut API.</param>
        /// <param name="limit">Maximum number of points to get.</param>
        /// <returns> List of trash filtered by countries in <see cref="Country"/>.</returns>
        public async Task<List<TrashFromTrashout>> GetPlaceListFromTrashoutAsync(String token, Int32? limit)
        {
            // add token to header from parameter
            TrashoutClient.DefaultRequestHeaders.Add("x-token", token);

            // beginning of url should be the API endpoint
            String formattedTrashouturl = "https://api.trashout.ngo/v1/trash/" +
                "?attributesNeeded=id,gpsShort,gpsFull,types,size,note,userInfo,anonymous," +
                "status,cleanedByMe,images,updateTime,url,created,accessibility,updateNeeded" +
                "&geoAreaContinent=Europe";

            // add count limit to query
            formattedTrashouturl += "&limit=" + (limit ?? 999999);

            // get the latest update of trash places
            DateTime? latestUpdate = _context.LatestUpdate();

            // if found
            if (latestUpdate != null)
            {
                // then format it into ISO 8601 and concat to the end of the url
                formattedTrashouturl += "&timeBoundaryFrom=" + latestUpdate.ToJson();
            }

            // send the url, wait for, and store response
            var response = await TrashoutClient.GetAsync(formattedTrashouturl).ConfigureAwait(false);
            // and it's content
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            List<TrashFromTrashout> deserializedModel = new List<TrashFromTrashout>();
            // deserialise the json response's content into FromTrashout DTOs
            deserializedModel = Serializer.Deserialize<List<TrashFromTrashout>>(responseContent);
            // and filter to countries declared in Country.cs' enum
            deserializedModel = deserializedModel
                .Where(w => Enum.GetNames(typeof(Country))
                .Contains(w.Gps.Area!.Country))
                .ToList();

            return deserializedModel;
        }

        public async Task GetPlaceListAsync(Int32? limit)
        {
            // requests times out after 5 minutes
            TrashoutClient.Timeout = TimeSpan.FromMinutes(5);

            // get token from Google's API
            var token = await GetTokenAsync();

            // get data into DTOs from Trashout's API
            var deserializedModel = await GetPlaceListFromTrashoutAsync(token, limit);

            var dbType = _config.GetValue<DbType>("DbType");

            // convert DTOs trash place
            var toAdd = deserializedModel
                .Select(x =>
                {
                    return new Trash(x);
                })
                .ToList();

            // convert DTO trash places, that were in the database already
            var toRemove = _context.Trashes
                .Where(x => x.TrashoutId != null && deserializedModel.Select(x => (Int32?)x.Id).Contains(x.TrashoutId))
                .ToList();

            try
            {
                _context.RemoveRange(toRemove);
                _context.AddRange(toAdd);
                _context.SaveChanges();
            }
            catch (DbUpdateException) { }
        }
    }
}
