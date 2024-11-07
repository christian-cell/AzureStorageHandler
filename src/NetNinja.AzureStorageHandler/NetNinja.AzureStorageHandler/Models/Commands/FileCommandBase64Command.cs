using System.Text.RegularExpressions;

namespace NetNinja.AzureStorageHandler.Models.Commands
{
    public class FileCommandBase64Command : FileCommand
    {
        private string _file;
        public string File
        {
            get { return _file; }
            set
            {
                _file = RemoveBase64Prefix(value);
            }
        }

        private string RemoveBase64Prefix(string base64WithPrefix)
        {
            var regex = new Regex(@"^data:[^;]+;base64,");
            return regex.Replace(base64WithPrefix, string.Empty);
        }
    }
};

