using Dijkstra.NET.ShortestPath;
using DroneSquad.Core.Application.Ports;
using DroneSquad.Core.Domain.Entities;
using DroneSquad.CrossCutting.Logging;
using DroneSquad.Infraestructure.Extensions;
using RoundRobin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Core.Application.UseCase.ShipmentOrders
{
    public class ShipmentOfOrders
    {

        protected ILocationRepository _packageRepository;
        protected IDroneRepository _droneRepository;
        protected ILogManager _logManager;

        public ShipmentOfOrders(ILocationRepository packageRepository,
                                 IDroneRepository droneRepository,
                                 ILogManager logManager


                               )
        {

            _packageRepository = packageRepository;
            _droneRepository = droneRepository;
            _logManager = logManager;
        }

        private List<Location> _locations = new();
        private List<Drone> _drones = new();
        public IReadOnlyCollection<Location> Locations => _locations.AsReadOnly();
        public int NumberOfTrip { get; private set; } = 0;
        public IReadOnlyCollection<Drone> Drones => _drones.AsReadOnly();

        public bool IsCompleted { get; private set; }

        public RoundRobinList<int> roundRobinList { get; private set; }
        public async Task<bool> Handler()
        {
            try
            {
                var packages = _packageRepository.GetAllAsync();
                var drones = _droneRepository.GetAllAsync();
                await Task.WhenAll(drones, packages);
                _locations = packages.Result;
                _drones = drones.Result;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Inputs:");
                sb.AppendLine("*******************************************");
                var outputDrones = Drones.Select(x => $"[{x.Name}],[{x.MaxWeigth}]").ToArray();
                var text = string.Join(',', outputDrones);
                sb.AppendLine(text);
                foreach (var location in Locations)
                {
                    sb.AppendLine($"[{location.Name}],[{location.Weigth}]");
                }

                sb.AppendLine("Outputs:");
                sb.AppendLine("*******************************************");
                _logManager.Information(sb.ToString());
                await ProccessDeliveryAsync();
                _drones.ForEach(Dron =>
                {
                    _logManager.Information($"[{Dron.Name}]");
                    foreach (var trip in Dron.TripsMade)
                    {

                        _logManager.Information(trip);
                    }

                });
                IsCompleted = true;

            }
            catch (Exception ex)
            {
                _logManager.Error(ex.Message);
            }
            return IsCompleted;
        }


        /// <summary>
        /// This method will be executed recursively until the drone is assigned the maximum weight it can carry.
        /// Therefore, you can make n trips until there are locations to be assigned that meet the weight that the drone can carry.
        /// the distribution algorithm is roundRobint
        /// </summary>
        /// <returns></returns>
        private async Task ProccessDeliveryAsync()
        {
            NumberOfTrip++;
            var packageAvailable = AssingLocationToDrone();
        
            foreach (var drone in Drones)
            {
                drone.OrderDelivery(NumberOfTrip);
            }

            if (packageAvailable)
                await ProccessDeliveryAsync();
        }


  
    


        private Tuple<int, List<int>, List<int>> GetOptimalNumbers(int[] arr, int target)
        {
            Array.Sort(arr);
            List<int> closestNumbers = new List<int>();
            List<int> indexValues = new List<int>();
            int closestSum = int.MaxValue;
            for (int i = 0; i < arr.Length; i++)
            {
                int sum = arr[i];
                List<int> subset = new List<int> { arr[i] };
                List<int> indexArr = new List<int> { i };
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (sum + arr[j] <= target)
                    {
                        subset.Add(arr[j]);
                        indexArr.Add(j);
                        sum += arr[j];
                    }
                    else if (Math.Abs(target - sum) < Math.Abs(target - closestSum))
                    {
                        closestSum = sum;
                        closestNumbers = new List<int>(subset);
                        indexValues = new List<int>(indexArr);
                        break;
                    }
                }
            }
            return new Tuple<int, List<int>, List<int>>(closestSum, closestNumbers, indexValues);
        }
        private List<Location> GetOptimalLocations(int[] optimal)
        {
            var loc = Locations.Where(x => x.IsAsigned == false).OrderBy(x => x.Weigth).ToList();
            var equivalentId = new List<int>(optimal.Length);
            for (var i = 0; i < optimal.Length; i++)
            {
                equivalentId.Add(loc[optimal[i]].Id);
            }
            var query = equivalentId.Join(Locations, p => p, r => r.Id, (p, r) => r);
            return query.ToList();
        }
        private void CheckListLocation(List<Location> locations)
        {
            foreach (var location in locations)
            {
                location.SetAsAssigned();
            }
        }

    
        private bool AssingLocationToDrone()
        {
            if (!Locations.Any())
                throw new Exception("There are no locations to assign.\r\n");

            foreach (var dron in Drones)
            {

                var packages = Locations.Where(x => x.IsAsigned == false).Select(x => x.Weigth).ToArray();
                var result = GetOptimalNumbers(packages, dron.MaxWeigth);
                if (result.Item2 is not null && result.Item2.Count > 0 && result.Item1 <= dron.MaxWeigth)
                {

                    var locations = this.GetOptimalLocations(result.Item3.ToArray());
                    dron.AddBulkPackage(locations);
                    this.CheckListLocation(locations);
                }
                else
                    dron.SetNotAvailable();
 
            }
            var hasRows = Locations.Any(x => x.IsAsigned == false);
            var isFull = Drones.Where(x => x.IsNotAvailable).Count() == Drones.Count();


            if (!isFull && hasRows)
            {

                AssingLocationToDrone();
            }

            var end = Drones.Count(x => x.AssignedLocations.Count == 0) == Drones.Count;
            if (end)
                return false;

            return hasRows;


        }
       


    }
}
