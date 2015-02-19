CasClient
=========
.NET PCL for using GeodanMaps services in client software

Simple CAS client
-----
Using a CAS protected service with the use of CasClient
```
using (var httpClient = CasClient.CreateLoggedInClient(username, password, ticketserverurl, serviceurl)
{
     var data = httpClient.GetStringAsync(resourceUrl).Result;
    // do something with the data
}
```

Document Service API
-----
API for creating, updating, deleting and retrieving documents and their data from the Geodan Document Services. A few examples:
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
