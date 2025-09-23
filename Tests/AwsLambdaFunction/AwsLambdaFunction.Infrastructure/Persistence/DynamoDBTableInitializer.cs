using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBTableInitializer", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence
{
    public static class DynamoDBTableInitializer
    {
        public static async Task Initialize(IAmazonDynamoDB client)
        {
            var requests = new List<CreateTableRequest>
            {
                new CreateTableRequest
                {
                    TableName = "dyn-affiliates",
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "S"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    }
                },

                new CreateTableRequest
                {
                    TableName = "dyn-clients",
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "S"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    }
                }
            };

            await Task.WhenAll(requests.Select(
                async request =>
                {
                    // Don't create the table if it already exists
                    try
                    {
                        var response = await client.DescribeTableAsync(request.TableName);
                        if (response.Table != null)
                        {
                            return;
                        }
                    }
                    catch (ResourceNotFoundException)
                    {
                        // NOP
                    }

                    await client.CreateTableAsync(request);
                }));
        }
    }
}