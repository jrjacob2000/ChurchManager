using ChurchManager.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using AutoMapper;
using System.Linq;
using System.Web.Hosting;

[assembly: OwinStartupAttribute(typeof(ChurchManager.Startup))]
namespace ChurchManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        //public void Configure(IAppBuilder app, HostingEnvironment env, IMapper autoMapper)
        //{
        //    autoMapper.ConfigurationProvider.AssertConfigurationIsValid();
        //}
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddAutoMapper();

        //}
        // In this method we will create default User roles and Admin user for login   


    }
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Person, PersonView>()
                .ForMember(dest => dest.SelectedFamilyId, src => src.MapFrom(x => x.Family.ID))
                .ForMember(dest => dest.SelectedFamilyRoleId, src => src.MapFrom(x => x.FamilyRole.Id))
                .ForMember(dest => dest.FamilyRoleDisplay, src => src.MapFrom(x => x.FamilyRole.Description))
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

                cfg.CreateMap<PersonView, Person>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

                cfg.CreateMap<Family, FamilyView>();

                cfg.CreateMap<Group, GroupView>();

            });
        }

       
    }
}
