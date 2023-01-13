using Dijkstra.NET.ShortestPath;
using DroneSquad.Core.Application.Ports;
using DroneSquad.Core.Domain.Entities;
using DroneSquad.CrossCutting.Logging;
using DroneSquad.Infraestructure.Extensions;
using RoundRobin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public RoundRobinList<int> roundRobinList {  get; private set; }
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
                var outputDrones= Drones.Select(x => $"{x.Name},{x.MaxWeigth}").ToArray();
                var text = string.Join(',', outputDrones);
                sb.AppendLine(text);
                foreach (var location in Locations)
                {
                    sb.AppendLine($"{location.Name},{location.Weigth}");
                }
              
                sb.AppendLine("Outputs:");
                sb.AppendLine("*******************************************");
                _logManager.Information(sb.ToString());
                await ProccessDeliveryAsync();
                _drones.ForEach(Dron =>
                {
                    _logManager.Information($"{Dron.Name}");
                    foreach (var trip in Dron.TripsMade)
                    {
                        _logManager.Information(trip);
                    }

                });
                IsCompleted = true;

            }
            catch(Exception ex)
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
            roundRobinList = Drones.ToRoundRobinList();
            var packageAvailable = AssingLocationToDrone();
            foreach (var drone in Drones)
            {
                drone.GetShortestPath();
            }
            foreach (var drone in Drones)
            {
                drone.OrderDelivery(NumberOfTrip);
            }

            if (packageAvailable)
                await ProccessDeliveryAsync();
        }
        private bool AssingLocationToDrone()
        {
            if (!Locations.Any())
                throw new Exception("There are no locations to assign.\r\n");

            List<int> _droneNotAvailable = new List<int>();
            var packages = Locations.Where(x => x.IsAsigned == false).OrderBy(x => x.Id).ToList();

            foreach (var p in packages)
            {
                var dr = roundRobinList.Next();
                var drone = Drones.First(x=>x.Id == dr);
       
                if (drone.IsFullWeigth == false && !packages.Any(x => drone.AssignedWeigth + x.Weigth <= drone.MaxWeigth && x.IsAsigned == false))
                {
                     drone.SetFullWeigth();
                    _droneNotAvailable.Add(drone.Id);
                }


                if (drone.AddPackage(p))
                {
                    p.SetAsAssigned();

                }

            }

          
            var hasRows = Locations.Any(x => x.IsAsigned == false);
            var isFull = Drones.Where(x => x.IsFullWeigth).Count() == Drones.Count();
        

            if (!isFull &&  hasRows)
            {
                var dronAvailable = Drones.Where(x => !_droneNotAvailable.Contains(x.Id)).ToList();
                roundRobinList = dronAvailable.ToRoundRobinList();
                AssingLocationToDrone();
            }

            var end = Drones.Count(x => x.AssignedLocations.Count == 0) == Drones.Count;
            if (end)
                return false;

            return hasRows;


        }

    
    }
}
