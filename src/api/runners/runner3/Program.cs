using Topshelf;

namespace rift.runner
{
    internal class Program
    {
        private static int Main()
        {
            var exitCode = HostFactory.Run(host =>
            {
                host.Service<Runner>(
                    service =>
                    {
                        service.ConstructUsing(() => new Runner("http://localhost:9003"));
                        service.WhenStarted(a => a.Start());
                        service.WhenStopped(a => a.Stop());
                    });

                host.SetDescription("A node runner #3");
                host.SetDisplayName("Runner app #3");
                host.SetServiceName("RunnerApp3");

                host.RunAsNetworkService();
            });

            return (int)exitCode;
        }
    }
}
