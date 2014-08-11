using System.Collections.Generic;

namespace MultiCommandConsole.Services
{
    /// <summary>
    /// defines a repository to interact with installed services
    /// </summary>
    public interface IServicesRepository
    {
        /// <summary>returns all installed services</summary>
        IEnumerable<Service> All();

        /// <summary>(re)installs a service</summary>
        void Save(Service options);

        /// <summary>installs a service</summary>
        void Add(Service options);

        /// <summary>deletes an installed service</summary>
        void Delete(string serviceName);

        /// <summary>gets the installed service for the current process</summary>
        Service GetCurrent();

        /// <summary>gets the installed service for the given process</summary>
        Service GetByProcessId(int processId);
    }
}