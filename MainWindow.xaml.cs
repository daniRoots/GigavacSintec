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

            //this.WindowState = WindowState.Maximized;

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

            cancellGetDateTask = new CancellationTokenSource();
            _ctGetDateTask = cancellGetDateTask.Token;
            getTimeTask = Task.Run(new Action(() => { GetDate(_ctGetDateTask); }));

            cancellGetStatusTask = new CancellationTokenSource();
            _ctGetStatusTask = cancellGetStatusTask.Token;
            opcUaGetStatusTask = Task.Run(/*new Action(*/() => { GetOpcUAState(_ctGetStatusTask); }/*)*/);

        }

        #region Private fields

        #region Opc UA fields
        private OpcUaClient opcUaClient;
        private ReadValueIdCollection nodesToRead;
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
        #endregion

        #region Tasks control
        private Task getTimeTask;
        private CancellationTokenSource cancellGetDateTask;
        private CancellationToken _ctGetDateTask;

        private Task opcUaGetStatusTask;
        private CancellationTokenSource cancellGetStatusTask;
        private CancellationToken _ctGetStatusTask;

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
                DateTime dateTime = DateTime.Now;
                dspDate.Dispatcher.Invoke(() => { dspDate.Text = dateTime.ToString(); });
            }
            return;
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

                    Thread.Sleep(100);
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
                    };
                while (!token.IsCancellationRequested)
                {
                    opcUaClient.Session.Read(
                                null,
                                0,
                                TimestampsToReturn.Both,
                                nodesToRead,
                                out DataValueCollection resultsValues,
                                out DiagnosticInfoCollection diagnosticInfos);

                    validateResponse(resultsValues, nodesToRead);
                    //Añadir lectura de variables

                    dspDeviceID.Dispatcher.Invoke(() => { dspDeviceID.Text = (string)resultsValues[0].Value; });
                    dspLotID.Dispatcher.Invoke(() => { dspLotID.Text = (string)resultsValues[1].Value; });
                    dspQty.Dispatcher.Invoke(() => { dspQty.Text = (string)resultsValues[2].Value; });
                    dspSelectedDvcID.Dispatcher.Invoke(() => { dspDeviceID.Text = (string)resultsValues[3].Value; });
                    dspSelectedLotID.Dispatcher.Invoke(() => { dspLotID.Text = (string)resultsValues[4].Value; });
                    dspSelectedQty.Dispatcher.Invoke(() => { dspQty.Text = (string)resultsValues[5].Value; });

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
                    if ((bool)resultsValues[9].Value == true)
                    {
                        lmpStopr1Opn.Dispatcher.Invoke(() => { lmpStopr1Opn.Fill = brushGreen; });
                    }
                    else
                    {
                        lmpStopr1Opn.Dispatcher.Invoke(() => { lmpStopr1Opn.Fill = brushGray; });
                    }

                    //lmpStoper2In
                    if ((bool)resultsValues[10].Value == true)
                    {
                        lmpStopr2Opn.Dispatcher.Invoke(() => { lmpStopr2Opn.Fill = brushGreen; });
                    }
                    else
                    {
                        lmpStopr2Opn.Dispatcher.Invoke(() => { lmpStopr2Opn.Fill = brushGray; });
                    }

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
                    if ((bool)resultsValues[28].Value == true)
                    {
                        lmpS2GrpCls.Dispatcher.Invoke(() => { lmpS2GrpCls.Fill = brushGreen; });
                    }
                    else
                    {
                        lmpS2GrpCls.Dispatcher.Invoke(() => { lmpS2GrpCls.Fill = brushGray; });
                    }

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

                    #endregion

                    Thread.Sleep(50);
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
            {
                cancellGetDateTask.Cancel();
            }
            if (opcUaGetStatusTask != null)
            {
                cancellGetStatusTask.Cancel();
            }

        }
        #endregion

        private void ClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseProcess();
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
    }
}
