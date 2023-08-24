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
using System.Windows.Shapes;

namespace GigavacFuseApp
{
    /// <summary>
    /// Lógica de interacción para MarkInfo.xaml
    /// </summary>
    public partial class MarkInfo : Window
    {
        public MarkInfo()
        {
            InitializeComponent();
        }

        public MarkInfo(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            if (_mainWindow.resultsValues[168].Value != null
                && _mainWindow.resultsValues[169].Value != null
                && _mainWindow.resultsValues[170].Value != null) 
            {
                cancelGetDataTask = new CancellationTokenSource();
                _ctGetDataTask = cancelGetDataTask.Token;
                getData = Task.Run(() => { GetData(_ctGetDataTask); });
            }
        }

        private MainWindow _mainWindow;
        private Task getData;
        private CancellationTokenSource cancelGetDataTask;
        private CancellationToken _ctGetDataTask;

        private void GetData(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                dspMrkData.Dispatcher.Invoke(() => { dspMrkData.Text = _mainWindow.resultsValues[168].Value.ToString(); });
                dspReadData.Dispatcher.Invoke(() => { dspReadData.Text = _mainWindow.resultsValues[169].Value.ToString(); });
                dspMrkOldData.Dispatcher.Invoke(() => { dspMrkOldData.Text = _mainWindow.resultsValues[170].Value.ToString(); });

                Thread.Sleep(50);
            }
        }
        private void BtnClsMrkInfo(object sender, RoutedEventArgs e)
        {
            if(_ctGetDataTask.CanBeCanceled)
            {
                cancelGetDataTask.Cancel();
                cancelGetDataTask.Dispose();
            }
            this.Close();
        }
    }
}
