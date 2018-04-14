using System;
using System.Runtime.Loader;
using System.Threading;

namespace DaemonBuilder
{
    public class DaemonExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, ICommandLineInterface> _cliFactory;
        private readonly Func<IServiceProvider, IDaemon> _daemonFactory;

        public DaemonExecutor(IServiceProvider _serviceProvider,
                              Func<IServiceProvider, ICommandLineInterface> cliFactory,
                              Func<IServiceProvider, IDaemon> daemonFactory)
        {
            _cliFactory = cliFactory;
            _daemonFactory = daemonFactory;
        }

        ///<summary>
        ///Runs the application with the follwing pattern:
        ///   If arguments are present, it runs the ICommandLineInterface passing along arguments and exits.
        ///   If no arguments are present, it runs the IDaemon.
        ///</summary>
        public void Run(string[] args)
        {
            if (args?.Length > 0 && _cliFactory != null)
            {
                RunCmdLineApp(args);
            }

            if (_daemonFactory != null)
            {
                RunDaemon();
            }
        }

        private void RunCmdLineApp(string[] args)
        {
            var exitCode = _cliFactory(_serviceProvider)?.Execute(args);

            Environment.Exit(exitCode ?? 0);
        }

        private void RunDaemon()
        {
            var exitEvent = new ManualResetEvent(false);

            // Allow exit with SIGTERM
            AssemblyLoadContext.Default.Unloading += (AssemblyLoadContext obj) =>
            {
                exitEvent.Set();
            };

            // Allow exit with CTRL+C
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            if (_daemonFactory != null)
            {
                var daemon = _daemonFactory(_serviceProvider);
                daemon.Start();

                // Block Thread until a exit signal is caught
                exitEvent.WaitOne();

                daemon.Stop();
            }
        }
    }
}