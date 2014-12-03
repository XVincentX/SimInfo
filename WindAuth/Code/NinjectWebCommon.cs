
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Syntax;
using Ninject.Web.Common;
using System;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using WindAuth.Code;
using System.Linq;
using WindAuth.Data;
using Microsoft.ServiceBus.Notifications;
using Microsoft.WindowsAzure.MobileServices;

[assembly: PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
public static class NinjectWebCommon
{
    private static readonly Bootstrapper bootstrapper = new Bootstrapper();

    /// Starts the application    
    public static void Start()
    {
        DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
        DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));

        bootstrapper.Initialize(CreateKernel);
    }
    /// Stops the application.
    public static void Stop()
    {
        bootstrapper.ShutDown();
    }

    /// Creates the kernel that will manage your application.
    private static IKernel CreateKernel()
    {
        var kernel = new StandardKernel();

        kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
        kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

        RegisterServices(kernel);

        // Install our Ninject-based IDependencyResolver into the Web API config
        GlobalConfiguration.Configuration.DependencyResolver =
                new NinjectDependencyResolver(kernel);

        return kernel;
    }

    /// Load your modules or register your services here!
    private static void RegisterServices(IKernel kernel)
    {
        kernel.Bind<ICreditInfoRetr>().To<WindRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<ICreditInfoRetr>().To<VodafoneRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<ICreditInfoRetr>().To<TimRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<ICreditInfoRetr>().To<CoopRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<ICreditInfoRetr>().To<H3GRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<ICreditInfoRetr>().To<NovercaRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<ICreditInfoRetr>().To<TiscaliRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<ICreditInfoRetr>().To<FastwebRetr>().WithConstructorArgument("UseProxy", false);
        kernel.Bind<DataContext>().ToSelf().InRequestScope();
        kernel.Bind<NotificationHubClient>().ToMethod(context => NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://siminfohub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=s0lmhoMLMh2Uc3Axii3sU/e/UjVfzAXGNBZ3M6LxsQk=", "siminfohub"));
        kernel.Bind<MobileServiceClient>().ToSelf().InRequestScope().WithConstructorArgument("applicationUrl", "https://siminfo.azure-mobile.net/").WithConstructorArgument("applicationKey", "ymzJInYzOBINEMfCskSfESqhClBeMZ75");
    }
}

public class NinjectDependencyScope : IDependencyScope
{
    IResolutionRoot resolver;
    public NinjectDependencyScope(IResolutionRoot resolver)
    {
        this.resolver = resolver;
    }
    public object GetService(Type serviceType)
    {
        if (resolver == null)
            throw new ObjectDisposedException("this", "This scope has been disposed");
        return resolver.TryGet(serviceType);
    }
    public System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType)
    {
        if (resolver == null)
            throw new ObjectDisposedException("this", "This scope has been disposed");
        return resolver.GetAll(serviceType);
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        IDisposable disposable = resolver as IDisposable;
        if (disposable != null)
            disposable.Dispose();
        resolver = null;
    }

}
public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
{
    IKernel kernel;
    public NinjectDependencyResolver(IKernel kernel)
        : base(kernel)
    {
        this.kernel = kernel;
    }
    public IDependencyScope BeginScope()
    {
        return new NinjectDependencyScope(kernel.BeginBlock());
    }
}