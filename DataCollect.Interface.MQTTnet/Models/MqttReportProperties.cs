using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.MQTTnet.Models
{
   
    #region 分拣机
    public class MqttReportSortingProperties4S : SetProperties
    {
        public MachineBasicInformationSorting4S properties { get; set; }
    }
    public class MqttReportSortingProperties1D : SetProperties
    {
        public MachineBasicInformationSorting1D properties { get; set; }
    }
    #endregion
    #region 导入台
    public class MqttReportInductionProperties4S : SetProperties
    {
        public MachineBasicInformationInduction4S properties { get; set; }
    }
    public class MqttReportInductionProperties1D : SetProperties
    {
        public MachineBasicInformationInduction1D properties { get; set; }
    }
    #endregion
    #region 灰度检测仪
    public class MqttReportGLDProperties4S : SetProperties
    {
        public MachineBasicInformationIGLD4S properties { get; set; }
    }
    public class MqttReportGLDProperties1D : SetProperties
    {
        public MachineBasicInformationIGLD1D properties { get; set; }
    }
    #endregion
    #region 扫描模块
    public class MqttReportScannerProperties4S : SetProperties
    {
        public MachineBasicInformationIScanner4S properties { get; set; }
    }
    public class MqttReportScannerProperties1D : SetProperties
    {
        public MachineBasicInformationIScanner1D properties { get; set; }
    }
    #endregion
    #region 急停按钮
    public class MqttReportESButtonProperties4S : SetProperties
    {
        public MachineBasicInformationESButton4S properties { get; set; }
    }
    public class MqttReportESButtonProperties1D : SetProperties
    {
        public MachineBasicInformationESButton1D properties { get; set; }
    }
    #endregion
    #region 打印机
    public class MqttReportPrinterProperties4S : SetProperties
    {
        public MachineBasicInformationPrinter4S properties { get; set; }
    }
    public class MqttReportPrinterProperties1D : SetProperties
    {
        public MachineBasicInformationPrinter1D properties { get; set; }
    }
    #endregion
    #region 48V供电
    public class MqttReport48VProperties4S : SetProperties
    {
        public MachineBasicInformation48V4S properties { get; set; }
    }
    public class MqttReport48VProperties1D : SetProperties
    {
        public MachineBasicInformation48V1D properties { get; set; }
    }
    #endregion
    #region 直线电机
    public class MqttReportMotorProperties4S : SetProperties
    {
        public MachineBasicInformationMotor4S properties { get; set; }
    }
    public class MqttReportMotorProperties1D : SetProperties
    {
        public MachineBasicInformationMotor1D properties { get; set; }
    }
    #endregion
    #region 格口
    public class MqttReportChutesProperties4S : SetProperties
    {
        public MachineBasicInformationChutes4S properties { get; set; }
    }
    public class MqttReportChutesProperties1D : SetProperties
    {
        public MachineBasicInformationChutes1D properties { get; set; }
    }
    #endregion
    #region 交叉带小车
    public class MqttReportCarriersProperties4S : SetProperties
    {
        public MachineBasicInformationCarriers4S properties { get; set; }
    }
    public class MqttReportCarriersProperties1D : SetProperties
    {
        public MachineBasicInformationCarriers1D properties { get; set; }
    }
    #endregion
}
