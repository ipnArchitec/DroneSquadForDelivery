using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneSquad.Core.Domain.Events;

namespace DroneSquad.Infraestructure
{
    public static class RegisterDomainEvents
    {


        //[ThreadStatic] //so that each thread has its own callbacks
        private static List<Delegate> actions;
     
   
     
       //Registers a callback for the given domain event
        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
                  if (actions == null)
                          actions = new List<Delegate>();
              
          actions.Add(callback);
                }
    
       //Clears callbacks passed to Register on the current thread
      public static void ClearCallbacks()
      {
           actions = null;
       }
     
       //Raises the given domain event
       public static void Raise<T>(T args) where T : IDomainEvent
       {
     
           
          if (actions != null)
                        foreach (var action in actions)
                           if (action is Action<T>)
                               ((Action<T>)action)(args);
            }
  
}
}
