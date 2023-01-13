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
    public class DroneRepository : IDroneRepository
    {
        protected IFileManager<string[]> _fileManager;

        public DroneRepository(IFileManager<string[]> fileManager)
        {
            _fileManager = fileManager;
        }
        public async Task<List<Drone>> GetAllAsync()
        {
            var content = await _fileManager.GetFileAsync();
            if (content == null)
                throw new NullReferenceException();
            if (content.Length < 2)
                throw new Exception("The configuration file has errors.");

            var rows = content[0].Split(',').ToList();
            List<string> DroneRows = new List<string>();  
            var drones = rows.Split(2);
            foreach (var drone in drones)
            {
                var records = drone.Select(x=>x.Trim()).ToArray();
                var record =string.Join(',', records);
                DroneRows.Add(record);
            }
            
            Func<string, int, int, Drone> entity = (a, b, c) => new Drone(a, b, c);
            return DroneRows.ToArray().GetList(entity);

        }
    }
}
