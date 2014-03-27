using System.Collections.Generic;

namespace MultiCommandConsole.Services
{
    public interface IServicesRepository
    {
        IEnumerable<Service> All();
        void Save(Service options);
        void Add(Service options);
        void Delete(string serviceName);
        Service GetCurrent();
    }
}