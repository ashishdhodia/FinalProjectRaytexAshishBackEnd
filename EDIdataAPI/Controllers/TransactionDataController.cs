using Microsoft.AspNetCore.Mvc;
using EDIdataAPI.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EDIdataAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionDataController : ControllerBase
    {
        private readonly SqlConnection _con;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable = new DataTable();

        private string connectionString = "Endpoint=sb://trngahmedabd.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kkNMAoMhIPaQouXaiB570uBPdiXSba1lHxOHebdJ5go=";
        private string topicName = "topic-ashish";
        private ServiceBusClient client;
        private ServiceBusSender sender;

        public TransactionDataController()
        {
            _con = new SqlConnection("Server=.; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
            _con.Open();
        }

        // GET: api/<TransactionDataController>
        [HttpGet]
        public List<TransactionData> Get()
        {
            List<TransactionData> transactionData = new List<TransactionData>();
            dataAdapter = new SqlDataAdapter("SELECT * FROM transactionData", _con);
            dataAdapter.Fill(dataTable);
            _con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                transactionData.Add(new TransactionData
                {
                    Id = Convert.ToInt32(row["id"]),
                    containerId = Convert.ToString(row["containerId"]),
                    containerFees = Convert.ToInt32(row["containerFees"]),
                    userId = Convert.ToString(row["userId"]),
                    cardOwnerName = Convert.ToString(row["cardOwnerName"]),
                    cardType = Convert.ToString(row["cardType"]),
                    cardNumber = Convert.ToInt32(row["cardNumber"]),
                    txnTime = Convert.ToString(row["txnTime"])
                });
            }
            return transactionData;
        }

        // POST api/<TransactionDataController>
        [HttpPost]
        public async void Post([Bind("Id,containerId,containerFees, userId, cardOwnerName, cardType, cardNumber, txnTime")] TransactionData transactionData)
        {
            transactionData.txnTime = DateTime.Now.ToString();
            string dataForTopic = JsonConvert.SerializeObject(transactionData);

            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topicName);
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            messageBatch.TryAddMessage(new ServiceBusMessage(dataForTopic));
            try
            {
                await sender.SendMessagesAsync(messageBatch);
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            //if (ModelState.IsValid)
            //{
            //    dataAdapter = new SqlDataAdapter($"INSERT INTO transactionData(containerId,containerFees, userId, cardOwnerName, cardType, cardNumber, txnTime) VALUES('{transactionData.containerId}','{transactionData.containerFees}', '{transactionData.userId}', '{transactionData.cardOwnerName}', '{transactionData.cardType}', '{transactionData.cardNumber}', '{transactionData.txnTime}')", _con);
            //    dataAdapter.Fill(dataTable);
            //}
        }
    }
}
