using Opc.Ua.Configuration;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Net.NetworkInformation;
using SensataSoftware_DEMO;
using System.Data;
using System.IO;
using static Opc.Ua.Utils;
using System.Windows.Media.Animation;
using static System.Net.Mime.MediaTypeNames;

namespace GigavacFuseApp
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnNewLot.IsEnabled = true;
            btnEndLot.IsEnabled = false;
            //this.WindowState = WindowState.Maximized;
            #region Color init
            whiteGradient.Color = Color.FromArgb(255, 255, 255, 255);
            whiteGradient.Offset = 0.50;
            greenTopGradient.Color = Color.FromArgb(255, 30, 150, 0);
            greenTopGradient.Offset = 1;
            greenBottomGradient.Color = Color.FromArgb(255, 30, 150, 0);
            greenBottomGradient.Offset = 0;
            redTopGradient.Color = Color.FromRgb(255, 0, 0);
            redTopGradient.Offset = 1;
            redBottomGradient.Color = Color.FromRgb(255, 0, 0);
            redBottomGradient.Offset = 0;
            grayTopGradient.Color = Color.FromRgb(128, 128, 128);
            grayTopGradient.Offset = 1;
            grayBottomGradient.Color = Color.FromRgb(128, 128, 128);
            grayBottomGradient.Offset = 0;

            brushGreen.GradientStops.Add(whiteGradient);
            brushGreen.GradientStops.Add(greenTopGradient);
            brushGreen.GradientStops.Add(greenBottomGradient);

            brushRed.GradientStops.Add(whiteGradient);
            brushRed.GradientStops.Add(redTopGradient);
            brushRed.GradientStops.Add(redBottomGradient);

            brushGray.GradientStops.Add(grayTopGradient);
            brushGray.GradientStops.Add(whiteGradient);
            brushGray.GradientStops.Add(grayBottomGradient);
            #endregion

            resultsValues = new DataValueCollection();
            oSQLServer = new SQLDatabase();

            cancelGetDateTask = new CancellationTokenSource();
            _ctGetDateTask = cancelGetDateTask.Token;
            getTimeTask = Task.Run(new Action(() => { GetDate(_ctGetDateTask); }));

            cancelGetStatusTask = new CancellationTokenSource();
            _ctGetStatusTask = cancelGetStatusTask.Token;
            opcUaGetStatusTask = Task.Run(/*new Action(*/() => { GetOpcUAState(_ctGetStatusTask); }/*)*/);

            ReadMachineConfigFile();

            #region Machine Labels
            dspServer.Text = cSVfile[(int)CSV_MachConfig.SQLServerName];
            dspDatabase.Text = cSVfile[(int)CSV_MachConfig.SQLDBName];
            dspMchnNo.Text = cSVfile[(int)CSV_MachConfig.MachineNumber];
            dspMchnName.Text = cSVfile[(int)CSV_MachConfig.MachineName];
            dspEqmntID.Text = cSVfile[(int)CSV_MachConfig.EquipmentID];
            dspMchnID.Text = cSVfile[(int)CSV_MachConfig.StationID];
            dspChckID.Text = cSVfile[(int)CSV_MachConfig.AssignChkCompID];
            #endregion

            #region SQL Database settings

            #region Machine
            //Station name
            oSQLServer.Station = cSVfile[(int)CSV_MachConfig.StationName];
            //Station ID
            oSQLServer.StationID = Convert.ToInt64(cSVfile[(int)CSV_MachConfig.StationID]);
            //Equipment ID
            oSQLServer.EquipmentID = Convert.ToInt64(cSVfile[(int)CSV_MachConfig.EquipmentID]);
            //Server name
            oSQLServer.Server = cSVfile[(int)CSV_MachConfig.SQLServerName];
            //Database name
            oSQLServer.DataBase = cSVfile[(int)CSV_MachConfig.SQLDBName];
            //User
            oSQLServer.SQLServerUser = cSVfile[(int)CSV_MachConfig.SQLUser];
            //App Role User
            oSQLServer.SQLqueryUser = cSVfile[(int)CSV_MachConfig.AppRoleUser];
            //Device ID table 
            oSQLServer.TblDeviceIDs = cSVfile[(int)CSV_MachConfig.DeviceIDTable];
            //Device Family 
            oSQLServer.DeviceFamily = cSVfile[(int)CSV_MachConfig.DeviceFamily];
            //Assign & Check Component ID
            oSQLServer.AssignChkCmpID = cSVfile[(int)CSV_MachConfig.AssignChkCompID];

            #region Common
            //Prior Op Check table
            oSQLServer.TblPriorOpCheck = cSVfile[(int)CSV_MachConfig.PriorOpCheck];
            //Update Device Status table
            oSQLServer.TblUpdateDeviceStatus = cSVfile[(int)CSV_MachConfig.UpdateDeviceStatus];
            //Check Components
            oSQLServer.TblCheckComponents = cSVfile[(int)CSV_MachConfig.CheckComponents];
            //Assign Component to Serial ID
            oSQLServer.TblAssignComponentsToSerial = cSVfile[(int)CSV_MachConfig.AssignComponents];
            //Check Components
            oSQLServer.TblScanComponents = cSVfile[(int)CSV_MachConfig.ScanComponent];
            //Assign Component to Serial ID
            oSQLServer.TblRequiredComponents = cSVfile[(int)CSV_MachConfig.RequiredComponents];
            #endregion

            #region Components Traceability
            //Station name
            oSQLServer.OScanCompQueries.Station = cSVfile[(int)CSV_MachConfig.ScanCompStationName];
            //Station ID
            oSQLServer.OScanCompQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.ScanCompStationID]);
            //Equipment ID
            oSQLServer.OScanCompQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.ScanCompEquipmentID]);
            #endregion

            #region Install Terminal Shields
            //Station name
            oSQLServer.OInstallTermShldQueries.Station = cSVfile[(int)CSV_MachConfig.InstallTermShldStationName];
            //Station ID
            oSQLServer.OInstallTermShldQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallTermShldStationID]);
            //Equipment ID
            oSQLServer.OInstallTermShldQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallTermShldEquipmentID]);
            //Limits table
            oSQLServer.OInstallTermShldQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Heating Results History
            oSQLServer.OInstallTermShldQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.InstallTermShldHistory];
            //Press Inner Can Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OInstallTermShldLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallTermShldMasterSamples]);
            #endregion

            #region Install M8 Inserts
            //Station name
            oSQLServer.OInstallM8InsrQueries.Station = cSVfile[(int)CSV_MachConfig.InstallM8InsrStationName];
            //Station ID
            oSQLServer.OInstallM8InsrQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallM8InsrStationID]);
            //Equipment ID
            oSQLServer.OInstallM8InsrQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallM8InsrEquipmentID]);
            //Limits table
            oSQLServer.OInstallM8InsrQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Press Inner Can Results History
            oSQLServer.OInstallM8InsrQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.InstallM8InsrResultsHistory];
            //Press Inner Can Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OInstallM8InsrLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallM8InsrMasterSamples]);
            #endregion

            #region Plunger & Armature Assy
            //Station name
            oSQLServer.OPlugrArmAssyQueries.Station = cSVfile[(int)CSV_MachConfig.PlugrArmAssyStationName];
            //Station ID
            oSQLServer.OPlugrArmAssyQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.PlugrArmAssyStationID]);
            //Equipment ID
            oSQLServer.OPlugrArmAssyQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.PlugrArmAssyEquipmentID]);
            //Limits table
            oSQLServer.OPlugrArmAssyQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Gross Leak Test Results History
            oSQLServer.OPlugrArmAssyQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.PlugrArmAssyResultsHistory];
            //Gross Leak Test Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OPlugrArmAssyLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.PlugrArmAssyMasterSamples]);
            #endregion

            #region Latch Assy
            //Station name
            oSQLServer.OLatchAssyQueries.Station = cSVfile[(int)CSV_MachConfig.LatchAssyStationName];
            //Station ID
            oSQLServer.OLatchAssyQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.LatchAssyStationID]);
            //Equipment ID
            oSQLServer.OLatchAssyQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.LatchAssyEquipmentID]);
            //Limits table
            oSQLServer.OLatchAssyQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Gross Leak Test Results History
            oSQLServer.OLatchAssyQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.LatchAssyResultsHistory];
            //Gross Leak Test Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OLatchAssyLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.LatchAssyMasterSamples]);
            #endregion

            #endregion

            #region Components Traceability
            //Station name
            oSQLServer.OScanCompQueries.Station = cSVfile[(int)CSV_MachConfig.ScanCompStationName];
            //Station ID
            oSQLServer.OScanCompQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.ScanCompStationID]);
            //Equipment ID
            oSQLServer.OScanCompQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.ScanCompEquipmentID]);
            #endregion

            #region Install Terminal Shields
            //Station name
            oSQLServer.OInstallTermShldQueries.Station = cSVfile[(int)CSV_MachConfig.InstallTermShldStationName];
            //Station ID
            oSQLServer.OInstallTermShldQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallTermShldStationID]);
            //Equipment ID
            oSQLServer.OInstallTermShldQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallTermShldEquipmentID]);
            //Limits table
            oSQLServer.OInstallTermShldQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Heating Results History
            oSQLServer.OInstallTermShldQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.InstallTermShldHistory];
            //Press Inner Can Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OInstallTermShldLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallTermShldMasterSamples]);
            #endregion

            #region Install M8 Inserts
            //Station name
            oSQLServer.OInstallM8InsrQueries.Station = cSVfile[(int)CSV_MachConfig.InstallM8InsrStationName];
            //Station ID
            oSQLServer.OInstallM8InsrQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallM8InsrStationID]);
            //Equipment ID
            oSQLServer.OInstallM8InsrQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallM8InsrEquipmentID]);
            //Limits table
            oSQLServer.OInstallM8InsrQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Press Inner Can Results History
            oSQLServer.OInstallM8InsrQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.InstallM8InsrResultsHistory];
            //Press Inner Can Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OInstallM8InsrLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.InstallM8InsrMasterSamples]);
            #endregion

            #region Plunger & Armature Assy
            //Station name
            oSQLServer.OPlugrArmAssyQueries.Station = cSVfile[(int)CSV_MachConfig.PlugrArmAssyStationName];
            //Station ID
            oSQLServer.OPlugrArmAssyQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.PlugrArmAssyStationID]);
            //Equipment ID
            oSQLServer.OPlugrArmAssyQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.PlugrArmAssyEquipmentID]);
            //Limits table
            oSQLServer.OPlugrArmAssyQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Gross Leak Test Results History
            oSQLServer.OPlugrArmAssyQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.PlugrArmAssyResultsHistory];
            //Gross Leak Test Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OPlugrArmAssyLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.PlugrArmAssyMasterSamples]);
            #endregion

            #region Latch Assy
            //Station name
            oSQLServer.OLatchAssyQueries.Station = cSVfile[(int)CSV_MachConfig.LatchAssyStationName];
            //Station ID
            oSQLServer.OLatchAssyQueries.StationID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.LatchAssyStationID]);
            //Equipment ID
            oSQLServer.OLatchAssyQueries.EquipmentID = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.LatchAssyEquipmentID]);
            //Limits table
            oSQLServer.OLatchAssyQueries.TblLimits = cSVfile[(int)CSV_MachConfig.LimitsOperParams];
            //Gross Leak Test Results History
            oSQLServer.OLatchAssyQueries.TblResultsHistory = cSVfile[(int)CSV_MachConfig.LatchAssyResultsHistory];
            //Gross Leak Test Master Parts Sequence View
            oSQLServer.TblMastersPartsSeqView = cSVfile[(int)CSV_MachConfig.MasterSequenceView];
            //Master Samples Quantity
            oSQLServer.OLatchAssyLimits.MasterSampleQty = Convert.ToInt32(cSVfile[(int)CSV_MachConfig.LatchAssyMasterSamples]);
            #endregion

            #endregion

            InitDatabaseTables();
            FillOutDatabaseTables();

            getDevieIDsTask = Task.Run(new Action(() => { GetDeviceIDs();}));

            #region Results
            //Install Terminal Shields
            ResultsInstallTermShld();
            //Insta M8 Inserts
            ResultsInstallNutIns();
            //Plunger & Armature Assy
            ResultsPlugrArmAssy();
            //Latch Assy
            ResultsLatchAssys();
            #endregion

            #region Prior Op Check
            //Install Terminal Shields
            PriorOPInstallTermShld();
            //Insta M8 Inserts
            PriorOPInstallNutIns();
            //Plunger & Armature Assy
            PriorOPPlugrArmAssy();
            //Latch Assy
            PriorOPLatchAssy();
            #endregion

        }

        #region Private fields

        MarkInfo markInfo;

        #region Product info
        private string deviceID;
        public string DeviceID
        {
            set { deviceID = value; }
            get { return deviceID; }
        }
        private string lotID;
        public string LotID
        {
            set { lotID = value; }
            get { return lotID; }
        }
        private int qty;
        public int Qty
        {
            set { qty = value; }
            get { return qty; }
        }
        #endregion

        #region Database connection

        private SQLDatabase oSQLServer;
        private bool newLotSucc;
        //Common
        DataTable oDBtableCommon = new DataTable();
        //Component Traceability
        DataTable oDBtableScanComp = new DataTable();
        DataTable oTblComponentTraceability = new DataTable();
        //Install Terminal Shields
        DataTable oDBtableInstallTermShld = new DataTable();
        DataTable oLimitsTblInstallTermShld = new DataTable();
        //Install M8 Inserts
        DataTable oDBtableInstallNutInsr = new DataTable();
        DataTable oLimitsTblInstallNutInsr = new DataTable();
        //Plunger & Armature Assy
        DataTable oDBtablePlungrArmAssy = new DataTable();
        DataTable oLimitsTblPlungrArmAssy = new DataTable();
        //Latch Assy
        DataTable oDBtableLatchAssy = new DataTable();
        DataTable oLimitsTblLatchAssy = new DataTable();

        #endregion

        #region CSV File machine settings
        public string pathProdData = "MachineData/MachineConfig.csv";
        //Machine config file 1 - 51
        private const int maxCSVfields = 51;
        private static string[] cSVfile = new string[maxCSVfields];
        public string[] CSVfile { get { return cSVfile; } set { cSVfile = value; } }
        #endregion

        static string[] mastersSeqMsg = new string[5];
        public string[] MastersSeqMsg
        {
            get
            {
                return mastersSeqMsg;
            }

            set
            {
                mastersSeqMsg = value;
            }
        }
        string[] MasterTargetSerial = { "00000", "00000", "00000" };

        #region Results
        //Install Terminal Shields
        public DataTable oTblResultsInstallTermShld = new DataTable();
        //Install M8 Inserts
        public DataTable oTblResultsInstallNutInsr = new DataTable();
        //Plunger & Armature Assy
        public DataTable oTblResultsPlungrArmAssy = new DataTable();
        //Latch Assy
        public DataTable oTblResultsLatchAssy = new DataTable();
        #endregion

        #region Prior Op Check
        //Install Terminal Shields
        public DataTable oTblPriorOPInstallTermShld = new DataTable();
        //Install M8 Inserts
        public DataTable oTblPriorOPInstallNutInsr = new DataTable();
        //Plunger & Armature Assy
        public DataTable oTblPriorOPPlungrArmAssy = new DataTable();
        //Latch Assy
        public DataTable oTblPriorOPLatchAssy = new DataTable();
        #endregion

        #region Opc UA fields
        private OpcUaClient opcUaClient;
        private ReadValueIdCollection nodesToRead;
        public DataValueCollection resultsValues;
        private DataValueCollection tempResultValues;
        private bool autoAccept = false;
        private static bool sessionConnected = false;
        private bool disconnectCmd;
        private string userName = null;
        private string userPassword = null;
        private bool renewedCertificate = false;
        private string password = null;
        private int timeout = Timeout.Infinite;
        private string applicationName = "GigavacFuseApp";
        private string configSectionName = "GigavacFuseApp";
        private Action<IList, IList> validateResponse;
        private object opcLock = new object();

        private WriteValueCollection boolToWrite = new WriteValueCollection();
        private WriteValueCollection stringToWrite = new WriteValueCollection();
        private WriteValueCollection realToWrite = new WriteValueCollection();
        #endregion

        #region Tasks control
        private Task getTimeTask;
        private CancellationTokenSource cancelGetDateTask;
        private CancellationToken _ctGetDateTask;

        private Task getDevieIDsTask;

        private Task opcUaGetStatusTask;
        private CancellationTokenSource cancelGetStatusTask;
        private CancellationToken _ctGetStatusTask;

        private Task opcUaGetVarsTask;
        private CancellationTokenSource cancelGetVarsTask;
        private CancellationToken _ctGetVarsTask;

        private Task OpcUaMonitor;
        private Task TasksMonitor;
        #endregion

        #region Colors
        private GradientStop whiteGradient = new GradientStop();
        private GradientStop greenTopGradient = new GradientStop();
        private GradientStop greenBottomGradient = new GradientStop();
        private GradientStop redTopGradient = new GradientStop();
        private GradientStop redBottomGradient = new GradientStop();
        private GradientStop grayTopGradient = new GradientStop();
        private GradientStop grayBottomGradient = new GradientStop();
        private LinearGradientBrush brushGreen = new LinearGradientBrush() { EndPoint = new Point(0.5,1), StartPoint = new Point(0.5,0) };
        private LinearGradientBrush brushRed = new LinearGradientBrush() { EndPoint = new Point(0.5, 1), StartPoint = new Point(0.5, 0) };
        private LinearGradientBrush brushGray = new LinearGradientBrush() { EndPoint = new Point(0.5, 1), StartPoint = new Point(0.5, 0) };
        #endregion

        #endregion

        #region Async methods

        private void GetDate(CancellationToken _ct)
        {
            while (!_ct.IsCancellationRequested)
            {
                try
                {
                    DateTime dateTime = DateTime.Now;
                    dspDate.Dispatcher.Invoke(() => { dspDate.Text = dateTime.ToString(); });
                } catch (Exception) { }
            }
            return;
        }
        private void GetDeviceIDs()
        {
            if(oSQLServer != null)
            {
                if (oSQLServer.DeviceIDs(oSQLServer.DeviceFamily))
                {
                    stpDBMsg.Dispatcher.Invoke(() => 
                    {
                        stpDBMsg.Children.Add(new TextBlock() 
                        {
                            Text = ($"{DateTime.Now}-----Device IDs ready."), 
                            Foreground = Brushes.Black 
                        });
                    });
                }
                else
                {
                    stpDBMsg.Dispatcher.Invoke(() => {
                        stpDBMsg.Children.Add(new TextBlock()
                        {
                            Text = ($"{DateTime.UtcNow}-----Device IDs can not be read."),
                            Foreground = Brushes.DarkRed
                        });
                    });
                }
            }
        }
        private async Task InitOpcUa()
        {
            try
            {
                Uri serverUrl = new Uri("opc.tcp://192.168.250.111:4840");
                dspUri.Dispatcher.Invoke(() => { dspUri.Text = serverUrl.AbsoluteUri; });
                CertificatePasswordProvider passwordProvider = new CertificatePasswordProvider(password);
                var config = new ApplicationConfiguration()
                {
                    ApplicationName = "GigavacFuseApp",
                    ApplicationUri = Utils.Format(@"urn:{0}:GigavacFuseApp", System.Net.Dns.GetHostName()),
                    ApplicationType = ApplicationType.Client,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier
                        {
                            StoreType = @"Directory",
                            StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault",
                            SubjectName = "OPCUAClient_Test"
                        },
                        TrustedIssuerCertificates = new CertificateTrustList
                        {
                            StoreType = @"Directory",
                            StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities"
                        },
                        TrustedPeerCertificates = new CertificateTrustList
                        {
                            StoreType = @"Directory",
                            StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications"
                        },
                        RejectedCertificateStore = new CertificateTrustList
                        {
                            StoreType = @"Directory",
                            StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
                        },
                        AutoAcceptUntrustedCertificates = true
                    },
                    TransportConfigurations = new TransportConfigurationCollection(),
                    TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                    ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                    TraceConfiguration = new TraceConfiguration()
                };

                config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
                }

                ApplicationInstance application = new ApplicationInstance
                {
                    ApplicationName = applicationName,
                    ApplicationType = ApplicationType.Client,
                    ConfigSectionName = configSectionName,
                    ApplicationConfiguration = config
                };

                if (renewedCertificate)
                {
                    await application.DeleteApplicationInstanceCertificate().ConfigureAwait(false);
                }

                bool haveAppCertificated = await application.CheckApplicationInstanceCertificate(false, minimumKeySize: 0).ConfigureAwait(false);
                if (!haveAppCertificated)
                {
                    throw new ApplicationException("ExitError");
                }

                #region Client connection
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                CancellationToken _ct = tokenSource.Token;
                OpcUaMonitor = new Task(new Action(() => { OpcUaReader(_ct); }));
                using (opcUaClient = new OpcUaClient(stpOpcUaLog, application.ApplicationConfiguration,
                        ClientBase.ValidateResponse)
                { AutoAccept = autoAccept })
                {
                    if (!String.IsNullOrEmpty(userName))
                    {
                        opcUaClient.UserIdentity = new UserIdentity(userName, userPassword ?? string.Empty);
                    }
                    do
                    {
                        if (!sessionConnected)
                        {
                            bool connected = await opcUaClient.ConnectAsync(serverUrl.ToString(), false);
                            if (connected/*opcUaClient.Session.Connected*/)
                            {
                                stpOpcUaLog.Dispatcher.Invoke(() => {
                                    stpOpcUaLog.Children.Add(new TextBlock() { Text = $"{DateTime.Now}-----Connected", Foreground = Brushes.DarkGreen });
                                });
                                OpcUaMonitor.Start();
                                TasksMonitor = Task.Run(() => { TasksMonitorMethod(_ct); });
                                opcUaClient.Session.TransferSubscriptionsOnReconnect = true;
                                sessionConnected = true;
                                validateResponse = ClientBase.ValidateResponse;

                                if (opcUaClient.Session == null || opcUaClient.Session.Connected == false)
                                {
                                    stpOpcUaLog.Dispatcher.Invoke(() =>
                                    {
                                        stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}-----Session not connected"), Foreground = Brushes.DarkRed });
                                    });
                                    return;
                                }
                            }
                            else { sessionConnected = false; }
                        }


                    } while (!disconnectCmd);
                    if (_ct.CanBeCanceled)
                    {
                        tokenSource.Cancel();
                        OpcUaMonitor.Wait();
                        stpOpcUaLog.Dispatcher.Invoke(() => {
                            stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}-----End monitor task"), Foreground = Brushes.Black });
                        });
                        tokenSource.Dispose();
                    }
                    opcUaClient.Disconnect();
                    sessionConnected = false;
                    stpOpcUaLog.Dispatcher.Invoke(() => {
                        stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}-----End session!") , Foreground = Brushes.Black});});
                    disconnectCmd = false;
                }
                #endregion
            }
            catch
            {

            }
        }
        private async void VarOpcUaMonitor(CancellationToken token)
        {
            bool oneShot0 = false;
            bool oneShot1 = false;
            bool oneShot2 = false;
            bool oneShot3 = false;
            bool oneShot4 = false;
            bool oneShot5 = false;
            bool oneShot6 = false;
            bool oneShot7 = false;

            try
            {   
                while (!token.IsCancellationRequested)
                {
                    if (sessionConnected && resultsValues != null && resultsValues.Count > 0)
                    {
                        dspDeviceID.Dispatcher.Invoke(() => { dspDeviceID.Text = (string)resultsValues[0].Value; });
                        dspLotID.Dispatcher.Invoke(() => { dspLotID.Text = (string)resultsValues[1].Value; });
                        dspQty.Dispatcher.Invoke(() => { dspQty.Text = (string)resultsValues[2].Value; });
                        dspSelectedDvcID.Dispatcher.Invoke(() => { dspSelectedDvcID.Text = (string)resultsValues[3].Value; });
                        dspSelectedLotID.Dispatcher.Invoke(() => { dspSelectedLotID.Text = (string)resultsValues[4].Value; });
                        dspSelectedQty.Dispatcher.Invoke(() => { dspSelectedQty.Text = (string)resultsValues[5].Value; });

                        #region lamps

                        //lmpTrayPos1
                        if ((bool)resultsValues[6].Value == true)
                        {
                            lmpTrayPos1.Dispatcher.Invoke(() => { lmpTrayPos1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrayPos1.Dispatcher.Invoke(() => { lmpTrayPos1.Fill = brushGray; });
                        }

                        //lmpTrayPos2
                        if ((bool)resultsValues[7].Value == true)
                        {
                            lmpTrayPos2.Dispatcher.Invoke(() => { lmpTrayPos2.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrayPos2.Dispatcher.Invoke(() => { lmpTrayPos2.Fill = brushGray; });
                        }

                        //lmpTrayPos3
                        if ((bool)resultsValues[8].Value == true)
                        {
                            lmpTrayPos3.Dispatcher.Invoke(() => { lmpTrayPos3.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrayPos3.Dispatcher.Invoke(() => { lmpTrayPos3.Fill = brushGray; });
                        }

                        //lmpStoper1In
                        if ((bool)resultsValues[9].Value == true){lmpStopr1Opn.Dispatcher.Invoke(() => { lmpStopr1Opn.Fill = brushGreen; });}
                        else{lmpStopr1Opn.Dispatcher.Invoke(() => { lmpStopr1Opn.Fill = brushGray; });}

                        //lmpStoper2In
                        if ((bool)resultsValues[10].Value == true){lmpStopr2Opn.Dispatcher.Invoke(() => { lmpStopr2Opn.Fill = brushGreen; });}
                        else{lmpStopr2Opn.Dispatcher.Invoke(() => { lmpStopr2Opn.Fill = brushGray; });}

                        //lmpHoldAOpn
                        if ((bool)resultsValues[11].Value == true)
                        {
                            lmpHoldAOpn.Dispatcher.Invoke(() => { lmpHoldAOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpHoldAOpn.Dispatcher.Invoke(() => { lmpHoldAOpn.Fill = brushGray; });
                        }

                        //lmpHoldACls
                        if ((bool)resultsValues[12].Value == true)
                        {
                            lmpHoldACls.Dispatcher.Invoke(() => { lmpHoldACls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpHoldACls.Dispatcher.Invoke(() => { lmpHoldACls.Fill = brushGray; });
                        }

                        //lmpHoldBOpn
                        if ((bool)resultsValues[13].Value == true)
                        {
                            lmpHoldBOpn.Dispatcher.Invoke(() => { lmpHoldBOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpHoldBOpn.Dispatcher.Invoke(() => { lmpHoldBOpn.Fill = brushGray; });
                        }

                        //lmpHoldBCls
                        if ((bool)resultsValues[14].Value == true)
                        {
                            lmpHoldBCls.Dispatcher.Invoke(() => { lmpHoldBCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpHoldBCls.Dispatcher.Invoke(() => { lmpHoldBCls.Fill = brushGray; });
                        }

                        //lmpHoldCOpn
                        if ((bool)resultsValues[15].Value == true)
                        {
                            lmpHoldCOpn.Dispatcher.Invoke(() => { lmpHoldCOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpHoldCOpn.Dispatcher.Invoke(() => { lmpHoldCOpn.Fill = brushGray; });
                        }

                        //lmpHoldCCls
                        if ((bool)resultsValues[16].Value == true)
                        {
                            lmpHoldCCls.Dispatcher.Invoke(() => { lmpHoldCCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpHoldCCls.Dispatcher.Invoke(() => { lmpHoldCCls.Fill = brushGray; });
                        }

                        //lmpEjectOpn
                        if ((bool)resultsValues[17].Value == true)
                        {
                            lmpEjectOpn.Dispatcher.Invoke(() => { lmpEjectOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpEjectOpn.Dispatcher.Invoke(() => { lmpEjectOpn.Fill = brushGray; });
                        }

                        //lmpEjectCls
                        if ((bool)resultsValues[18].Value == true)
                        {
                            lmpEjectCls.Dispatcher.Invoke(() => { lmpEjectCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpEjectCls.Dispatcher.Invoke(() => { lmpEjectCls.Fill = brushGray; });
                        }

                        //lmpCheckSide
                        if ((bool)resultsValues[19].Value == true)
                        {
                            lmpCheckSide.Dispatcher.Invoke(() => { lmpCheckSide.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpCheckSide.Dispatcher.Invoke(() => { lmpCheckSide.Fill = brushGray; });
                        }

                        //lmpRbt1GrpCls
                        if ((bool)resultsValues[20].Value == true)
                        {
                            lmpRbt1GrpCls.Dispatcher.Invoke(() => { lmpRbt1GrpCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbt1GrpCls.Dispatcher.Invoke(() => { lmpRbt1GrpCls.Fill = brushGray; });
                        }

                        //lmpRbt1GrpOpn
                        if ((bool)resultsValues[21].Value == true)
                        {
                            lmpRbt1GrpOpn.Dispatcher.Invoke(() => { lmpRbt1GrpOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbt1GrpOpn.Dispatcher.Invoke(() => { lmpRbt1GrpOpn.Fill = brushGray; });
                        }

                        //lmpRbtToolA
                        if ((bool)resultsValues[22].Value == true)
                        {
                            lmpRbtToolA.Dispatcher.Invoke(() => { lmpRbtToolA.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbtToolA.Dispatcher.Invoke(() => { lmpRbtToolA.Fill = brushGray; });
                        }

                        //lmpRbtToolB
                        if ((bool)resultsValues[23].Value == true)
                        {
                            lmpRbtToolB.Dispatcher.Invoke(() => { lmpRbtToolB.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbtToolB.Dispatcher.Invoke(() => { lmpRbtToolB.Fill = brushGray; });
                        }

                        //lmpRbtToolC
                        if ((bool)resultsValues[24].Value == true)
                        {
                            lmpRbtToolC.Dispatcher.Invoke(() => { lmpRbtToolC.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbtToolC.Dispatcher.Invoke(() => { lmpRbtToolC.Fill = brushGray; });
                        }

                        //lmpRbt1Pick
                        if ((bool)resultsValues[25].Value == true)
                        {
                            lmpRbt1Pick.Dispatcher.Invoke(() => { lmpRbt1Pick.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbt1Pick.Dispatcher.Invoke(() => { lmpRbt1Pick.Fill = brushGray; });
                        }

                        //lmpS1GrpCls
                        if ((bool)resultsValues[26].Value == true)
                        {
                            lmpS1GrpCls.Dispatcher.Invoke(() => { lmpS1GrpCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS1GrpCls.Dispatcher.Invoke(() => { lmpS1GrpCls.Fill = brushGray; });
                        }

                        //lmpS1GrpOpn
                        if ((bool)resultsValues[27].Value == true)
                        {
                            lmpS1GrpOpn.Dispatcher.Invoke(() => { lmpS1GrpOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS1GrpOpn.Dispatcher.Invoke(() => { lmpS1GrpOpn.Fill = brushGray; });
                        }

                        //lmpS2GrpCls
                        if ((bool)resultsValues[28].Value == true){lmpS2GrpCls.Dispatcher.Invoke(() => { lmpS2GrpCls.Fill = brushGreen; });}
                        else{lmpS2GrpCls.Dispatcher.Invoke(() => { lmpS2GrpCls.Fill = brushGray; });}

                        //lmpS2GrpOpn
                        if ((bool)resultsValues[29].Value == true)
                        {
                            lmpS2GrpOpn.Dispatcher.Invoke(() => { lmpS2GrpOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS2GrpOpn.Dispatcher.Invoke(() => { lmpS2GrpOpn.Fill = brushGray; });
                        }

                        //lmpT1LoadOpn
                        if ((bool)resultsValues[30].Value == true)
                        {
                            lmpT1LoadOpn.Dispatcher.Invoke(() => { lmpT1LoadOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT1LoadOpn.Dispatcher.Invoke(() => { lmpT1LoadOpn.Fill = brushGray; });
                        }

                        //lmpT1LoadCls
                        if ((bool)resultsValues[31].Value == true)
                        {
                            lmpT1LoadCls.Dispatcher.Invoke(() => { lmpT1LoadCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT1LoadCls.Dispatcher.Invoke(() => { lmpT1LoadCls.Fill = brushGray; });
                        }

                        //lmpT1LimitCls
                        if ((bool)resultsValues[32].Value == true)
                        {
                            lmpT1LimitCls.Dispatcher.Invoke(() => { lmpT1LimitCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT1LimitCls.Dispatcher.Invoke(() => { lmpT1LimitCls.Fill = brushGray; });
                        }

                        //lmpT1LimitOpn
                        if ((bool)resultsValues[33].Value == true)
                        {
                            lmpT1LimitOpn.Dispatcher.Invoke(() => { lmpT1LimitOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT1LimitOpn.Dispatcher.Invoke(() => { lmpT1LimitOpn.Fill = brushGray; });
                        }

                        //lmpStopr1Cls
                        if ((bool)resultsValues[34].Value == true)
                        {
                            lmpStopr1Cls.Dispatcher.Invoke(() => { lmpStopr1Cls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpStopr1Cls.Dispatcher.Invoke(() => { lmpStopr1Cls.Fill = brushGray; });
                        }

                        //lmpStopr2Cls
                        if ((bool)resultsValues[35].Value == true)
                        {
                            lmpStopr2Cls.Dispatcher.Invoke(() => { lmpStopr2Cls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpStopr2Cls.Dispatcher.Invoke(() => { lmpStopr2Cls.Fill = brushGray; });
                        }

                        //lmpNest0
                        if ((bool)resultsValues[36].Value == true)
                        {
                            lmpNest0.Dispatcher.Invoke(() => { lmpNest0.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpNest0.Dispatcher.Invoke(() => { lmpNest0.Fill = brushGray; });
                        }

                        //lmpNest2
                        if ((bool)resultsValues[37].Value == true)
                        {
                            lmpNest2.Dispatcher.Invoke(() => { lmpNest2.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpNest2.Dispatcher.Invoke(() => { lmpNest2.Fill = brushGray; });
                        }

                        //lmpNest1
                        if ((bool)resultsValues[38].Value == true)
                        {
                            lmpNest1.Dispatcher.Invoke(() => { lmpNest1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpNest1.Dispatcher.Invoke(() => { lmpNest1.Fill = brushGray; });
                        }

                        //lmpS3GrpCls
                        if ((bool)resultsValues[39].Value == true)
                        {
                            lmpS3GrpCls.Dispatcher.Invoke(() => { lmpS3GrpCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS3GrpCls.Dispatcher.Invoke(() => { lmpS3GrpCls.Fill = brushGray; });
                        }

                        //lmpS3GrpOpn
                        if ((bool)resultsValues[40].Value == true)
                        {
                            lmpS3GrpOpn.Dispatcher.Invoke(() => { lmpS3GrpOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS3GrpOpn.Dispatcher.Invoke(() => { lmpS3GrpOpn.Fill = brushGray; });
                        }

                        //lmpS3LoadOpn
                        if ((bool)resultsValues[41].Value == true)
                        {
                            lmpS3LoadOpn.Dispatcher.Invoke(() => { lmpS3LoadOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS3LoadOpn.Dispatcher.Invoke(() => { lmpS3LoadOpn.Fill = brushGray; });
                        }

                        //lmpS3LoadCls
                        if ((bool)resultsValues[42].Value == true)
                        {
                            lmpS3LoadCls.Dispatcher.Invoke(() => { lmpS3LoadCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS3LoadCls.Dispatcher.Invoke(() => { lmpS3LoadCls.Fill = brushGray; });
                        }

                        //lmpS3GyreOpn
                        if ((bool)resultsValues[43].Value == true)
                        {
                            lmpS3GyreOpn.Dispatcher.Invoke(() => { lmpS3GyreOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS3GyreOpn.Dispatcher.Invoke(() => { lmpS3GyreOpn.Fill = brushGray; });
                        }

                        //lmpS3GyreCls
                        if ((bool)resultsValues[44].Value == true)
                        {
                            lmpS3GyreCls.Dispatcher.Invoke(() => { lmpS3GyreCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS3GyreCls.Dispatcher.Invoke(() => { lmpS3GyreCls.Fill = brushGray; });
                        }

                        //lmpS4GrpNCls
                        if ((bool)resultsValues[45].Value == true)
                        {
                            lmpS4GrpNCls.Dispatcher.Invoke(() => { lmpS4GrpNCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpNCls.Dispatcher.Invoke(() => { lmpS4GrpNCls.Fill = brushGray; });
                        }

                        //lmpS4GrpNOpn
                        if ((bool)resultsValues[46].Value == true)
                        {
                            lmpS4GrpNOpn.Dispatcher.Invoke(() => { lmpS4GrpNOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpNOpn.Dispatcher.Invoke(() => { lmpS4GrpNOpn.Fill = brushGray; });
                        }

                        //lmpS4GrpRCls
                        if ((bool)resultsValues[47].Value == true)
                        {
                            lmpS4GrpRCls.Dispatcher.Invoke(() => { lmpS4GrpRCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpRCls.Dispatcher.Invoke(() => { lmpS4GrpRCls.Fill = brushGray; });
                        }

                        //lmpS4GrpROpn
                        if ((bool)resultsValues[48].Value == true)
                        {
                            lmpS4GrpROpn.Dispatcher.Invoke(() => { lmpS4GrpROpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpROpn.Dispatcher.Invoke(() => { lmpS4GrpROpn.Fill = brushGray; });
                        }

                        //lmpS4GrpLCls
                        if ((bool)resultsValues[49].Value == true)
                        {
                            lmpS4GrpLCls.Dispatcher.Invoke(() => { lmpS4GrpLCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpLCls.Dispatcher.Invoke(() => { lmpS4GrpLCls.Fill = brushGray; });
                        }

                        //lmpS4GrpLOpn
                        if ((bool)resultsValues[50].Value == true)
                        {
                            lmpS4GrpLOpn.Dispatcher.Invoke(() => { lmpS4GrpLOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpLOpn.Dispatcher.Invoke(() => { lmpS4GrpLOpn.Fill = brushGray; });
                        }

                        //lmpS4GyreOpn
                        if ((bool)resultsValues[51].Value == true)
                        {
                            lmpS4GyreOpn.Dispatcher.Invoke(() => { lmpS4GyreOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GyreOpn.Dispatcher.Invoke(() => { lmpS4GyreOpn.Fill = brushGray; });
                        }

                        //lmpS4GyreCls
                        if ((bool)resultsValues[52].Value == true)
                        {
                            lmpS4GyreCls.Dispatcher.Invoke(() => { lmpS4GyreCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GyreCls.Dispatcher.Invoke(() => { lmpS4GyreCls.Fill = brushGray; });
                        }

                        //lmpS4UpDwnOpn
                        if ((bool)resultsValues[53].Value == true)
                        {
                            lmpS4UpDwnOpn.Dispatcher.Invoke(() => { lmpS4UpDwnOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4UpDwnOpn.Dispatcher.Invoke(() => { lmpS4UpDwnOpn.Fill = brushGray; });
                        }

                        //lmpS4UpDwnCls
                        if ((bool)resultsValues[54].Value == true)
                        {
                            lmpS4UpDwnCls.Dispatcher.Invoke(() => { lmpS4UpDwnCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4UpDwnCls.Dispatcher.Invoke(() => { lmpS4UpDwnCls.Fill = brushGray; });
                        }

                        //lmpS4TransOpn
                        if ((bool)resultsValues[55].Value == true)
                        {
                            lmpS4TransOpn.Dispatcher.Invoke(() => { lmpS4TransOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4TransOpn.Dispatcher.Invoke(() => { lmpS4TransOpn.Fill = brushGray; });
                        }

                        //lmpS4TransCls
                        if ((bool)resultsValues[56].Value == true)
                        {
                            lmpS4TransCls.Dispatcher.Invoke(() => { lmpS4TransCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4TransCls.Dispatcher.Invoke(() => { lmpS4TransCls.Fill = brushGray; });
                        }

                        //lmpS4LoadOpn
                        if ((bool)resultsValues[57].Value == true)
                        {
                            lmpS4LoadOpn.Dispatcher.Invoke(() => { lmpS4LoadOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4LoadOpn.Dispatcher.Invoke(() => { lmpS4LoadOpn.Fill = brushGray; });
                        }

                        //lmpS4LoadCls
                        if ((bool)resultsValues[58].Value == true)
                        {
                            lmpS4LoadCls.Dispatcher.Invoke(() => { lmpS4LoadCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4LoadCls.Dispatcher.Invoke(() => { lmpS4LoadCls.Fill = brushGray; });
                        }

                        //lmpS4GrpTCls
                        if ((bool)resultsValues[59].Value == true)
                        {
                            lmpS4GrpTCls.Dispatcher.Invoke(() => { lmpS4GrpTCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpTCls.Dispatcher.Invoke(() => { lmpS4GrpTCls.Fill = brushGray; });
                        }

                        //lmpS4GrpTOpn
                        if ((bool)resultsValues[60].Value == true)
                        {
                            lmpS4GrpTOpn.Dispatcher.Invoke(() => { lmpS4GrpTOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4GrpTOpn.Dispatcher.Invoke(() => { lmpS4GrpTOpn.Fill = brushGray; });
                        }

                        //lmpS4ShieldR
                        if ((bool)resultsValues[61].Value == true)
                        {
                            lmpS4ShieldR.Dispatcher.Invoke(() => { lmpS4ShieldR.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4ShieldR.Dispatcher.Invoke(() => { lmpS4ShieldR.Fill = brushGray; });
                        }

                        //lmpS4ShieldL
                        if ((bool)resultsValues[62].Value == true)
                        {
                            lmpS4ShieldL.Dispatcher.Invoke(() => { lmpS4ShieldL.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS4ShieldL.Dispatcher.Invoke(() => { lmpS4ShieldL.Fill = brushGray; });
                        }

                        //lmpS5PressROpn
                        if ((bool)resultsValues[63].Value == true)
                        {
                            lmpS5PressROpn.Dispatcher.Invoke(() => { lmpS5PressROpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5PressROpn.Dispatcher.Invoke(() => { lmpS5PressROpn.Fill = brushGray; });
                        }

                        //lmpS5PressRCls
                        if ((bool)resultsValues[64].Value == true)
                        {
                            lmpS5PressRCls.Dispatcher.Invoke(() => { lmpS5PressRCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5PressRCls.Dispatcher.Invoke(() => { lmpS5PressRCls.Fill = brushGray; });
                        }

                        //lmpS5GrpTCls
                        if ((bool)resultsValues[65].Value == true)
                        {
                            lmpS5GrpTCls.Dispatcher.Invoke(() => { lmpS5GrpTCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5GrpTCls.Dispatcher.Invoke(() => { lmpS5GrpTCls.Fill = brushGray; });
                        }

                        //lmpS5GrpTOpn
                        if ((bool)resultsValues[66].Value == true)
                        {
                            lmpS5GrpTOpn.Dispatcher.Invoke(() => { lmpS5GrpTOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5GrpTOpn.Dispatcher.Invoke(() => { lmpS5GrpTOpn.Fill = brushGray; });
                        }

                        //lmpS5LoadOpn
                        if ((bool)resultsValues[67].Value == true)
                        {
                            lmpS5LoadOpn.Dispatcher.Invoke(() => { lmpS5LoadOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5LoadOpn.Dispatcher.Invoke(() => { lmpS5LoadOpn.Fill = brushGray; });
                        }

                        //lmpS5LoadCls
                        if ((bool)resultsValues[68].Value == true)
                        {
                            lmpS5LoadCls.Dispatcher.Invoke(() => { lmpS5LoadCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5LoadCls.Dispatcher.Invoke(() => { lmpS5LoadCls.Fill = brushGray; });
                        }

                        //lmpS5GyreOpn
                        if ((bool)resultsValues[69].Value == true)
                        {
                            lmpS5GyreOpn.Dispatcher.Invoke(() => { lmpS5GyreOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5GyreOpn.Dispatcher.Invoke(() => { lmpS5GyreOpn.Fill = brushGray; });
                        }

                        //lmpS5GyreCls
                        if ((bool)resultsValues[70].Value == true)
                        {
                            lmpS5GyreCls.Dispatcher.Invoke(() => { lmpS5GyreCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5GyreCls.Dispatcher.Invoke(() => { lmpS5GyreCls.Fill = brushGray; });
                        }

                        //lmpS5GrpNCls
                        if ((bool)resultsValues[71].Value == true)
                        {
                            lmpS5GrpNCls.Dispatcher.Invoke(() => { lmpS5GrpNCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5GrpNCls.Dispatcher.Invoke(() => { lmpS5GrpNCls.Fill = brushGray; });
                        }

                        //lmpS5GrpNOpn
                        if ((bool)resultsValues[72].Value == true)
                        {
                            lmpS5GrpNOpn.Dispatcher.Invoke(() => { lmpS5GrpNOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5GrpNOpn.Dispatcher.Invoke(() => { lmpS5GrpNOpn.Fill = brushGray; });
                        }

                        //lmpS6Plug
                        if ((bool)resultsValues[73].Value == true)
                        {
                            lmpS6Plug.Dispatcher.Invoke(() => { lmpS6Plug.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6Plug.Dispatcher.Invoke(() => { lmpS6Plug.Fill = brushGray; });
                        }

                        //lmpS6EjctrOpn
                        if ((bool)resultsValues[74].Value == true)
                        {
                            lmpS6EjctrOpn.Dispatcher.Invoke(() => { lmpS6EjctrOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6EjctrOpn.Dispatcher.Invoke(() => { lmpS6EjctrOpn.Fill = brushGray; });
                        }

                        //lmpS6EjctrCls
                        if ((bool)resultsValues[75].Value == true)
                        {
                            lmpS6EjctrCls.Dispatcher.Invoke(() => { lmpS6EjctrCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6EjctrCls.Dispatcher.Invoke(() => { lmpS6EjctrCls.Fill = brushGray; });
                        }

                        //lmpS6TransCls
                        if ((bool)resultsValues[76].Value == true)
                        {
                            lmpS6TransCls.Dispatcher.Invoke(() => { lmpS6TransCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6TransCls.Dispatcher.Invoke(() => { lmpS6TransCls.Fill = brushGray; });
                        }

                        //lmpS6TransOpn
                        if ((bool)resultsValues[77].Value == true)
                        {
                            lmpS6TransOpn.Dispatcher.Invoke(() => { lmpS6TransOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6TransOpn.Dispatcher.Invoke(() => { lmpS6TransOpn.Fill = brushGray; });
                        }

                        //lmpS6UpDwnOpn
                        if ((bool)resultsValues[78].Value == true)
                        {
                            lmpS6UpDwnOpn.Dispatcher.Invoke(() => { lmpS6UpDwnOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6UpDwnOpn.Dispatcher.Invoke(() => { lmpS6UpDwnOpn.Fill = brushGray; });
                        }

                        //lmpS6UpDwnCls
                        if ((bool)resultsValues[79].Value == true)
                        {
                            lmpS6UpDwnCls.Dispatcher.Invoke(() => { lmpS6UpDwnCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6UpDwnCls.Dispatcher.Invoke(() => { lmpS6UpDwnCls.Fill = brushGray; });
                        }

                        //lmpS6InsertOpn
                        if ((bool)resultsValues[80].Value == true)
                        {
                            lmpS6InsertOpn.Dispatcher.Invoke(() => { lmpS6InsertOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6InsertOpn.Dispatcher.Invoke(() => { lmpS6InsertOpn.Fill = brushGray; });
                        }

                        //lmpS6InsertCls
                        if ((bool)resultsValues[81].Value == true)
                        {
                            lmpS6InsertCls.Dispatcher.Invoke(() => { lmpS6InsertCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6InsertCls.Dispatcher.Invoke(() => { lmpS6InsertCls.Fill = brushGray; });
                        }

                        //lmpS6SuckOpn
                        if ((bool)resultsValues[82].Value == true)
                        {
                            lmpS6SuckOpn.Dispatcher.Invoke(() => { lmpS6SuckOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6SuckOpn.Dispatcher.Invoke(() => { lmpS6SuckOpn.Fill = brushGray; });
                        }

                        //lmpS6GrpNCls
                        if ((bool)resultsValues[83].Value == true)
                        {
                            lmpS6GrpNCls.Dispatcher.Invoke(() => { lmpS6GrpNCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6GrpNCls.Dispatcher.Invoke(() => { lmpS6GrpNCls.Fill = brushGray; });
                        }

                        //lmpS6GrpNOpn
                        if ((bool)resultsValues[84].Value == true)
                        {
                            lmpS6GrpNOpn.Dispatcher.Invoke(() => { lmpS6GrpNOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6GrpNOpn.Dispatcher.Invoke(() => { lmpS6GrpNOpn.Fill = brushGray; });
                        }

                        //lmpS6GrpTCls
                        if ((bool)resultsValues[85].Value == true)
                        {
                            lmpS6GrpTCls.Dispatcher.Invoke(() => { lmpS6GrpTCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6GrpTCls.Dispatcher.Invoke(() => { lmpS6GrpTCls.Fill = brushGray; });
                        }

                        //lmpS6GrpTOpn
                        if ((bool)resultsValues[86].Value == true)
                        {
                            lmpS6GrpTOpn.Dispatcher.Invoke(() => { lmpS6GrpTOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6GrpTOpn.Dispatcher.Invoke(() => { lmpS6GrpTOpn.Fill = brushGray; });
                        }

                        //lmpS6LoadOpn
                        if ((bool)resultsValues[87].Value == true)
                        {
                            lmpS6LoadOpn.Dispatcher.Invoke(() => { lmpS6LoadOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6LoadOpn.Dispatcher.Invoke(() => { lmpS6LoadOpn.Fill = brushGray; });
                        }

                        //lmpS6LoadCls
                        if ((bool)resultsValues[88].Value == true)
                        {
                            lmpS6LoadCls.Dispatcher.Invoke(() => { lmpS6LoadCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS6LoadCls.Dispatcher.Invoke(() => { lmpS6LoadCls.Fill = brushGray; });
                        }

                        //lmpS7GrpNCls
                        if ((bool)resultsValues[89].Value == true)
                        {
                            lmpS7GrpNCls.Dispatcher.Invoke(() => { lmpS7GrpNCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7GrpNCls.Dispatcher.Invoke(() => { lmpS7GrpNCls.Fill = brushGray; });
                        }

                        //lmpS7GrpNOpn
                        if ((bool)resultsValues[90].Value == true)
                        {
                            lmpS7GrpNOpn.Dispatcher.Invoke(() => { lmpS7GrpNOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7GrpNOpn.Dispatcher.Invoke(() => { lmpS7GrpNOpn.Fill = brushGray; });
                        }

                        //lmpS7LoadOpn
                        if ((bool)resultsValues[91].Value == true)
                        {
                            lmpS7LoadOpn.Dispatcher.Invoke(() => { lmpS7LoadOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7LoadOpn.Dispatcher.Invoke(() => { lmpS7LoadOpn.Fill = brushGray; });
                        }

                        //lmpS7LoadCls
                        if ((bool)resultsValues[92].Value == true)
                        {
                            lmpS7LoadCls.Dispatcher.Invoke(() => { lmpS7LoadCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7LoadCls.Dispatcher.Invoke(() => { lmpS7LoadCls.Fill = brushGray; });
                        }

                        //lmpS7PressOpn
                        if ((bool)resultsValues[93].Value == true)
                        {
                            lmpS7PressOpn.Dispatcher.Invoke(() => { lmpS7PressOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7PressOpn.Dispatcher.Invoke(() => { lmpS7PressOpn.Fill = brushGray; });
                        }

                        //lmpS7PressCls
                        if ((bool)resultsValues[94].Value == true)
                        {
                            lmpS7PressCls.Dispatcher.Invoke(() => { lmpS7PressCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7PressCls.Dispatcher.Invoke(() => { lmpS7PressCls.Fill = brushGray; });
                        }

                        //lmpS7PinCls
                        if ((bool)resultsValues[95].Value == true)
                        {
                            lmpS7PinCls.Dispatcher.Invoke(() => { lmpS7PinCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7PinCls.Dispatcher.Invoke(() => { lmpS7PinCls.Fill = brushGray; });
                        }

                        //lmpS7PinOpn
                        if ((bool)resultsValues[96].Value == true)
                        {
                            lmpS7PinOpn.Dispatcher.Invoke(() => { lmpS7PinOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7PinOpn.Dispatcher.Invoke(() => { lmpS7PinOpn.Fill = brushGray; });
                        }

                        //lmpS7LoadAOpn
                        if ((bool)resultsValues[97].Value == true)
                        {
                            lmpS7LoadAOpn.Dispatcher.Invoke(() => { lmpS7LoadAOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7LoadAOpn.Dispatcher.Invoke(() => { lmpS7LoadAOpn.Fill = brushGray; });
                        }

                        //lmpS7LoadACls
                        if ((bool)resultsValues[98].Value == true)
                        {
                            lmpS7LoadACls.Dispatcher.Invoke(() => { lmpS7LoadACls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7LoadACls.Dispatcher.Invoke(() => { lmpS7LoadACls.Fill = brushGray; });
                        }

                        //lmpS7LoadBOpn
                        if ((bool)resultsValues[99].Value == true)
                        {
                            lmpS7LoadBOpn.Dispatcher.Invoke(() => { lmpS7LoadBOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7LoadBOpn.Dispatcher.Invoke(() => { lmpS7LoadBOpn.Fill = brushGray; });
                        }

                        //lmpS7LoadBCls
                        if ((bool)resultsValues[100].Value == true)
                        {
                            lmpS7LoadBCls.Dispatcher.Invoke(() => { lmpS7LoadBCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7LoadBCls.Dispatcher.Invoke(() => { lmpS7LoadBCls.Fill = brushGray; });
                        }

                        //lmpS7GrpTCls
                        if ((bool)resultsValues[101].Value == true)
                        {
                            lmpS7GrpTCls.Dispatcher.Invoke(() => { lmpS7GrpTCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7GrpTCls.Dispatcher.Invoke(() => { lmpS7GrpTCls.Fill = brushGray; });
                        }

                        //lmpS7GrpTOpn
                        if ((bool)resultsValues[102].Value == true)
                        {
                            lmpS7GrpTOpn.Dispatcher.Invoke(() => { lmpS7GrpTOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7GrpTOpn.Dispatcher.Invoke(() => { lmpS7GrpTOpn.Fill = brushGray; });
                        }

                        //lmpS7Nut
                        if ((bool)resultsValues[103].Value == true)
                        {
                            lmpS7Nut.Dispatcher.Invoke(() => { lmpS7Nut.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7Nut.Dispatcher.Invoke(() => { lmpS7Nut.Fill = brushGray; });
                        }

                        //lmpS7LvlTest
                        if ((bool)resultsValues[104].Value == true)
                        {
                            lmpS7LvlTest.Dispatcher.Invoke(() => { lmpS7LvlTest.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS7LvlTest.Dispatcher.Invoke(() => { lmpS7LvlTest.Fill = brushGray; });
                        }

                        //lmpS8GrpNCls
                        if ((bool)resultsValues[105].Value == true)
                        {
                            lmpS8GrpNCls.Dispatcher.Invoke(() => { lmpS8GrpNCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8GrpNCls.Dispatcher.Invoke(() => { lmpS8GrpNCls.Fill = brushGray; });
                        }

                        //lmpS8GrpNOpn
                        if ((bool)resultsValues[106].Value == true)
                        {
                            lmpS8GrpNOpn.Dispatcher.Invoke(() => { lmpS8GrpNOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8GrpNOpn.Dispatcher.Invoke(() => { lmpS8GrpNOpn.Fill = brushGray; });
                        }

                        //lmpS8LoadOpn
                        if ((bool)resultsValues[107].Value == true)
                        {
                            lmpS8LoadOpn.Dispatcher.Invoke(() => { lmpS8LoadOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8LoadOpn.Dispatcher.Invoke(() => { lmpS8LoadOpn.Fill = brushGray; });
                        }

                        //lmpS8LoadCls
                        if ((bool)resultsValues[108].Value == true)
                        {
                            lmpS8LoadCls.Dispatcher.Invoke(() => { lmpS8LoadCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8LoadCls.Dispatcher.Invoke(() => { lmpS8LoadCls.Fill = brushGray; });
                        }

                        //lmpS8PressOpn
                        if ((bool)resultsValues[109].Value == true)
                        {
                            lmpS8PressOpn.Dispatcher.Invoke(() => { lmpS8PressOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8PressOpn.Dispatcher.Invoke(() => { lmpS8PressOpn.Fill = brushGray; });
                        }

                        //lmpS8PressCls
                        if ((bool)resultsValues[110].Value == true)
                        {
                            lmpS8PressCls.Dispatcher.Invoke(() => { lmpS8PressCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8PressCls.Dispatcher.Invoke(() => { lmpS8PressCls.Fill = brushGray; });
                        }

                        //lmpS8PinCls
                        if ((bool)resultsValues[111].Value == true)
                        {
                            lmpS8PinCls.Dispatcher.Invoke(() => { lmpS8PinCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8PinCls.Dispatcher.Invoke(() => { lmpS8PinCls.Fill = brushGray; });
                        }

                        //lmpS8PinOpn
                        if ((bool)resultsValues[112].Value == true)
                        {
                            lmpS8PinOpn.Dispatcher.Invoke(() => { lmpS8PinOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8PinOpn.Dispatcher.Invoke(() => { lmpS8PinOpn.Fill = brushGray; });
                        }

                        //lmpS8LoadAOpn
                        if ((bool)resultsValues[113].Value == true)
                        {
                            lmpS8LoadAOpn.Dispatcher.Invoke(() => { lmpS8LoadAOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8LoadAOpn.Dispatcher.Invoke(() => { lmpS8LoadAOpn.Fill = brushGray; });
                        }

                        //lmpS8LoadACls
                        if ((bool)resultsValues[114].Value == true)
                        {
                            lmpS8LoadACls.Dispatcher.Invoke(() => { lmpS8LoadACls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8LoadACls.Dispatcher.Invoke(() => { lmpS8LoadACls.Fill = brushGray; });
                        }

                        //lmpS8LoadBOpn
                        if ((bool)resultsValues[115].Value == true)
                        {
                            lmpS8LoadBOpn.Dispatcher.Invoke(() => { lmpS8LoadBOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8LoadBOpn.Dispatcher.Invoke(() => { lmpS8LoadBOpn.Fill = brushGray; });
                        }

                        //lmpS8LoadBCls
                        if ((bool)resultsValues[116].Value == true)
                        {
                            lmpS8LoadBCls.Dispatcher.Invoke(() => { lmpS8LoadBCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8LoadBCls.Dispatcher.Invoke(() => { lmpS8LoadBCls.Fill = brushGray; });
                        }

                        //lmpS8GrpTCls
                        if ((bool)resultsValues[117].Value == true)
                        {
                            lmpS8GrpTCls.Dispatcher.Invoke(() => { lmpS8GrpTCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8GrpTCls.Dispatcher.Invoke(() => { lmpS8GrpTCls.Fill = brushGray; });
                        }

                        //lmpS8GrpTOpn
                        if ((bool)resultsValues[118].Value == true)
                        {
                            lmpS8GrpTOpn.Dispatcher.Invoke(() => { lmpS8GrpTOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8GrpTOpn.Dispatcher.Invoke(() => { lmpS8GrpTOpn.Fill = brushGray; });
                        }

                        //lmpS8Nut
                        if ((bool)resultsValues[119].Value == true)
                        {
                            lmpS8Nut.Dispatcher.Invoke(() => { lmpS8Nut.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8Nut.Dispatcher.Invoke(() => { lmpS8Nut.Fill = brushGray; });
                        }

                        //lmpS8LvlTest
                        if ((bool)resultsValues[120].Value == true)
                        {
                            lmpS8LvlTest.Dispatcher.Invoke(() => { lmpS8LvlTest.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS8LvlTest.Dispatcher.Invoke(() => { lmpS8LvlTest.Fill = brushGray; });
                        }

                        //lmpS9GrpNCls
                        if ((bool)resultsValues[121].Value == true)
                        {
                            lmpS9GrpNCls.Dispatcher.Invoke(() => { lmpS9GrpNCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS9GrpNCls.Dispatcher.Invoke(() => { lmpS9GrpNCls.Fill = brushGray; });
                        }

                        //lmpS9GrpNOpn
                        if ((bool)resultsValues[122].Value == true)
                        {
                            lmpS9GrpNOpn.Dispatcher.Invoke(() => { lmpS9GrpNOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS9GrpNOpn.Dispatcher.Invoke(() => { lmpS9GrpNOpn.Fill = brushGray; });
                        }

                        //lmpS9GyreOpn
                        if ((bool)resultsValues[123].Value == true)
                        {
                            lmpS9GyreOpn.Dispatcher.Invoke(() => { lmpS9GyreOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS9GyreOpn.Dispatcher.Invoke(() => { lmpS9GyreOpn.Fill = brushGray; });
                        }

                        //lmpS9GyreCls
                        if ((bool)resultsValues[124].Value == true)
                        {
                            lmpS9GyreCls.Dispatcher.Invoke(() => { lmpS9GyreCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS9GyreCls.Dispatcher.Invoke(() => { lmpS9GyreCls.Fill = brushGray; });
                        }

                        //lmpT2TransCls
                        if ((bool)resultsValues[125].Value == true)
                        {
                            lmpT2TransCls.Dispatcher.Invoke(() => { lmpT2TransCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT2TransCls.Dispatcher.Invoke(() => { lmpT2TransCls.Fill = brushGray; });
                        }

                        //lmpT2TransOpn
                        if ((bool)resultsValues[126].Value == true)
                        {
                            lmpT2TransOpn.Dispatcher.Invoke(() => { lmpT2TransOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT2TransOpn.Dispatcher.Invoke(() => { lmpT2TransOpn.Fill = brushGray; });
                        }

                        //lmpT3LockIn
                        if ((bool)resultsValues[127].Value == true)
                        {
                            lmpT3LockIn.Dispatcher.Invoke(() => { lmpT3LockIn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT3LockIn.Dispatcher.Invoke(() => { lmpT3LockIn.Fill = brushGray; });
                        }

                        //lmpT3LockOut
                        if ((bool)resultsValues[128].Value == true)
                        {
                            lmpT3LockOut.Dispatcher.Invoke(() => { lmpT3LockOut.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT3LockOut.Dispatcher.Invoke(() => { lmpT3LockOut.Fill = brushGray; });
                        }

                        //lmpT3TransOpn
                        if ((bool)resultsValues[128].Value == true)
                        {
                            lmpT3TransOpn.Dispatcher.Invoke(() => { lmpT3TransOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT3TransOpn.Dispatcher.Invoke(() => { lmpT3TransOpn.Fill = brushGray; });
                        }

                        //lmpT3TransCls
                        if ((bool)resultsValues[130].Value == true)
                        {
                            lmpT3TransCls.Dispatcher.Invoke(() => { lmpT3TransCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpT3TransCls.Dispatcher.Invoke(() => { lmpT3TransCls.Fill = brushGray; });
                        }

                        //lmpS5PressLOpn
                        if ((bool)resultsValues[131].Value == true)
                        {
                            lmpS5PressLOpn.Dispatcher.Invoke(() => { lmpS5PressLOpn.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5PressLOpn.Dispatcher.Invoke(() => { lmpS5PressLOpn.Fill = brushGray; });
                        }

                        //lmpS5PressLCls
                        if ((bool)resultsValues[132].Value == true)
                        {
                            lmpS5PressLCls.Dispatcher.Invoke(() => { lmpS5PressLCls.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpS5PressLCls.Dispatcher.Invoke(() => { lmpS5PressLCls.Fill = brushGray; });
                        }

                        //lmpRbt1Home
                        if ((bool)resultsValues[133].Value == true)
                        {
                            lmpRbt1Home1.Dispatcher.Invoke(() => { lmpRbt1Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbt1Home1.Dispatcher.Invoke(() => { lmpRbt1Home1.Fill = brushGray; });
                        }

                        //lmpMarkReady
                        if ((bool)resultsValues[134].Value == true)
                        {
                            lmpMarkReady1.Dispatcher.Invoke(() => { lmpMarkReady1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpMarkReady1.Dispatcher.Invoke(() => { lmpMarkReady1.Fill = brushGray; });
                        }

                        //lmpTrans1Home
                        if ((bool)resultsValues[135].Value == true)
                        {
                            lmpTrans1Home1.Dispatcher.Invoke(() => { lmpTrans1Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrans1Home1.Dispatcher.Invoke(() => { lmpTrans1Home1.Fill = brushGray; });
                        }

                        //lmpInletCVHome
                        if ((bool)resultsValues[136].Value == true)
                        {
                            lmpInletCVHome1.Dispatcher.Invoke(() => { lmpInletCVHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpInletCVHome1.Dispatcher.Invoke(() => { lmpInletCVHome1.Fill = brushGray; });
                        }

                        //lmpPressShldHome
                        if ((bool)resultsValues[137].Value == true)
                        {
                            lmpPressShldHome1.Dispatcher.Invoke(() => { lmpPressShldHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpPressShldHome1.Dispatcher.Invoke(() => { lmpPressShldHome1.Fill = brushGray; });
                        }

                        //lmpMaxymosReady
                        if ((bool)resultsValues[138].Value == true)
                        {
                            lmpMaxymosReady1.Dispatcher.Invoke(() => { lmpMaxymosReady1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpMaxymosReady1.Dispatcher.Invoke(() => { lmpMaxymosReady1.Fill = brushGray; });
                        }

                        //lmpInsertShldHome
                        if ((bool)resultsValues[139].Value == true)
                        {
                            lmpInsertShldHome1.Dispatcher.Invoke(() => { lmpInsertShldHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpInsertShldHome1.Dispatcher.Invoke(() => { lmpInsertShldHome1.Fill = brushGray; });
                        }

                        //lmpPlugHome
                        if ((bool)resultsValues[140].Value == true)
                        {
                            lmpPlugHome1.Dispatcher.Invoke(() => { lmpPlugHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpPlugHome1.Dispatcher.Invoke(() => { lmpPlugHome1.Fill = brushGray; });
                        }

                        //lmpTrans2Home
                        if ((bool)resultsValues[141].Value == true)
                        {
                            lmpTrans2Home1.Dispatcher.Invoke(() => { lmpTrans2Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrans2Home1.Dispatcher.Invoke(() => { lmpTrans2Home1.Fill = brushGray; });
                        }

                        //lmpTrans4Home
                        if ((bool)resultsValues[142].Value == true)
                        {
                            lmpTrans4Home1.Dispatcher.Invoke(() => { lmpTrans4Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrans4Home1.Dispatcher.Invoke(() => { lmpTrans4Home1.Fill = brushGray; });
                        }

                        //lmpTrans5Home
                        if ((bool)resultsValues[143].Value == true)
                        {
                            lmpTrans5Home1.Dispatcher.Invoke(() => { lmpTrans5Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrans5Home1.Dispatcher.Invoke(() => { lmpTrans5Home1.Fill = brushGray; });
                        }

                        //lmpTrans6Home
                        if ((bool)resultsValues[144].Value == true)
                        {
                            lmpTrans6Home1.Dispatcher.Invoke(() => { lmpTrans6Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTrans6Home1.Dispatcher.Invoke(() => { lmpTrans6Home1.Fill = brushGray; });
                        }

                        //lmpNutPressHome
                        if ((bool)resultsValues[145].Value == true)
                        {
                            lmpNutPressHome1.Dispatcher.Invoke(() => { lmpNutPressHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpNutPressHome1.Dispatcher.Invoke(() => { lmpNutPressHome1.Fill = brushGray; });
                        }

                        //lmpMaxymosNutReady1
                        if ((bool)resultsValues[146].Value == true)
                        {
                            lmpMaxymosNutReady1.Dispatcher.Invoke(() => { lmpMaxymosNutReady1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpMaxymosNutReady1.Dispatcher.Invoke(() => { lmpMaxymosNutReady1.Fill = brushGray; });
                        }

                        //lmpLoadersNutHome
                        if ((bool)resultsValues[147].Value == true)
                        {
                            lmpLoadersNutHome1.Dispatcher.Invoke(() => { lmpLoadersNutHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpLoadersNutHome1.Dispatcher.Invoke(() => { lmpLoadersNutHome1.Fill = brushGray; });
                        }

                        //lmpRotaryHome
                        if ((bool)resultsValues[148].Value == true)
                        {
                            lmpRotaryHome1.Dispatcher.Invoke(() => { lmpRotaryHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRotaryHome1.Dispatcher.Invoke(() => { lmpRotaryHome1.Fill = brushGray; });
                        }

                        //lmpServoHome
                        if ((bool)resultsValues[149].Value == true)
                        {
                            lmpServoHome1.Dispatcher.Invoke(() => { lmpServoHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpServoHome1.Dispatcher.Invoke(() => { lmpServoHome1.Fill = brushGray; });
                        }

                        //lmpMaxymosPlngReady
                        if ((bool)resultsValues[150].Value == true)
                        {
                            lmpMaxymosPlngReady1.Dispatcher.Invoke(() => { lmpMaxymosPlngReady1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpMaxymosPlngReady1.Dispatcher.Invoke(() => { lmpMaxymosPlngReady1.Fill = brushGray; });
                        }

                        //lmpRbt2Home
                        if ((bool)resultsValues[151].Value == true)
                        {
                            lmpRbt2Home1.Dispatcher.Invoke(() => { lmpRbt2Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbt2Home1.Dispatcher.Invoke(() => { lmpRbt2Home1.Fill = brushGray; });
                        }

                        //lmpRbt3Home
                        if ((bool)resultsValues[152].Value == true)
                        {
                            lmpRbt3Home1.Dispatcher.Invoke(() => { lmpRbt3Home1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpRbt3Home1.Dispatcher.Invoke(() => { lmpRbt3Home1.Fill = brushGray; });
                        }

                        //lmpLoadArmaHome
                        if ((bool)resultsValues[153].Value == true)
                        {
                            lmpLoadArmaHome1.Dispatcher.Invoke(() => { lmpLoadArmaHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpLoadArmaHome1.Dispatcher.Invoke(() => { lmpLoadArmaHome1.Fill = brushGray; });
                        }

                        //lmpTorque1Ready
                        if ((bool)resultsValues[154].Value == true)
                        {
                            lmpTorque1Ready1.Dispatcher.Invoke(() => { lmpTorque1Ready1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTorque1Ready1.Dispatcher.Invoke(() => { lmpTorque1Ready1.Fill = brushGray; });
                        }

                        //lmpTorque2Ready
                        if ((bool)resultsValues[155].Value == true)
                        {
                            lmpTorque2Ready1.Dispatcher.Invoke(() => { lmpTorque2Ready1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpTorque2Ready1.Dispatcher.Invoke(() => { lmpTorque2Ready1.Fill = brushGray; });
                        }

                        //lmpSpringHome
                        if ((bool)resultsValues[156].Value == true)
                        {
                            lmpSpringHome1.Dispatcher.Invoke(() => { lmpSpringHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpSpringHome1.Dispatcher.Invoke(() => { lmpSpringHome1.Fill = brushGray; });
                        }

                        //lmpLatchHome
                        if ((bool)resultsValues[157].Value == true)
                        {
                            lmpLatchHome1.Dispatcher.Invoke(() => { lmpLatchHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpLatchHome1.Dispatcher.Invoke(() => { lmpLatchHome1.Fill = brushGray; });
                        }

                        //lmpBoltHome
                        if ((bool)resultsValues[158].Value == true)
                        {
                            lmpBoltHome1.Dispatcher.Invoke(() => { lmpBoltHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpBoltHome1.Dispatcher.Invoke(() => { lmpBoltHome1.Fill = brushGray; });
                        }

                        //lmpPosCorrectHome
                        if ((bool)resultsValues[159].Value == true)
                        {
                            lmpPressHome1.Dispatcher.Invoke(() => { lmpPressHome1.Fill = brushGreen; });
                        }
                        else
                        {
                            lmpPressHome1.Dispatcher.Invoke(() => { lmpPressHome1.Fill = brushGray; });
                        }


                        #endregion

                        #region Shield insert

                        this.Dispatcher.Invoke(() =>
                        {
                            //Get serial numbers
                            dspShldSerialNo.Text = resultsValues[166].Value.ToString();
                            dspShldOldSerial.Text = resultsValues[167].Value.ToString();
                            //Get process result
                            oTblResultsInstallTermShld.Rows[(int)eInstallTermShld_Prod_Results.PartStatus][1] = resultsValues[228].Value.ToString();

                            dspShldDistance1.Text = resultsValues[160].Value.ToString();
                            oTblResultsInstallTermShld.Rows[(int)eInstallTermShld_Prod_Results.ThresholdX_Min][1] = resultsValues[160].Value.ToString();

                            dspShldDistance2.Text = resultsValues[161].Value.ToString();
                            oTblResultsInstallTermShld.Rows[(int)eInstallTermShld_Prod_Results.ThresholdY_Min][1] = resultsValues[161].Value.ToString();

                            dspShldForce1.Text = resultsValues[162].Value.ToString();
                            oTblResultsInstallTermShld.Rows[(int)eInstallTermShld_Prod_Results.ThresholdX_Max][1] = resultsValues[162].Value.ToString();

                            dspShldForce2.Text = resultsValues[163].Value.ToString();
                            oTblResultsInstallTermShld.Rows[(int)eInstallTermShld_Prod_Results.ThresholdY_Max][1] = resultsValues[163].Value.ToString();
                            //Get production information
                            dspShldTotPc.Text = resultsValues[190].Value.ToString();
                            barShldTotOkPc.Maximum = (Int16)resultsValues[190].Value;
                            barShldTotNgPc.Maximum = (Int16)resultsValues[190].Value;
                            
                            dspShldTotOkPc.Text = resultsValues[191].Value.ToString();
                            barShldTotOkPc.Value = (Int16)resultsValues[191].Value;

                            dspShldTotNgPc.Text = resultsValues[192].Value.ToString();
                            barShldTotNgPc.Value = (Int16)resultsValues[192].Value;

                            dspShldLTotOkPc.Text = resultsValues[193].Value.ToString();
                            barShldLTotOk.Value = (Int16)resultsValues[193].Value;

                            dspShldLTotNgPc.Text = resultsValues[194].Value.ToString();
                            barShldLTotNg.Value = (Int16)resultsValues[194].Value;

                            dspShldRTotOkPc.Text = resultsValues[195].Value.ToString();
                            barShldRTotOk.Value = (Int16)resultsValues[195].Value;

                            dspShldRTotNgPc.Text = resultsValues[196].Value.ToString();
                            barShldRTotNg.Value = (Int16)resultsValues[196].Value;

                            if (resultsValues[164].Value.ToString() == "GOOD")
                            {
                                dspShldJdg1.Text = resultsValues[164].Value.ToString();
                                dspShldJdg1.Foreground = Brushes.DarkGreen;
                            }
                            else if (resultsValues[164].Value.ToString() == "NOT GOOD")
                            {
                                dspShldJdg1.Text = resultsValues[164].Value.ToString();
                                dspShldJdg1.Foreground = Brushes.DarkRed;
                            }
                            else
                            {
                                dspShldJdg1.Text = resultsValues[164].Value.ToString();
                                dspShldJdg1.Foreground = Brushes.Black;
                            }

                            if (resultsValues[165].Value.ToString() == "GOOD")
                            {
                                dspShldJdg2.Text = resultsValues[165].Value.ToString();
                                dspShldJdg2.Foreground = Brushes.DarkGreen;
                            }
                            else if (resultsValues[165].Value.ToString() == "NOT GOOD")
                            {
                                dspShldJdg2.Text = resultsValues[165].Value.ToString();
                                dspShldJdg2.Foreground = Brushes.DarkRed;
                            }
                            else
                            {
                                dspShldJdg2.Text = resultsValues[165].Value.ToString();
                                dspShldJdg2.Foreground = Brushes.Black;
                            }
                        });

                        //Check operation
                        if ((bool)resultsValues[197].Value && !oneShot0)
                            await Task.Run(() => { ShieldCheckOp(); });
                        oneShot0 = (bool)resultsValues[197].Value;
                        //Update results
                        if ((bool)resultsValues[198].Value && !oneShot1)
                            await Task.Run(() => { ShieldUpdateCommand(); });
                        oneShot1 = (bool)resultsValues[198].Value;

                        #endregion

                        #region Nut insert
                        this.Dispatcher.Invoke(() =>
                        {
                            //Get serial numbers
                            dspNutLSerialNo.Text = resultsValues[176].Value.ToString();
                            dspNutLOldSerial.Text = resultsValues[177].Value.ToString();
                            dspNutRSerialNo.Text = resultsValues[178].Value.ToString();
                            dspNutROldSerial.Text = resultsValues[179].Value.ToString();
                            //Get process result
                            oTblResultsInstallNutInsr.Rows[(int)eInstallM8Insr_Prod_Results.PartStatus][1] = resultsValues[229].Value.ToString();

                            oTblResultsInstallNutInsr.Rows[(int)eInstallM8Insr_Prod_Results.Force_Left][1] = resultsValues[206].Value.ToString();
                            dspNutLForce.Text = resultsValues[206].Value.ToString();

                            oTblResultsInstallNutInsr.Rows[(int)eInstallM8Insr_Prod_Results.Distance_Left][1] = resultsValues[205].Value.ToString();
                            dspNutLDistance.Text = resultsValues[205].Value.ToString();

                            oTblResultsInstallNutInsr.Rows[(int)eInstallM8Insr_Prod_Results.Force_Right][1] = resultsValues[212].Value.ToString();
                            dspNutRForce.Text = resultsValues[212].Value.ToString();

                            oTblResultsInstallNutInsr.Rows[(int)eInstallM8Insr_Prod_Results.Distance_Right][1] = resultsValues[211].Value.ToString();
                            dspNutRDistance.Text = resultsValues[211].Value.ToString();

                            //Get production information
                            dspNutLTotPc.Text = resultsValues[208].Value.ToString();
                            barNutLTotOk.Maximum = (Int16)resultsValues[208].Value;
                            barNutLTotNg.Maximum = (Int16)resultsValues[208].Value;
                            
                            dspNutRTotPc.Text = resultsValues[214].Value.ToString();
                            barNutRTotOk.Maximum = (Int16)resultsValues[214].Value;
                            barNutRTotNg.Maximum = (Int16)resultsValues[214].Value;

                            dspNutLTotOk.Text = resultsValues[209].Value.ToString();
                            dspNutLTotNg.Text = resultsValues[210].Value.ToString();

                            dspNutRTotOk.Text = resultsValues[215].Value.ToString();
                            dspNutRTotNg.Text = resultsValues[216].Value.ToString();

                            if (resultsValues[207].Value.ToString() == "GOOD")
                            {
                                dspNutLJdg.Text = resultsValues[207].Value.ToString();
                                dspNutLJdg.Foreground = Brushes.DarkGreen;
                            }
                            else if (resultsValues[207].Value.ToString() == "NOT GOOD")
                            {
                                dspNutLJdg.Text = resultsValues[207].Value.ToString();
                                dspNutLJdg.Foreground = Brushes.DarkRed;
                            }
                            else
                            {
                                dspNutLJdg.Text = resultsValues[207].Value.ToString();
                                dspNutLJdg.Foreground = Brushes.Black;
                            }

                            if (resultsValues[213].Value.ToString() == "GOOD")
                            {
                                dspNutRJdg.Text = resultsValues[213].Value.ToString();
                                dspNutRJdg.Foreground = Brushes.DarkGreen;
                            }
                            else if (resultsValues[213].Value.ToString() == "NOT GOOD")
                            {
                                dspNutRJdg.Text = resultsValues[213].Value.ToString();
                                dspNutRJdg.Foreground = Brushes.DarkRed;
                            }
                            else
                            {
                                dspNutRJdg.Text = resultsValues[213].Value.ToString();
                                dspNutRJdg.Foreground = Brushes.Black;
                            }
                        });

                        //Check operation
                        if ((bool)resultsValues[199].Value && !oneShot2)
                            await Task.Run(() => { NutCheckOp(); });
                        oneShot2 = (bool)resultsValues[199].Value;
                        //Update results
                        if ((bool)resultsValues[200].Value && !oneShot3)
                            await Task.Run(() => { NutUpdateCommand(); });
                        oneShot3 = (bool)resultsValues[200].Value;

                        #endregion

                        #region Plunger & armature
                        this.Dispatcher.Invoke(() =>
                        {
                            //Get serial numbers
                            dspPlngArmSerialNo.Text = resultsValues[180].Value.ToString();
                            dspPlngArmOldSerial.Text = resultsValues[181].Value.ToString();
                        });
                        //Get process result
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.PartStatus][1] = resultsValues[224].Value.ToString();
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.Torque][1] = resultsValues[217].Value.ToString();
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.Angle][1] = resultsValues[218].Value.ToString();
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.Test1_Force][1] = resultsValues[219].Value.ToString();
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.Test2_Force][1] = resultsValues[220].Value.ToString();
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.Test1_Displacement][1] = resultsValues[221].Value.ToString();
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.Test2_Displacement][1] = resultsValues[222].Value.ToString();
                        oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.ConstK][1] = resultsValues[223].Value.ToString();

                        //Check operation
                        if ((bool)resultsValues[201].Value && !oneShot4)
                            await Task.Run(() => { PlngArmCheckOp(); });
                        oneShot4 = (bool)resultsValues[201].Value;
                        //Update results
                        if ((bool)resultsValues[202].Value && !oneShot5)
                            await Task.Run(() => { PlngArmUpdateCommand(); });
                        oneShot5 = (bool)resultsValues[202].Value;

                        #endregion

                        #region Latch assy
                        this.Dispatcher.Invoke(() =>
                        {
                            //Get serial numbers
                            dspLatchSerialNo.Text = resultsValues[182].Value.ToString();
                            dspLatchOldSerial.Text = resultsValues[183].Value.ToString();
                        });
                        //Get process result
                        oTblResultsLatchAssy.Rows[(int)eLatchAssy_Prod_Results.PartStatus][1] = resultsValues[225].Value.ToString();
                        oTblResultsLatchAssy.Rows[(int)eLatchAssy_Prod_Results.Torque][1] = resultsValues[226].Value.ToString();
                        oTblResultsLatchAssy.Rows[(int)eLatchAssy_Prod_Results.Angle][1] = resultsValues[227].Value.ToString();

                        //Check operation
                        if ((bool)resultsValues[203].Value && !oneShot6)
                            await Task.Run(() => { LatchCheckOp(); });
                        oneShot6 = (bool)resultsValues[203].Value;
                        //Update results
                        if ((bool)resultsValues[204].Value && !oneShot7)
                            await Task.Run(() => { LatchUpdateCommand(); });
                        oneShot7 = (bool)resultsValues[204].Value;

                        #endregion

                        #region Prepare
                        if ((bool)resultsValues[171].Value) { lmpDeviceID.Dispatcher.Invoke(() => { lmpDeviceID.Fill = brushGreen; }); }
                        else { lmpDeviceID.Dispatcher.Invoke(() => { lmpDeviceID.Fill = brushGray; }); }

                        if ((bool)resultsValues[172].Value) { lmpLotID.Dispatcher.Invoke(() => { lmpLotID.Fill = brushGreen; }); }
                        else { lmpLotID.Dispatcher.Invoke(() => { lmpLotID.Fill = brushGray; }); }

                        if ((bool)resultsValues[173].Value) { lmpQty.Dispatcher.Invoke(() => { lmpQty.Fill = brushGreen; }); }
                        else { lmpQty.Dispatcher.Invoke(() => { lmpQty.Fill = brushGray; }); }
                        #endregion

                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception e)
            {
                stpOpcUaLog.Dispatcher.Invoke(() =>
                {
                    stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}-----Variables reading error: {e}"), Foreground = Brushes.DarkRed });
                });
                MessageBox.Show("Error de lectura de variables");
                DisconnectionRequest();
            }
        }
        private void GetOpcUAState(CancellationToken _ct)
        {
            try 
            {
                stpOpcUaLog.Dispatcher.Invoke(() => { stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}       Opc UA state monitor running")}); });
                while (!_ct.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                    if (opcUaClient != null)
                    {
                        if (opcUaClient.Session != null)
                        {
                            sessionConnected = opcUaClient.Session.Connected;
                            if (sessionConnected)
                            {
                                this.Dispatcher.Invoke(() => { 
                                    sbmConnect.IsEnabled = false;
                                    sbmDisconnect.IsEnabled = true;
                                    lmpOpcUaConnect.Fill = brushGreen;
                                    lmpOpcUaDisconnect.Fill = brushGray;
                                    dspOpcStatus.Text = "Connected";
                                    lmpOpcStatus.Fill = Brushes.Green;
                                });
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() => {
                                    sbmConnect.IsEnabled = true;
                                    sbmDisconnect.IsEnabled = false;
                                    lmpOpcUaConnect.Fill = brushGray;
                                    lmpOpcUaDisconnect.Fill = brushRed;
                                    dspOpcStatus.Text = "Disconnected";
                                    lmpOpcStatus.Fill = Brushes.Red;
                                });
                            }
                        } else
                        {
                            this.Dispatcher.Invoke(() => {
                                sbmConnect.IsEnabled = true;
                                sbmDisconnect.IsEnabled = false;
                                lmpOpcUaConnect.Fill = brushGray;
                                lmpOpcUaDisconnect.Fill = brushRed;
                                dspOpcStatus.Text = "Disconnected";
                                lmpOpcStatus.Fill = Brushes.Red;
                            });
                        }
                    } else
                    {
                        this.Dispatcher.Invoke(() => {
                            sbmConnect.IsEnabled = true;
                            sbmDisconnect.IsEnabled = false;
                            lmpOpcUaConnect.Fill = brushGray;
                            lmpOpcUaDisconnect.Fill = brushRed;
                            dspOpcStatus.Text = "Disconnected";
                            lmpOpcStatus.Fill = Brushes.Red;
                        });
                    }
                }
                return;
            } catch { }
        }
        private void TasksMonitorMethod(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(2000);
                    if (OpcUaMonitor != null)
                    { 
                        if (OpcUaMonitor.Status == TaskStatus.Running)
                        {
                            //stpOpcUaLog.Dispatcher.Invoke(() => {
                            //    stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}       OPC UA tasks monitor running"), Foreground = Brushes.DarkGreen });
                            //});

                        }
                        else
                        {
                            //stpOpcUaLog.Dispatcher.Invoke(() => {
                            //    stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.UtcNow}      OPC UA tasks monitor stopped"), Foreground = Brushes.Black });
                            //});
                            OpcUaMonitor = Task.Run(() => { OpcUaReader(token); });
                        }
                    }
                }
                return;
            }
            catch(Exception ex)
            {
                stpOpcUaLog.Dispatcher.Invoke(() => { stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}-----Excepción: {ex.Message}"), Foreground = Brushes.DarkRed}); });
            }
            
        }
        private void OpcUaReader(CancellationToken token)
        {
            try
            {
                nodesToRead = new ReadValueIdCollection() {
                        new ReadValueId() {NodeId = "ns=4;s=dspDeviceID",           AttributeId = Attributes.Value},/*0*/
                        new ReadValueId() {NodeId = "ns=4;s=dspLotID",              AttributeId = Attributes.Value},/*1*/
                        new ReadValueId() {NodeId = "ns=4;s=dspQty",                AttributeId = Attributes.Value},/*2*/
                        new ReadValueId() {NodeId = "ns=4;s=dspSelectedDvcID",      AttributeId = Attributes.Value},/*3*/
                        new ReadValueId() {NodeId = "ns=4;s=dspSelectedLotID",      AttributeId = Attributes.Value},/*4*/
                        new ReadValueId() {NodeId = "ns=4;s=dspSelectedQty",        AttributeId = Attributes.Value},/*5*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0TrayPos1",           AttributeId = Attributes.Value},/*6*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0TrayPos2",           AttributeId = Attributes.Value},/*7*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0TrayPos3",           AttributeId = Attributes.Value},/*8*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0StprIn1",            AttributeId = Attributes.Value},/*9*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0StprIn2",            AttributeId = Attributes.Value},/*10*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0HoldAIn",            AttributeId = Attributes.Value},/*11*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0HoldAOut",           AttributeId = Attributes.Value},/*12*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0HoldBIn",            AttributeId = Attributes.Value},/*13*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0HoldBOut",           AttributeId = Attributes.Value},/*14*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0HoldCIn",            AttributeId = Attributes.Value},/*15*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0HoldCOut",           AttributeId = Attributes.Value},/*16*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0EjectIn",            AttributeId = Attributes.Value},/*17*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0EjectOut",           AttributeId = Attributes.Value},/*18*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0ChckSide",           AttributeId = Attributes.Value},/*19*/
                        new ReadValueId() {NodeId = "ns=4;s=iRbt1GrpCls",           AttributeId = Attributes.Value},/*20*/
                        new ReadValueId() {NodeId = "ns=4;s=iRbt1GrpOpn",           AttributeId = Attributes.Value},/*21*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0NstToolA",           AttributeId = Attributes.Value},/*22*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0NstToolB",           AttributeId = Attributes.Value},/*23*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0NstToolC",           AttributeId = Attributes.Value},/*24*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0RbtPick",            AttributeId = Attributes.Value},/*25*/
                        new ReadValueId() {NodeId = "ns=4;s=iS1GrpCls",             AttributeId = Attributes.Value},/*26*/
                        new ReadValueId() {NodeId = "ns=4;s=iS1GrpOpn",             AttributeId = Attributes.Value},/*27*/
                        new ReadValueId() {NodeId = "ns=4;s=iS2GrpCls",             AttributeId = Attributes.Value},/*28*/
                        new ReadValueId() {NodeId = "ns=4;s=iS2GrpOpn",             AttributeId = Attributes.Value},/*29*/
                        new ReadValueId() {NodeId = "ns=4;s=iT1LoaderIn",           AttributeId = Attributes.Value},/*30*/
                        new ReadValueId() {NodeId = "ns=4;s=iT1LoaderOut",          AttributeId = Attributes.Value},/*31*/
                        new ReadValueId() {NodeId = "ns=4;s=iT1TransLmtR",          AttributeId = Attributes.Value},/*32*/
                        new ReadValueId() {NodeId = "ns=4;s=iT1TransLmtL",          AttributeId = Attributes.Value},/*33*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0StprOut1",   AttributeId = Attributes.Value},/*34*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0StprOut2",   AttributeId = Attributes.Value},/*35*/
                        new ReadValueId() {NodeId = "ns=4;s=iS0Nest",       AttributeId = Attributes.Value},/*36*/
                        new ReadValueId() {NodeId = "ns=4;s=iS2Nest",       AttributeId = Attributes.Value},/*37*/
                        new ReadValueId() {NodeId = "ns=4;s=iS1Nest",       AttributeId = Attributes.Value},/*38*/
                        new ReadValueId() {NodeId = "ns=4;s=iS3GrpCls",     AttributeId = Attributes.Value},/*39*/
                        new ReadValueId() {NodeId = "ns=4;s=iS3GrpOpn",     AttributeId = Attributes.Value},/*40*/
                        new ReadValueId() {NodeId = "ns=4;s=iS3LoadIn",     AttributeId = Attributes.Value},/*41*/
                        new ReadValueId() {NodeId = "ns=4;s=iS3LoadOut",    AttributeId = Attributes.Value},/*42*/
                        new ReadValueId() {NodeId = "ns=4;s=iS3Gyre0",      AttributeId = Attributes.Value},/*43*/
                        new ReadValueId() {NodeId = "ns=4;s=iS3Gyre180",    AttributeId = Attributes.Value},/*44*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpNCls",    AttributeId = Attributes.Value},/*45*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpNOpn",    AttributeId = Attributes.Value},/*46*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpRCls",    AttributeId = Attributes.Value},/*47*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpROpn",    AttributeId = Attributes.Value},/*48*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpLCls",    AttributeId = Attributes.Value},/*49*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpLOpn",    AttributeId = Attributes.Value},/*50*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4Gyre0",      AttributeId = Attributes.Value},/*51*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4Gyre90",     AttributeId = Attributes.Value},/*52*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4Upload",     AttributeId = Attributes.Value},/*53*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4Dwnload",    AttributeId = Attributes.Value},/*54*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4LimitL",     AttributeId = Attributes.Value},/*55*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4LimitR",     AttributeId = Attributes.Value},/*56*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4LoadIn",     AttributeId = Attributes.Value},/*57*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4LoadOut",    AttributeId = Attributes.Value},/*58*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpTCls",    AttributeId = Attributes.Value},/*59*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4GrpTOpn",    AttributeId = Attributes.Value},/*60*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4ShieldR",    AttributeId = Attributes.Value},/*61*/
                        new ReadValueId() {NodeId = "ns=4;s=iS4ShieldL",    AttributeId = Attributes.Value},/*62*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5PrssRUp",    AttributeId = Attributes.Value},/*63*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5PrssRDw",    AttributeId = Attributes.Value},/*64*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5GrpTCls",    AttributeId = Attributes.Value},/*65*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5GrpTOpn",    AttributeId = Attributes.Value},/*66*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5LoadIn",     AttributeId = Attributes.Value},/*67*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5LoadOut",    AttributeId = Attributes.Value},/*68*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5Gyre0",      AttributeId = Attributes.Value},/*69*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5Gyre180",    AttributeId = Attributes.Value},/*70*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5GrpNCls",    AttributeId = Attributes.Value},/*71*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5GrpNOpn",    AttributeId = Attributes.Value},/*72*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6Plug",       AttributeId = Attributes.Value},/*73*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6EjctIn",     AttributeId = Attributes.Value},/*74*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6EjctOut",    AttributeId = Attributes.Value},/*75*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6LimitF",     AttributeId = Attributes.Value},/*76*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6LimitB",     AttributeId = Attributes.Value},/*77*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6Upload",     AttributeId = Attributes.Value},/*78*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6Dwnload",    AttributeId = Attributes.Value},/*79*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6ReachUp",    AttributeId = Attributes.Value},/*80*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6ReachDw",    AttributeId = Attributes.Value},/*81*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6Vacuum",     AttributeId = Attributes.Value},/*82*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6GrpNCls",    AttributeId = Attributes.Value},/*83*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6GrpNOpn",    AttributeId = Attributes.Value},/*84*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6GrpTCls",    AttributeId = Attributes.Value},/*85*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6GrpTOpn",    AttributeId = Attributes.Value},/*86*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6LoadIn",     AttributeId = Attributes.Value},/*87*/
                        new ReadValueId() {NodeId = "ns=4;s=iS6LoadOut",    AttributeId = Attributes.Value},/*88*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7GrpNCls",    AttributeId = Attributes.Value},/*89*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7GrpNOpn",    AttributeId = Attributes.Value},/*90*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7LoadIn",     AttributeId = Attributes.Value},/*91*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7LoadOut",    AttributeId = Attributes.Value},/*92*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7PressUp",    AttributeId = Attributes.Value},/*93*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7PressDw",    AttributeId = Attributes.Value},/*94*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7PinUp",      AttributeId = Attributes.Value},/*95*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7PinDw",      AttributeId = Attributes.Value},/*96*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7LoadAIn",    AttributeId = Attributes.Value},/*97*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7LoadAOut",   AttributeId = Attributes.Value},/*98*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7LoadBIn",    AttributeId = Attributes.Value},/*99*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7LoadBOut",   AttributeId = Attributes.Value},/*100*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7GrpTCls",    AttributeId = Attributes.Value},/*101*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7GrpTOpn",    AttributeId = Attributes.Value},/*102*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7Nut",        AttributeId = Attributes.Value},/*103*/
                        new ReadValueId() {NodeId = "ns=4;s=iS7LevelTest",  AttributeId = Attributes.Value},/*104*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8GrpNCls",    AttributeId = Attributes.Value},/*105*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8GrpNOpn",    AttributeId = Attributes.Value},/*106*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8LoadIn",     AttributeId = Attributes.Value},/*107*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8LoadOut",    AttributeId = Attributes.Value},/*108*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8PressUp",    AttributeId = Attributes.Value},/*109*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8PressDw",    AttributeId = Attributes.Value},/*110*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8PinUp",      AttributeId = Attributes.Value},/*111*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8PinDw",      AttributeId = Attributes.Value},/*112*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8LoadAIn",    AttributeId = Attributes.Value},/*113*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8LoadAOut",   AttributeId = Attributes.Value},/*114*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8LoadBIn",    AttributeId = Attributes.Value},/*115*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8LoadBOut",   AttributeId = Attributes.Value},/*116*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8GrpTCls",    AttributeId = Attributes.Value},/*117*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8GrpTOpn",    AttributeId = Attributes.Value},/*118*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8Nut",        AttributeId = Attributes.Value},/*119*/
                        new ReadValueId() {NodeId = "ns=4;s=iS8LevelTest",  AttributeId = Attributes.Value},/*120*/
                        new ReadValueId() {NodeId = "ns=4;s=iS9GrpNCls",    AttributeId = Attributes.Value},/*121*/
                        new ReadValueId() {NodeId = "ns=4;s=iS9GrpNOpn",    AttributeId = Attributes.Value},/*122*/
                        new ReadValueId() {NodeId = "ns=4;s=iS9Gyre0",      AttributeId = Attributes.Value},/*123*/
                        new ReadValueId() {NodeId = "ns=4;s=iS9Gyre90",     AttributeId = Attributes.Value},/*124*/
                        new ReadValueId() {NodeId = "ns=4;s=iT2TransLmtR",  AttributeId = Attributes.Value},/*125*/
                        new ReadValueId() {NodeId = "ns=4;s=iT2TransLmtL",  AttributeId = Attributes.Value},/*126*/
                        new ReadValueId() {NodeId = "ns=4;s=iT3LocPinIn",   AttributeId = Attributes.Value},/*127*/
                        new ReadValueId() {NodeId = "ns=4;s=iT3LocPinOut",  AttributeId = Attributes.Value},/*128*/
                        new ReadValueId() {NodeId = "ns=4;s=iT3TransLmtB",  AttributeId = Attributes.Value},/*129*/
                        new ReadValueId() {NodeId = "ns=4;s=iT3TransLmtF",  AttributeId = Attributes.Value},/*130*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5PrssLUp",    AttributeId = Attributes.Value},/*131*/
                        new ReadValueId() {NodeId = "ns=4;s=iS5PrssLDw",    AttributeId = Attributes.Value},/*132*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpRbt1Home",           AttributeId = Attributes.Value},/*133*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpMarkReady",          AttributeId = Attributes.Value},/*134*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpTrans1Home",         AttributeId = Attributes.Value},/*135*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpInletCvHome",        AttributeId = Attributes.Value},/*136*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpPressShldHome",      AttributeId = Attributes.Value},/*137*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpMaxymosReady",       AttributeId = Attributes.Value},/*138*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpInsertShldHome",     AttributeId = Attributes.Value},/*139*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpPlugHome",           AttributeId = Attributes.Value},/*140*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpTrans2Home",         AttributeId = Attributes.Value},/*141*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpTrans4Home",         AttributeId = Attributes.Value},/*142*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpTrans5Home",         AttributeId = Attributes.Value},/*143*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpTrans6Home",         AttributeId = Attributes.Value},/*144*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpNutPressHome",       AttributeId = Attributes.Value},/*145*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpMaxymosNutReady",    AttributeId = Attributes.Value},/*146*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpNutLoadHome",        AttributeId = Attributes.Value},/*147*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpRotaryHome",         AttributeId = Attributes.Value},/*148*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpServoHome",          AttributeId = Attributes.Value},/*149*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpMaxymosPlngReady",   AttributeId = Attributes.Value},/*150*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpRbt2Home",           AttributeId = Attributes.Value},/*151*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpRbt3Home",           AttributeId = Attributes.Value},/*152*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpLoadArmaHome",       AttributeId = Attributes.Value},/*153*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpTorque1Ready",       AttributeId = Attributes.Value},/*154*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpTorque2Ready",       AttributeId = Attributes.Value},/*155*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpSpringHome",         AttributeId = Attributes.Value},/*156*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpLatchHome",          AttributeId = Attributes.Value},/*157*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpBoltHome",           AttributeId = Attributes.Value},/*158*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpPosCorrectHome",     AttributeId = Attributes.Value},/*159*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldDistance1",      AttributeId = Attributes.Value},/*160*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldDistance2",      AttributeId = Attributes.Value},/*161*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldForce1",         AttributeId = Attributes.Value},/*162*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldForce2",         AttributeId = Attributes.Value},/*163*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldJdg1",           AttributeId = Attributes.Value},/*164*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldJdg2",           AttributeId = Attributes.Value},/*165*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldSerialNo",       AttributeId = Attributes.Value},/*166*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldOldSerial",      AttributeId = Attributes.Value},/*167*/
                        new ReadValueId() {NodeId = "ns=4;s=dspMrkData",            AttributeId = Attributes.Value},/*168*/
                        new ReadValueId() {NodeId = "ns=4;s=dspReadData",           AttributeId = Attributes.Value},/*169*/
                        new ReadValueId() {NodeId = "ns=4;s=dspMrkOldData",         AttributeId = Attributes.Value},/*170*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpDvcIDSlctd",         AttributeId = Attributes.Value},/*171*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpLotIDSlctd",         AttributeId = Attributes.Value},/*172*/
                        new ReadValueId() {NodeId = "ns=4;s=lmpQtySlctd",           AttributeId = Attributes.Value},/*173*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldSerialNo",       AttributeId = Attributes.Value},/*174*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldOldSerial",      AttributeId = Attributes.Value},/*175*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutLSerialNo",       AttributeId = Attributes.Value},/*176*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutLOldSerial",      AttributeId = Attributes.Value},/*177*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutRSerialNo",       AttributeId = Attributes.Value},/*178*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutROldSerial",      AttributeId = Attributes.Value},/*179*/
                        new ReadValueId() {NodeId = "ns=4;s=dspPlngArmSerialNo",    AttributeId = Attributes.Value},/*180*/
                        new ReadValueId() {NodeId = "ns=4;s=dspPlngArmOldSerial",   AttributeId = Attributes.Value},/*181*/
                        new ReadValueId() {NodeId = "ns=4;s=dspLatchSerialNo",      AttributeId = Attributes.Value},/*182*/
                        new ReadValueId() {NodeId = "ns=4;s=dspLatchOldSerial",     AttributeId = Attributes.Value},/*183*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldDistance1",      AttributeId = Attributes.Value},/*184*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldDistance2",      AttributeId = Attributes.Value},/*185*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldForce1",         AttributeId = Attributes.Value},/*186*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldForce2",         AttributeId = Attributes.Value},/*187*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldJdg1",           AttributeId = Attributes.Value},/*188*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldJdg2",           AttributeId = Attributes.Value},/*189*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldTotPc",          AttributeId = Attributes.Value},/*190*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldTotOkPc",        AttributeId = Attributes.Value},/*191*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldTotNgPc",        AttributeId = Attributes.Value},/*192*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldLTotOkPc",       AttributeId = Attributes.Value},/*193*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldLTotNgPc",       AttributeId = Attributes.Value},/*194*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldRTotOkPc",       AttributeId = Attributes.Value},/*195*/
                        new ReadValueId() {NodeId = "ns=4;s=dspShldRTotNgPc",       AttributeId = Attributes.Value},/*196*/
                        new ReadValueId() {NodeId = "ns=4;s=shldChckOp",            AttributeId = Attributes.Value},/*197*/
                        new ReadValueId() {NodeId = "ns=4;s=shldUpdtCmd",           AttributeId = Attributes.Value},/*198*/
                        new ReadValueId() {NodeId = "ns=4;s=nutChckOp",             AttributeId = Attributes.Value},/*199*/
                        new ReadValueId() {NodeId = "ns=4;s=nutUpdtCmd",            AttributeId = Attributes.Value},/*200*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmChckOp",         AttributeId = Attributes.Value},/*201*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmUpdtCmd",        AttributeId = Attributes.Value},/*202*/
                        new ReadValueId() {NodeId = "ns=4;s=latchChckOp",           AttributeId = Attributes.Value},/*203*/
                        new ReadValueId() {NodeId = "ns=4;s=latchUpdtCmd",          AttributeId = Attributes.Value},/*204*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutDistance1",       AttributeId = Attributes.Value},/*205*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutForce1",          AttributeId = Attributes.Value},/*206*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutJdg1",            AttributeId = Attributes.Value},/*207*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutLTotPc",          AttributeId = Attributes.Value},/*208*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutLTotOk",          AttributeId = Attributes.Value},/*209*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutLTotNg",          AttributeId = Attributes.Value},/*210*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutDistance2",       AttributeId = Attributes.Value},/*211*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutForce2",          AttributeId = Attributes.Value},/*212*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutJdg2",            AttributeId = Attributes.Value},/*213*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutRTotPc",          AttributeId = Attributes.Value},/*214*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutRTotOk",          AttributeId = Attributes.Value},/*215*/
                        new ReadValueId() {NodeId = "ns=4;s=dspNutRTotNg",          AttributeId = Attributes.Value},/*216*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmTorqueRslt",     AttributeId = Attributes.Value},/*217*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmAngleRslt",      AttributeId = Attributes.Value},/*218*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmForce1Rslt",     AttributeId = Attributes.Value},/*219*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmForce2Rslt",     AttributeId = Attributes.Value},/*220*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmDistance1Rslt",  AttributeId = Attributes.Value},/*221*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmDistance2Rslt",  AttributeId = Attributes.Value},/*222*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmConstKRslt",     AttributeId = Attributes.Value},/*223*/
                        new ReadValueId() {NodeId = "ns=4;s=plngArmPartRslt",       AttributeId = Attributes.Value},/*224*/
                        new ReadValueId() {NodeId = "ns=4;s=latchPartRslt",         AttributeId = Attributes.Value},/*225*/
                        new ReadValueId() {NodeId = "ns=4;s=latchTorqueRslt",       AttributeId = Attributes.Value},/*226*/
                        new ReadValueId() {NodeId = "ns=4;s=latchAngleRslt",        AttributeId = Attributes.Value},/*227*/
                        new ReadValueId() {NodeId = "ns=4;s=shldJudge",             AttributeId = Attributes.Value},/*228*/
                        new ReadValueId() {NodeId = "ns=4;s=nutJudge",              AttributeId = Attributes.Value},/*229*/

                    };
                opcUaGetVarsTask = new Task(new Action(() => { VarOpcUaMonitor(_ctGetVarsTask); }));
                cancelGetVarsTask = new CancellationTokenSource();
                _ctGetVarsTask = cancelGetVarsTask.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        opcUaClient.Session.Read(
                                    null,
                                    0,
                                    TimestampsToReturn.Both,
                                    nodesToRead,
                                    out tempResultValues/*DataValueCollection resultsValues*/,
                                    out DiagnosticInfoCollection diagnosticInfos);

                        validateResponse(tempResultValues, nodesToRead);
                        resultsValues = tempResultValues;
                        if (opcUaGetVarsTask.Status != TaskStatus.Running)
                        {
                            opcUaGetVarsTask.Start();
                        }

                        Thread.Sleep(50);
                    } catch (Exception) { }
                }
                stpOpcUaLog.Dispatcher.Invoke(() => {
                    stpOpcUaLog.Children.Add(new TextBlock() { Text = $"{DateTime.Now}-----Monitor stopped", Foreground = Brushes.DarkRed }); });
                return;
            }
            catch (Exception ex)
            {
                stpOpcUaLog.Dispatcher.Invoke(() => {
                    stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.Now}-----Error message Monitor: {ex.Message}"), Foreground = Brushes.DarkRed});
                });
            }
        }
        private void OpcUaBoolWriter(string nodeId, bool state) {
            try
            {
                lock (opcLock)
                {
                    boolToWrite.Clear();
                    WriteValue boolValue = new WriteValue();
                    boolValue.NodeId = new NodeId(nodeId);
                    boolValue.AttributeId = Attributes.Value;
                    boolValue.Value = new DataValue(true);
                    boolValue.Value.Value = state;
                    boolToWrite.Add(boolValue);
                    StatusCodeCollection results = null;
                    opcUaClient.Session.Write(null, boolToWrite, out results, out DiagnosticInfoCollection diagnosticInfos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de escritura: " + ex.Message);
            }
        }
        private void OpcUaStringWriter(string nodeId, string text)
        {
            try
            {
                lock (opcLock)
                {
                    stringToWrite.Clear();
                    WriteValue stringValue = new WriteValue();
                    stringValue.NodeId = new NodeId(nodeId);
                    stringValue.AttributeId = Attributes.Value;
                    stringValue.Value = new DataValue(true);
                    stringValue.Value.Value = text;
                    stringToWrite.Add(stringValue);
                    StatusCodeCollection results = null;
                    opcUaClient.Session.Write(null, stringToWrite, out results, out DiagnosticInfoCollection diagnosticInfos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de escritura: " + ex.Message);
            }
        }
        private bool OpcUaLimitsWriter()
        {
            bool correctOp;
            float[] limits = new float[34];
            string[] nodeId = new string[34];

            #region collection
            nodeId[0] = "ns=4;s=shldMaxDistance";
            correctOp = float.TryParse(oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MaxDistance], out limits[0]);
            nodeId[1] = "ns=4;s=shldMinDistance";
            correctOp &= float.TryParse(oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MinDistance], out limits[1]);
            nodeId[2] = "ns=4;s=shldMaxForce";
            correctOp &= float.TryParse(oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MaxForce], out limits[2]);
            nodeId[3] = "ns=4;s=shldMinForce";
            correctOp &= float.TryParse(oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MinForce], out limits[3]);
            nodeId[4] = "ns=4;s=nutMaxDistance";
            correctOp &= float.TryParse(oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdX_Max], out limits[4]);
            nodeId[5] = "ns=4;s=nutMinDistance";
            correctOp &= float.TryParse(oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdX_Min], out limits[5]);
            nodeId[6] = "ns=4;s=nutMaxForce";
            correctOp &= float.TryParse(oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdY_Max], out limits[6]);
            nodeId[7] = "ns=4;s=nutMinForce";
            correctOp &= float.TryParse(oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdY_Min], out limits[7]);
            nodeId[8] = "ns=4;s=plngMaxAngle";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Angle_Max], out limits[8]);
            nodeId[9] = "ns=4;s=plngMinAngle";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Angle_Min], out limits[9]);
            nodeId[10] = "ns=4;s=plngMaxTorque";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Torque_Max], out limits[10]);
            nodeId[11] = "ns=4;s=plngMinTorque";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Torque_Min], out limits[11]);
            nodeId[12] = "ns=4;s=plngMaxConstK";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ConstantK_max], out limits[12]);
            nodeId[13] = "ns=4;s=plngMinConstK";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ConstantK_max], out limits[13]);
            nodeId[14] = "ns=4;s=plngMaxTest1Distance";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChXmax], out limits[14]);
            nodeId[15] = "ns=4;s=plngMinTest1Distance";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChXmin], out limits[15]);
            nodeId[16] = "ns=4;s=plngMaxTest2Distance";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChXmax], out limits[16]);
            nodeId[17] = "ns=4;s=plngMinTest2Distance";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChXmin], out limits[17]);
            nodeId[18] = "ns=4;s=plngMaxTest1Force";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChYmax], out limits[18]);
            nodeId[19] = "ns=4;s=plngMinTest1Force";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChYmin], out limits[19]);
            nodeId[20] = "ns=4;s=plngMaxTest2Force";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChYmax], out limits[20]);
            nodeId[21] = "ns=4;s=plngMinTest2Force";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChYmin], out limits[21]);
            nodeId[22] = "ns=4;s=plngTest1Speed";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1Speed], out limits[22]);
            nodeId[23] = "ns=4;s=plngTest1Position";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1Pos], out limits[23]);
            nodeId[24] = "ns=4;s=plngTest2Speed";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2Speed], out limits[24]);
            nodeId[25] = "ns=4;s=plngTest2Position";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2Pos], out limits[25]);
            nodeId[26] = "ns=4;s=plngReadyPosition";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ReadyPos], out limits[26]);
            nodeId[27] = "ns=4;s=plngReadySpeed";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ReadySpeed], out limits[27]);
            nodeId[28] = "ns=4;s=plngInspection1";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.SpringInspection], out limits[28]);
            nodeId[29] = "ns=4;s=plngInspection2";
            correctOp &= float.TryParse(oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ArmatureInspection], out limits[29]);
            nodeId[30] = "ns=4;s=latchMaxAngle";
            correctOp &= float.TryParse(oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Angle_Max], out limits[30]);
            nodeId[31] = "ns=4;s=latchMinAngle";
            correctOp &= float.TryParse(oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Angle_Min], out limits[31]);
            nodeId[32] = "ns=4;s=latchMaxTorque";
            correctOp &= float.TryParse(oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Torque_Max], out limits[32]);
            nodeId[33] = "ns=4;s=latchMinTorque";
            correctOp &= float.TryParse(oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Torque_Min], out limits[33]);
            #endregion

            if (correctOp)
            {
                try
                {
                    lock (opcLock)
                    {
                        realToWrite.Clear();
                        WriteValue realValue = new WriteValue();
                        for (int j = 0; j < 34; j++)
                        {
                            realValue.NodeId = new NodeId(nodeId[j]);
                            realValue.AttributeId = Attributes.Value;
                            realValue.Value = new DataValue(true);
                            realValue.Value.Value = limits[j];
                            realToWrite.Add(realValue);
                        }
                        StatusCodeCollection results = null;
                        opcUaClient.Session.Write(null, realToWrite, out results, out DiagnosticInfoCollection diagnosticInfos);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de escritura: " + ex.Message);
                    return false;
                }
            }
            else return false;

        }
        private async void ConnectionRequest()
        {
            if (sessionConnected)
            {
                stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.UtcNow}-----Connection already"), Foreground = Brushes.DarkRed });
            }
            else
            {
                stpOpcUaLog.Children.Clear();
                stpOpcUaLog.Children.Add(new TextBlock() { Text = ($"{DateTime.UtcNow}-----Connection request"), Foreground = Brushes.Black });
                disconnectCmd = false;
                await Task.Run(() => InitOpcUa());
            }
        }

        #endregion

        #region Methods

        private void DisconnectionRequest()
        {
            if (sessionConnected)
            {
                disconnectCmd = true;
            }
        }
        private void CloseProcess()
        {
            DisconnectionRequest();
            if (getTimeTask != null)
            { cancelGetDateTask.Cancel();}

            if (opcUaGetStatusTask != null)
            { cancelGetStatusTask.Cancel();}

            if (opcUaGetVarsTask != null)
            { cancelGetVarsTask.Cancel();}

        }
        public void ReadMachineConfigFile()
        {
            try
            {
                #region Machine config CSV file
                //Open the Production Data file to query the Device ID loaded
                String List;
                int i = 0;
                //Query the file Log
                StreamReader _File = new StreamReader(pathProdData);
                while ((List = _File.ReadLine()) != null)
                {
                    string[] Data = List.Split(',');
                    //Configuration File
                    cSVfile[i] = Data[1];
                    i++;
                }
                _File.Close();
                #endregion
            }
            catch (Exception a)
            {
                //Message
                //SystemMessages("Error to read the Machine Config CSV file\n", "Error");
                Console.WriteLine(a);
            }
        }
        void InitDatabaseTables()
        {
            #region Common queries for all stations
            //Defines of the Columns of the Database Table
            oDBtableCommon.Columns.Add("Name", typeof(string));
            oDBtableCommon.Columns.Add("Path", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdCommonTbl.DataContext = oDBtableCommon;
            #endregion

            #region Component Traceability
            //Defines of the Columns of the Database Table
            oDBtableScanComp.Columns.Add("Name", typeof(string));
            oDBtableScanComp.Columns.Add("Path", typeof(string));
            //Bind the Table to Data Grid Viewer
            //tbl_DBtables_ScanComp.DataSource = ODBtable_ScanComp;
            #endregion

            #region Install Terminal Shields
            //Defines of the Columns of the Database Table
            oDBtableInstallTermShld.Columns.Add("Name", typeof(string));
            oDBtableInstallTermShld.Columns.Add("Path", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdShldTbl.DataContext = oDBtableInstallTermShld;
            //Defines of the Columns of the Limits Table
            oLimitsTblInstallTermShld.Columns.Add("Limits", typeof(string));
            oLimitsTblInstallTermShld.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdShldLimits.DataContext = oLimitsTblInstallTermShld;
            #endregion

            #region Install Nut Inserts
            //Defines of the Columns of the Database Table
            oDBtableInstallNutInsr.Columns.Add("Name", typeof(string));
            oDBtableInstallNutInsr.Columns.Add("Path", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdNutTbl.DataContext = oDBtableInstallNutInsr;
            //Defines of the Columns of the Limits Table
            oLimitsTblInstallNutInsr.Columns.Add("Limits", typeof(string));
            oLimitsTblInstallNutInsr.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdNutLimits.DataContext = oLimitsTblInstallNutInsr;
            #endregion

            #region Plunger & Armature Assy
            //Defines of the Columns of the Database Table
            oDBtablePlungrArmAssy.Columns.Add("Name", typeof(string));
            oDBtablePlungrArmAssy.Columns.Add("Path", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdPlngrTbl.DataContext = oDBtablePlungrArmAssy;
            //Defines of the Columns of the Limits Table
            oLimitsTblPlungrArmAssy.Columns.Add("Limits", typeof(string));
            oLimitsTblPlungrArmAssy.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdPlngrLimits.DataContext = oLimitsTblPlungrArmAssy;
            #endregion

            #region Latch Assy
            //Defines of the Columns of the Database Table
            oDBtableLatchAssy.Columns.Add("Name", typeof(string));
            oDBtableLatchAssy.Columns.Add("Path", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdLatchTbl.DataContext = oDBtableLatchAssy;
            //Defines of the Columns of the Limits Table
            oLimitsTblLatchAssy.Columns.Add("Limits", typeof(string));
            oLimitsTblLatchAssy.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdLatchLimits.DataContext = oLimitsTblLatchAssy;
            #endregion

        }
        void FillOutDatabaseTables()
        {
            #region Common queries for all stations
            oDBtableCommon.Rows.Clear();
            oDBtableCommon.Rows.Add("Check Components", oSQLServer.TblCheckComponents);
            oDBtableCommon.Rows.Add("Prior Op Check", oSQLServer.TblPriorOpCheck);
            oDBtableCommon.Rows.Add("Assign Components", oSQLServer.TblAssignComponentsToSerial);
            oDBtableCommon.Rows.Add("Update Device Status", oSQLServer.TblUpdateDeviceStatus);
            oDBtableCommon.Rows.Add("Device IDs", oSQLServer.TblDeviceIDs);
            #endregion

            #region Component Traceability
            //ODBtable_ScanComp.Rows.Add("Station Name", OSQLServer.OScanCompQueries.Station);
            //ODBtable_ScanComp.Rows.Add("Station ID", OSQLServer.OScanCompQueries.StationID);
            //ODBtable_ScanComp.Rows.Add("Equipment ID", OSQLServer.OScanCompQueries.EquipmentID);
            //ODBtable_ScanComp.Rows.Add("Scan Component", OSQLServer.TblScanComponents);
            //ODBtable_ScanComp.Rows.Add("Required Component", OSQLServer.TblRequiredComponents);
            #endregion

            #region Install Terminal Shields
            //Database
            oDBtableInstallTermShld.Rows.Clear();
            oDBtableInstallTermShld.Rows.Add("Station Name", oSQLServer.OInstallTermShldQueries.Station);
            oDBtableInstallTermShld.Rows.Add("Station ID", oSQLServer.OInstallTermShldQueries.StationID);
            oDBtableInstallTermShld.Rows.Add("Equipment ID", oSQLServer.OInstallTermShldQueries.EquipmentID);
            oDBtableInstallTermShld.Rows.Add("Limits", oSQLServer.OInstallTermShldQueries.TblLimits);
            oDBtableInstallTermShld.Rows.Add("Store Results", oSQLServer.OInstallTermShldQueries.TblResultsHistory);
            #endregion

            #region Install Nut Inserts
            //Database
            oDBtableInstallNutInsr.Rows.Clear();
            oDBtableInstallNutInsr.Rows.Add("Station Name", oSQLServer.OInstallM8InsrQueries.Station);
            oDBtableInstallNutInsr.Rows.Add("Station ID", oSQLServer.OInstallM8InsrQueries.StationID);
            oDBtableInstallNutInsr.Rows.Add("Equipment ID", oSQLServer.OInstallM8InsrQueries.EquipmentID);
            oDBtableInstallNutInsr.Rows.Add("Limits", oSQLServer.OInstallM8InsrQueries.TblLimits);
            oDBtableInstallNutInsr.Rows.Add("Store Results", oSQLServer.OInstallM8InsrQueries.TblResultsHistory);
            #endregion

            #region Plunger & Armature Assy
            //Database
            oDBtablePlungrArmAssy.Rows.Clear();
            oDBtablePlungrArmAssy.Rows.Add("Station Name", oSQLServer.OPlugrArmAssyQueries.Station);
            oDBtablePlungrArmAssy.Rows.Add("Station ID", oSQLServer.OPlugrArmAssyQueries.StationID);
            oDBtablePlungrArmAssy.Rows.Add("Equipment ID", oSQLServer.OPlugrArmAssyQueries.EquipmentID);
            oDBtablePlungrArmAssy.Rows.Add("Limits", oSQLServer.OPlugrArmAssyQueries.TblLimits);
            oDBtablePlungrArmAssy.Rows.Add("Store Results", oSQLServer.OPlugrArmAssyQueries.TblResultsHistory);
            #endregion

            #region Latch Assy
            //Database
            oDBtableLatchAssy.Rows.Clear();
            oDBtableLatchAssy.Rows.Add("Station Name", oSQLServer.OLatchAssyQueries.Station);
            oDBtableLatchAssy.Rows.Add("Station ID", oSQLServer.OLatchAssyQueries.StationID);
            oDBtableLatchAssy.Rows.Add("Equipment ID", oSQLServer.OLatchAssyQueries.EquipmentID);
            oDBtableLatchAssy.Rows.Add("Limits", oSQLServer.OLatchAssyQueries.TblLimits);
            oDBtableLatchAssy.Rows.Add("Store Results", oSQLServer.OLatchAssyQueries.TblResultsHistory);
            #endregion
        }
        //Install Terminal Shields (Setting up)
        public void ResultsInstallTermShld()
        {
            //Defines of the Columns of the Master parameters Table
            oTblResultsInstallTermShld.Columns.Add("Result", typeof(string));
            oTblResultsInstallTermShld.Columns.Add("Value", typeof(string));

            oTblResultsInstallTermShld.Rows.Add("Part Status");
            oTblResultsInstallTermShld.Rows.Add("Force Left");
            oTblResultsInstallTermShld.Rows.Add("Displacement Left");
            oTblResultsInstallTermShld.Rows.Add("Force Right");
            oTblResultsInstallTermShld.Rows.Add("Displacement Right");

            //Bind the Table to Data Grid Viewer
            grdShldRslt.DataContext = oTblResultsInstallTermShld;
        }
        //Install Nut Inserts (Setting up)
        public void ResultsInstallNutIns()
        {
            //Defines of the Columns of the Master parameters Table
            oTblResultsInstallNutInsr.Columns.Add("Result", typeof(string));
            oTblResultsInstallNutInsr.Columns.Add("Value", typeof(string));

            oTblResultsInstallNutInsr.Rows.Add("Part Status");
            oTblResultsInstallNutInsr.Rows.Add("Force Left");
            oTblResultsInstallNutInsr.Rows.Add("Displacement Left");
            oTblResultsInstallNutInsr.Rows.Add("Force Right");
            oTblResultsInstallNutInsr.Rows.Add("Displacement Right");
            //Bind the Table to Data Grid Viewer
            grdNutRslt.DataContext = oTblResultsInstallNutInsr;
        }
        //Plunger & Armature Assy(Setting up)
        public void ResultsPlugrArmAssy()
        {
            //Defines of the Columns of the Master parameters Table
            oTblResultsPlungrArmAssy.Columns.Add("Result", typeof(string));
            oTblResultsPlungrArmAssy.Columns.Add("Value", typeof(string));

            oTblResultsPlungrArmAssy.Rows.Add("Part Status");
            oTblResultsPlungrArmAssy.Rows.Add("Torque");
            oTblResultsPlungrArmAssy.Rows.Add("Angle");
            oTblResultsPlungrArmAssy.Rows.Add("Clamp");
            oTblResultsPlungrArmAssy.Rows.Add("Seating Point");
            oTblResultsPlungrArmAssy.Rows.Add("Clamp Angle");
            oTblResultsPlungrArmAssy.Rows.Add("Test1 Force");
            oTblResultsPlungrArmAssy.Rows.Add("Test1 Displacement");
            oTblResultsPlungrArmAssy.Rows.Add("Test2 Force");
            oTblResultsPlungrArmAssy.Rows.Add("Test2 Displacement");
            oTblResultsPlungrArmAssy.Rows.Add("Constant K");
            //Bind the Table to Data Grid Viewer
            grdPlngrRslt.DataContext = oTblResultsPlungrArmAssy;
        }
        //Latch Assy (Setting up)
        public void ResultsLatchAssys()
        {
            //Defines of the Columns of the Master parameters Table
            oTblResultsLatchAssy.Columns.Add("Result", typeof(string));
            oTblResultsLatchAssy.Columns.Add("Value", typeof(string));

            oTblResultsLatchAssy.Rows.Add("Part Status");
            oTblResultsLatchAssy.Rows.Add("Torque");
            oTblResultsLatchAssy.Rows.Add("Angle");
            oTblResultsLatchAssy.Rows.Add("Clamp");
            oTblResultsLatchAssy.Rows.Add("Seating Point");
            oTblResultsLatchAssy.Rows.Add("Clamp Angle");
            //Bind the Table to Data Grid Viewer
            grdLatchRslt.DataContext = oTblResultsLatchAssy;
        }
        //Install Terminal Shields (Add)
        public void GetLimitsInstallTermShld()
        {
            //Database
            oLimitsTblInstallTermShld.Rows.Clear();
            oLimitsTblInstallTermShld.Rows.Add("Max Distance", oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MaxDistance]);
            oLimitsTblInstallTermShld.Rows.Add("Min Distance", oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MinDistance]);
            oLimitsTblInstallTermShld.Rows.Add("Max Force", oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MaxForce]);
            oLimitsTblInstallTermShld.Rows.Add("Min Force", oSQLServer.OInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.MinForce]);
        }
        //Install M8 Inserts (Add)
        public void GetLimitsInstallNutIns()
        {
            //Database
            oLimitsTblInstallNutInsr.Rows.Clear();
            oLimitsTblInstallNutInsr.Rows.Add("Displacement Max", oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdX_Max]);
            oLimitsTblInstallNutInsr.Rows.Add("Displacement Min", oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdX_Min]);
            oLimitsTblInstallNutInsr.Rows.Add("Force Max", oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdY_Max]);
            oLimitsTblInstallNutInsr.Rows.Add("Force Min", oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdY_Min]);
            oLimitsTblInstallNutInsr.Rows.Add("Vision Recipe", oSQLServer.OInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.VisionRecipe]);
        }
        //Plunger & Armature Assy (Add)
        public void GetLimitsPlugrArmAssy()
        {
            //Database
            oLimitsTblPlungrArmAssy.Rows.Clear();
            oLimitsTblPlungrArmAssy.Rows.Add("Angle Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Angle_Max]);
            oLimitsTblPlungrArmAssy.Rows.Add("Angle Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Angle_Min]);
            oLimitsTblPlungrArmAssy.Rows.Add("Torque Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Torque_Max]);
            oLimitsTblPlungrArmAssy.Rows.Add("Torque Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Torque_Min]);
            oLimitsTblPlungrArmAssy.Rows.Add("Clamp Angle Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampAngle_Max]);
            oLimitsTblPlungrArmAssy.Rows.Add("Clamp Angle Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampAngle_Min]);
            oLimitsTblPlungrArmAssy.Rows.Add("Clamp Torque Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampTorque_Max]);
            oLimitsTblPlungrArmAssy.Rows.Add("Clamp Torque Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampTorque_Min]);
            oLimitsTblPlungrArmAssy.Rows.Add("Seating Point Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.SP_Max]);
            oLimitsTblPlungrArmAssy.Rows.Add("Seating Point Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.SP_Min]);
            oLimitsTblPlungrArmAssy.Rows.Add("Constant K Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ConstantK_max]);
            oLimitsTblPlungrArmAssy.Rows.Add("Constant K Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ConstantK_min]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test1 ChX Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChXmax]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test1 ChX Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChXmin]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test1 ChY Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChYmax]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test1 ChY Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChYmin]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test1 Position", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1Pos]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test1 Speed", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1Speed]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test2 ChX Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChXmax]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test2 ChX Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChXmin]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test2 ChY Max", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChYmax]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test2 ChY Min", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChYmin]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test2 Position", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2Pos]);
            oLimitsTblPlungrArmAssy.Rows.Add("Test2 Speed", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2Speed]);
            oLimitsTblPlungrArmAssy.Rows.Add("Standby Position", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.StandbyPos]);
            oLimitsTblPlungrArmAssy.Rows.Add("Standby Speed", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.StandbySpeed]);
            oLimitsTblPlungrArmAssy.Rows.Add("Ready Position", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ReadyPos]);
            oLimitsTblPlungrArmAssy.Rows.Add("Ready Speed", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ReadySpeed]);
            oLimitsTblPlungrArmAssy.Rows.Add("Screw Position", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ScrewdrivingPos]);
            oLimitsTblPlungrArmAssy.Rows.Add("Screw Speed", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ScrewdrivingSpeed]);
            oLimitsTblPlungrArmAssy.Rows.Add("Armature Inspection", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ArmatureInspection]);
            oLimitsTblPlungrArmAssy.Rows.Add("Spring Inspection", oSQLServer.OPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.SpringInspection]);
        }
        //Latch Assy (Add)
        public void GetLimitsLatchAssys()
        {
            //Database
            oLimitsTblLatchAssy.Rows.Clear();
            oLimitsTblLatchAssy.Rows.Add("Angle Max", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Angle_Max]);
            oLimitsTblLatchAssy.Rows.Add("Angle Min", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Angle_Min]);
            oLimitsTblLatchAssy.Rows.Add("Torque Max", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Torque_Max]);
            oLimitsTblLatchAssy.Rows.Add("Torque Min", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Torque_Min]);
            oLimitsTblLatchAssy.Rows.Add("Clamp Angle Max", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampAngle_Max]);
            oLimitsTblLatchAssy.Rows.Add("Clamp Angle Min", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampAngle_Min]);
            oLimitsTblLatchAssy.Rows.Add("Clamp Torque Max", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampTorque_Max]);
            oLimitsTblLatchAssy.Rows.Add("Clamp Torque Min", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampTorque_Min]);
            oLimitsTblLatchAssy.Rows.Add("Seating Point Max", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.SP_Max]);
            oLimitsTblLatchAssy.Rows.Add("Seating Point Min", oSQLServer.OLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.SP_Min]);
        }
        //Install Terminal Shields (Setting up)
        public void PriorOPInstallTermShld()
        {
            //Defines of the Columns of the Master parameters Table
            oTblPriorOPInstallTermShld.Columns.Add("Parameter", typeof(string));
            oTblPriorOPInstallTermShld.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdShldPriorOp.DataContext = oTblPriorOPInstallTermShld;
        }
        public void GetPriorOPInstallTermShld(string MessageText, string LastStation, string PartStatus)
        {
            //Database
            oTblPriorOPInstallTermShld.Rows.Clear();
            oTblPriorOPInstallTermShld.Rows.Add("Message text", MessageText);
            oTblPriorOPInstallTermShld.Rows.Add("Last Station", LastStation);
            oTblPriorOPInstallTermShld.Rows.Add("Part Status", PartStatus);
        }
        //Install Nut Inserts (Setting up)
        public void PriorOPInstallNutIns()
        {
            //Defines of the Columns of the Master parameters Table
            oTblPriorOPInstallNutInsr.Columns.Add("Parameter", typeof(string));
            oTblPriorOPInstallNutInsr.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdNutPriorOp.DataContext = oTblPriorOPInstallNutInsr;
        }
        public void GetPriorOPInstallNutIns(string MessageText, string LastStation, string PartStatus)
        {
            //Database
            oTblPriorOPInstallNutInsr.Rows.Clear();
            oTblPriorOPInstallNutInsr.Rows.Add("Message text", MessageText);
            oTblPriorOPInstallNutInsr.Rows.Add("Last Station", LastStation);
            oTblPriorOPInstallNutInsr.Rows.Add("Part Status", PartStatus);
        }
        //Plunger & Armature Assy(Setting up)
        public void PriorOPPlugrArmAssy()
        {
            //Defines of the Columns of the Master parameters Table
            oTblPriorOPPlungrArmAssy.Columns.Add("Parameter", typeof(string));
            oTblPriorOPPlungrArmAssy.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdPlngrPriorOp.DataContext = oTblPriorOPPlungrArmAssy;
        }
        public void GetPriorOPPlugrArmAssy(string MessageText, string LastStation, string PartStatus)
        {
            //Database
            oTblPriorOPPlungrArmAssy.Rows.Clear();
            oTblPriorOPPlungrArmAssy.Rows.Add("Message text", MessageText);
            oTblPriorOPPlungrArmAssy.Rows.Add("Last Station", LastStation);
            oTblPriorOPPlungrArmAssy.Rows.Add("Part Status", PartStatus);
        }
        //Latch Assy (Setting up)
        public void PriorOPLatchAssy()
        {
            //Defines of the Columns of the Master parameters Table
            oTblPriorOPLatchAssy.Columns.Add("Parameter", typeof(string));
            oTblPriorOPLatchAssy.Columns.Add("Value", typeof(string));
            //Bind the Table to Data Grid Viewer
            grdLatchPriorOp.DataContext = oTblPriorOPLatchAssy;
        }
        public void GetPriorOPLatchAssy(string MessageText, string LastStation, string PartStatus)
        {
            //Database
            oTblPriorOPLatchAssy.Rows.Clear();
            oTblPriorOPLatchAssy.Rows.Add("Message text", MessageText);
            oTblPriorOPLatchAssy.Rows.Add("Last Station", LastStation);
            oTblPriorOPLatchAssy.Rows.Add("Part Status", PartStatus);
        }
        private async Task<bool> NewLot()
        {
            bool done = false;
            bool correctQty = false;

            deviceID = "";
            dspDeviceID.Dispatcher.Invoke(() => { deviceID = dspDeviceID.Text; });
            lotID = "";
            dspLotID.Dispatcher.Invoke(() => { lotID = dspLotID.Text; });
            qty = 0;
            correctQty = dspQty.Dispatcher.Invoke(bool () => { return Int32.TryParse(dspQty.Text, out qty); });

            if (lotID == "" || lotID.Length > 20 ||
                qty == 0 || deviceID == "") 
            {
                stpDBMsg.Dispatcher.Invoke(() => {
                    stpDBMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Parámetros incorrectos."),
                        Foreground = Brushes.DarkRed 
                    });
                });
            }
            else
            {
                stpDBMsg.Dispatcher.Invoke(() => {
                    stpDBMsg.Children.Clear();
                    stpDBMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Obteniendo límites..."),
                        Foreground = Brushes.Black
                    });
                });

                if (oSQLServer.InstallTermShldLimitsDB(deviceID)) 
                {
                    done = true;
                    GetLimitsInstallTermShld();
                }
                else
                    stpDBMsg.Dispatcher.Invoke(() =>
                    {
                        stpDBMsg.Children.Add(new TextBlock()
                        {
                            Text = ($"{DateTime.Now}-----Error al obtener límites de terminal shield."),
                            Foreground = Brushes.DarkRed
                        });
                    });

                if (done) done &= oSQLServer.InstallM8InsrLimitsDB(deviceID);
                if (done) GetLimitsInstallNutIns();
                else
                    stpDBMsg.Dispatcher.Invoke(() =>
                    {
                        stpDBMsg.Children.Add(new TextBlock()
                        {
                            Text = ($"{DateTime.Now}-----Error al obtener límites de inserción de nut."),
                            Foreground = Brushes.DarkRed
                        });
                    });

                if (done) done &= oSQLServer.PlugrArmAssyLimitsDB(deviceID);
                if (done) GetLimitsPlugrArmAssy();
                else
                    stpDBMsg.Dispatcher.Invoke(() =>
                    {
                        stpDBMsg.Children.Add(new TextBlock()
                        {
                            Text = ($"{DateTime.Now}-----Error al obtener límites plunger y armature."),
                            Foreground = Brushes.DarkRed
                        });
                    });

                if (done) done &= oSQLServer.LatchAssyLimitsDB(deviceID);
                if (done) GetLimitsLatchAssys();
                else
                    stpDBMsg.Dispatcher.Invoke(() =>
                    {
                        stpDBMsg.Children.Add(new TextBlock()
                        {
                            Text = ($"{DateTime.Now}-----Error al obtener límites inserción de latch."),
                            Foreground = Brushes.DarkRed
                        });
                    });
                done = true;
                if (done)
                    done &= await Task.Run(OpcUaLimitsWriter);
                else
                    stpDBMsg.Dispatcher.Invoke(() =>
                    {
                        stpDBMsg.Children.Add(new TextBlock()
                        {
                            Text = ($"{DateTime.Now}-----Error de envío de límites a controlador."),
                            Foreground = Brushes.DarkRed
                        });
                    });
                if (done)
                    stpDBMsg.Dispatcher.Invoke(() =>
                    {
                        stpDBMsg.Children.Add(new TextBlock()
                        {
                            Text = ($"{DateTime.Now}-----Lote {lotID} cargado correctamente."),
                            Foreground = Brushes.Black
                        });
                    });

            }
            return done;
        }
        private async void ShieldCheckOp()
        {
            Int64 serialNumber = 0;
            bool cnctDone = false;
            bool correctSerial = false;

            correctSerial = this.Dispatcher.Invoke(bool () => 
            {
                return Int64.TryParse(dspShldSerialNo.Text, out serialNumber);
            });

            if (!correctSerial)
            {
                stpShieldMsg.Children.Clear();
                stpShieldMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Número serial incorrecto"),
                    Foreground = Brushes.DarkRed
                });
                return;
            }

            cnctDone = oSQLServer.CheckComponents(oSQLServer.OInstallTermShldQueries, deviceID);
            if (cnctDone && oSQLServer.CheckComponents_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpShieldMsg.Children.Clear();
                    dspShldChckComp.Text = oSQLServer.CheckComponents_Msg;
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Revisión de componentes correcta."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                stpShieldMsg.Children.Clear();
                dspShldChckComp.Text = oSQLServer.CheckComponents_Msg;
                stpShieldMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                    Foreground = Brushes.Black
                });
                stpShieldMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Revisión de componentes incorrecta."),
                    Foreground = Brushes.DarkRed
                });
            }

            if (cnctDone) cnctDone &= oSQLServer.PriorOpCheck(oSQLServer.OInstallTermShldQueries, serialNumber, deviceID);

            if (cnctDone && oSQLServer.PriorOpChk_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op correcto."),
                        Foreground = Brushes.Black
                    });
                });
                GetPriorOPInstallTermShld(oSQLServer.POChkMsg, oSQLServer.POChkMsg2, oSQLServer.POChkMsg3);
            }
            else
            {
                stpShieldMsg.Dispatcher.Invoke(() =>
                {
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op incorrecto."),
                        Foreground = Brushes.DarkRed
                    });
                });
            }
            
            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=shldChckOpRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=shldChckOpRslt", "NOT GOOD"); }); }
        }
        private async void ShieldUpdateCommand()
        {
            bool cnctDone = false;
            Int64 serialNumber = 0;
            object[] value1 = new object[5];
            object[] value2 = new object[5];

            value1[(int)eInstallTermShld_Prod_Results.PartStatus] = oTblResultsInstallTermShld.Rows[(int)eInstallTermShld_Prod_Results.PartStatus][1];
            for (int i = 0; i < 5; i++) 
            {
                value2[i] = oTblResultsInstallTermShld.Rows[i][1];
            }

            dspShldSerialNo.Dispatcher.Invoke(() => { serialNumber = Int64.Parse(dspShldSerialNo.Text); });
            
            cnctDone = oSQLServer.AssignComponentsToSerial(oSQLServer.OInstallTermShldQueries, serialNumber, deviceID, lotID);

            if (cnctDone && oSQLServer.AssignComponentsToSerial_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspShldAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspShldAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial incorrecta."),
                        Foreground = Brushes.DarkRed
                    });
                });

            if (cnctDone) 
                cnctDone &= oSQLServer.UpdateDeviceStatus(oSQLServer.OInstallTermShldQueries, serialNumber, deviceID, value1[(int)eInstallTermShld_Prod_Results.PartStatus].ToString());
            if (cnctDone && oSQLServer.UpdateDevice_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspShldUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspShldUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo incorrecta."),
                        Foreground = Brushes.Black
                    });
                });

            if (cnctDone)
            {
                cnctDone &= oSQLServer.InstallTermShldResultsDB(serialNumber, deviceID, lotID,
                     value2[(int)eInstallTermShld_Prod_Results.PartStatus].ToString(),
                     value2[(int)eInstallTermShld_Prod_Results.ThresholdX_Max].ToString(),
                     value2[(int)eInstallTermShld_Prod_Results.ThresholdX_Min].ToString(),
                     value2[(int)eInstallTermShld_Prod_Results.ThresholdY_Max].ToString(),
                     value2[(int)eInstallTermShld_Prod_Results.ThresholdY_Min].ToString());
                this.Dispatcher.Invoke(() =>
                {
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados correctamente."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpShieldMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados incorrectamente."),
                        Foreground = Brushes.DarkRed
                    });
                });

            }

            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=shldUpdtRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=shldUpdtRslt", "NOT GOOD"); }); }

        }
        private async void NutCheckOp()
        {
            Int64 serialNumber = 0;
            bool cnctDone = false;
            bool correctSerial = false;

            correctSerial = this.Dispatcher.Invoke(bool () =>
            {
                return Int64.TryParse(dspNutLSerialNo.Text, out serialNumber);
            });

            if (!correctSerial)
            {
                stpNutLMsg.Children.Clear();
                stpNutLMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Número serial incorrecto"),
                    Foreground = Brushes.DarkRed
                });
                return;
            }

            cnctDone = oSQLServer.CheckComponents(oSQLServer.OInstallM8InsrQueries, deviceID);
            if (cnctDone && oSQLServer.CheckComponents_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpNutLMsg.Children.Clear();
                    dspNutChckComp.Text = oSQLServer.CheckComponents_Msg;
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Revisión de componentes correcta."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                stpNutLMsg.Children.Clear();
                dspNutChckComp.Text = oSQLServer.CheckComponents_Msg;
                stpNutLMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                    Foreground = Brushes.Black
                });
                stpNutLMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Revisión de componentes incorrecta."),
                    Foreground = Brushes.DarkRed
                });
            }

            if (cnctDone) cnctDone &= oSQLServer.PriorOpCheck(oSQLServer.OInstallM8InsrQueries, serialNumber, deviceID);

            if (cnctDone && oSQLServer.PriorOpChk_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op correcto."),
                        Foreground = Brushes.Black
                    });
                });
                GetPriorOPInstallNutIns(oSQLServer.POChkMsg, oSQLServer.POChkMsg2, oSQLServer.POChkMsg3);
            }
            else
            {
                stpNutLMsg.Dispatcher.Invoke(() =>
                {
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op incorrecto."),
                        Foreground = Brushes.DarkRed
                    });
                });
            }

            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=nutChckOpRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=nutChckOpRslt", "NOT GOOD"); }); }
        }
        private async void NutUpdateCommand()
        {
            bool cnctDone = false;
            Int64 serialNumber = 0;
            object[] value1 = new object[5];
            object[] value2 = new object[5];

            value1[(int)eInstallM8Insr_Prod_Results.PartStatus] = oTblResultsInstallNutInsr.Rows[(int)eInstallM8Insr_Prod_Results.PartStatus][1];
            for (int i = 0; i < 5; i++)
            {
                value2[i] = oTblResultsInstallNutInsr.Rows[i][1];
            }

            dspNutLSerialNo.Dispatcher.Invoke(() => { serialNumber = Int64.Parse(dspNutLSerialNo.Text); });

            cnctDone = oSQLServer.AssignComponentsToSerial(oSQLServer.OInstallM8InsrQueries, serialNumber, deviceID, lotID);

            if (cnctDone && oSQLServer.AssignComponentsToSerial_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspNutAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspNutAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial incorrecta."),
                        Foreground = Brushes.DarkRed
                    });
                });

            if (cnctDone)
                cnctDone &= oSQLServer.UpdateDeviceStatus(oSQLServer.OInstallM8InsrQueries, serialNumber, deviceID, value1[(int)eInstallM8Insr_Prod_Results.PartStatus].ToString());
            if (cnctDone && oSQLServer.UpdateDevice_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspNutUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspNutUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo incorrecta."),
                        Foreground = Brushes.Black
                    });
                });

            if (cnctDone)
            {
                cnctDone &= oSQLServer.InstallM8InsrResultsDB(serialNumber, deviceID, lotID,
                     value2[(int)eInstallM8Insr_Prod_Results.PartStatus].ToString(),
                     value2[(int)eInstallM8Insr_Prod_Results.Force_Left].ToString(),
                     value2[(int)eInstallM8Insr_Prod_Results.Distance_Left].ToString(),
                     value2[(int)eInstallM8Insr_Prod_Results.Force_Right].ToString(),
                     value2[(int)eInstallM8Insr_Prod_Results.Distance_Right].ToString());
                this.Dispatcher.Invoke(() =>
                {
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados correctamente."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpNutLMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados incorrectamente."),
                        Foreground = Brushes.DarkRed
                    });
                });

            }

            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=nutUpdtRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=nutUpdtRslt", "NOT GOOD"); }); }

        }
        private async void PlngArmCheckOp()
        {
            Int64 serialNumber = 0;
            bool cnctDone = false;
            bool correctSerial = false;

            correctSerial = this.Dispatcher.Invoke(bool () =>
            {
                return Int64.TryParse(dspPlngArmSerialNo.Text, out serialNumber);
            });

            if (!correctSerial)
            {
                stpPlngArmMsg.Children.Clear();
                stpPlngArmMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Número serial incorrecto"),
                    Foreground = Brushes.DarkRed
                });
                return;
            }

            cnctDone = oSQLServer.CheckComponents(oSQLServer.OPlugrArmAssyQueries, deviceID);
            if (cnctDone && oSQLServer.CheckComponents_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpPlngArmMsg.Children.Clear();
                    dspPlngrChckComp.Text = oSQLServer.CheckComponents_Msg;
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Revisión de componentes correcta."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                stpPlngArmMsg.Children.Clear();
                dspPlngrChckComp.Text = oSQLServer.CheckComponents_Msg;
                stpPlngArmMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                    Foreground = Brushes.Black
                });
                stpPlngArmMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Revisión de componentes incorrecta."),
                    Foreground = Brushes.DarkRed
                });
            }

            if (cnctDone) cnctDone &= oSQLServer.PriorOpCheck(oSQLServer.OPlugrArmAssyQueries, serialNumber, deviceID);

            if (cnctDone && oSQLServer.PriorOpChk_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op correcto."),
                        Foreground = Brushes.Black
                    });
                });
                GetPriorOPPlugrArmAssy(oSQLServer.POChkMsg, oSQLServer.POChkMsg2, oSQLServer.POChkMsg3);
            }
            else
            {
                stpPlngArmMsg.Dispatcher.Invoke(() =>
                {
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op incorrecto."),
                        Foreground = Brushes.DarkRed
                    });
                });
            }

            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=nutChckOpRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=nutChckOpRslt", "NOT GOOD"); }); }
        }
        private async void PlngArmUpdateCommand()
        {
            bool cnctDone = false;
            Int64 serialNumber = 0;
            object[] value1 = new object[5];
            object[] value2 = new object[11];

            value1[(int)ePlugrArmAssy_Prod_Results.PartStatus] = oTblResultsPlungrArmAssy.Rows[(int)ePlugrArmAssy_Prod_Results.PartStatus][1];
            for (int i = 0; i < 11; i++)
            {
                value2[i] = oTblResultsPlungrArmAssy.Rows[i][1];
            }

            dspPlngArmSerialNo.Dispatcher.Invoke(() => { serialNumber = Int64.Parse(dspPlngArmSerialNo.Text); });

            cnctDone = oSQLServer.AssignComponentsToSerial(oSQLServer.OPlugrArmAssyQueries, serialNumber, deviceID, lotID);

            if (cnctDone && oSQLServer.AssignComponentsToSerial_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspPlngrAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspPlngrAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial incorrecta."),
                        Foreground = Brushes.DarkRed
                    });
                });

            if (cnctDone)
                cnctDone &= oSQLServer.UpdateDeviceStatus(oSQLServer.OPlugrArmAssyQueries, serialNumber, deviceID, value1[(int)ePlugrArmAssy_Prod_Results.PartStatus].ToString());
            if (cnctDone && oSQLServer.UpdateDevice_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspPlngrUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspPlngrUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo incorrecta."),
                        Foreground = Brushes.Black
                    });
                });

            if (cnctDone)
            {
                cnctDone &= oSQLServer.PlugrArmAssyResultsDB(serialNumber, deviceID, lotID,
                                 value2[(int)ePlugrArmAssy_Prod_Results.PartStatus].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.Torque].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.Angle].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.Clamp].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.SP].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.ClampAngle].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.Test1_Force].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.Test1_Displacement].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.Test2_Force].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.Test2_Displacement].ToString(),
                                 value2[(int)ePlugrArmAssy_Prod_Results.ConstK].ToString());
                this.Dispatcher.Invoke(() =>
                {
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados correctamente."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpPlngArmMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados incorrectamente."),
                        Foreground = Brushes.DarkRed
                    });
                });

            }

            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=plngArmUpdtRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=plngArmUpdtRslt", "NOT GOOD"); }); }

        }
        private async void LatchCheckOp()
        {
            Int64 serialNumber = 0;
            bool cnctDone = false;
            bool correctSerial = false;

            correctSerial = this.Dispatcher.Invoke(bool () =>
            {
                return Int64.TryParse(dspLatchSerialNo.Text, out serialNumber);
            });

            if (!correctSerial)
            {
                stpLatchMsg.Children.Clear();
                stpLatchMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Número serial incorrecto"),
                    Foreground = Brushes.DarkRed
                });
                return;
            }

            cnctDone = oSQLServer.CheckComponents(oSQLServer.OLatchAssyQueries, deviceID);
            if (cnctDone && oSQLServer.CheckComponents_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpLatchMsg.Children.Clear();
                    dspLatchChckComp.Text = oSQLServer.CheckComponents_Msg;
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Revisión de componentes correcta."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                stpLatchMsg.Children.Clear();
                dspLatchChckComp.Text = oSQLServer.CheckComponents_Msg;
                stpLatchMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----{oSQLServer.CheckComponents_Msg}"),
                    Foreground = Brushes.Black
                });
                stpLatchMsg.Children.Add(new TextBlock()
                {
                    Text = ($"{DateTime.Now}-----Revisión de componentes incorrecta."),
                    Foreground = Brushes.DarkRed
                });
            }

            if (cnctDone) cnctDone &= oSQLServer.PriorOpCheck(oSQLServer.OLatchAssyQueries, serialNumber, deviceID);

            if (cnctDone && oSQLServer.PriorOpChk_Return == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op correcto."),
                        Foreground = Brushes.Black
                    });
                });
                GetPriorOPLatchAssy(oSQLServer.POChkMsg, oSQLServer.POChkMsg2, oSQLServer.POChkMsg3);
            }
            else
            {
                stpLatchMsg.Dispatcher.Invoke(() =>
                {
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Prior Op incorrecto."),
                        Foreground = Brushes.DarkRed
                    });
                });
            }

            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=latchChckOpRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=latchChckOpRslt", "NOT GOOD"); }); }
        }
        private async void LatchUpdateCommand()
        {
            bool cnctDone = false;
            Int64 serialNumber = 0;
            object[] value1 = new object[5];
            object[] value2 = new object[6];

            value1[(int)eLatchAssy_Prod_Results.PartStatus] = oTblResultsLatchAssy.Rows[(int)eLatchAssy_Prod_Results.PartStatus][1];
            for (int i = 0; i < 11; i++)
            {
                value2[i] = oTblResultsLatchAssy.Rows[i][1];
            }

            dspLatchSerialNo.Dispatcher.Invoke(() => { serialNumber = Int64.Parse(dspLatchSerialNo.Text); });

            cnctDone = oSQLServer.AssignComponentsToSerial(oSQLServer.OLatchAssyQueries, serialNumber, deviceID, lotID);

            if (cnctDone && oSQLServer.AssignComponentsToSerial_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspLatchAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspLatchAssingComp.Text = oSQLServer.AssignComponentsToSerial_Msg;
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.AssignComponentsToSerial_Msg}"),
                        Foreground = Brushes.Black
                    });
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Asignación de componentes a serial incorrecta."),
                        Foreground = Brushes.DarkRed
                    });
                });

            if (cnctDone)
                cnctDone &= oSQLServer.UpdateDeviceStatus(oSQLServer.OLatchAssyQueries, serialNumber, deviceID, value1[(int)eLatchAssy_Prod_Results.PartStatus].ToString());
            if (cnctDone && oSQLServer.UpdateDevice_Return == 0)
                this.Dispatcher.Invoke(() =>
                {
                    dspLatchUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo correcta."),
                        Foreground = Brushes.Black
                    });
                });
            else
                this.Dispatcher.Invoke(() =>
                {
                    dspLatchUpdtDvcSts.Text = oSQLServer.UpDevStatMsg;
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----{oSQLServer.UpDevStatMsg}"),
                        Foreground = Brushes.Black
                    });
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Actualización de dispositivo incorrecta."),
                        Foreground = Brushes.Black
                    });
                });

            if (cnctDone)
            {
                cnctDone &= oSQLServer.LatchAssyResultsDB(serialNumber, deviceID, lotID,
                                 value2[(int)eLatchAssy_Prod_Results.PartStatus].ToString(),
                                 value2[(int)eLatchAssy_Prod_Results.Torque].ToString(),
                                 value2[(int)eLatchAssy_Prod_Results.Angle].ToString(),
                                 value2[(int)eLatchAssy_Prod_Results.Clamp].ToString(),
                                 value2[(int)eLatchAssy_Prod_Results.SP].ToString(),
                                 value2[(int)eLatchAssy_Prod_Results.ClampAngle].ToString());
                this.Dispatcher.Invoke(() =>
                {
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados correctamente."),
                        Foreground = Brushes.Black
                    });
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    stpLatchMsg.Children.Add(new TextBlock()
                    {
                        Text = ($"{DateTime.Now}-----Resultados cargados incorrectamente."),
                        Foreground = Brushes.DarkRed
                    });
                });

            }

            if (cnctDone) { await Task.Run(() => { OpcUaStringWriter("ns=4;s=latchUpdtRslt", "GOOD"); }); }
            else { await Task.Run(() => { OpcUaStringWriter("ns=4;s=latchUpdtRslt", "NOT GOOD"); }); }

        }

        #endregion

        private async void ClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await Task.Run(() => { CloseProcess(); });
            if(markInfo != null && markInfo.IsActive)
            {
                markInfo.Close();
            }
            MessageBox.Show("Opc Ua disconnected.");
        }
        private void MnuConnectClick(object sender, RoutedEventArgs e)
        {
            ConnectionRequest();
        }
        private void MnuDisconnectClick(object sender, RoutedEventArgs e)
        {
            DisconnectionRequest();
        }
        private void BtnConnectClick(object sender, RoutedEventArgs e)
        {
            ConnectionRequest();
        }
        private void BtnDisconnectClick(object sender, RoutedEventArgs e)
        {
            DisconnectionRequest();
        }
        private void MnuExitClick(object sender, RoutedEventArgs e)
        {
            CloseProcess();
            this.Close();
        }

        #region Buttons HMI
        private async void BtnTrans1HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans1Home", true); });
            } 
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }
        private async void BtnTrans1HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans1Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }
        private async void BtnRbt1HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRbt1Home", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }
        private async void BtnRbt1HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRbt1Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnInletCvHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnInletCvHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnInletCvHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnInletCvHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void btnPressShldPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnPressShldHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnPressShldRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnPressShldHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnPlaceShieldPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnInsrtShldHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnPlaceShieldRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnInsrtShldHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnPlacePlugHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnInsrtPlugHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnPlacePlugHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnInsrtPlugHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnTrans2HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans2Home", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnTrans2HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans2Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnTrans4HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans4Home", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnTrans4HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans4Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnTrans5HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans5Home", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnTrans5HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans5Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnTrans6HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans6Home", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnTrans6HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnTrans6Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnNutPressHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnNutPressHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnNutPressHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnNutPressHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnNutLoadHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnNutLoadHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnNutLoadHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnNutLoadHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnRotaryHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRotaryHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnRotaryHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRotaryHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnServoHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnServoHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnServoHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnServoHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnRbt2HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRbt2Home", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnRbt2HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRbt2Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnRbt3HomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRbt3Home", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnRbt3HomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnRbt3Home", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnLoaderPlngHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLoaderPlngHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnLoaderPlngHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLoaderPlngHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnLatchHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLatchHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnLatchHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLatchHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnBoltHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnBoltHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnBoltHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnBoltHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnSpringHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnSpringHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnSpringHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnSpringHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnLatchPressHomePress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLatchPressHome", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnLatchPressHomeRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLatchPressHome", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnStp1InPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStp1In", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnStp1InRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStp1In", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnStpr1OutPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStpr1Out", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnStpr1OutRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStpr1Out", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnStpr2InPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStpr2In", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnStpr2InRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStpr2In", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnStpr2OutPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStpr2Out", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnStpr2OutRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnStpr2Out", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldAInPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldAIn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldAInRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldAIn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldAOutPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldAOut", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldAOutRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldAOut", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldBInPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldBIn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldBInRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldBIn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldBOutPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldBOut", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldBOutRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldBOut", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldCInPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldCIn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnHoldCInRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldCIn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldCOutPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldCOut", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnHoldCOutRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnHoldCOut", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private void BtnS0EjectInPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS0EjectInRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS0EjectOutPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS0EjectOutRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnRbt1OpnPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnRbt1OpnRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnRbt1ClsPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnRbt1ClsRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS1GrpInPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS1GrpInRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS1GrpOutPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS1GrpOutRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS2GrpInPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS2GrpInRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS2GrpOutPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS2GrpOutRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnT1LimitLPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnT1LimitRPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnT1LimitLRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnResetPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnResetRelease(object sender, MouseButtonEventArgs e)
        {

        }

        #endregion

        private void BtnT1LimitRRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnT1LoadInPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnT1LoadInRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnT1LoadOutPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnT1LoadOutRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3GrpOpnPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3GrpOpnRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3GrpClsPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3GrpClsRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3LoadInPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3LoadInRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3LoadOutPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3LoadOutRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3Gyre0Press(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3Gyre180Release(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS3Gyre180Press(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpNOpnPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpNOpnRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpNClsPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpNClsRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpROpnPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpROpnRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpRClsPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpRClsRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpLOpnPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpLOpnRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpLClsPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpLClsRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4Gyre0Press(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4Gyre0Release(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4Gyre90Press(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4Gyre90Release(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4UploadPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4UploadRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4DownloadPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4DownloadRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LimitLPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LimitLRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LimitRPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LimitRRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LoadInPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LoadInRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LoadOutPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4LoadOutRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpTOpnPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpTOpnRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpTClsPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS4GrpTClsRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private async void BtnS5PressRUpPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressRUp", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnS5PressRUpRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressRUp", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5PressRDwnPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressRDwn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5PressRDwnRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressRDwn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5PressLUpPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressLUp", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5PressLUpRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressLUp", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5PressLDwnPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressLDwn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5PressLDwnRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5PressLDwn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5GrpTOpnPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpTOpn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnS5GrpTOpnRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpTOpn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnS5GrpTClsPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpTCls", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5GrpTClsRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpTCls", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5LoadInPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5LoadIn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5LoadInRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5LoadIn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5LoadOutPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5LoadOut", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnS5LoadOutRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5LoadOut", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5Gyre0Press(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5Gyre0", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnS5Gyre0Release(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5Gyre0", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5Gyre180Press(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5Gyre180", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5Gyre180Release(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5Gyre180", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5GrpNOpnPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpNOpn", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5GrpNOpnRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpNOpn", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnS5GrpNClsPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpNCls", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnS5GrpNClsRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnS5GrpNCls", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private void BtnS6EjectorDwnPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS6EjectorDwnRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS6EjectorUpPress(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnS6EjectorUpRelease(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnShwMrkInfo(object sender, RoutedEventArgs e)
        {
            if(sessionConnected)
            {
                markInfo = new MarkInfo(this);
                markInfo.Show();
            }
        }

        private async void BtnDeviceIDPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnDeviceID", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnDeviceIDRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnDeviceID", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnLotIDPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLotID", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnLotIDRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnLotID", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }

        }

        private async void BtnQtyPress(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnQty", true); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnQtyRelease(object sender, MouseButtonEventArgs e)
        {
            if (sessionConnected)
            {
                await Task.Run(() => { OpcUaBoolWriter("ns=4;s=btnQty", false); });
            }
            else
            {
                MessageBox.Show("There is not OPC UA connection.", "Error OPC UA");
            }
        }

        private async void BtnNewLot(object sender, RoutedEventArgs e)
        {
            if (sessionConnected)
            {
                btnNewLot.IsEnabled = false;
                newLotSucc = await Task.Run(NewLot); 
                if (newLotSucc)
                {
                    btnEndLot.IsEnabled = true;
                    btnNewLot.IsEnabled = false;
                    btnDeviceID.IsEnabled = false;
                    btnLotID.IsEnabled = false;
                    btnQty.IsEnabled = false;

                    barShldTotPc.Maximum = int.Parse(dspQty.Text);
                    barNutLTotPc.Maximum = int.Parse(dspQty.Text);
                    barNutRTotPc.Maximum = int.Parse(dspQty.Text);


                    await Task.Run(() => { OpcUaBoolWriter("ns=4;s=newLotReady", true); });

                    //dspSelectedDvcID.Text = dspDeviceID.Text;
                    //dspSelectedLotID.Text = dspSelectedLotID.Text;
                    //dspSelectedQty.Text = dspQty.Text;
                }
                else
                {
                    btnEndLot.IsEnabled = false;
                    btnNewLot.IsEnabled = true;
                    btnDeviceID.IsEnabled = true;
                    btnLotID.IsEnabled = true;
                    btnQty.IsEnabled = true;

                    await Task.Run(() => { OpcUaBoolWriter("ns=4;s=newLotReady", false); });

                    dspSelectedDvcID.Text = "";
                    dspSelectedLotID.Text = "";
                    dspSelectedQty.Text = "";

                }
            }
            else { MessageBox.Show("Error: No existe conexión OPC UA."); }
        }
    }
}
