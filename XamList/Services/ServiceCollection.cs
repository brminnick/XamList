using System;
using Autofac;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ServiceCollection
    {
        readonly static Lazy<IContainer> _containerHolder = new Lazy<IContainer>(CreateContainer);

        public static IContainer Container => _containerHolder.Value;

        static IContainer CreateContainer()
        {
            Xamarin.Forms.Device.SetFlags(new[] { "Markup_Experimental" });

            var builder = new ContainerBuilder();

            //Xamarin.Essentials
            builder.RegisterType<AppInfoImplementation>().As<IAppInfo>().SingleInstance();
            builder.RegisterType<BrowserImplementation>().As<IBrowser>().SingleInstance();
            builder.RegisterType<DeviceInfoImplementation>().As<IDeviceInfo>().SingleInstance();
            builder.RegisterType<EmailImplementation>().As<IEmail>().SingleInstance();
            builder.RegisterType<FileSystemImplementation>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<LauncherImplementation>().As<ILauncher>().SingleInstance();
            builder.RegisterType<MainThreadImplementation>().As<IMainThread>().SingleInstance();
            builder.RegisterType<PreferencesImplementation>().As<IPreferences>().SingleInstance();
            builder.RegisterType<SecureStorageImplementation>().As<ISecureStorage>().SingleInstance();
            builder.RegisterType<VersionTrackingImplementation>().As<IVersionTracking>().SingleInstance();

            //Databases
            builder.RegisterType<ContactDatabase>().SingleInstance();

            //Services
            builder.RegisterType<ApiService>().SingleInstance();
            builder.RegisterType<AppCenterService>().SingleInstance();
            builder.RegisterType<ConnectivityService>().SingleInstance();
            builder.RegisterType<DatabaseSyncService>().SingleInstance();

#if DEBUG
            builder.RegisterType<UITestBackdoorMethodService>().SingleInstance();
#endif

            //Pages
            builder.RegisterType<ContactDetailPage>().WithParameter(new TypedParameter(typeof(bool), "isNewContact")).WithParameter(new TypedParameter(typeof(ContactModel), "selectedContact"));
            builder.RegisterType<ContactsListPage>();

            //ViewModels
            builder.RegisterType<ContactDetailViewModel>();
            builder.RegisterType<ContactsListViewModel>();

            return builder.Build();
        }
    }
}
