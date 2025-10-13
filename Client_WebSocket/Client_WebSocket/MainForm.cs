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
using System.Windows.Forms.DataVisualization.Charting;
using Client_WebSocket.CalculationMethods;
using Client_WebSocket.Models;
using NLog;

namespace Client_WebSocket
{
    public partial class MainForm : Form
    {
        private Logger loggerMainForm = LogManager.GetCurrentClassLogger();
        private CalculationData calcData;
        private int sleepTime;
        private DateTime timeWorkingDateStart = DateTime.Today.AddHours(8);
        private DateTime timeWorkingDateEnd = DateTime.Today.AddHours(24);
        private int countElements = 0;
        private TimeSpan timeSpan;
        private int colorIndex = 0;

        private Color[] distinctColors =
        {
            Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple, Color.Teal, Color.Magenta, Color.Lime,
            Color.Brown, Color.Pink,
            Color.Cyan, Color.Gold, Color.Navy, Color.Maroon, Color.Olive, Color.IndianRed, Color.LightCoral,
            Color.Salmon, Color.DarkSalmon,
            Color.LightSalmon, Color.Crimson, Color.Firebrick, Color.DarkRed, Color.DeepPink, Color.HotPink,
            Color.LightPink, Color.PaleVioletRed,
            Color.MediumVioletRed, Color.Coral, Color.Tomato, Color.OrangeRed, Color.DarkOrange, Color.Goldenrod,
            Color.DarkGoldenrod, Color.RosyBrown,
            Color.Sienna, Color.SaddleBrown, Color.Chocolate, Color.Peru, Color.SandyBrown, Color.BurlyWood, Color.Tan,
            Color.Wheat,
            Color.NavajoWhite, Color.Bisque, Color.BlanchedAlmond, Color.Cornsilk, Color.LemonChiffon,
            Color.LightGoldenrodYellow, Color.PapayaWhip,
            Color.Moccasin, Color.PeachPuff, Color.PaleGoldenrod, Color.Khaki, Color.DarkKhaki, Color.Lavender,
            Color.Thistle, Color.Plum,
            Color.Violet, Color.Orchid, Color.MediumOrchid, Color.MediumPurple, Color.BlueViolet, Color.DarkViolet,
            Color.DarkOrchid, Color.DarkMagenta
        };

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
            Task.Run(async () =>
            {
                try
                {
                    bool isConnected = CheckInternetConnection();
                    if (isConnected)
                    {
                        sleepTime = 180000;
                        calcData = new CalculationData(sleepTime, timeWorkingDateStart, timeWorkingDateEnd,
                            true);
                        calcData.DataResponse += OnDataResponse;
                        await calcData.DataCreationAsync();
                    }
                    else
                    {
                        sleepTime = 30000;
                        calcData = new CalculationData(sleepTime, timeWorkingDateStart, timeWorkingDateEnd,
                            false);
                        calcData.DataResponse += OnDataResponse;
                        await calcData.DataCreationAsync();
                    }
                }
                catch (Exception ex)
                {
                    loggerMainForm.Error($"{ex.Message}");
                }
            });
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

