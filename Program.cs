using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using System.Transactions;

namespace ChangeFeedWireFormatResponse
{
    class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "ToDoList";
        private string containerId = "Items";

        // <Main>
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.GetStartedDemoAsync();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }
        // </Main>

        // <GetStartedDemoAsync>
        /// <summary>
        /// Entry point to call methods that operate on Azure Cosmos DB resources in this sample
        /// </summary>
        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "ChangeFeedWireFormatResponse" });
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemsToContainerAsync();
            await this.ChangeFeedIteatorWithWiredFormatResponseAsync();
            //await this.DeleteDatabaseAndCleanupAsync();
            //await this.ChangeFeedIteatorWithoutWiredFormatResponseAsync();
        }
        // </GetStartedDemoAsync>

        // <CreateDatabaseAsync>
        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }
        // </CreateDatabaseAsync>

        // <CreateContainerAsync>
        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/partitionKey" as the partition key path since we're storing family information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }
        // </CreateContainerAsync>

        // <AddItemsToContainerAsync>
        /// <summary>
        /// Add Family items to the container
        /// </summary>
        private async Task AddItemsToContainerAsync()
        {
            // Create a family object for the Andersen family
            Family andersenFamily = new Family
            {
                Id = "Andersen.1",
                PartitionKey = "Andersen",
                LastName = "Andersen",
                Parents = new Parent[]
                {
                    new Parent { FirstName = "Thomas" },
                    new Parent { FirstName = "Mary Kay" }
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FirstName = "Henriette Thaulow",
                        Gender = "female",
                        Grade = 5,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Fluffy" }
                        }
                    }
                },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = false
            };

            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Family> andersenFamilyResponse = await this.container.ReadItemAsync<Family>(andersenFamily.Id, new PartitionKey(andersenFamily.PartitionKey));
                Console.WriteLine("Item in database with id: {0} already exists\n", andersenFamilyResponse.Resource.Id);
            }
            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<Family> andersenFamilyResponse = await this.container.CreateItemAsync<Family>(andersenFamily, new PartitionKey(andersenFamily.PartitionKey));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
            }

            // Create a family object for the Wakefield family
            Family wakefieldFamily = new Family
            {
                Id = "Wakefield.7",
                PartitionKey = "Wakefield",
                LastName = "Wakefield",
                Parents = new Parent[]
                {
                    new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
                    new Parent { FamilyName = "Miller", FirstName = "Ben" }
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FamilyName = "Merriam",
                        FirstName = "Jesse",
                        Gender = "female",
                        Grade = 8,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Goofy" },
                            new Pet { GivenName = "Shadow" }
                        }
                    },
                    new Child
                    {
                        FamilyName = "Miller",
                        FirstName = "Lisa",
                        Gender = "female",
                        Grade = 1
                    }
                },
                Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                IsRegistered = true
            };

            try
            {
                // Read the item to see if it exists
                ItemResponse<Family> wakefieldFamilyResponse = await this.container.ReadItemAsync<Family>(wakefieldFamily.Id, new PartitionKey(wakefieldFamily.PartitionKey));
                Console.WriteLine("Item in database with id: {0} already exists\n", wakefieldFamilyResponse.Resource.Id);
            }
            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
                ItemResponse<Family> wakefieldFamilyResponse = await this.container.CreateItemAsync<Family>(wakefieldFamily, new PartitionKey(wakefieldFamily.PartitionKey));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", wakefieldFamilyResponse.Resource.Id, wakefieldFamilyResponse.RequestCharge);
            }
        }
        // </AddItemsToContainerAsync>

        // <DeleteDatabaseAndCleanupAsync>
        /// <summary>
        /// Delete the database and dispose of the Cosmos Client instance
        /// </summary>
        private async Task DeleteDatabaseAndCleanupAsync()
        {
            DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();
            // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", this.databaseId);

            //Dispose of CosmosClient
            this.cosmosClient.Dispose();
        }
        // </DeleteDatabaseAndCleanupAsync>

        // <ChangeFeedIteatorWithWiredFormatResponseAsync>
        /// <summary>
        /// Change feed iterator to read changes with wired format response
        /// </summary>
        private async Task ChangeFeedIteatorWithWiredFormatResponseAsync()
        {
            if (this.container == null)
            {
                this.container = this.cosmosClient.GetContainer(this.databaseId, this.containerId);
            }

            FeedIterator<FamilyWiredFormatResponse> iteratorForTheEntireContainer = this.container.GetChangeFeedIterator<FamilyWiredFormatResponse>(
                ChangeFeedStartFrom.Beginning(), ChangeFeedMode.LatestVersion, new ChangeFeedRequestOptions
                {
                    AddRequestHeaders = headers =>
                    {
                        headers.Add("x-ms-cosmos-changefeed-wire-format-version", "2021-09-15");
                    },
                });

            while (iteratorForTheEntireContainer.HasMoreResults)
            {
                FeedResponse<FamilyWiredFormatResponse> responses = await iteratorForTheEntireContainer.ReadNextAsync();

                if (responses.StatusCode == HttpStatusCode.NotModified)
                {
                    Console.WriteLine($"No new changes");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                else
                {
                    foreach (FamilyWiredFormatResponse response in responses)
                    {
                        Family family = response.Current;
                        Metadata metadata = response.Metadata;
                        Console.WriteLine($"Detected change for family {family}");
                        Console.WriteLine($"Lsn {metadata.Lsn}, Conflict Resolved timestamp {metadata.Crts}, timestamp {family.Ts}");
                    }
                }
            }
        }
        // </ChangeFeedIteatorWithWiredFormatResponseAsync>

        // <ChangeFeedIteatorWithoutWiredFormatResponseAsync>
        /// <summary>
        /// Change feed iterator to read changes without wired format response
        /// </summary>
        private async Task ChangeFeedIteatorWithoutWiredFormatResponseAsync()
        {
            FeedIterator<Family> iteratorForTheEntireContainer = this.container.GetChangeFeedIterator<Family>(
                ChangeFeedStartFrom.Beginning(), ChangeFeedMode.LatestVersion);

            while (iteratorForTheEntireContainer.HasMoreResults)
            {
                FeedResponse<Family> response = await iteratorForTheEntireContainer.ReadNextAsync();

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    Console.WriteLine($"No new changes");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                else
                {
                    foreach (Family family in response)
                    {
                        Console.WriteLine($"Detected change for family {family}");
                    }
                }
            }
        }
        // </ChangeFeedIteatorWithoutWiredFormatResponseAsync>
    }
}
