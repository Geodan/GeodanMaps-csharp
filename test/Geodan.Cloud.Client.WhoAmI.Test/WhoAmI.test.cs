using NUnit.Framework;

namespace Geodan.Cloud.Client.WhoAmI.test
{
    [TestFixture]
    public class WhoAmITest
    {
        [Test]
        public static void TellMe()
        {
            const string username = "";
            const string password = "";
            const string casTicketServiceUrl = "";
            const string whoAmIUrl = "";

            var whoAmI = new Core.Services.WhoAmI(username, password, casTicketServiceUrl, whoAmIUrl);
            var response = whoAmI.TellMe().Result;
            StringAssert.Contains("Tim", response.Result.FirstName);
        }
    }
}
