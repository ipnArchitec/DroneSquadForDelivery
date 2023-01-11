using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Core.Domain.Entities
{
    public class Location
    {

        public Location(string name, int weigth, int id)
        {
            Name = name;
            Weigth = weigth;
            Id = id;
        }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Weigth { get; private set; }
        public bool IsAsigned { get; private set; }
        public void SetAsAssigned()
        {
            IsAsigned = true;
        }

    }
}
