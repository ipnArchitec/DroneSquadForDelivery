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


        public int NumberOfTrip { get; private set; } = 0;
        public List<Location> Locations { get; private set; }

        public List<Drone> Drones { get; private set; }

        public bool IsCompleted { get; private set; }

        public RoundRobinList<int> roundRobinList {  get; private set; }
        public async Task<bool> Handler()
        {
            try
            {
                var packages = _packageRepository.GetAllAsync();
                var drones = _droneRepository.GetAllAsync();
                await Task.WhenAll(drones, packages);
                Locations = packages.Result;
                Drones = drones.Result;
                var outputDrones= Drones.Select(x => $"Name:{x.Name}, MaxWeight:{x.MaxWeigth}").ToArray();
                _logManager.Information("Inputs:");
                _logManager.Information("*******************************************:");
                var text = string.Join(',', outputDrones);
                _logManager.Information(text);
                foreach (var location in Locations)
                {
                    _logManager.Information($"Name:{location.Name}, Weigth:{location.Weigth}");
                }
                _logManager.Information("Outputs:");
                _logManager.Information("*******************************************:");

                await ProccessDeliveryAsync();
                IsCompleted = true;

            }
            catch(Exception ex)
            {
                _logManager.Error(ex.Message);
            }
            return IsCompleted;
        }

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

            var end = Drones.Count(x => x.AssignedLocations.Count() == 0) == Drones.Count();
            if (end)
                return false;

            return hasRows;


        }


    }
}
