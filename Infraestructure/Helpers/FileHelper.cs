using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Infraestructure.Helpers
{
    public class FileHelper
    {
        public static string AssemblyDirectory
        {
            get
            {
           
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }
        public static async Task<Stream> ReadFileAsync(string path)
        {
            byte[] result;

            using (FileStream SourceStream = File.Open(path, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
                return new MemoryStream(result);
            }


        }
    }
}
