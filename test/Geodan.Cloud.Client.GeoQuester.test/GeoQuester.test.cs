using NUnit.Framework;

namespace Geodan.Cloud.Client.GeoQuester.test
{
    [TestFixture]
    public class GeoQuesterTest
    {
        private const string Organisation = "";
        private const string CasTicketServiceUrl = "";
        private const string Serviceurl = "";
        private const string Username = "";
        private const string Password = "";
        private const string DocumentServiceUrl = "";  

        [Test]
        public static void IsOverlapTest()
        {
            var geoQuester = new GeoQuester(Username, Password, CasTicketServiceUrl, Serviceurl, DocumentServiceUrl);
            const string location2 = "{\"type\": \"Point\",\"coordinates\": [150243.3280, 451319.6505]}";
            const string location1 = "{\"type\": \"Polygon\",\"coordinates\": [[ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0],[100.0, 1.0], [100.0, 0.0] ]]}";
            const string layer = "agrarisch";
            const int buffer = 100000000;

            var response1 = geoQuester.IsOverlap(layer, Organisation, location1, buffer).Result;
            var response2 = geoQuester.IsOverlap(layer, Organisation, location2).Result;

            Assert.IsTrue(response1.Result.IsOverlap);
            Assert.IsFalse(response2.Result.IsOverlap);
        }

        [Test]
        public static void GetAllLayersTest()
        { 
            const string documentService = "geoquest";
            const string documentName = "";   

            var geoQuester = new GeoQuester(Username, Password, CasTicketServiceUrl, Serviceurl, DocumentServiceUrl);            
            var response = geoQuester.GetAllLayers(Organisation, documentService, documentName).Result;
            
            StringAssert.Contains("agrarisch",response.Result[0].Name);           
        }
    }
}
