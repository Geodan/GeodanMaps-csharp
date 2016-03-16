using System.Collections.Generic;
using System.Net;
using Geodan.Cloud.Client.Routing.RequestParams;
using NUnit.Framework;

namespace Geodan.Cloud.Client.Test
{
    [TestFixture]
    public class RoutingTest
    {
        private const string Username = "";
        private const string Password = "";
        private const string ServiceUrl = "";
        private const string TicketServiceUrl = "";

        [Test]
        public void TestRoute()
        {
            var routingApi = new Routing.Api(Username, Password, TicketServiceUrl, ServiceUrl);

            //Route with all default parameters
            var route = routingApi.GetRoute(4.91311, 52.34232, 5.29969 , 51.69176).Result;

            Assert.AreEqual(route.HttpStatusCode, HttpStatusCode.OK);

            double distance;
            if (route.Result.FeatureCollection.Features[0].TryGetDistance(out distance))
                Assert.AreEqual(distance, 84.11127);
            else            
                Assert.Fail("Unable to get distance from route response");            

            double duration;
            if (route.Result.FeatureCollection.Features[0].TryGetDuration(out duration))            
                Assert.AreEqual(duration, 56.29011);            
            else            
                Assert.Fail("Unable to get duration from route response");            
        }

        [Test]
        public void TestRouteBatch()
        {
            var routingApi = new Routing.Api(Username, Password, TicketServiceUrl, ServiceUrl);
            var batch = new List<BatchLocation>
            {
                new BatchLocation {FromCoordX = 4.91311, FromCoordY = 52.34232, ToCoordX = 5.29969, ToCoordY = 51.69176},
                new BatchLocation {FromCoordX = 4.81213, FromCoordY = 52.23156, ToCoordX = 5.75421, ToCoordY = 51.48545},
                new BatchLocation {FromCoordX = 4.74588, FromCoordY = 52.12345, ToCoordX = 5.78945, ToCoordY = 51.96541}
            };

            var route = routingApi.GetBatchRoute(batch).Result;

            Assert.AreEqual(route.HttpStatusCode, HttpStatusCode.OK);
            Assert.AreEqual(route.Result.FeatureCollection.Features.Count, 3);
        }

        [Test]
        public void TestTsp()
        {
            var routingApi = new Routing.Api(Username, Password, TicketServiceUrl, ServiceUrl);
            var batch = new List<TspLocation>
            {
                new TspLocation {CoordX = 4.906, CoordY = 52.386},
                new TspLocation {CoordX = 4.905, CoordY = 52.378},
                new TspLocation {CoordX = 4.900, CoordY = 52.385},
                new TspLocation {CoordX = 4.908, CoordY = 52.382},
            };

            var response = routingApi.GetTsp(batch, tspMode: TspMode.openend).Result;
            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
        }
    }
}