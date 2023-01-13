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
        public static List<List<T>> Split<T>(this IList<T> source, int length)
        {
            return Enumerable.Range(0, (source.Count + length - 1) / length)
                             .Select(n => source.Skip(n * length).Take(length).ToList()).ToList();
        }
    }
}
