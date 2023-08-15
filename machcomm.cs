//Mario A. Dominguez Guerrero 
//April - 2023

#region System Libraries
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

#region project Libraries

#endregion

namespace SensataSoftware_DEMO
{
    class machcomm
    {
        
    }

    #region CSV File Machine Settings
    enum CSV_MachConfig
    {
        SoftwareVersion,
        SoftwareVersionDate,
        MachineName,
        //Scan Components
        ScanCompStationName,
        ScanCompStationID,
        ScanCompEquipmentID,
        //Install Terminal Shields
        InstallTermShldStationName,
        InstallTermShldStationID,
        InstallTermShldEquipmentID,
        //Install M8 Inserts
        InstallM8InsrStationName,
        InstallM8InsrStationID,
        InstallM8InsrEquipmentID,
        //Plunger & Armature Assy
        PlugrArmAssyStationName,
        PlugrArmAssyStationID,
        PlugrArmAssyEquipmentID,
        //Latch Assy
        LatchAssyStationName,
        LatchAssyStationID,
        LatchAssyEquipmentID,
        //Machine
        StationID,
        EquipmentID,
        PartListID,
        ElectricalDrawingID,
        SerialMachine,
        SQLServerName,
        SQLDBName,
        SQLUser,
        AppRoleUser,
        PriorOpCheck,
        UpdateDeviceStatus,
        DeviceIDTable,
        //Machine
        StationName,
        PCName,
        PC_IP,
        PC_DNS,
        MachineNumber,
        LimitsOperParams,
        //Install Terminal Shields
        InstallTermShldHistory,
        InstallTermShldMasterSamples,
        //Install M8 Inserts
        InstallM8InsrResultsHistory,
        InstallM8InsrMasterSamples,
        //Plunger & Armature Assy
        PlugrArmAssyResultsHistory,
        PlugrArmAssyMasterSamples,
        //Latch Assy
        LatchAssyResultsHistory,
        LatchAssyMasterSamples,
        //Masters
        MasterSequenceView,
        DeviceFamily,
        //Assign & Check Components
        CheckComponents,
        AssignComponents,
        AssignChkCompID,
        //Components Traceability
        ScanComponent,
        RequiredComponents
    }

    #endregion

    #region SQL Server & Batabase Parameters
    //Lot & Production
    enum Prod_Lot
    {
        DeviceID,
        LotID,
        PartStatus
    }

    #region Master Parameters
    enum MasterProcessStatus
    {
        Omitted,
        Waiting,
        Done,
        Pending
    }
    enum MasterStatus_Param
    {
        Status,
        Counter
    }
    enum MastersMessages
    {
        Disable,
        Done,
        MasterPartSN,
    }

    ///Masters
    /// [0] = Serial
    /// [1] = Device ID
    /// [2] = Station
    /// [3] = Status
    /// [4] = Expected Value
    /// [5] = Sequence Number
    /// [6] = Nest
    enum MastersParameters
    {
        Serial,
        DeviceID,
        Station,
        Status,
        ExpecValue,
        SequenceNo,
        Nest
    }
    #endregion

    //Production Limits

    //Install Terminal Shields
    enum eInstallTermShld_Prod_Limits
    {
        DeviceID,
        ThresholdX_Max,
        ThresholdX_Min,
        ThresholdY_Max,
        ThresholdY_Min,
        Settings
    }
    enum eInstallTermShld_Prod_Results
    {
        PartStatus,
        ThresholdX_Max,
        ThresholdX_Min,
        ThresholdY_Max,
        ThresholdY_Min,
    }
    //Install M8 Inserts
    enum eInstallM8Insr_Prod_Limits
    {
        DeviceID,
        ThresholdX_Max,
        ThresholdX_Min,
        ThresholdY_Max,
        ThresholdY_Min,
        VisionRecipe,
        Settings
    }
    enum eInstallM8Insr_Prod_Results
    {
        PartStatus,
        ThresholdX_Max,
        ThresholdX_Min,
        ThresholdY_Max,
        ThresholdY_Min,
    }
    //Plunger & Armature Assy
    enum ePlugrArmAssy_Prod_Limits
    {
        DeviceID,
        Angle_Max,
        Angle_Min,
        ArmatureInspection,
        ClampAngle_Max,
        ClampAngle_Min,
        ClampTorque_Max,
        ClampTorque_Min,
        SP_Max,
        SP_Min,
        SpringInspection,
        Torque_Max,
        Torque_Min,
        StandbyPos,
        ReadyPos,
        Test1Pos,
        Test2Pos,
        ScrewdrivingPos,
        StandbySpeed,
        ReadySpeed,
        Test1Speed,
        Test2Speed,
        ScrewdrivingSpeed,
        ConstantK_min,
        ConstantK_max,
        Test1ChXmin,
        Test1ChXmax,
        Test1ChYmin,
        Test1ChYmax,
        Test2ChXmin,
        Test2ChXmax,
        Test2ChYmin,
        Test2ChYmax,
        Settings
    }
    enum ePlugrArmAssy_Prod_Results
    {
        PartStatus,
        Torque,
        Angle,
        Clamp,
        SP,
        ClampAngle,
        Test1_Force,
        Test1_Displacement,
        Test2_Force,
        Test2_Displacement,
        ConstK,
    }
    //Latch Assy
    enum eLatchAssy_Prod_Limits
    {
        DeviceID,
        Angle_Max,
        Angle_Min,
        ClampAngle_Max,
        ClampAngle_Min,
        ClampTorque_Max,
        ClampTorque_Min,
        SP_Max,
        SP_Min,
        Torque_Max,
        Torque_Min,
        Settings
    }
    enum eLatchAssy_Prod_Results
    {
        PartStatus,
        Torque,
        Angle,
        Clamp,
        SP,
        ClampAngle,
    }
    #endregion
}
