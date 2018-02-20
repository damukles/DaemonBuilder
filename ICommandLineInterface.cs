namespace DaemonBuilder
{
    ///<summary>
    ///A command line interface, that will run instead of the daemon if args are present.
    ///</summary>
    public interface ICommandLineInterface
    {
        int Execute(string[] args);
    }
}