using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Infraestructure.Extensions
{
    public static class ArrayStringExtensions
    {
        public static List<T> GetList<T>(this string[] input, Func<string, int, int, T> entity) where T : class
        {
            List<T> data = new List<T>();
            int id = 10;
            foreach (var item in input)
            {
                var information = item.Split(",");
                int w = int.Parse(information[1]);
                data.Add(entity(information[0], w, id));
                id++;
            }
            return data;
        }
    }
}
