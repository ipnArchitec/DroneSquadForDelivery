using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Core.Domain.Events
{
    public class PackageDelivered : IDomainEvent
    {
        public string Message { get; set; }
    }
}
