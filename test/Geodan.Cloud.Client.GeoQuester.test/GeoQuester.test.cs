using NUnit.Framework;

namespace Geodan.Cloud.Client.GeoQuester.test
{
    [TestFixture]
    public class GeoQuesterTest
    {
        private const string Organisation = "";

        [Test]
        public static void IsOverlapTest()
        {
            //arrange
            const string serviceurl = "";
            const string username = "";
            const string password = "";
            const string casTicketServiceUrl = "";
            
            var geoQuester = new GeoQuester(username, password,casTicketServiceUrl,serviceurl);
            const string location2 = "{\"type\": \"Point\",\"coordinates\": [150243.3280, 451319.6505]}";
            const string location1 = "{\"type\": \"Polygon\",\"coordinates\": [[ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0],[100.0, 1.0], [100.0, 0.0] ]]}";
            const string layer = "agrarisch";
            const int buffer = 100000000;

            //act
            var response1 = geoQuester.IsOverlap(layer, Organisation, location1, buffer).Result;
            var response2 = geoQuester.IsOverlap(layer, Organisation, location2).Result;

            //assert
            Assert.IsTrue(response1.Result.IsOverlap);
            Assert.IsFalse(response2.Result.IsOverlap);

        }

        [Test]
        public static void GetAllLayersTest()
        {
            //arrange
            const string serviceurl = "";
            const string username = "";
            const string password = "";
            const string casTicketServiceUrl = "";
            const string documentUrl = "";
            
            var geoQuester = new GeoQuester(username, password,casTicketServiceUrl,serviceurl);
            //act
            var response = geoQuester.GetAllLayers(documentUrl, Organisation).Result;

            //assert
            StringAssert.Contains("agrarisch",response.Result[0].Name);
           
            }
    }
}
