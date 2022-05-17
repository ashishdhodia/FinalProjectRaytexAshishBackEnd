using Microsoft.Azure.Cosmos;

namespace EDidataToCosmos
{
    public class CosmosConnect
    {
        private static readonly string EndpointURI = "https://localhost:8081";
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        private CosmosClient cosmosClient;
        private Microsoft.Azure.Cosmos.Database database;
        private Container container;

        private string databaseId = "EDIdb";
        private string containerId = "EDIdata";

        public CosmosConnect()
        {
            try
            {
                Console.WriteLine("Establish Connection..");
                this.cosmosClient = new CosmosClient(EndpointURI, PrimaryKey);
            }
            catch (CosmosException cosmosException)
            {
                Console.WriteLine($"CosmosDb exception {cosmosException.StatusCode}: {cosmosException}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e}");
            }
        }
        public async Task CreateDatabaseAsync()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine($"Database Created : {this.database.Id}");
        }
        public async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(this.containerId, "/id");
            Console.WriteLine($"Container Created : {this.containerId}");
        }

        public async Task AddItemsToContainerAsync(dynamic data)
        {
            try
            {
                ItemResponse<dynamic> response = await this.container.ReadItemAsync<dynamic>(data["id"].ToString(), new PartitionKey(data["id"].ToString()));
                ItemResponse<dynamic> responsee = await this.container.ReplaceItemAsync<dynamic>(data, data["id"].ToString(), new PartitionKey(data["id"].ToString()));
            }
            catch (Exception e)
            {
                ItemResponse<dynamic> response = await this.container.CreateItemAsync<dynamic>(data, new PartitionKey(data["id"].ToString()));
            }
        }
    }
}
