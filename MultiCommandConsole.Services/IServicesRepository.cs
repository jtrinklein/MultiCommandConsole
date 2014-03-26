using System.Collections.Generic;
using System.ServiceProcess;

namespace MultiCommandConsole.Services
{
    public interface IServicesRepository
    {
        IEnumerable<ServiceController> List(string serviceName);
        void Save(Service options);
        void Add(Service options);
        void Delete(string serviceName);
    }
}