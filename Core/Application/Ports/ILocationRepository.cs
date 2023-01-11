using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneSquad.Core.Domain.Entities;

namespace DroneSquad.Core.Application.Ports
{
    public interface ILocationRepository
    {
        public Task<List<Location>> GetAllAsync();
    }
}
