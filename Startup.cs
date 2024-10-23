using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Repository;
using PCNW.Services;
using SolrNet;
using System.Net;
using System.Text;

namespace PCNW
{
    public class Startup
    {
        // This is not the best idea to use a static variable (especially when you have more than one active uploading session at the same time) Test
        public static int Progress { get; set; }
        public static int FileCount { get; set; } = 0;
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Configure strongly typed app seting objects
            var appSettingSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);
            var appSettings = appSettingSection.Get<AppSettings>();
            var Key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.Configure<ChargeBeeInfo>(Configuration.GetSection("ChargeBeeInfo"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Connections
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure();
                            });

            }, ServiceLifetime.Scoped);

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            ConfigureChargebee();
            services.AddScoped<ILoggerManager, LoggerService>();
            services.AddScoped<IManageExsingUserRepository, ManageExsingUserRepository>();
            services.AddScoped<IEntityRepository, EntityRepository>();
            services.AddScoped<ChargeBeeAPIService>();

            var solrUrl = Configuration["AppSettings:SolrURL"];
            services.AddSolrNet(solrUrl);
            services.AddAuthorization(
            options =>
            {
                options.AddPolicy("CreateRolePolicy",
                                    policy => policy.RequireClaim("Create Role", "true"));
                options.AddPolicy("EditRolePolicy",
                                    policy => policy.RequireClaim("Edit Role", "true"));
                options.AddPolicy("DeleteRolePolicy",
                                    policy => policy.RequireClaim("Delete Role", "true"));
                options.AddPolicy("ViewRolePolicy",
                                    policy => policy.RequireClaim("View Role", "true"));
                options.AddPolicy("AdminRolePolicy",
                                    policy => policy.RequireRole("Admin"));
            });
            services.AddIdentityCore<IdentityUser>(options =>
                                                   options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            });
            services.Configure<PasswordHasherOptions>(options =>
                options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
            );

            // Default Policy
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowOrigin",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44391", "http://localhost:44391", "http://localhost:4200")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });


            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IEmailServiceManager, EmailServiceManager>();
            services.AddScoped<ILoggerManager, LoggerService>();
            services.AddScoped<IUtilityManager, UtilityManager>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IImplementRepository, ImplementRepository>();
            services.AddScoped<ICommonRepository, CommonRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IGlobalMasterRepository, GlobalMasterRepository>();
            services.AddScoped<IAdminProjectRepository, AdminProjectRepository>();
            services.AddScoped<IGlobalRepository, GlobalRepository>();
            //services.AddSingleton<OAuth2Client>();
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddControllersWithViews();
            services.AddRazorPages();

            // Add services to the container.
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("Your IP Address"));
            });
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            // Add Authontication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Error";
                    options.AccessDeniedPath = "/NotFound";
                });



            services.AddCors();
            services.AddSession();
            services.AddMvc();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            });
            //.AddOpenIdConnect(options =>
            //{
            //    options.SignInScheme = "Cookies";
            //    options.Authority = "-your-identity-provider-";
            //    options.RequireHttpsMetadata = true;
            //    options.ClientId = "-your-clientid-";
            //    options.ClientSecret = "-your-client-secret-from-user-secrets-or-keyvault";
            //    options.ResponseType = "code";
            //    options.UsePkce = true;
            //    options.Scope.Add("profile");
            //    options.SaveTokens = true;
            //    options.RequireHttpsMetadata = false;
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment() || env.IsEnvironment("localhost") || env.IsEnvironment("staging"))
            {
                DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions
                {
                    SourceCodeLineCount = 5
                };
                app.UseDeveloperExceptionPage(developerExceptionPageOptions);
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors(builder =>
            builder.WithOrigins("https://localhost:44391", "http://localhost:44391", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());

            app.UseCors("AllowOrigin");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseFileServer();

            app.UseRouting();

            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                  name: "Admin",
                  pattern: "{controller=Admin}/{action=Login}/{id?}");
                endpoints.MapControllerRoute(
                  name: "StaffAccount",
                  pattern: "{controller=StaffAccount}/{action=Login}/{id?}");
                endpoints.MapControllerRoute(
                  name: "Member",
                  pattern: "{controller=Account}/{action=Login}/{id?}");
                endpoints.MapRazorPages();
            });
        }
        private void ConfigureChargebee()
        {
            ChargeBee.Api.ApiConfig.Configure("plancenternw-test", "test_J2YjBnV9hJECNiemv3iT6CG7KcddbO3rh");
        }
    }

}