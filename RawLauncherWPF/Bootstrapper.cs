using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using Caliburn.Micro;
using ModernApplicationFramework.Native.TrinetCoreNtfs;

namespace RawLauncherWPF
{
    public class Bootstrapper : BootstrapperBase
    {
        private CompositionContainer _container;

        private List<Assembly> _priorityAssemblies;

        public Bootstrapper()
        {
            Initialize();
        }

        internal IList<Assembly> PriorityAssemblies => _priorityAssemblies;

        protected virtual void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);
            batch.AddExportedValue(this);
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        protected override void PrepareApplication()
        {
            base.PrepareApplication();
            var exassemlby = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(exassemlby);
            //This makes sure we can share any applications through the Internet
            ClearUrlZonesInDirectory(directory);
        }

        protected override void Configure()
        {
            // Add all assemblies to AssemblySource (using a temporary DirectoryCatalog).
            var directoryCatalog = new DirectoryCatalog(@".");
            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));

            _priorityAssemblies = SelectAssemblies().ToList();
            var priorityCatalog = new AggregateCatalog(_priorityAssemblies.Select(x => new AssemblyCatalog(x)));
            var priorityProvider = new CatalogExportProvider(priorityCatalog);

            var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !_priorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));
            var mainProvider = new CatalogExportProvider(mainCatalog);

            _container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = _container;
            mainProvider.SourceProvider = _container;

            var batch = new CompositionBatch();

            BindServices(batch);
            batch.AddExportedValue(mainCatalog);

            if (!Directory.Exists(ModernApplicationFramework.Configuration.ProductConfiguration.AppdataPath))
                Directory.CreateDirectory(ModernApplicationFramework.Configuration.ProductConfiguration.AppdataPath);

            using (var writer = new StreamWriter(Path.Combine(ModernApplicationFramework.Configuration.ProductConfiguration.AppdataPath, "FoundCatalog.txt"), true))
                foreach (var catalog in priorityCatalog.Catalogs)
                    writer.WriteLine(catalog);

            using (var writer = new StreamWriter(Path.Combine(ModernApplicationFramework.Configuration.ProductConfiguration.AppdataPath, "FoundCatalog.txt"), true))
                foreach (var catalog in mainCatalog.Catalogs)
                    writer.WriteLine(catalog);

            _container.Compose(batch);
        }

        protected static IEnumerable<string> GetFilesWithUrlZonesInDirectory(string directoryPath)
        {
            return from filePath in Directory.EnumerateFiles(directoryPath)
                let zone = Zone.CreateFromUrl(filePath)
                where zone.SecurityZone != SecurityZone.MyComputer
                select Path.GetFileName(filePath);
        }

        protected static void ClearUrlZonesInDirectory(string directoryPath)
        {
            foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*.dll", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(filePath);
                fileInfo.DeleteAlternateDataStream("Zone.Identifier");
            }
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            var enumerable = exports as object[] ?? exports.ToArray();
            if (enumerable.Any())
                return enumerable.First();
            throw new Exception($"Could not locate any instances of contract {contract}.");
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                Assembly.GetEntryAssembly()
            };
        }
    }
}