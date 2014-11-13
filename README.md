CasClient
=========

.NET PCL to logon to GeodanMaps


Usage
-----
```
using (var httpClient = CasClient.CreateLoggedInClient(username, password, ticketserverurl, serviceurl)
{
     var data = httpClient.GetStringAsync(resourceUrl).Result;
    // do something with the data
}
```
