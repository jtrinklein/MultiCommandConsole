using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Logging;

namespace MultiCommandConsole.Util
{
    public static class Assemblies
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Assemblies));

        private static Assembly[] _assemblies;

        public static Assembly[] GetFromBin(
            string searchPattern = "*.dll", 
            Predicate<AssemblyName> assemblyNameFilter = null)
        {
            if (_assemblies == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();
                var loadedAssemblies = assemblies.Select(a => a.GetName().FullName).ToSet();

                var bin = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                _assemblies = Directory.GetFiles(bin, searchPattern, SearchOption.TopDirectoryOnly)
                                       .Select(AssemblyName.GetAssemblyName)
                                       .Distinct()
                                       .Where(name => !loadedAssemblies.Contains(name.FullName))
                                       .Where(name => assemblyNameFilter == null || assemblyNameFilter(name))
                                       .Select(Assembly.Load)
                                       .Union(assemblies)
                                       .ToArray();
                if (Log.IsInfoEnabled)
                {
                    Log.InfoFormat("loaded the following assemblies:\n\t{0}", string.Join("\n\t", _assemblies.OrderBy(a => a.FullName).Select(a => a.FullName)));
                }
            }
            return _assemblies;
        }

        public static IEnumerable<Type> GetTypes(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes());
        }
    }
}