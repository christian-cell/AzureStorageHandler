![alt text](ReadmeImages/image.png)

# AzureStorageHandler

# Usage
¿ How to inject ?

```
    //program.cs

   var storageConfiguration = new StorageConfiguration
   {
       BlobName = "myblob",
       ContainerName = builder.Configuration["AzureStorage:StgContainerName"] ?? string.Empty,
       ConnectionString = builder.Configuration["AzureStorage:StgConnectionString"] ?? string.Empty,
       FilePath = builder.Configuration["AzureStorage:Uri"] ?? string.Empty,
       ExpiresOnInHours = Int32.Parse(builder.Configuration["AzureStorage:ExpiresOnInHours"]!)
   };

   builder.Services.AddSingleton(storageConfiguration);
   
   builder.Services.AddSingleton(x => new BlobServiceClient(
           builder.Configuration["AzureStorage:StgConnectionString"] ?? string.Empty
       )
   );
   
   builder.Services.AddTransient<IStorageHandler, StorageHandler>();
```

Terms:

- ConnectionString : connection string of your storage account

- BlobName : you don't need to set here, you will when you use then methods to upload

- ContainerName : you can set here one by default or set in uploads methods dynamically.

- FilePath : the path to your container

- ExpiresOnInHours : the time a sasUri takes to expire.


# Blob Methods

1. **UploadIFormFileToBlobAsync**:
    Upload a file as multipart it will return a blobReference with blob info with a sasUri included

```
   var blobReference = await _storageHandler.UploadIFormFileToBlobAsync(multipartFile);

   return blobReference;     
```

2. **Uploadbase64BlobAsync**
   Upload a file as base64 it will return a blobReference with blob info with a sasUri included

```
    var blobReference = await _storageHandler.Uploadbase64BlobAsync(base64Command);

    return blobReference;
```

3. **GenerateSasUriFromBlobName**
   generate a sasUri

```
    _storageHandler.GenerateSasUriFromBlobName(blobName);
```

4. **GetContainersInBlobs**
   get the blobs in a given container

```
    _storageHandler.ListAllBlobs(containerName);
```

# Container Methods

4. **CreateContainer**
   create a new container 

```
    CreateContainer
```

5. **DeleteContainer**
   🚨🚨delete container🚨🚨

```
   _storageHandler.DeleteContainer(containerName);
```


