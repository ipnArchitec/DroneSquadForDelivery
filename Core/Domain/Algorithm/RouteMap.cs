using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Core.Domain.Algorithm
{
    public class RouteMap
    {
        public static int[,] GetMap(int[] locations)
        {
     
            Random rand = new Random();
            Dictionary<string, int> map = new Dictionary<string, int>();
            Dictionary<string, int> map1 = new Dictionary<string, int>();

            
            List<List<int>> distancesA = new List<List<int>>();
            int[,] m1 = new int[locations.Length,locations.Length];
            for (var  x =0;  x < locations.Length; x++)
            {
                List<int> distances = new List<int>();
                for (var y = 0; y < locations.Length; y++)
                {
                    string key = $"{locations[x]}-{locations[y]}";
                    var r = key.Split("-");
                    var found = $"{r[1]}-{r[0]}";
                    if (locations[x] == locations[y])
                    {

                        map.Add(found, 0);
                        map1.Add(key, 0);
                        distances.Add(0);
                        m1[x, y] = 0;
                    }
                    else
                    {
                        if (!map.ContainsKey(found))
                        {
                            int number = rand.Next(1, 50);
                            map.Add(key, number);
                            map1.Add(key, number);
                            distances.Add(number);
                            m1[x, y] = number;

                        }
                        else
                        {
                            map1.Add(key, map[found]);
                            distances.Add(map[found]);
                            m1[x, y] = map[found];
                        }

                    }
                }
                distancesA.Add(distances);


            } 
            return m1;
        }
    }
}
