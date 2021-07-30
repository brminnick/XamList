using System;
using System.Net.Http;
using Autofac;
using Xamarin.Essentials.Interfaces;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList.UnitTests
{
    public class ServiceCollection
    {
        readonly static Lazy<IContainer> _containerHolder = new Lazy<IContainer>(CreateContainer);

        public static IContainer Container => _containerHolder.Value;

        static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            //Xamarin.Essentials
            builder.RegisterType<MockAppInfo>().As<IAppInfo>().SingleInstance();
            builder.RegisterType<MockBrowser>().As<IBrowser>().SingleInstance();
            builder.RegisterType<MockDeviceInfo>().As<IDeviceInfo>().SingleInstance();
            builder.RegisterType<MockFileSystem>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<MockLauncher>().As<ILauncher>().SingleInstance();
            builder.RegisterType<MockMainThread>().As<IMainThread>().SingleInstance();
            builder.RegisterType<MockPreferences>().As<IPreferences>().SingleInstance();
            builder.RegisterType<MockSecureStorage>().As<ISecureStorage>().SingleInstance();
            builder.RegisterType<MockVersionTracking>().As<IVersionTracking>().SingleInstance();

            //Databases
            builder.RegisterType<ContactDatabase>().SingleInstance();

            //Services
            builder.RegisterType<ApiService>().SingleInstance();
            builder.RegisterType<AppCenterService>().SingleInstance();
            builder.RegisterType<ConnectivityService>().SingleInstance();
            builder.RegisterType<DatabaseSyncService>().SingleInstance();
            builder.RegisterInstance(RefitExtensions.For<IXamListAPI>(BackendConstants.AzureAPIUrl)).SingleInstance();
            builder.RegisterInstance(RefitExtensions.For<IXamListFunction>(BackendConstants.AzureFunctionUrl)).SingleInstance();

#if DEBUG
            builder.RegisterType<UITestBackdoorMethodService>().SingleInstance();
#endif

            //ViewModels
            builder.RegisterType<ContactDetailViewModel>();
            builder.RegisterType<ContactsListViewModel>();

            return builder.Build();
        }
    }
}
