using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Core.Domain.Algorithm
{
    public class TSP
    {
        private int[,] adjacencyMatrix;
        private int numberOfCities;
        private int[] path;
        private bool[] visited;

        public TSP(int[,] adjacencyMatrix)
        {
            this.adjacencyMatrix = adjacencyMatrix;
            numberOfCities = adjacencyMatrix.GetLength(0);
            path = new int[numberOfCities + 1];
            visited = new bool[numberOfCities];
        }
        public int[] Solve()
        {
            int currentCity = 0;
            int nearestCity;
            int shortestDistance;
            int pathIndex = 1;
            int totalDistance = 0;
            visited[currentCity] = true;
            path[pathIndex] = currentCity;
            pathIndex++;
            while (pathIndex <= numberOfCities)
            {
                nearestCity = -1;
                shortestDistance = int.MaxValue;
                for (int i = 0; i < numberOfCities; i++)
                {
                    if (!visited[i] && adjacencyMatrix[currentCity, i] < shortestDistance)
                    {
                        nearestCity = i;
                        shortestDistance = adjacencyMatrix[currentCity, i];
                    }
                }
                if (nearestCity == -1)
                {
                    Console.WriteLine("An error occured");
                    return null;
                }
                visited[nearestCity] = true;
                path[pathIndex] = nearestCity;
                pathIndex++;
                totalDistance += shortestDistance;
                currentCity = nearestCity;
            }
    
            var nr = path.ToList();
            nr.RemoveAt(0);
            return nr.ToArray();
        }
    }
}
