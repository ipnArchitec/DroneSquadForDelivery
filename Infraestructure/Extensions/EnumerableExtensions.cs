using DroneSquad.Core.Domain.Entities;
using RoundRobin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Infraestructure.Extensions
{
    public static class EnumerableExtensions
    {
        public static RoundRobinList<int> ToRoundRobinList<T>(this IEnumerable<T> items) where T : Drone
        {
            var rows = items.Select(x => x.Id).ToList();
            return new RoundRobinList<int>(rows);
        }

    }
}
