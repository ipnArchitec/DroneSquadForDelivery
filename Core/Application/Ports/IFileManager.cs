using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Core.Application.Ports
{
    public interface IFileManager<T>
    {
        Task<T> GetFileAsync();
    }
}
