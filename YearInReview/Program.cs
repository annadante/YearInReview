using CsvHelper;
using GraphQL;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YearInReview
{
    class Program
    {
        static AppSettings appSettings = new AppSettings();

        async static Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            ConfigurationBinder.Bind(configuration.GetSection("AppSettings"), appSettings);

            Console.WriteLine(appSettings.Token);

            await CreateProductsDatabase();
        }

        public static async Task CreateProductsDatabase()
        {
            var graphQLClient = new GraphQLHttpClient(appSettings.GraphQLEndpoint, new NewtonsoftJsonSerializer());

            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {appSettings.Token}");

            var initialPosts = await GetPosts(10, null, graphQLClient);
            var cursor = initialPosts.posts.pageInfo.endCursor;
            var hasNextPage = initialPosts.posts.pageInfo.hasNextPage;

            //Create first 10 records
            using (var writer = new StreamWriter(appSettings.PathToBase))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(initialPosts.posts.edges);
            }

            while(hasNextPage)
            {
                //So the API will not block me 
                Random rnd = new Random();
                var delayCount = rnd.Next(0, 9);
                await Task.Delay(delayCount*10000);
                var posts = await GetPosts(10, cursor, graphQLClient);

                hasNextPage = posts.posts.pageInfo.hasNextPage;
                cursor = posts.posts.pageInfo.endCursor;

                AppendToCSV(posts.posts.edges);
            } 
        }

        static async Task<ProductHuntPost> GetPosts(int count, string cursor, GraphQLHttpClient graphQLHttpClient)
        {
            var postsRequest = new GraphQLRequest
            {

                Query = @"
query GetPosts($cursor: String, $count: Int)
{
  posts(first:$count, after:$cursor) {
     pageInfo{
                    hasNextPage,
                    endCursor
                }
    edges {
                    cursor
      node{
                    name,
                    slug,
                    website,
                    url,
                    createdAt,
                    votesCount,
                    description,
                    tagline,
                    commentsCount
                }
            }
  }
    }",
                OperationName = "GetPosts",
                Variables = new
                {
                    cursor = cursor,
                    count = count
                },
            };
            var somth = await graphQLHttpClient.SendQueryAsync<ProductHuntPost>(postsRequest);
            return somth.Data;     
        }


        static void AppendToCSV(List<Edges> records)
        {
            using var stream = File.Open(appSettings.PathToBase, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            // Don't write the header again.
            csv.Configuration.HasHeaderRecord = false;
            csv.WriteRecords(records);
            writer.Flush();
            Console.WriteLine(0);
        }

    }
}
