using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
//using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Container = Microsoft.Azure.Cosmos.Container;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EDIdataAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class edidataController : ControllerBase
    {
        private static readonly string EndpointURI = "https://localhost:8081";
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        private string databaseId = "EDIdb";
        private string containerId = "EDIdata";

        private CosmosClient cosmosClient;
        private Database database;
        private Container container;


        // GET: api/<edidataController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string query = "SELECT c.id FROM c";

            this.cosmosClient = new CosmosClient(EndpointURI, PrimaryKey);
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(this.databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(this.containerId, "/id");

            QueryDefinition queryDefinition = new QueryDefinition(query);
            using FeedIterator<IDictionary<string, object>> queryResultSetIterator = this.container.GetItemQueryIterator<IDictionary<string, object>>(queryDefinition);

            List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<IDictionary<string, object>> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (IDictionary<string, object> data in currentResultSet)
                {
                    result.Add(data);
                }
            }

            JObject temp = JObject.Parse("{ 'Data': []}");
            IList<string> FinalResult = new List<string>();

            foreach (var data in result)
            {
                ResponseMessage response = await container.ReadItemStreamAsync(data["id"].ToString(), new PartitionKey(data["id"].ToString()));
                StreamReader reader = new StreamReader(response.Content);
                dynamic txnData = JsonConvert.DeserializeObject(reader.ReadToEnd().ToString());
                ((JArray)temp["Data"]).Add(txnData);
            }
            return Ok(JsonConvert.SerializeObject(temp));
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetItemAsync(string id)
        //{
        //    JObject temp = JObject.Parse("{ 'Data': []}");
        //    try
        //    {
        //        ResponseMessage response = await container.ReadItemStreamAsync(id, new PartitionKey(id));
        //        StreamReader reader = new StreamReader(response.Content);
        //        dynamic txnData = JsonConvert.DeserializeObject(reader.ReadToEnd().ToString());
        //        ((JArray)temp["Data"]).Add(txnData);
        //        return Ok(JsonConvert.SerializeObject(temp));

        //    }
        //    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        return null;
        //    }
        //}
    }
}
