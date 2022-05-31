using Autofac;
using Autofac.Integration.Mvc;
using Sky.CDN.IServices;
using Sky.CDN.Services;
using Sky.CMS.IServices;
using Sky.CMS.Services;
using Sky.Core.IServices;
using Sky.Core.Services;
using Sky.Generic.IServices;
using Sky.Generic.Services;
using Sky.Log.IServices;
using Sky.Log.Services;
using Sky.Mors.IServices;
using Sky.Mors.Services;
using Sky.Ramesses.IServices;
using Sky.Ramesses.Services;
using Sky.Repository.Dapper;
using Sky.SuperUser.IServices;
using Sky.SuperUser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Web.Infrastructure;

namespace UI.Web.App_Start
{
    public class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //  DB Connection
            builder.Register(ctx =>
            {
                var connectionStringCDN = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringCDN"];
                if (string.IsNullOrWhiteSpace(connectionStringCDN.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringCDN can not be null or empty");

                var connectionStringCMS = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringCMS"];
                if (string.IsNullOrWhiteSpace(connectionStringCMS.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringCMS can not be null or empty");

                var connectionStringMors = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringMors"];
                if (string.IsNullOrWhiteSpace(connectionStringMors.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringMors can not be null or empty");

                var connectionStringLog = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringLog"];
                if (string.IsNullOrWhiteSpace(connectionStringLog.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringLog can not be null or empty");

                var connectionStringGLog = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringGLog"];
                if (string.IsNullOrWhiteSpace(connectionStringGLog.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringGLog can not be null or empty");

                var connectionStringSuperUser = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringSuperUser"];
                if (string.IsNullOrWhiteSpace(connectionStringSuperUser.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringSuperUser can not be null or empty");

                var connectionStringGeneric = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringGeneric"];
                if (string.IsNullOrWhiteSpace(connectionStringGeneric.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringGeneric can not be null or empty");

                var connectionStringCore = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringCore"];
                if (string.IsNullOrWhiteSpace(connectionStringCore.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringCore can not be null or empty");

                var connectionStringRamesses = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringRamesses"];
                if (string.IsNullOrWhiteSpace(connectionStringRamesses.ConnectionString))
                    throw new InvalidOperationException(
                        "ConnectionStringRamesses can not be null or empty");

                return new DapperRepository(
                    new System.Data.SqlClient.SqlConnection(connectionStringCDN.ConnectionString),
                    new System.Data.SqlClient.SqlConnection(connectionStringCMS.ConnectionString),
                    new System.Data.SqlClient.SqlConnection(connectionStringMors.ConnectionString),
                    null,
                    new System.Data.SqlClient.SqlConnection(connectionStringLog.ConnectionString),
                    new System.Data.SqlClient.SqlConnection(connectionStringGLog.ConnectionString),
                    new System.Data.SqlClient.SqlConnection(connectionStringSuperUser.ConnectionString),
                    new System.Data.SqlClient.SqlConnection(connectionStringGeneric.ConnectionString),
                    null,
                    new System.Data.SqlClient.SqlConnection(connectionStringCore.ConnectionString),
                    new System.Data.SqlClient.SqlConnection(connectionStringRamesses.ConnectionString)
                    );

            }).As<IMicroOrmRepository>().InstancePerLifetimeScope();

            builder.RegisterType<AdministratorService>().As<IAdministratorService>().InstancePerLifetimeScope();
            builder.RegisterType<Sky.SuperUser.Services.SessionService>().As<Sky.SuperUser.IServices.ISessionService>().InstancePerLifetimeScope();
            builder.RegisterType<CDNService>().As<ICDNService>().InstancePerLifetimeScope();
            builder.RegisterType<MorsService>().As<IMorsService>().InstancePerLifetimeScope();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<GenericService>().As<IGenericService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<GeneralContentService>().As<IGeneralContentService>().InstancePerLifetimeScope();
            builder.RegisterType<TypeService>().As<ITypeService>().InstancePerLifetimeScope();
            builder.RegisterType<RaffleService>().As<IRaffleService>().InstancePerLifetimeScope();
            builder.RegisterType<AppService>().As<IAppService>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerLifetimeScope();

            builder.RegisterType<MorsOperations>().InstancePerLifetimeScope();

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}