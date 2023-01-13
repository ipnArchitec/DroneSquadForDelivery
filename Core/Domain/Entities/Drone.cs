using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using DroneSquad.Core.Domain.Algorithm;
using DroneSquad.Core.Domain.Events;
using DroneSquad.Infraestructure;
using DroneSquad.Infraestructure.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Core.Domain.Entities
{
    public class Drone
    {
        private const uint _startingPoint = 1;
        public Drone(string name, int maxWeigth, int id)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (maxWeigth <= 0)
                throw new ArgumentOutOfRangeException("the weigth is incorret.");
            Name = name;
            MaxWeigth = maxWeigth;
            Id = id;

        }
        private List<Location> _assignedLocations = new();
        public IReadOnlyCollection<Location> AssignedLocations
            => _assignedLocations.AsReadOnly();

        public int Id { get; private set; }
        public string Name { get; private set; }
        public int MaxWeigth { get; private set; }

        public int AssignedWeigth { get; private set; }
        public int AvaibleWeigth { get; private set; }
        public bool IsFullWeigth { get; private set; }

        private List<string> _tripsMade = new();
        public IReadOnlyCollection<string> TripsMade  => _tripsMade.AsReadOnly();
        public bool AddPackage(Location location)
        {


            if (!(AssignedWeigth + location.Weigth > MaxWeigth))
            {
                _assignedLocations.Add(location);
                AssignedWeigth = AssignedWeigth + location.Weigth;
                AvaibleWeigth = MaxWeigth - AssignedWeigth;
                return true;
            }

            return false;
        }
        public bool DeletePackage(Location location)
        {
            _assignedLocations.Remove(location);
            return true;
        }
        public void SetFullWeigth()
        {
            IsFullWeigth = true;
        }
        public bool OrderDelivery(int numberOfTrip)
        {
            if (!AssignedLocations.Any())
                return false;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Trip #{numberOfTrip}");
            var outputLocations= AssignedLocations.Select(x => $"{x.Name}").ToArray();
            var text = string.Join(',', outputLocations);
            sb.AppendLine(text);
            _tripsMade.Add(sb.ToString());
            //RegisterDomainEvents.Raise(new PackageDelivered() { Message = sb.ToString() });
            IsFullWeigth = false;
            AvaibleWeigth = MaxWeigth;
            AssignedWeigth = 0;
            _assignedLocations = new();
            return true;
        }
  
        public bool  GetShortestPath()
        {

            if (!AssignedLocations.Any())
                return false;
            var locations = AssignedLocations.Select(x => x.Id).ToList();
            //  We add the location 0 that represents our starting point from where the drones will leave.
                        locations.Insert(0, 0);
            // We simulate a map with distances for each location.
            var map = RouteMap.GetMap(locations.ToArray());
            TSP algoritm = new(map);
            // we obtain the most optimal route to deliver the assigned packets.
            var shortRoute =  algoritm.Solve();
            var equivalentId = new List<int>(shortRoute.Length - 1);
            for (var i = 1; i < shortRoute.Length; i++)
            {
                equivalentId.Add(locations[shortRoute[i]]);
            }
    
            var query = equivalentId.Join(AssignedLocations, p => p, r => r.Id, (p, r) => r);
            _assignedLocations = query.ToList();
            return true;    
        }


    }




   
}
