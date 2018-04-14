using Microsoft.Extensions.DependencyInjection;
using System;

namespace DaemonBuilder
{
    ///<summary>
    ///A builder, that helps you run a daemon application.
    ///</summary>
    public class DaemonBuilder
    {
        private Func<IServiceCollection> _configureServicesAction = null;
        private Type _cliType = null;
        private Type _daemonType = null;

        public DaemonBuilder ConfigureServices(Func<IServiceCollection> configureServices)
        {
            _configureServicesAction = configureServices ?? throw new ArgumentNullException(nameof(configureServices));
            return this;
        }

        public DaemonBuilder AddCli<T>() where T : ICommandLineInterface
        {
            if (_cliType != null)
                throw new InvalidOperationException("You cannot add more than one command line interface.");

            _cliType = typeof(T);
            return this;
        }

        public DaemonBuilder AddDaemon<T>() where T : IDaemon
        {
            if (_daemonType != null)
                throw new InvalidOperationException("You cannot add more than one daemon.");

            _daemonType = typeof(T);
            return this;
        }

        public DaemonExecutor Build()
        {
            if (_configureServicesAction == null)
                throw new ArgumentNullException(nameof(_configureServicesAction));

            var provider = _configureServicesAction()
                .BuildServiceProvider();

            Func<IServiceProvider, ICommandLineInterface> cliFactory = _cliType != null
                ? (serviceProvider) => (ICommandLineInterface)ActivatorUtilities.CreateInstance(serviceProvider, _cliType)
                : (Func<IServiceProvider, ICommandLineInterface>)null;

            Func<IServiceProvider, IDaemon> daemonFactory = _daemonType != null
                ? (serviceProvider) => (IDaemon)ActivatorUtilities.CreateInstance(serviceProvider, _daemonType)
                : (Func<IServiceProvider, IDaemon>)null;

            return new DaemonExecutor(provider, cliFactory, daemonFactory);
        }
    }
}
