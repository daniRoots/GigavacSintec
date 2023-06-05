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

            this.WindowState = WindowState.Maximized;

            whiteGradient.Color = Color.FromArgb(255, 255, 255, 255);
            whiteGradient.Offset = 0.50;
            greenTopGradient.Color = Color.FromArgb(255, 30, 250, 0);
            greenTopGradient.Offset = 1;
            greenBottomGradient.Color = Color.FromArgb(255, 30, 250, 0);
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
                Uri serverUrl = new Uri("opc.tcp://192.168.250.10:4840");
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
                        new ReadValueId() {NodeId = "ns=4;s=lmpArray", AttributeId = Attributes.Value },
                        new ReadValueId() {NodeId = "ns=4;s=countData", AttributeId = Attributes.Value },
                        new ReadValueId() {NodeId = "ns=4;s=intValue", AttributeId = Attributes.Value }
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
    }
}
