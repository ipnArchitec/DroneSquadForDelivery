using DroneSquad.Core.Application.Ports;
using DroneSquad.Core.Domain.Entities;
using DroneSquad.Infraestructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Infraestructure.Adapters
{
    public class LocationRepository : ILocationRepository
    {
        protected IFileManager<string[]> _fileManager;

        public LocationRepository(IFileManager<string[]> fileManager)
        {
            _fileManager = fileManager;
        }
        public async Task<List<Location>> GetAllAsync()
        {
            var content = await _fileManager.GetFileAsync();
            if (content == null)
                throw new NullReferenceException();
            if (content.Length < 2)
                throw new Exception("The configuration file has errors.");

            List<string> locations = new List<string>(content.Length - 1);
            for(var i =1; i < content.Length; i ++)
            {
                locations.Add(content[i].Replace("[", "").Replace("]", ""));
            }
            Func<string, int, int, Location> entity = (a, b, c) => new Location(a, b, c);
            return locations.ToArray().GetList(entity);

        }
    }
}
