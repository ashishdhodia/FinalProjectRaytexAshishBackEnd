using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using TransactionServiceBus.Model;

namespace TransactionServiceBus
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionServiceBus : StatelessService
    {
        public TransactionServiceBus(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        static SqlConnection _con;
        static SqlDataAdapter dataAdapter;
        static DataTable dataTable = new DataTable();

        static string connectionString = "Endpoint=sb://trngahmedabd.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kkNMAoMhIPaQouXaiB570uBPdiXSba1lHxOHebdJ5go=";
        static string topicName = "topic-ashish";
        static string subscriptionName = "subscription-ashish";
        static ServiceBusClient client;
        static ServiceBusSender sender;
        static ServiceBusProcessor processor;
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {

            static async Task MessageHandler(ProcessMessageEventArgs args)
            {
                string body = args.Message.Body.ToString();

                _con = new SqlConnection("Server=.; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
                _con.Open();

                Console.WriteLine($"Received: {body} from subscription: {subscriptionName}");

                string filePath = @"D:\intership\ex\test.txt";

                //string dataForPost = JsonConvert.SerializeObject(body);

                //string dataForIdRemoval = JsonConvert.SerializeObject(body);

                dynamic data = JsonConvert.DeserializeObject(body);
                data.Property("Id").Remove();

                //dynamic data = JsonConvert.DeserializeObject(objectData);

                TransactionData transactionData = new TransactionData();
                transactionData.containerId = data.containerId;
                transactionData.containerFees = data.containerFees;
                transactionData.userId = data.userId;
                transactionData.cardOwnerName = data.cardOwnerName;
                transactionData.cardType = data.cardType;
                transactionData.cardNumber = data.cardNumber;
                transactionData.txnTime = data.txnTime;

                dataAdapter = new SqlDataAdapter($"INSERT INTO transactionData(containerId,containerFees, userId, cardOwnerName, cardType, cardNumber, txnTime) VALUES('{transactionData.containerId}','{transactionData.containerFees}', '{transactionData.userId}', '{transactionData.cardOwnerName}', '{transactionData.cardType}', '{transactionData.cardNumber}', '{transactionData.txnTime}')", _con);
                dataAdapter.Fill(dataTable);

                File.AppendAllText(filePath, data.ToString());

                _con.Close();

                // complete the message. messages is deleted from the subscription. 
                await args.CompleteMessageAsync(args.Message);
            }

            // handle any errors when receiving messages
            static Task ErrorHandler(ProcessErrorEventArgs args)
            {
                Console.WriteLine(args.Exception.ToString());
                return Task.CompletedTask;
            }

            static async Task Main()
            {
                client = new ServiceBusClient(connectionString);
                processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

                try
                {
                    processor.ProcessMessageAsync += MessageHandler;

                    // add handler to process any errors
                    processor.ProcessErrorAsync += ErrorHandler;

                    // start processing 
                    await processor.StartProcessingAsync();

                    Console.WriteLine("Wait for a minute and then press any key to end the processing");
                    Console.ReadKey();

                    // stop processing 
                    Console.WriteLine("\nStopping the receiver...");
                    await processor.StopProcessingAsync();
                    Console.WriteLine("Stopped receiving messages");
                }
                finally
                {
                    // Calling DisposeAsync on client types is required to ensure that network
                    // resources and other unmanaged objects are properly cleaned up.
                    await processor.DisposeAsync();
                    await client.DisposeAsync();
                }
            }
        }
    }
}
