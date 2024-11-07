namespace NetNinja.AzureStorageHandler.Configurations
{
    public class StorageConfiguration
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public string? FilePath { get; set; }
        public int ExpiresOnInHours { get; set; }
    }
};

