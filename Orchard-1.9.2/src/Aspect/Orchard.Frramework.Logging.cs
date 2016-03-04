using AspectDotNet;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.ShellBuilders;
using Orchard.Logging;

namespace Aspect
{
    public class Class1 : AspectDotNet.Aspect
    {
        public static ILogger Logger { get; set; }
        private static ShellContext _context;

        //метод внедряется
        // [AspectAction("%before %call *DefaultOrchardHost.BuildCurrent() && %withincode(*IOrchardHost.Initialize())")]
        public static void Initialize()
        {
            Logger.Information("Initializing");
        }
        //метод внедряется
        //  [AspectAction("%after %call *DefaultOrchardHost.BuildCurrent() && %withincode(*IOrchardHost.Initialize())")]
        public static void Initialize1()
        {
            Logger.Information("Initialized");
        }

        //метод внедряется
        // [AspectAction("%before %call *DefaultOrchardHost.BeginRequest() && %withincode(*IOrchardHost.BeginRequest())")]
        public static void BeginRequest()
        {
            Logger.Debug("BeginRequest");
        }

        //метод внедряется
        // [AspectAction("%before %call *DefaultOrchardHost.EndRequest() && %withincode(*IOrchardHost.EndRequest())")]
        public static void EndRequest()
        {
            Logger.Debug("EndRequest");
        }

        //метод внедряется
        // [AspectAction("%after %call *DefaultOrchardHost.ActivateShell(ShellContext) && args(..)")]
        public static void Initialize1(ShellContext context)
        {
            _context = context;
        }

        //метод не внедряется
        [AspectAction("%before %call *IOrchardShell.Activate() && %withincode(*DefaultOrchardHost.ActivateShell(ShellContext))")]
        public static void ActivateShell(ShellContext context)
        {
            Logger.Debug("Activating context for tenant {0}", _context.Settings.Name);
        }


        //метод не внедряется
        [AspectAction("%before %call *IShellContextFactory.Activate() && %withincode(*DefaultOrchardHost.CreateSetupContext())")]
        public static void CreateSetupContext()
        {
            Logger.Debug("Creating shell context for root setup");
        }

    }
}
