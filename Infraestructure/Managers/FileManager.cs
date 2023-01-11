using DroneSquad.Core.Application.Ports;
using DroneSquad.Infraestructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Infraestructure.Managers
{
    public class FileManager : IFileManager<string[]>
    {


        public async Task<string[]> GetFileAsync()
        {
            var fileDirPath = FileHelper.AssemblyDirectory;
            string txtFilePath = string.Format("{0}\\Infraestructure\\Sources\\Configuration.txt", fileDirPath);
            string text = await File.ReadAllTextAsync(txtFilePath);
            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
