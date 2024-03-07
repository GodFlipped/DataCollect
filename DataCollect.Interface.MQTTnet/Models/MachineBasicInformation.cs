using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.MQTTnet.Models
{
    #region 分拣机
    public class MachineBasicInformationSorting4S
    {
        //设备分拣条件
        [JsonProperty("sorting-module.sorter-condition")]
        public List<SortingCondition> sortingCondition { get; set; }
        //首车同步状态
        [JsonProperty("sorting-module.first-car-syn-status")]
        public List<SortingFirstCarSynStatus> sortingFirstCarSynStatus { get; set; }
        //速度达标状态
        [JsonProperty("sorting-module.speed-status")]
        public List<SortingSpeedStatus> sortingSpeedStatus { get; set; }
        //设备控制模式
        [JsonProperty("sorting-module.initialize-status")]
        public List<SortingInitializeStatus> sortingInitializeStatus { get; set; }
        //总运行里程
        [JsonProperty("sorting-module.total-mileage")]
        public List<SortingTotalMileage> sortingTotalMileage { get; set; }
        //今日运行里程
        [JsonProperty("sorting-module.today-mileage")]
        public List<SortingTodayMileage> sortingTodayMileage { get; set; }
        //总运行时间
        [JsonProperty("sorting-module.total-running-time")]
        public List<SortingTotalRunningTime> sortingTotalRunningTime { get; set; }
        //今日运行时间
        [JsonProperty("sorting-module.today-running-time")]
        public List<SortingTodayRunningTime> sortingTodayRunningTime { get; set; }
        //总供件效率
        [JsonProperty("sorting-module.induce-capacity")]
        public List<SortingInduceCapacity> sortingInduceCapacity { get; set; }
        //小车满载率
        [JsonProperty("sorting-module.load-factor")]
        public List<SortingLoadFactor> sortingLoadFactor { get; set; }
        //当前运行加速度
        //[JsonProperty("sorting-module.device-acceleration")]
        //public List<SortingDeviceAcceleration> sortingDeviceAcceleration { get; set; }
        //设备速度
        [JsonProperty("sorting-module.device-speed")]
        public List<SortingDeviceSpeed> sortingDeviceSpeed { get; set; }
        //设备控制模式
        [JsonProperty("sorting-module.control-mode")]
        public List<SortingControlMode> sortingControlMode { get; set; }
        //设备故障状态
        [JsonProperty("sorting-module.fault-status")]
        public List<SortingFaultStatus> sortingFaultStatus { get; set; }
        //启停状态
        [JsonProperty("sorting-module.start-stop-status")]
        public List<SortingStartStopStatus> sortingStartStopStatus { get; set; }

        //设备急停状态
        [JsonProperty("sorting-module.e-stop-status")]
        public List<SortingEStopStatus> sortingEStopStatus { get; set; }
    }
    public class MachineBasicInformationSorting1D
    {
        //设备总数
        [JsonProperty("sorting-module.device-amount")]
        public int sortingDeviceAmount { get; set; }
        //设备基础信息
        [JsonProperty("sorting-module.device-base-info")]
        public List<SortingDeviceBaseInfo> sortingDeviceBaseInfo { get; set; }
        //设备速度范围
        [JsonProperty("sorting-module.device-speed-scope")]
        public SortingDeviceSpeedScope sortingDeviceSpeedScope { get; set; }
        //设备类型
        [JsonProperty("sorting-module.device-type")]
        public string sortingDeviceType { get; set; }
        //货物长度范围
        [JsonProperty("sorting-module.package-length-scope")]
        public SortingPackageLengthScope sortingPackageLengthScope { get; set; }
        //货物宽度范围
        [JsonProperty("sorting-module.package-width-scope")]
        public SortingPackageWidthScope sortingPackageWidthScope { get; set; }
        //货物高度范围
        [JsonProperty("sorting-module.package-height-scope")]
        public SortingPackageHeightScope sortingPackageHeightScope { get; set; }
        //货物重量范围
        [JsonProperty("sorting-module.package-weight-scope")]
        public SortingPackageWeightScope sortingPackageWeightScope { get; set; }
        //分拣效率范围
        [JsonProperty("sorting-module.sort-capacity-scope")]
        public List<SortingCapacityScope> sortingCapacityScope { get; set; }
    }

    #endregion
    #region  导入台
    public class MachineBasicInformationInduction4S
    {
        //导入台设备故障状态
        [JsonProperty("induction-of-sorter.fault-status")]
        public List<InductionFaultStatus> inductionFaultStatus { get; set; }

        //导入台设备控制模式
        [JsonProperty("induction-of-sorter.control-mode")]
        public List<InductionControlMode> inductionControlMode { get; set; }

        //导入台手动扫码状态
        [JsonProperty("induction-of-sorter.handheld-scanner-status")]
        public List<InductionScannerStatus> inductionScannerStatus { get; set; }

        //导入台启停状态
        [JsonProperty("induction-of-sorter.start-stop-status")]
        public List<InductionStartStopStatus> inductionStartStopStatus { get; set; }

        //导入台供件效率
        [JsonProperty("induction-of-sorter.induce-capacity")]
        public List<InductionInduceCapacity> inductionInduceCapacity { get; set; }
        //导入台设备急停状态
        [JsonProperty("induction-of-sorter.e-stop-status")]
        public List<InductionEstopStatus> inductionEstopStatus { get; set; }
    }
    public class MachineBasicInformationInduction1D
    {
        //导入台基础信息
        [JsonProperty("induction-of-sorter.device-base-info")]
        public List<InductionBaseInfo> inductionBaseInfo { get; set; }
        //导入台数量
        [JsonProperty("induction-of-sorter.device-amount")]
        public int inductionDeviceAmount { get; set; }
        //导入台设备类型
        [JsonProperty("induction-of-sorter.device-type")]
        public List<InductionDeviceType> inductionDeviceType { get; set; }
    }
    #endregion
    #region  灰度检测仪
    public class MachineBasicInformationIGLD4S
    {
        //灰度检测仪设备故障状态
        [JsonProperty("grayscale-detector.fault-status")]
        public List<GrayscaleFaultStatus> grayscaleFaultStatus { get; set; }
    }
    public class MachineBasicInformationIGLD1D
    {
        //灰度检测仪基础信息
        [JsonProperty("grayscale-detector.device-base-info")]
        public List<GrayscaleDeviceBaseInfo> grayscaleDeviceBaseInfo { get; set; }

        //灰度检测仪设备总数
        [JsonProperty("grayscale-detector.device-amount")]
        public int grayscaleDdeviceAmount { get; set; }
    }
    #endregion
    #region 扫描模块
    public class MachineBasicInformationIScanner1D
    {
        //扫描模块基础信息
        [JsonProperty("scan-module.device-base-info")]
        public List<ScanDeviceBaseInfo> scanDeviceBaseInfo { get; set; }

        //扫描模块设备总数
        [JsonProperty("scan-module.device-amount")]
        public int scanDeviceAmount { get; set; }

        //扫描模块设备类型
        [JsonProperty("scan-module.device-type")]
        public List<ScanDeviceType> scanDeviceType { get; set; }
    }
    public class MachineBasicInformationIScanner4S
    {
        //扫描模块设备故障状态
        [JsonProperty("scan-module.fault-status")]
        public List<ScanFaultStatus> scanFaultStatus { get; set; }
    }
    #endregion
    #region  急停按钮
    public class MachineBasicInformationESButton4S
    {
        //急停按钮主控急停状态
        [JsonProperty("e-stop-buttons.master-e-stop-status")]
        public List<EStopButtonsMasterEStopStatus> eStopButtonsMasterEStopStatus { get; set; }

        //急停按钮设备急停状态
        [JsonProperty("e-stop-buttons.e-stop-status")]
        public List<EStopButtonsEStopStatus> eStopButtonsEStopStatus { get; set; }
    }
    public class MachineBasicInformationESButton1D
    {
        //急停按钮设备总数
        [JsonProperty("e-stop-buttons.device-amount")]
        public int eStopButtonsDeviceAmount { get; set; }

        //急停按钮基础信息
        [JsonProperty("e-stop-buttons.device-base-info")]
        public List<EStopButtonsDeviceBaseInfo> eStopButtonsDeviceBaseInfo { get; set; }

        //急停按钮种类数量
        [JsonProperty("e-stop-buttons.device-number")]
        public List<EStopButtonsDeviceNumber> eStopButtonsDeviceNumber { get; set; }
    }
    #endregion
    #region  打印机
    public class MachineBasicInformationPrinter4S
    {
        //打印机设备故障状态
        [JsonProperty("printer.fault-status")]
        public List<PrinterFaultStatus> printerFaultStatus { get; set; }
    }
    public class MachineBasicInformationPrinter1D
    {
        //打印机设备总数
        [JsonProperty("printer.device-amount")]
        public int printerDeviceAmount { get; set; }
        //打印机设备基础信息
        [JsonProperty("printer.device-base-info")]
        public List<PrinterDeviceBaseInfo> printerDeviceBaseInfo { get; set; }

        //打印机设备格口范围
        [JsonProperty("printer.chute-scope")]
        public List<PrinterChuteScope> printerChuteScope { get; set; }
    }
    #endregion
    #region  48V供电
    public class MachineBasicInformation48V4S
    {
        //48V供电设备
        [JsonProperty("power-of-sorter.fault-status")]
        public List<PowerFaultStatus> powerFaultStatus { get; set; }
    }
    public class MachineBasicInformation48V1D
    {
        //48V供电设备总数
        [JsonProperty("power-of-sorter.device-amount")]
        public int powerDeviceAmount { get; set; }
        //48V供电设备基础信息
        [JsonProperty("power-of-sorter.device-base-info")]
        public List<PowerDeviceBaseInfo> powerDeviceBaseInfo { get; set; }
    }
    #endregion
    #region 直线电机
    public class MachineBasicInformationMotor4S
    {
        //直线电机设备故障状态
        [JsonProperty("motor-of-sorter.fault-status")]
        public List<MotorFaultStatus> motorFaultStatus { get; set; }

        //直线电机设备故障状态
        [JsonProperty("motor-of-sorter.transformer-frequency")]
        public List<MotorTransformerFrequency> motorTransformerFrequency { get; set; }
    }
    public class MachineBasicInformationMotor1D
    {
        //直线电机设备总数
        [JsonProperty("motor-of-sorter.device-amount")]
        public int motorDeviceAmount { get; set; }
        //直线电机设备基础信息
        [JsonProperty("motor-of-sorter.device-base-info")]
        public List<MotorDeviceBaseInfo> motorDeviceBaseInfo { get; set; }
    }
    #endregion
    #region 格口
    public class MachineBasicInformationChutes4S
    {
        //格口设备故障状态
        [JsonProperty("chute.fault-status")]
        public List<ChuteFaultStatus> chuteFaultStatus { get; set; }

        //格口格口状态
        [JsonProperty("chute.device-status")]
        public List<ChuteDeviceStatus> chuteDeviceStatus { get; set; }
    }
    public class MachineBasicInformationChutes1D
    {
        //格口总数
        [JsonProperty("chute.device-amount")]
        public int chuteDeviceAmount { get; set; }
        //格口宽度
        [JsonProperty("chute.device-width")]
        public int chuteDeviceWidth { get; set; }
        //格口基础信息
        [JsonProperty("chute.device-base-info")]
        public List<ChuteDeviceBaseInfo> chuteDeviceBaseInfo { get; set; }
        //格口格口类型
        [JsonProperty("chute.device-type")]
        public List<ChuteDeviceType> chuteDeviceType { get; set; }
    }
    #endregion
    #region 交叉带小车
    public class MachineBasicInformationCarriers4S
    {
        //设备编号
        [JsonProperty("car-of-sorter.device-serial-number")]
        public List<CarDeviceSerialNumber> carDeviceSerialNumber { get; set; }
        //设备状态
        [JsonProperty("car-of-sorter.device-run-status")]
        public List<CarDeviceRunStatus> carDeviceRunStatus { get; set; }
        //设备故障状态
        [JsonProperty("car-of-sorter.fault-status")]
        public List<CarFaultStatus> carFaultStatus { get; set; }
    }
    public class MachineBasicInformationCarriers1D
    {
        //小车总数
        [JsonProperty("car-of-sorter.device-amount")]
        public int carDeviceAmount { get; set; }
        //小车节距
        [JsonProperty("car-of-sorter.car-length")]
        public int carLength { get; set; }
        //带面宽度
        [JsonProperty("car-of-sorter.belt-width")]
        public int carBeltWidth { get; set; }
        //带面长度
        [JsonProperty("car-of-sorter.belt-length")]
        public int carBeltLength { get; set; }

        //设备基础信息
        [JsonProperty("car-of-sorter.device-base-info")]
        public List<CarDeviceBaseInfo> carDeviceBaseInfo { get; set; }
        
    }
    #endregion




    #region 分拣机
    public class SortingEStopStatus:BasePropertity
    {
        [JsonProperty("component-e-stop-status")]
        public string eStopStatus { get; set; }
    }
    public class SortingTodayRunningTime : BasePropertity
    {
        [JsonProperty("today-running-time")]
        public float todayRunningTime { get; set; }
    }
    public class SortingTotalRunningTime:BasePropertity
    {
        [JsonProperty("total-running-time")]
        public float totalRunningTime { get; set; }
    }
    public class SortingPackageHeightScope
    {
        [JsonProperty("min-height")]
        public int minHeight { get; set; }
        [JsonProperty("max-height")]
        public int maxHeight { get; set; }
    }
    public class SortingStartStopStatus : BasePropertity
    {
        [JsonProperty("component-start-stop-status")]
        public string componentStartStopStatus { get; set; }
    }
    public class SortingFaultStatus : BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    public class SortingControlMode : BasePropertity
    {
        [JsonProperty("component-control-mode")]
        public string componentControlMode { get; set; }
    }
    public class SortingDeviceSpeed : BasePropertity
    {
        [JsonProperty("speed")]
        public float speed { get; set; }
    }
    public class SortingDeviceAcceleration : BasePropertity
    {
        [JsonProperty("acceleration")]
        public float acceleration { get; set; }
    }
    public class SortingLoadFactor : BasePropertity
    {
        [JsonProperty("load-factor")]
        public float loadFactor { get; set; }
    }
    public class SortingInduceCapacity : BasePropertity
    {
        [JsonProperty("induce-capacity")]
        public int induceCapacity { get; set; }
    }
    public class SortingTodayMileage : BasePropertity
    {
        [JsonProperty("today-mileage")]
        public float todayMileage { get; set; }
    }
    public class SortingTotalMileage : BasePropertity
    {
        [JsonProperty("total-mileage")]
        public float totalMileage { get; set; }
    }
    public class SortingInitializeStatus : BasePropertity
    {
        [JsonProperty("initialize-status")]
        public string initializeStatus { get; set; }
    }
    public class SortingSpeedStatus : BasePropertity
    {
        [JsonProperty("speed-status")]
        public string speedStatus { get; set; }
    }
    public class SortingFirstCarSynStatus : BasePropertity
    {
        [JsonProperty("first-car-syn-status")]
        public string firstCarSynStatus { get; set; }
    }
    public class SortingCondition : BasePropertity
    {
        [JsonProperty("sorter-condition")]
        public string sorterCondition { get; set; }
    }
    public class SortingCapacityScope : BasePropertity
    {
        [JsonProperty("min-capacity")]
        public int minCapacity { get; set; }
        [JsonProperty("max-capacity")]
        public int maxCapacity { get; set; }
    }
    public class SortingPackageWeightScope
    {
        [JsonProperty("min-weight")]
        public float minWeight { get; set; }
        [JsonProperty("max-weight")]
        public float maxWeight { get; set; }
    }
    public class SortingPackageWidthScope
    {
        [JsonProperty("min-width")]
        public int minWidth { get; set; }
        [JsonProperty("max-width")]
        public int maxWidth { get; set; }
    }
    public class SortingPackageLengthScope
    {
        [JsonProperty("min-length")]
        public int minLength { get; set; }
        [JsonProperty("max-length")]
        public int maxLength { get; set; }
    }
    public class SortingDeviceBaseInfo : BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }

        [JsonProperty("software-version")]
        public string softwareVersion { get; set; }
    }
    public class SortingDeviceSpeedScope
    {
        [JsonProperty("min-speed")]
        public float minSpeed { get; set; }
        [JsonProperty("max-speed")]
        public float maxSpeed { get; set; }
    }
    #endregion
    #region 交叉带小车
    public class CarDeviceSerialNumber
    {
        [JsonProperty("component-type")]
        public string componentType { get; set; }
        [JsonProperty("component-number")]
        public int componentNumber { get; set; }
        [JsonProperty("component-no-list")]
        public string componentNoList { get; set; }
    }
    public class CarDeviceRunStatus:BasePropertity
    {
        [JsonProperty("component-status")]
        public string componentStatus { get; set; }
    }
    public class CarFaultStatus:BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    public class CarDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }
    }
    #endregion
    #region 格口
    public class ChuteDeviceStatus : BasePropertity
    {
        [JsonProperty("component-status")]
        public string componentStatus { get; set; }
    }
    public class ChuteDeviceType:BasePropertity
    {
        [JsonProperty("component-type")]
        public string componentType { get; set; }
    }
}
    public class ChuteFaultStatus : BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    public class ChuteDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }
    }
    #endregion
    #region 直线电机
    public class MotorTransformerFrequency:BasePropertity
    {
        [JsonProperty("transformer-frequency")]
        public float transformerFrequency { get; set; }
    }
    public class MotorFaultStatus:BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    public  class MotorDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }
    }
    #endregion
    #region 48V供电
    public class PowerFaultStatus:BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    public class PowerDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }
    }

    #endregion
    #region 打印机
    /// <summary>
    /// 打印机设备故障状态
    /// </summary>
    public class PrinterFaultStatus:BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    /// <summary>
    /// 打印机设备格口范围
    /// </summary>
    public class PrinterChuteScope:BasePropertity
    {
        [JsonProperty("chute-scope")]
        public string chuteScope { get; set; }
    }
    /// <summary>
    /// 打印机设备基础信息
    /// </summary>
    public class PrinterDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }
    }


    #endregion
    #region 急停按钮
    public class EStopButtonsEStopStatus:BasePropertity
    {
        [JsonProperty("component-type")]
        public string componentType { get; set; }
        [JsonProperty("component-e-stop-status")]
        public string componentEstopStatus { get; set; }
    }
    /// <summary>
    /// 急停按钮主控急停状态
    /// </summary>
    public class EStopButtonsMasterEStopStatus:BasePropertity
    {
        [JsonProperty("component-e-stop-status")]
        public string eStopStatus { get; set; }
    }
    /// <summary>
    /// 急停按钮种类数量
    /// </summary>
    public class EStopButtonsDeviceNumber
    {
        [JsonProperty("component-type")]
        public string componentType { get; set; }
        [JsonProperty("component-number")]
        public int componentNumber { get; set; }
    }
    /// <summary>
    /// 急停按钮基础信息
    /// </summary>
    public class EStopButtonsDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }
    }
    #endregion
    #region  扫描模块
    /// <summary>
    /// 扫描模块设备故障状态
    /// </summary>
    public class ScanFaultStatus:BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    /// <summary>
    /// 扫描模块设备类型
    /// </summary>
    public class ScanDeviceType:BasePropertity
    {
        [JsonProperty("component-type")]
        public string componentType { get; set; }
    }
    /// <summary>
    /// 扫描模块基础信息
    /// </summary>
    public class ScanDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }

        [JsonProperty("software-version")]
        public string softwareVersion { get; set; }
    }
    #endregion
    #region  灰度检测仪
    /// <summary>
    /// 灰度检测仪设备故障状态
    /// </summary>
    public class GrayscaleFaultStatus:BasePropertity
    {
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }
    /// <summary>
    /// 灰度检测仪基础信息
    /// </summary>
    public class GrayscaleDeviceBaseInfo:BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }

        [JsonProperty("software-version")]
        public string softwareVersion { get; set; }
    }
    #endregion
    #region 导入台
    /// <summary>
    /// 
    /// </summary>
    public class InductionEstopStatus:BasePropertity
    {
        [JsonProperty("component-e-stop-status")]
        public string eStopStatus { get; set; }
}
    /// <summary>
    /// 导入台供件效率
    /// </summary>
    public class InductionInduceCapacity : BasePropertity
    {
        [JsonProperty("induce-capacity")]
        public int induceCapacity { get; set; }
    }
    /// <summary>
    /// 导入台启停状态
    /// </summary>
    public class InductionStartStopStatus: BasePropertity
    {
        [JsonProperty("component-start-stop-status")]
        public string componentStartStopStatus { get; set; }
    }
    /// <summary>
    /// 上包机手动扫码状态
    /// </summary>
    public class InductionScannerStatus: BasePropertity
    {
        [JsonProperty("component-handheld-status")]
        public string componentHandheldStatus { get; set; }
    }
    /// <summary>
    /// 导入台设备控制模式
    /// </summary>
    public class InductionControlMode: BasePropertity
    {
        [JsonProperty("component-control-mode")]
        public string componentControlMmode { get; set; }

    }
    /// <summary>
    /// 导入台设备故障状态
    /// </summary>
    public class InductionFaultStatus: BasePropertity
    {
    [JsonProperty("component-fault-status")]
    public string componentFaultStatus { get; set; }
    }
    /// <summary>
    /// 导入台设备类型
    /// </summary>
    public class InductionDeviceType: BasePropertity
    {
        [JsonProperty("component-type")]
        public string componentType { get; set; }
    }
    /// <summary>
    /// 导入台基础信息
    /// </summary>
    public class InductionBaseInfo: BasePropertity
    {
        [JsonProperty("production-date")]
        public string productionDate { get; set; }

        [JsonProperty("device-sn")]
        public string deviceSn { get; set; }

        [JsonProperty("model-number")]
        public string modelNumber { get; set; }

        [JsonProperty("manufacturer-name")]
        public string manufacturerName { get; set; }

        [JsonProperty("software-version")]
        public string softwareVersion { get; set; }
    }
    #endregion
    public class BasePropertity
    {
        [JsonProperty("component-no")]
        public string componentNo { get; set; }
    }
   



