# Consuming the Azure Cosmos DB Change Feed with wired format response

This sample shows you how yo use the [Azure Cosmos DB SDK V3](https://github.com/Azure/azure-cosmos-dotnet-v3) to consume Azure Cosmos DB's [Change Feed](https://docs.microsoft.com/azure/cosmos-db/change-feed) and return changes with wired format response happening in a container.

## Requirements

- An active Azure Cosmos account or the [Azure Cosmos DB Emulator](https://docs.microsoft.com/azure/cosmos-db/local-emulator) - If you don't have an account, refer to the [Create a database account](https://docs.microsoft.com/azure/cosmos-db/create-sql-api-dotnet#create-an-azure-cosmos-db-account) article.
- [NET Core SDK](https://dotnet.microsoft.com/download) or Visual Studio 2017 (or higher)

## Description

This sample will create 1 database `ToDoList` and 1 container `Items`. We'll be listening for changes on `Items`.

The code creates an instance of a Change Feed Iterator with wire format header:

    private async Task ChangeFeedIteatorWithWiredFormatResponseAsync()
        FeedIterator<FamilyWiredFormatResponse> iteratorForTheEntireContainer = this.container.GetChangeFeedIterator<FamilyWiredFormatResponse>(
                ChangeFeedStartFrom.Beginning(), ChangeFeedMode.LatestVersion, new ChangeFeedRequestOptions
                {
                    AddRequestHeaders = headers =>
                    {
                        headers.Add("x-ms-cosmos-changefeed-wire-format-version", "2021-09-15");
                    },
                });

It will return response as following wire format:

    {
        current: {
            {document content},
            _rid: "",
            _self: "",
            _etag: "",
            _attachments: "",
            _ts: 1689719645
        },
        metadata: {lsn: 1, crts: 1689719645}
    }

Without wire format in ChangeFeedIteatorWithoutWiredFormatResponseAsync, returning as following:

    {
        {document content},
        _rid: "",
        _self: "",
        _etag: "",
        _attachments: "",
        _ts: 1689719645
        _lsn: 1
    }

## Running this sample

1. Clone this repository or download the zip file.
2. Retrieve the **PrimaryKey** value from the Keys blade of your Azure Cosmos account in the Azure portal. For more information on obtaining the Keys for your Azure Cosmos account refer to [View, copy, and regenerate access keys and passwords](https://docs.microsoft.com/azure/cosmos-db/secure-access-to-data#master-keys).
    * If you are working with the Azure Cosmos DB Emulator, refer to [Develop locally with the Azure Cosmos Emulator](https://docs.microsoft.com/azure/cosmos-db/local-emulator#authenticating-requests).
3. In the [App.config] file located in the project root, find **EndpointUri** and **PrimaryKey** replace the placeholder value with the value obtained for your account.
4. Run `dotnet run` or press **F5** from within Visual Studio.
5. It will generate 2 records in `Items` and prints the content in change feed iterator.

## More information

- [Azure Cosmos DB Documentation](https://docs.microsoft.com/azure/cosmos-db)
- [Azure Cosmos DB .NET SDK](https://docs.microsoft.com/azure/cosmos-db/sql-api-sdk-dotnet)
- [Azure Cosmos DB .NET SDK Reference Documentation](https://docs.microsoft.com/dotnet/api/overview/azure/cosmosdb)
