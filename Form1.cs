using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            string ipAddress = txtIpAddress.Text;
            lstResult.Items.Clear();
            progressBar.Value = 0;
            progressBar.Maximum = 65535; // Установить максимальное значение прогресса

            await Task.Run(() => ScanPorts(ipAddress));
        }

        private async Task ScanPorts(string ipAddress)
        {
            int openPortsCount = 0;
            int totalPorts = 65535;
            ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 100 };

            Parallel.For(1, totalPorts + 1, parallelOptions, async port =>
            {
                bool isOpen = await IsPortOpen(ipAddress, port);
                if (isOpen)
                {
                    Invoke(new Action(() => lstResult.Items.Add($"Port {port} is open.")));
                    openPortsCount++;
                }

                // Обновляем прогрессбар
                Invoke(new Action(() => progressBar.Value++));
            });
        }

        private async Task<bool> IsPortOpen(string ipAddress, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var connectTask = client.ConnectAsync(ipAddress, port);
                    var timeoutTask = Task.Delay(200); // 200 ms timeout
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    if (completedTask == connectTask)
                    {
                        await connectTask; // re-throw exception if connectTask faulted
                        return true;
                    }
                }
            }
            catch
            {
                // Ignored
            }
            return false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
