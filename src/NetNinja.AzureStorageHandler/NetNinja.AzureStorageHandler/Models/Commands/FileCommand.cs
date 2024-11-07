namespace NetNinja.AzureStorageHandler.Models.Commands
{
    public class FileCommand
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ContainerName { get; set; }
        
        public Dictionary<string,string> Metadata { get; set; }
    }
};

