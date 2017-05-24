using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;

namespace RawLauncherWPF
{
    public class Bootstrapper : ModernApplicationFramework.Basics.Bootstrapper
    {
        private CompositionContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        internal IList<Assembly> PriorityAssemblies => _priorityAssemblies;

        protected override void BindServices(CompositionBatch batch)
        {
            base.BindServices(batch);
            batch.AddExportedValue(this);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
        }

        protected override void OnExit(object sender, EventArgs e)
        {
        }
    }
}