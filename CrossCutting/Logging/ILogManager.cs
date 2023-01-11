using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSquad.CrossCutting.Logging
{
    public interface ILogManager
    {

        void Information(string message);
        void Error(string message);
        void Warning(string message);

    }
}
