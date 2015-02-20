CasClient
=========
.NET PCL for using GeodanMaps services in client-side software

Simple CAS client
-----
Using a CAS protected service with the use of CasClient

Usage
```
using (var httpClient = CasClient.CreateLoggedInClient(username, password, ticketserverurl, serviceurl)
{
     var data = httpClient.GetStringAsync(resourceUrl).Result;
    // do something with the data
}
```

WhoAmI Service API
-----
API for retrieving user/organisation information.

Usage
```
private _whoAmI = new Geodan.Cloud.Client.Core.WhoAmI(username, password, ticketserverurl, serviceurl);

private async void GetUserInfo()
{
	var response = await _whoAmI.TellMe();
	if(response.Success)
	{
		 var user = response.Result;
         Console.WriteLine("ID: {0}, FirstName: {1}, LastName: {2}, Organisation: {3}, OrganisationCode: {4}, OrganisationID: {5}", user.Id, user.FirstName, user.LastName, user.Organisation, user.OrganisationCode, user.OrganisationId);
	}	
}
```

Document Service API
-----
API for creating, updating, deleting and retrieving documents and their data from the Geodan Document Service.

Usage examples
```
private _documentService = new Geodan.Cloud.Client.DocumentService(username, password, ticketserverurl, serviceurl);

private async void GetAllDocuments()
{
     var response = await _documentService.GetAllDocuments();
     //Check for response.Success and do something with the documents
}

private async void UploadImageDocument()
{
     var document = new DataDocument
     {
          Title = "test image",
          Name = "MyUniqueName",
          Description = "testing image upload",
          Account = "MyAccountName",
          IsPublic = false,
          Service = "MyService",
          Type = "image/jpeg"
     };
     
     Stream stream = File.OpenRead(@"c:\file.jpg");
     var response = await _documentService.CreateNewDocument(document, new MultipartFile 
          { 
               Filename = "file.jpg",
               Data = stream 
          });
     
     if (response.Success)
          //Check if the upload was successful
}

private async void GetDocumentData()
{
     var response = await _documentService.GetDocumentData(Account, Service, DocumentName);
     if (response.Success)
          //Do something with the data from response.Result depending on the content-type
}

```
