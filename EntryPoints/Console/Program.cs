// See https://aka.ms/new-console-template for more information


using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using DroneSquad.Core.Application.Ports;
using DroneSquad.Core.Application.UseCase.ShipmentOrders;
using DroneSquad.Core.Domain.Entities;
using DroneSquad.Core.Domain.Events;
using DroneSquad.CrossCutting.Logging;
using DroneSquad.Infraestructure;
using DroneSquad.Infraestructure.Adapters;
using DroneSquad.Infraestructure.Extensions;
using DroneSquad.Infraestructure.Managers;
using RoundRobin;
using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;


Action<PackageDelivered> LogEvent = (a) => { Console.WriteLine(a.Message); };
Action<PackageDelivered> action = new Action<PackageDelivered>(LogEvent);
RegisterDomainEvents.Register(action);
IFileManager<string[]> fileManager = new FileManager();
IDroneRepository droneRepository = new DroneRepository(fileManager);
ILocationRepository packageRepository = new LocationRepository(fileManager);
ILogManager logManager = new LogManager();
ShipmentOfOrders shipmentOfOrders = new ShipmentOfOrders(packageRepository, droneRepository, logManager);
await shipmentOfOrders.Handler();
Console.ReadKey();