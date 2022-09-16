using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Sample.gRPC.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sample.gRPC.v1.CountryService;

namespace CountryService.Client
{
    internal class Sample
    {
        public async Task TestMethod()
        {
            var loggerFactory = LoggerFactory.Create(logging =>
            {
                //logging..AddConsole(); 
                logging.SetMinimumLevel(LogLevel.Trace);
            });



            var channel = GrpcChannel.ForAddress("https://localhost:7112", new GrpcChannelOptions
            {
                LoggerFactory = loggerFactory
            });
            ///// <summary>
            ///// Get
            ///// </summary>
            var countryClient = new CountryServiceClient(channel);
            var countryIdRequest = new CountryIdRequest { Id = 1 };
            try
            {
                var countryCall = countryClient.GetAsync(countryIdRequest, deadline: DateTime.UtcNow.AddSeconds(30));
                var country = await countryCall.ResponseAsync; Console.WriteLine($"{country.Id}: {country.Name}");
                // Read headers and Trailers
                var countryCallHeaders = await countryCall.ResponseHeadersAsync; var countryCallTrailers = countryCall.GetTrailers();
                var myHeaderValue = countryCallHeaders.GetValue("myHeaderName"); var myTrailerValue = countryCallTrailers.GetValue("myTrailerName");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine($"Get country with Id: {countryIdRequest.Id} has timed out");
                var trailers = ex.Trailers;
                var correlationId = trailers.GetValue("correlationId");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"An error occured while getting the country with Id:{ countryIdRequest.Id}    ");
                var trailers = ex.Trailers;
                var correlationId = trailers.GetValue("correlationId");
            }

            //var countryCall = countryClient.GetAsync(new CountryIdRequest { Id = 1 });
            //var country = await countryCall.ResponseAsync;
            //Console.WriteLine($"{country.Id}: {country.Name}");
            /// <summary>
            /// /GetAll
            /// </summary>
            using var serverStreamingCall = countryClient.GetAll(new Empty());
            await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{response.Name}: {response.Description}");
            }

            using var clientStreamingCall = countryClient.Delete();
            var countriesToDelete = new List<CountryIdRequest>
{
new CountryIdRequest { Id = 1
},
new CountryIdRequest { Id = 2
}
};

            /// <summary>
            /// Delete
            /// </summary>
            // Write
            foreach (var countryToDelete in countriesToDelete)
            {
                await clientStreamingCall.RequestStream.WriteAsync(countryToDelete);
                Console.WriteLine($"Country with Id {countryToDelete.Id} set for deletion");
            }

            // Tells server that request streaming is done
            await clientStreamingCall.RequestStream.CompleteAsync();
            // Finish the call by getting the response
            var emptyResponse = await clientStreamingCall.ResponseAsync;

            /// <summary>
            /// Create
            /// </summary>
            using var bidirectionnalStreamingCall = countryClient.Create();
            var countriesToCreate = new List<CountryCreationRequest>
{
new CountryCreationRequest {
    Name = "France",
    Description = "Western european country",
    CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind (DateTime.UtcNow, DateTimeKind.Utc))
},
new CountryCreationRequest { Name = "Poland",
Description = "Eastern european country",
CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind (DateTime.UtcNow, DateTimeKind.Utc))
}
};

            // Write
            foreach (var countryToCreate in countriesToCreate)
            {
                await bidirectionnalStreamingCall.RequestStream.WriteAsync(countryToCreate);
                Console.WriteLine($"Country {countryToCreate.Name} set for creation");
            }




            /// 
            // Tells server that request streaming is done
            await bidirectionnalStreamingCall.RequestStream.CompleteAsync();
            // Read
            await foreach (var createdCountry in bidirectionnalStreamingCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{createdCountry.Name} has been created with Id:{ createdCountry.Id}    ");
            }



            // Read headers and Trailers
            //var clientStreamingCallHeaders = await clientStreamingCall.ResponseHeadersAsync;
            //var clientStreamingCallTrailers = clientStreamingCall.GetTrailers();
            //var myHeaderValue = clientStreamingCallHeaders.GetValue("myHeaderName"); 
            //var myTrailerValue = clientStreamingCallTrailers.GetValue("myTrailerName");
            // var emptyResponse = await clientStreamingCall; // Works as well but cannot read headers and Trailers


        }


    }
}
