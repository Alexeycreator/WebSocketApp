using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client_WebSocket.CalculationMethods;
using NLog;

namespace Client_WebSocket
{
    public partial class MainForm : Form
    {
        private Logger loggerMainForm = LogManager.GetCurrentClassLogger();
        private CalculationData calcData;
        private int sleepTime;
        private DateTime timeWorkingDateStart = DateTime.Today.AddHours(8);
        private DateTime timeWorkingDateEnd = DateTime.Today.AddHours(17);
        private int countElements = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitElements();
        }

        private void InitElements()
        {
            cmbxVariableData.Items.Add("Получение данных");
        }

        private void cmbxVariableData_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParserThread();
        }

        private void ParserThread()
        {
            Thread parserThread = new Thread(() =>
            {
                try
                {
                    bool isConnected = CheckInternetConnection();
                    if (isConnected)
                    {
                        sleepTime = 180000;
                        calcData = new CalculationData(sleepTime, timeWorkingDateStart, timeWorkingDateEnd,
                            true);
                        calcData.DataCreation();
                    }
                    else
                    {
                        sleepTime = 30000;
                        calcData = new CalculationData(sleepTime, timeWorkingDateStart, timeWorkingDateEnd,
                            false);
                        calcData.DataCreation();
                    }
                }
                catch (Exception ex)
                {
                    loggerMainForm.Error($"{ex.Message}");
                }
            });

            parserThread.Start();
        }

        private static bool CheckInternetConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 3000); // Google DNS, таймаут 3 секунды
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private void btnCheckedAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < countElements; i++)
            {
                chbxSeriesGraph.SetItemCheckState(i, CheckState.Checked);
            }

            loggerMainForm.Info($"Добавлено выделение для всех элементов на графике");
        }

        private void btnUnCheckedAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < countElements; i++)
            {
                chbxSeriesGraph.SetItemCheckState(i, CheckState.Unchecked);
            }

            loggerMainForm.Info($"Снято выделение со всех элементов на графике");
        }
    }
}