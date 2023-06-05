using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace GigavacFuseApp
{
    internal class OpcUaClient : IDisposable
    {
        #region Constructors

        public OpcUaClient(StackPanel messagePanel, ApplicationConfiguration configuration, Action<IList, IList> validateResponse)
        {
            txtLog = messagePanel;
            uaConfiguration = configuration;
            uaValidateResponse = validateResponse;
            uaConfiguration.CertificateValidator.CertificateValidation += CertificateValidation;
        }

        #endregion

        #region Private fields

        private object _lock = new object();
        private ApplicationConfiguration uaConfiguration;
        private SessionReconnectHandler uaReconnectHandler;
        private Session uaSession;
        private readonly Action<IList, IList> uaValidateResponse;
        private StackPanel txtLog;
        private delegate void SafeTextBlockCall(string txt, Brush brush);

        #endregion

        #region Control access methods

        private void MessageLogCaller(string text, Brush brush)
        {
            if (!txtLog.CheckAccess())
            {
                SafeTextBlockCall myTxtLogSafeCall = new SafeTextBlockCall(MessageLogCaller);
                txtLog.Dispatcher.Invoke(myTxtLogSafeCall, new object[] { text, brush });
            }
            else
            {
                txtLog.Children.Add(new TextBlock() { Text = text, Foreground = brush });
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Utils.SilentDispose(uaSession);
            uaConfiguration.CertificateValidator.CertificateValidation -= CertificateValidation;
        }

        #endregion

        #region Public properties

        Action<IList, IList> ValidateResponse => uaValidateResponse;
        public Session Session => uaSession;
        public int KeepAliveInterval { get; set; } = 5000;
        public int ReconnectedPeriod { get; set; } = 5000;
        public uint SessionLifeTime { get; set; } = 0;
        public IUserIdentity UserIdentity { get; set; } = new UserIdentity();
        public bool AutoAccept { get; set; } = false;

        #endregion

        #region Public Methods

        public async Task<bool> ConnectAsync(string serverUrl, bool useSecurity = false)
        {
            if (serverUrl == null) throw new ArgumentNullException(nameof(serverUrl));
            try
            {
                if (uaSession != null && uaSession.Connected == true) { MessageLogCaller($"{DateTime.Now}   Session already connected", Brushes.DarkRed); }
                else
                {
                    MessageLogCaller($"{DateTime.Now}-----Connecting to: {serverUrl}", Brushes.Black);
                    EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(
                        uaConfiguration,
                        serverUrl,
                        useSecurity);
                    EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(uaConfiguration);
                    ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                    var session = await Opc.Ua.Client.Session.Create(
                        uaConfiguration,
                        endpoint,
                        true,
                        false,
                        uaConfiguration.ApplicationName,
                        SessionLifeTime,
                        UserIdentity,
                        null).ConfigureAwait(true);

                    if (session != null && session.Connected)
                    {
                        uaSession = session;
                        uaSession.KeepAliveInterval = KeepAliveInterval;
                        uaSession.KeepAlive += Session_KeepAlive;
                    }
                    MessageLogCaller($"{DateTime.Now}-----New session created with SessionName:{uaSession.SessionName}", Brushes.Black);
                }
                return true;
            }
            catch (Exception ex) { MessageLogCaller($"{DateTime.Now}-----Create session error: {ex.Message}", Brushes.DarkRed); return false; }
        }

        public void Disconnect()
        {
            try
            {
                if (uaSession != null)
                {
                    MessageLogCaller($"{DateTime.Now}-----Disconecting...", Brushes.Black);
                    lock (_lock)
                    {
                        uaSession.KeepAlive -= Session_KeepAlive;
                        uaReconnectHandler?.Dispose();
                    }

                    uaSession.Close();
                    uaSession.Dispose();
                    uaSession = null;
                }
                else { MessageLogCaller($"{DateTime.Now}-----Session no created", Brushes.Black); }
            }
            catch (Exception ex) { MessageLogCaller($"{DateTime.Now}-----Disconnect error: {ex.Message}", Brushes.DarkRed); }

        }

        private void Session_KeepAlive(ISession session, KeepAliveEventArgs e)
        {
            try
            {
                if (!Object.ReferenceEquals(session, uaSession))
                { return; }

                if (ServiceResult.IsBad(e.Status))
                {
                    if (ReconnectedPeriod <= 0)
                    {
                        Utils.LogWarning($"KeepAlive status {e.Status}, but reconnect is disabled.");
                        return;
                    }
                    lock (_lock)
                    {
                        if (uaReconnectHandler == null)
                        {
                            Utils.LogInfo($"KeepAlive status {e.Status}, reconnecting in {ReconnectedPeriod} ms.");
                            MessageLogCaller($"{DateTime.Now}   Reconnecting... {e.Status}", Brushes.Black);
                            uaReconnectHandler = new SessionReconnectHandler(true);
                            uaReconnectHandler.BeginReconnect(uaSession, ReconnectedPeriod, Client_ReconnectComplete);
                        }
                        else
                        {
                            Utils.LogInfo($"KeepAlive status {e.Status}, reconnect in progess.");
                        }
                    }
                    return;
                }
            }
            catch (Exception ex) { Utils.LogError(ex, "Error in OnKeepAlive"); }
        }

        private void Client_ReconnectComplete(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(sender, uaReconnectHandler)) { return; }
            lock (_lock)
            {
                if (uaReconnectHandler != null)
                {
                    uaSession = uaReconnectHandler.Session as Session;
                }
                uaReconnectHandler.Dispose();
                uaReconnectHandler = null;
            }
            MessageLogCaller($"{DateTime.Now}-----Reconnected", Brushes.DarkGreen);
        }

        #endregion

        #region Protected Method

        protected virtual void CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            bool certificateAccepted = false;

            ServiceResult error = e.Error;
            Console.WriteLine(error);
            if (error.StatusCode == StatusCodes.BadCertificateUntrusted && AutoAccept)
            {
                certificateAccepted = true;
            }
            if (certificateAccepted)
            {
                MessageLogCaller($"{DateTime.Now}-----Untrusted certificate accepted. Subject {e.Certificate.Subject}", Brushes.Black);
                e.Accept = true;
            }
            else
            {
                MessageLogCaller($"{DateTime.Now}-----Untrusted certificate rejected. Subject {e.Certificate.Subject}", Brushes.DarkRed);
            }
        }

        #endregion

    }
}
