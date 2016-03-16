using System.Diagnostics;
using System.Net;
using NUnit.Framework;

namespace Geodan.Cloud.Client.GeoQuester.test
{
    [TestFixture]
    public class GeoquestTest
    {
        private const string Organisation = "GEOD8602XXXX";
        private const string Serviceurl = "http://acc.geodan.nl/public/beta/geoquest";
        private const string ServiceKey = "";

        [Test]
        public static void IntersectTest()
        {
            var geoQuester = new Api(Serviceurl, ServiceKey);
            const string location2 = "{\"type\": \"Point\",\"coordinates\": [150243.3280, 451319.6505]}";
            const string location1 = "{\"type\": \"Polygon\",\"coordinates\": [[ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0],[100.0, 1.0], [100.0, 0.0] ]]}";

            var response1 = geoQuester.IntersectsPost(Organisation, "bestuurlijkegrenzen", location1).Result;
            var response2 = geoQuester.IntersectsPost(Organisation, "bestuurlijkegrenzen", location2, 1).Result;

            Assert.IsFalse(response1.Result.Datasets[0].FeatureFound);
            Assert.IsTrue(response2.Result.Datasets[0].FeatureFound);
        }

        [Test]
        public static void GetAllConfigurations()
        {                         
            var geoQuester = new Api(Serviceurl, "");
            var response = geoQuester.GetConfigurations(Organisation).Result;
            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);                                            
        }
    }
}
