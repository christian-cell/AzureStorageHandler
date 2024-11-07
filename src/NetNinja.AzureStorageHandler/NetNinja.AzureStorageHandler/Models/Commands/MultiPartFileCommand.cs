using Microsoft.AspNetCore.Http;

namespace NetNinja.AzureStorageHandler.Models.Commands
{
    public class MultiPartFileCommand : FileCommand
    {
       public IFormFile File { get; set; }
    }
};

