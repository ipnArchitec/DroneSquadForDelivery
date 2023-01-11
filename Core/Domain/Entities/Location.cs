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
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (weigth <= 0)
                throw new ArgumentOutOfRangeException("the weigth is incorret.");
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
