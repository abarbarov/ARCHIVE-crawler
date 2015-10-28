using Topshelf;

namespace rift.head
{
    internal class Program
    {
        private static int Main()
        {
            var exitCode = HostFactory.Run(host =>
            {
                host.Service<HeadApp>(
                    service =>
                    {
                        service.ConstructUsing(() => new HeadApp());
                        service.WhenStarted(a => a.Start());
                        service.WhenStopped(a => a.Stop());
                    });

                host.SetDescription("An application to manage distributed runners network.");
                host.SetDisplayName("Head app");
                host.SetServiceName("HeadApp");

                host.RunAsNetworkService();
            });

            return (int)exitCode;
        }
    }
}
