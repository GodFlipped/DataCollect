using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Core.Entities.Mqtt
{
    public class CollectPlcData : IEntity,IEntityTypeBuilder<CollectPlcData>
    {
        [Required, MaxLength(40)]
        public string Id { get; set; }
        /// <summary>
        /// OPCUA地址
        /// </summary>
        [MaxLength(60)]
        public string OpcValue { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        [MaxLength(60)]
        public string Message { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(60)]
        public string IpAddress { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        [MaxLength(80)]
        public string DeviceCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [MaxLength(40)]
        public string IType { get; set; }
        /// <summary>
        /// 转换消息
        /// </summary>
        [MaxLength(100)]
        public string Value { get; set; }
        /// <summary>
        /// 原来消息
        /// </summary>
        [MaxLength(100)]
        public string OldValue { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        [MaxLength(40)]
        public string DeviceType { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        [MaxLength(40)]
        public string DeviceNumber { get; set; }

        [MaxLength(40)]
        public string ComponentPropertyType { get; set; }
        [MaxLength(40)]
        public string ComponentProperty { get; set; }

        public void Configure(EntityTypeBuilder<CollectPlcData> entityBuilder, DbContext dbContext, Type dbContextLocator)
        {
            entityBuilder.HasKey(u => u.Id);
        }
    }
}
