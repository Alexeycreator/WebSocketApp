using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            cmbxVariableData.Items.Add("Данные по умолчанию");
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
                    calcData = new CalculationData(3000, DateTime.Today.AddHours(8), DateTime.Today.AddHours(17));
                    calcData.DataCreation();
                }
                catch (Exception ex)
                {
                    loggerMainForm.Error($"{ex.Message}");
                }
            });

            parserThread.Start();
        }
    }
}