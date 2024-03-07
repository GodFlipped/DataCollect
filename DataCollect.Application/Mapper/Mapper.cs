using Mapster;

namespace DataCollect.Application.Mapper
{
    /// <summary>
    /// 自定义Mapper映射规则
    /// </summary>
    public class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //config.ForType<SystemConfiguration, SystemConfigurationDto>()
            //     .Map(dest => dest.creator, src => src.creator + src.creatTime);
        }
    }
}