        private void OnDataResponse(List<ResponseBankModel> _responseModels)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<List<ResponseBankModel>>(OnDataResponse), _responseModels);
                return;
            }

            DisplayDataInChart(_responseModels);
            loggerMainForm.Info($"Получено {_responseModels.Count} записей с сервера");
        }

        private void DisplayDataInChart(List<ResponseBankModel> _responseModels)
        {
            try
            {
                if (DateTime.Now >= timeWorkingDateStart && DateTime.Now <= timeWorkingDateEnd)
                {
                    countElements = _responseModels.Count;
                    var letterCode = _responseModels.Select(lc => lc.LetterCode).ToList();
                    chbxSeriesGraph.ItemCheck += (s, e) =>
                    {
                        if (e.Index >= 0 && e.Index < chartPrintData.Series.Count)
                        {
                            chartPrintData.Series[e.Index].Enabled = (e.NewValue == CheckState.Checked);
                        }
                    };
                    foreach (var name in letterCode)
                    {
                        if (chartPrintData.Series.Any(n => n.Name == name))
                        {
                            continue;
                        }

                        var series = new Series(name)
                        {
                            ChartType = SeriesChartType.Line,
                            Color = GetColorSeries(),
                            MarkerStyle = MarkerStyle.Circle,
                            MarkerSize = 8,
                            Enabled = true,
                            IsXValueIndexed = false,
                            IsValueShownAsLabel = true,
                        };
                        chartPrintData.Series.Add(series);
                        chbxSeriesGraph.Items.Add(name, true);
                        loggerMainForm.Info($"Создано {chartPrintData.Series.Count} серий");
                        btnCheckedAll.Enabled = true;
                        btnUnCheckedAll.Enabled = true;
                    }

                    foreach (var data in _responseModels)
                    {
                        var series = chartPrintData.Series[data.LetterCode];
                        TimeSpan interval = TimeSpan.FromMinutes(ConversionToMinutes());
                        DateTime xValue = DateTime.Now.AddMinutes(series.Points.Count * interval.TotalMinutes);
                        double yValue = Convert.ToDouble(data.Rate);
                        DataPoint point = new DataPoint(xValue.ToOADate(), yValue)
                        {
                            MarkerStyle = MarkerStyle.Circle,
                            MarkerSize = 8,
                            MarkerColor = series.Color,
                        };
                        series.Points.Add(point);
                        loggerMainForm.Info($"Отрисована точка {xValue} со значением {yValue}");
                    }

                    SettingsChart();
                    loggerMainForm.Info($"Отрисованы точки на графике и настроено отображение");
                }
            }
            catch (Exception ex)
            {
                loggerMainForm.Error(ex.Message);
            }
        }

        private void SettingsChart()
        {
            var chGraphAreas = chartPrintData.ChartAreas[0];

            //отключаем автоматическое положение начала отсчета и задаем интервалы
            chGraphAreas.AxisX.IsMarginVisible = false;
            chGraphAreas.AxisX.Minimum = timeWorkingDateStart.ToOADate();
            chGraphAreas.AxisX.Maximum = timeWorkingDateEnd.ToOADate();
            chGraphAreas.AxisX.LabelStyle.Format = "HH:mm";
            chGraphAreas.AxisY.Minimum = 0;
            chGraphAreas.AxisY.Maximum = 250;

            //настройка интервала
            chGraphAreas.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            //chGraphAreas.AxisX.Interval = 30;
            chGraphAreas.AxisX.Interval = ConversionToMinutes();
            chGraphAreas.AxisY.Interval = 30;

            //настройка сетки отображения данных
            chGraphAreas.BackColor = Color.White;
            chGraphAreas.BackGradientStyle = GradientStyle.None;
            chGraphAreas.BackSecondaryColor = Color.White;
            chGraphAreas.AxisX.MajorGrid.LineColor = Color.LightGray;
            chGraphAreas.AxisY.MajorGrid.LineColor = Color.LightGray;
            chGraphAreas.AxisX.MajorGrid.LineWidth = 1;
            chGraphAreas.AxisY.MajorGrid.LineWidth = 1;
            chGraphAreas.AxisX.LineColor = Color.Gray;
            chGraphAreas.AxisY.LineColor = Color.Gray;
            chGraphAreas.AxisX.MajorTickMark.LineColor = Color.Gray;
            chGraphAreas.AxisY.MajorTickMark.LineColor = Color.Gray;
            chGraphAreas.AxisX.LabelStyle.ForeColor = Color.Black;
            chGraphAreas.AxisY.LabelStyle.ForeColor = Color.Black;
            chGraphAreas.AxisX.TitleForeColor = Color.Black;
            chGraphAreas.AxisY.TitleForeColor = Color.Black;

            //настройка курсоров и подписи к осям
            chGraphAreas.AxisX.Title = "Время корректировки";
            chGraphAreas.AxisY.Title = "Цена валюты (руб)";
            chGraphAreas.Name = "График курса валют ЦБ РФ";
            chGraphAreas.CursorX.IsUserEnabled = true;
            chGraphAreas.CursorY.IsUserEnabled = true;
        }

        private int ConversionToMinutes()
        {
            timeSpan = TimeSpan.FromMilliseconds(sleepTime);
            return (int)timeSpan.TotalMinutes;
        }

        private Color GetColorSeries()
        {
            return distinctColors[colorIndex++];
        }
    }
}