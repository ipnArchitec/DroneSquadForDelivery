using DroneSquad.CrossCutting.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.Infraestructure.Managers
{
    public class LogManager : ILogManager
    {
        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Information(string message)
        {
           Console.WriteLine(message);
        }

        public void Warning(string message)
        {
            throw new NotImplementedException();
        }
    }
}
