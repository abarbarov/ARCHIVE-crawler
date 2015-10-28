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
                        service.ConstructUsing(() => new Runner("http://localhost:9001"));
                        service.WhenStarted(a => a.Start());
                        service.WhenStopped(a => a.Stop());
                    });

                host.SetDescription("A node runner #1");
                host.SetDisplayName("Runner app #1");
                host.SetServiceName("RunnerApp1");

                host.RunAsNetworkService();
            });

            return (int)exitCode;
        }
    }
}
