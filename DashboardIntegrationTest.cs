// -----------------------------------------------------------------------
// <copyright file="DashboardIntegrationTest.cs" company="Royal London Group">
// Copyright (c) Royal London Group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Accelerator.Dashboard.Services.IntegrationTests
{
    using System.Collections.Generic;

    using Accelerator.Common.ServiceClients.Clients;
    using Accelerator.Core.BusinessLogic.Model;
    using Accelerator.Core.Logging;
    using Accelerator.Core.Security;
    using Accelerator.Core.ServiceClients.Context;
    using Accelerator.Core.ServiceClients.Logging;
    using Accelerator.Core.Services.Contracts.Context;
    using Accelerator.Dashboard.DataAccess.EntityFramework;
    using Accelerator.Dashboard.DataAccess.EntityFramework.Repository;
    using Accelerator.Dashboard.ServiceClients.Clients;
    using Moq;
    
    using DomainRequestContext = Accelerator.Core.BusinessLogic.Context.RequestContext;
    using IRequestContextProvider = Accelerator.Core.BusinessLogic.Context.IRequestContextProvider; 
    
    /// <summary>Dashboard integration test.</summary>
    public abstract class DashboardIntegrationTest
    {
        /// <summary>The default roles.</summary>
        private List<string> defaultRoles;

        /// <summary>The entities.</summary>
        private DashboardEntities entities;

        /// <summary>Initializes a new instance of the DashboardIntegrationTest class with default RequestContext.</summary>
        protected DashboardIntegrationTest()
        {
            this.RequestContext = new RequestContext
                                        {
                                            UserId = 9,
                                            UserName = "DFMFirmSuperuser",
                                            ClientId = 9,
                                            LocationId = DefaultLocationId,
                                            UserRoles = this.DefaultRoles
                                        };
        }

        /// <summary>Gets the default location identifier.</summary>
        /// <value>The default location identifier.</value>
        public static int DefaultLocationId
        {
            get { return 5; }
        }

        /// <summary>Gets the default roles.</summary>
        /// <value>The default roles.</value>
        public List<string> DefaultRoles
        {
            get
            {
                return this.defaultRoles ??
                       (this.defaultRoles =
                        new List<string>
                            {
                                LoginRoles.Dashboard,
                                LoginRoles.ModelPortfolio,
                                DashboardRoles.IsProfessionalUser,
                                DashboardRoles.Carousel,
                                MPRoles.AccessToModelManagement,
                                MPRoles.AccessToClientWrapperManagement,
                                "Dashboard.Viewer",
                                "Professional.Viewer",
                                "Professional.Trader"
                            });
            }
        }

        /// <summary>Gets the entities for direct DB access.</summary>
        /// <value>The entities.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "DashboardEntities object cannot be disposed as it is used by the clients of this property; Also disposing it is not necessary in this case as at the end of tests all resources are freed out.")]
        public DashboardEntities Entities
        {
            get
            {
                if (this.entities == null)
                {
                    var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DashboardEntities"].ConnectionString;
                    this.entities = new DashboardEntities(connectionString)
                        {
                            RequestContextProvider = this.DashboardServerContextProviderMock
                        };
                }

                return this.entities;
            }
        }

        /// <summary>Gets or sets a context for the request.</summary>
        /// <value>The request context.</value>
        public RequestContext RequestContext { get; set; }

        /// <summary>Gets the asset service client.</summary>
        /// <value>The asset service client.</value>
        public IAssetClient AssetServiceClient
        {
            get
            {
                return new AssetClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        public IAccountInvestmentClient AccountInvestmentClient
        {
            get
            {
                return new AccountInvestmentClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the location service client.</summary>
        /// <value>The location service client.</value>
        public ILocationClient LocationServiceClient
        {
            get
            {
                return new LocationClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the search service client.</summary>
        /// <value> The search service client. </value>
        public ISearchClient SearchServiceClient
        {
            get
            {
                return new SearchClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the dashboard service client.</summary>
        /// <value> The dashboard service client. </value>
        public IDashboardClient DashboardServiceClient
        {
            get
            {
                return new DashboardClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the redirection client.</summary>
        /// <value>The redirection client.</value>
        public IRedirectionClient RedirectionClient
        {
            get
            {
                return new RedirectionClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the account service client.</summary>
        /// <value> The account service client. </value>
        public IAccountClient AccountClient
        {
            get
            {
                return new AccountClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the fixed business card client.</summary>
        /// <value>The fixed business card client.</value>
        public IFixedBusinessCardClient FixedBusinessCardClient
        {
            get
            {
                return new FixedBusinessCardClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the breadcrumbs client.</summary>
        /// <value>The breadcrumbs client.</value>
        public IBreadcrumbsClient BreadcrumbsClient
        {
            get
            {
                return new BreadcrumbsClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the security client.</summary>
        /// <value>The security client.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public IAuthenticatedSecurityServiceClient SecurityClient
        {
            get
            {
                ////return new AuthenticatedSecurityServiceClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
                return new AuthenticatedSecurityServiceClient(this.DashboardClientContextProviderMock);
            }
        }

        /// <summary>Gets the custom pages service client.</summary>
        /// <value>The custom pages service client.</value>
        public ICustomPagesServiceClient CustomPagesServiceClient
        {
            get
            {
                return new CustomPagesServiceClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the configuration client mock.</summary>
        /// <value>The configuration client mock.</value>
        protected virtual IConfigurationClient ConfigurationClientMock
        {
            get
            {
                return Mock.Of<IConfigurationClient>();
            }
        }

        /// <summary>Gets the dashboard service client.</summary>
        /// <value> The dashboard service client. </value>
        protected IDynamicReportClient DynamicReportServiceClient
        {
            get
            {
                return new DynamicReportClient(this.DashboardClientContextProviderMock, SecurityLoggerMock);
            }
        }

        /// <summary>Gets the logger mock.</summary>
        /// <value>The logger mock.</value>
        private static ILogger LoggerMock
        {
            get
            {
                return new Mock<ILogger>().Object;
            }
        }

        /// <summary>Gets the invocation details provider mock.</summary>
        /// <value>The invocation details provider mock.</value>
        private static IInvocationDetailsProvider InvocationDetailsProviderMock
        {
            get
            {
                var mock = new Mock<IInvocationDetailsProvider>();
                mock.Setup(m => m.InvocationDetails).Returns(new InvocationDetailsMock("TEST"));
                return mock.Object;
            }
        }

        /// <summary>Gets the security logger mock.</summary>
        /// <value>The security logger mock.</value>
        private static ISecurityLogger SecurityLoggerMock
        {
            get
            {
                return new SecurityLogger(LoggerMock, InvocationDetailsProviderMock);
            }
        }

        /// <summary>Gets the dashboard client context provider mock.</summary>
        /// <value>The dashboard client context provider mock.</value>
        private IClientContextProvider<RequestContext> DashboardClientContextProviderMock
        {
            get
            {
                var mock = new Mock<IClientContextProvider<RequestContext>>();
                mock.Setup(m => m.GetClientContext()).Returns(this.RequestContext);
                return mock.Object;
            }
        }

        /// <summary>Gets the dashboard server context provider mock.</summary>
        /// <value>The dashboard server context provider mock.</value>
        private IRequestContextProvider DashboardServerContextProviderMock
        {
            get
            {
                var mock = new Mock<IRequestContextProvider>();
                mock.Setup(m => m.Context).Returns(
                    new DomainRequestContext
                    {
                        UserId = this.RequestContext.UserId,
                        UserName = this.RequestContext.UserName,
                        LocationId = this.RequestContext.LocationId
                    });
                return mock.Object;
            }
        }

        /// <summary>Gets the dashboard repository base.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <returns>The dashboard repository base</returns>
        public DashboardRepositoryBase<TEntity> GetDashboardRepositoryBase<TEntity>() where TEntity : BusinessEntity
        {
            return new DashboardRepositoryBase<TEntity>(this.Entities);
        }

        /// <summary>Ensures next access to Entities will give a fresh context.</summary>
        public void RefreshEntities()
        {
            this.entities = null;
        }
    }
}