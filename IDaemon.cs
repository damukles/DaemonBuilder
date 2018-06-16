namespace Hosting
{
    ///<summary>
    ///A daemon style application that will run until stopped
    ///</summary>
    public interface IDaemon
    {
        void Start();
        void Stop();
    }
}
