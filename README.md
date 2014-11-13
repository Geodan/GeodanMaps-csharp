CasClient
=========

.NET PCL to logon to GeodanMaps


```
using (var httpClient = CasClient.CreateLoggedInClient(username, password, ticketserverurl, serviceurl)
{
     var data = httpClient.GetStringAsync(resourceUrl).Result;
    // do something with the data
}
```
