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
                        service.ConstructUsing(() => new Runner("http://localhost:9002"));
                        service.WhenStarted(a => a.Start());
                        service.WhenStopped(a => a.Stop());
                    });

                host.SetDescription("A node runner #2");
                host.SetDisplayName("Runner app #2");
                host.SetServiceName("RunnerApp2");

                host.RunAsNetworkService();
            });

            return (int)exitCode;
        }
    }
}
