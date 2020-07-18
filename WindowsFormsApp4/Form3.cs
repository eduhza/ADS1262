using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ArduinoUploader;
using ArduinoUploader.Hardware;
using Cyotek.Collections.Generic;
using MathNet.Numerics;
using ScottPlot;

namespace BioMensurae
{
    public partial class Form3 : Form
    {

        #region Variables

        System.Windows.Forms.Timer aTimer;
        
        Thread Serial_thread; //Made it global to finish tread on close app event

        DateTime tSample = DateTime.Now;
        
        private SerialPort MySerial = new SerialPort();
        
        string pathCalib = @"CalibConfig.txt";

        string[] Ports_avaliable = SerialPort.GetPortNames();

        List<double> 
            Dados = new List<double>(),
            AmostraKG = new List<double>(),
            AmostraMV = new List<double>(),
            AmostraLida = new List<double>(),
            LinRegEq = new List<double>();

        List<string> Leitura = new List<string>();

        List<int> Channel = new List<int>();

        bool
            //SampleMode = false,
            //PrimeiraLeitura = false,
            DataReceived = false,
            isTare = false,
            isRecording = false,
            //isSampling = false,
            isCalibrating = false,
            FyChart,
            MxChart,
            MzChart,
            isFirstSample = true, 
            isMoved = false;
            //CalibInProgress = false;

        volatile bool
            RecordTrigger = false;

        int nSample = 0,
            nSens = 3,
            RecordTime = 1,
            nSamples = 0, //time to record in seconds;
            Numer_of_COMPorts = 0,
            nCalibSample = 0;

        static readonly int bit = 32;
        
        static readonly double 
            VREF = 2.5,
            resolution_mV = (VREF / Math.Pow(2, (bit - 1))) * 1000;

        double 
            calibPeso = 0,
            coordX = 0,
            coordZ = 0,
            Ch1Mean = 0,
            Ch2Mean = 0,
            Ch3Mean = 0,
            Ch1Tara,
            Ch2Tara,
            Ch3Tara,
            Gravity = 9.80665,
            SampleRate = 1000,
            ChartYsize = 80,
            LastXmax,
            LastXmin,
            LastYmax,
            LastYmin,
            xMin,
            xMax,
            yMin,
            yMax;

        CircularBuffer<double>
            Ch1CircularBuffer,
            Ch2CircularBuffer,
            Ch3CircularBuffer,
            Ch1RecordBuffer,
            Ch2RecordBuffer,
            Ch3RecordBuffer,
            Ch1TareCircularBuffer,
            Ch2TareCircularBuffer,
            Ch3TareCircularBuffer;

        List<byte>
            SerialSlideBytes = new List<byte>(),
            byteDivisor = new List<byte>() { 13, 13, 13, 13 }; //uControler /r /r /r /r;

        Stopwatch RecordWatch = new Stopwatch();

        #endregion

        public Form3()
        {
            InitializeComponent();
            UpdateSizeBuffers("ChartBuffer", 30000);
            ScottPlotRaw();
            CopPlotRaw(0, 0, 50, 50);

            Timer_config();
        }
        void UpdateSizeBuffers(string circBuffer, int n)
        {
            if (circBuffer == "ChartBuffer")
            {
                SerialSlideBytes.Clear();// = new CircularBuffer<byte>(20*25*2);
                Ch1CircularBuffer = new CircularBuffer<double>(n); //Creates a circular buffer with n items 
                Ch2CircularBuffer = new CircularBuffer<double>(n); //Creates a circular buffer with n items
                Ch3CircularBuffer = new CircularBuffer<double>(n); //Creates a circular buffer with n items 
                Ch1TareCircularBuffer = new CircularBuffer<double>(2000);
                Ch2TareCircularBuffer = new CircularBuffer<double>(2000);
                Ch3TareCircularBuffer = new CircularBuffer<double>(2000);
                nSamples = 0; //Reset number of received samples
            }
            if (circBuffer == "RecordBuffer")
            {
                Ch1RecordBuffer = new CircularBuffer<double>(n);
                Ch2RecordBuffer = new CircularBuffer<double>(n);
                Ch3RecordBuffer = new CircularBuffer<double>(n);
                //RecordSamples = 0;
            }
        }

        void Timer_config()
        {
            aTimer = new System.Windows.Forms.Timer { Interval = 100 };
            aTimer.Tick += new EventHandler(OnTimedEvent);
            aTimer.Start();
        }

        private void OnTimedEvent(object source, EventArgs e)
        {
            if (MySerial.IsOpen)
            {
                //if (isRecording)
                //{
                //    Blink_Label(StatusText);
                //}

                if (Ch1CircularBuffer.Size > 200)
                {
                    UpdateUI_Live_Chart();
                }
            }
            else
            {
                Ports_avaliable = SerialPort.GetPortNames();
                if (Ports_avaliable.Length != Numer_of_COMPorts || (Ports_avaliable.Length > 0 && SerialPortsBox.Text == ""))
                {
                    SerialPortsBox.Items.Clear();
                    SerialPortsBox.Items.AddRange(Ports_avaliable);
                    Numer_of_COMPorts = Ports_avaliable.Length;

                    if (Numer_of_COMPorts > 0)
                    {
                        SerialPortsBox.Text = Ports_avaliable[0];
                        if (TipoBox.Text != "")
                        {
                            Config_Button.Enabled = true;
                        }
                        else
                        {
                            Config_Button.Enabled = false;
                        }
                    }
                    else
                    {
                        SerialPortsBox.Text = null;
                        Config_Button.Enabled = false;
                    }
                    SerialPortsBox.Enabled = true;
                }
            }
        }

        void UpdateUI_Live_Chart()
        {
            double[] Ch1array = Ch1CircularBuffer.ToArray();
            double[] Ch2array = Ch2CircularBuffer.ToArray();
            double[] Ch3array = Ch3CircularBuffer.ToArray();

            UpdateScottPlot(Ch1array, Ch2array, Ch3array);

            int MeanSize = 200;
            double Ch1Sum = 0, Ch2Sum = 0, Ch3Sum = 0;

            int[] minimo = new int[] { Ch1array.Length, Ch2array.Length, Ch3array.Length };
            int min = minimo.Min();

            for (int i = min - 1; i >= min - 200; i--)
            {
                Ch1Sum += Ch1array[i];
                Ch2Sum += Ch2array[i];
                Ch3Sum += Ch3array[i];
            }
            Ch1Mean = Ch1Sum / MeanSize;
            Ch2Mean = Ch2Sum / MeanSize;
            Ch3Mean = Ch3Sum / MeanSize;

            if (nSens == 1) 
            { 
                SetText(FyLeituraBox, "Fy: " + Ch1Mean.ToString("#00.000") + " mV");
                SetText(MxLeituraBox, "");
                SetText(MzLeituraBox, "");
            }
            else
            {
                SetText(FyLeituraBox, "Fy: " + Ch1Mean.ToString("#00.000") + " mV"); 
                SetText(MxLeituraBox, "Mx: " + Ch2Mean.ToString("#00.000") + " mV"); 
                SetText(MzLeituraBox, "Mz: " + Ch3Mean.ToString("#00.000") + " mV"); 
            }
        }

        private delegate void SetTextDelegate(TextBox tb, string Text); //protected delegate void
        private void SetText(TextBox tb, string Text) //protected void
        {
            if (tb.InvokeRequired)
            {
                try
                {
                    tb.Invoke(new SetTextDelegate(SetText), tb, Text);
                    return;
                }
                catch { return; }
            }
            tb.Text = Text;
        }

        void UpdateCalibLog(string texto)
        {
            this.BeginInvoke(new Action(() =>
            {
                CalibLogTextBox.AppendText(texto + Environment.NewLine);
            }));
        }



        List<double> UpdateLinRegEquation(double[] KG, double[] MV)
        {
            List<double> linregTemp = new List<double>();
            if (KG.Count() > 2)
            {
                double[] p = Fit.Polynomial(MV, KG, 1);
                double rSqr = GoodnessOfFit.RSquared(MV.Select(x => p[1] * x + p[0]), KG);
                linregTemp.Add(p[1]);
                linregTemp.Add(p[0]);
                linregTemp.Add(rSqr);
                //this.BeginInvoke(new Action(() =>
                //{
                //    if (nSens == 1)
                //        EquationBox.Text = "Fy: " + Math.Round(linregTemp[0], 5) + "x + (" + Math.Round(linregTemp[1], 5) + ") [Kg] --> " + "R2: " + Math.Round(linregTemp[2], 5);
                //    //EquationBox.Text = "Fy: " + Math.Round(SLOPE, 5) + "x + (" + Math.Round(INTERCEPT, 5) + ") [Kg] --> " + "R2: " + Math.Round(RSQR, 5);
                //    else if (nSens == 3)
                //    {
                //        if (FyCheckBox.Checked)
                //            EquationBox.Text = "Fy: " + Math.Round(linregTemp[0], 5) + "x + (" + Math.Round(linregTemp[1], 5) + ") [Kg] --> " + "R2: " + Math.Round(linregTemp[2], 5);
                //        if (MxCheckBox.Checked)
                //            EquationBox.Text = "Mx: " + Math.Round(linregTemp[0], 5) + "x + (" + Math.Round(linregTemp[1], 5) + ") [Kg] --> " + "R2: " + Math.Round(linregTemp[2], 5);
                //        if (MzCheckBox.Checked)
                //            EquationBox.Text = "Mz: " + Math.Round(linregTemp[0], 5) + "x + (" + Math.Round(linregTemp[1], 5) + ") [Kg] --> " + "R2: " + Math.Round(linregTemp[2], 5);
                //    }
                //}));
            }
            return linregTemp;
        }

        #region Chart Controls

        void ScottPlotRaw()
        {
            formsPlot1.plt.Clear();
            Color[] colors = { Color.FromArgb(51, 205, 117), Color.Black, Color.Red, Color.Blue, Color.FromArgb(255, 128, 0), Color.Magenta };
            double maxY = 0;
            double minY = 0;

            List<List<double>> rawValue = ReadRawInitFile();
            int n = rawValue[0].Count;

            for (int i = 0; i < 3; i++)
            {
                formsPlot1.plt.PlotSignal(rawValue[i].ToArray(), color: colors[i]);
                    double maxYS = rawValue[i].Max();
                    double minYS = rawValue[i].Min();
                    if (maxYS > maxY)
                        maxY = maxYS;
                    if (minYS < minY)
                        minY = minYS;
            }

            formsPlot1.plt.Ticks(
                    displayTicksX: true,
                    displayTicksXminor: false,
                    displayTickLabelsX: true,
                    displayTicksY: true,
                    displayTicksYminor: true,
                    displayTickLabelsY: true,
                    color: Color.Black,
                    useMultiplierNotation: false,
                    useOffsetNotation: false,
                    useExponentialNotation: false,
                    fontName: "Arial",
                    fontSize: 10);
            formsPlot1.Configure(
                    enablePanning: true,
                    enableZooming: true,
                    enableRightClickMenu: false,
                    lowQualityWhileDragging: false,
                    enableDoubleClickBenchmark: false,
                    lockVerticalAxis: false,
                    lockHorizontalAxis: false,
                    equalAxes: false);
            formsPlot1.plt.YLabel("Tensão (mV)", fontSize: 10);
            formsPlot1.plt.XLabel("Tempo (s)", fontSize: 10);
            formsPlot1.plt.Title(enable: false);

            double Ysize = maxY - minY;
            double Ytick = Ysize / 4.0;
            double[] YtickPositions = {
                    maxY,
                    maxY - 1.0 * Ytick,
                    maxY - 2.0 * Ytick,
                    maxY - 3.0 * Ytick,
                    maxY - 4.0 * Ytick };
            string[] YtickLabels = {
                    maxY.ToString("0.00"),
                    (maxY - 1.0 * Ytick).ToString("0.00"),
                    (maxY - 2.0 * Ytick).ToString("0.00"),
                    (maxY - 3.0 * Ytick).ToString("0.00"),
                    (maxY - 4.0 * Ytick).ToString("0.00") };
            formsPlot1.plt.YTicks(YtickPositions, YtickLabels);

            double Xmax = n;
            double Xmin = 0;
            double Xsize = Xmax - Xmin;
            double Xtick = Xsize / 4.0;
            double[] XtickPositions = {
                    Xmax,
                    Xmax - 1.0 * Xtick,
                    Xmax - 2.0 * Xtick,
                    Xmax - 3.0 * Xtick,
                    Xmax - 4.0 * Xtick };
            string[] XtickLabels = {
                    Xmax.ToString(),
                    (Xmax - 1.0 * Xtick).ToString(),
                    (Xmax - 2.0 * Xtick).ToString(),
                    (Xmax - 3.0 * Xtick).ToString(),
                    (Xmax - 4.0 * Xtick).ToString() };
            formsPlot1.plt.XTicks(XtickPositions, XtickLabels);
            formsPlot1.plt.Axis(0, Xmax, minY, maxY);


            formsPlot1.plt.TightenLayout(padding: 0, render: true);
            formsPlot1.plt.Frame(drawFrame: true, left: true, right: true, top: true, bottom: true, frameColor: null);
            formsPlot1.plt.Style(figBg: Color.Transparent);//FromArgb(246, 249, 0));
            formsPlot1.BackColor = Color.Transparent;//FromArgb(246, 249, 0));
            formsPlot1.Render();


        }

        void UpdateScottPlot(double[] Ch1array, double[] Ch2array, double[] Ch3array)
        {
            try
            {
                double chartXsizeSec = 30000 / SampleRate;
                xMin = isFirstSample ? 0 : formsPlot1.plt.GetSettings().axes.x.min;
                xMax = isFirstSample ? chartXsizeSec : formsPlot1.plt.GetSettings().axes.x.max;
                double xSize = xMax - xMin;

                yMin = isFirstSample ? -50 : formsPlot1.plt.GetSettings().axes.y.min;
                yMax = isFirstSample ? 100 : formsPlot1.plt.GetSettings().axes.y.max;
                double ySize = yMax - yMin;

                formsPlot1.plt.GetPlottables().Clear();

                UpdateChekBoxes();
                if (FyChart) formsPlot1.plt.PlotSignal(Ch1array, sampleRate: 1000, markerSize: 1, color: Color.FromArgb(51, 205, 117));
                if (MxChart) formsPlot1.plt.PlotSignal(Ch2array, sampleRate: 1000, markerSize: 1, color: Color.Blue);
                if (MzChart) formsPlot1.plt.PlotSignal(Ch3array, sampleRate: 1000, markerSize: 1, color: Color.Magenta);

                double bufferSizeSec = Ch1CircularBuffer.Size / SampleRate; //The circular buffer samples counter (same as nSamples when nSamples <= nMax)
                double nSamplesSec = nSamples / SampleRate;

                if (isFirstSample)
                {
                    isFirstSample = false;
                    formsPlot1.plt.Axis(0, bufferSizeSec, -50, 100);
                    LastXmax = xMax;
                    LastXmin = xMin;
                    LastYmax = yMax;
                    LastYmin = yMin;
                }
                else
                {
                    if (((Math.Abs(xMax - LastXmax) > 0.25 || Math.Abs(xMin - LastXmin) > 0.25) && (Math.Abs(yMax - LastYmax) > 0.25 || Math.Abs(yMin - LastYmin) > 0.25)) && nSamplesSec > 2)
                        isMoved = true;
                    formsPlot1.plt.Axis(
                        isMoved ? xMin : bufferSizeSec > chartXsizeSec ? bufferSizeSec - chartXsizeSec : 0,
                        isMoved ? xMax : bufferSizeSec,
                        yMin,
                        yMax);
                }

                double Ytick = ySize / 4.0;
                double[] YtickPositions = {
                    yMax,
                    yMax - 1.0 * Ytick,
                    yMax - 2.0 * Ytick,
                    yMax - 3.0 * Ytick,
                    yMax - 4.0 * Ytick };
                string[] YtickLabels = {
                    (yMax).ToString("0.00"),
                    (yMax - 1.0 * Ytick).ToString("0.00"),
                    (yMax - 2.0 * Ytick).ToString("0.00"),
                    (yMax - 3.0 * Ytick).ToString("0.00"),
                    (yMax - 4.0 * Ytick).ToString("0.00") };
                formsPlot1.plt.YTicks(YtickPositions, YtickLabels);

                double xAxisRange = formsPlot1.plt.GetSettings().axes.x.max - formsPlot1.plt.GetSettings().axes.x.min;
                double Xtick = xAxisRange / 4.0;
                double[] XtickPositions = {
                    bufferSizeSec - 4.0 * Xtick,
                    bufferSizeSec - 3.0 * Xtick,
                    bufferSizeSec - 2.0 * Xtick,
                    bufferSizeSec - 1.0 * Xtick,
                    bufferSizeSec};
                string[] XtickLabels = {
                    (nSamplesSec - 4.0 * Xtick).ToString("0.00"),
                    (nSamplesSec - 3.0 * Xtick).ToString("0.00"),
                    (nSamplesSec - 2.0 * Xtick).ToString("0.00"),
                    (nSamplesSec - 1.0 * Xtick).ToString("0.00"),
                    nSamplesSec.ToString("0.00")};
                formsPlot1.plt.XTicks(XtickPositions, XtickLabels);

                LastXmax = xMax;
                LastXmin = xMin;
                LastYmax = yMax;
                LastYmin = yMin;

                if (formsPlot1.plt.GetPlottables().Count > 0)
                    formsPlot1.Render();

                // this reduces flicker and helps keep the program responsive
                //System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception e1)
            {
                //Update_RichTextBox(EventLog_TextBox, e1.ToString());
            }
        }

        void CopPlotRaw(double X, double Z, int SizeZ, int SizeX)
        {
            if (COPFormsPlot.plt.GetPlottables().Count > 0)
                COPFormsPlot.plt.Clear();

            COPFormsPlot.plt.PlotHLine(SizeZ / 2, lineWidth: 3, color: Color.DarkBlue);
            COPFormsPlot.plt.PlotHLine(-SizeZ / 2, lineWidth: 5, color: Color.DarkBlue);
            COPFormsPlot.plt.PlotVLine(SizeX / 2, lineWidth: 5, color: Color.DarkBlue);
            COPFormsPlot.plt.PlotVLine(-SizeX / 2, lineWidth: 3, color: Color.DarkBlue);
            COPFormsPlot.plt.PlotLine(-SizeX, SizeZ, SizeX, -SizeZ, lineWidth: 1, color: Color.DarkGray, lineStyle: LineStyle.Dash);
            COPFormsPlot.plt.PlotLine(-SizeX, -SizeZ, SizeX, SizeZ, lineWidth: 1, color: Color.DarkGray, lineStyle: LineStyle.Dash);

            COPFormsPlot.plt.Style(dataBg: Color.White);

            COPFormsPlot.plt.Grid(enable: true, xSpacing: 10, ySpacing: 10);
            COPFormsPlot.plt.Axis(-(SizeZ / 2), SizeZ / 2, -(SizeX / 2), SizeX / 2);

            COPFormsPlot.plt.YLabel("Z (cm)", fontSize: 10, enable: true);
            COPFormsPlot.plt.XLabel("X (cm)", fontSize: 10, enable: true);
            COPFormsPlot.plt.Title(title: "Mapa de Carga", enable: false);

            COPFormsPlot.plt.Ticks(
                    displayTicksX: true,
                    displayTicksXminor: false,
                    displayTickLabelsX: true,
                    displayTicksY: true,
                    displayTicksYminor: false,
                    displayTickLabelsY: true,
                    color: Color.Black,
                    useMultiplierNotation: false,
                    useOffsetNotation: false,
                    useExponentialNotation: false);
            COPFormsPlot.Configure(
                    enablePanning: false,
                    enableZooming: false,
                    enableRightClickMenu: false,
                    lowQualityWhileDragging: false,
                    enableDoubleClickBenchmark: false,
                    lockVerticalAxis: false,
                    lockHorizontalAxis: false,
                    equalAxes: false);
            COPFormsPlot.ContextMenuStrip = null;
            COPFormsPlot.plt.TightenLayout(padding: 0, render: true);
            COPFormsPlot.plt.Frame(drawFrame: true, left: true, right: true, top: true, bottom: true, frameColor: null);
            COPFormsPlot.plt.Style(figBg: Color.Transparent);//FromArgb(246, 249, 0));
            COPFormsPlot.BackColor = Color.Transparent;//FromArgb(246, 249, 0));
            COPFormsPlot.Render();
        }


        #endregion






        List<List<double>> ReadRawInitFile()
        {
            List<List<double>> rawData = new List<List<double>>();
            TextReader tw = new StreamReader(@"rawSampleFile.txt");
            string a = tw.ReadToEnd();
            tw.Close();



            var lines = a.Split(new char[] { '\n' }).Skip(6).ToList();           // big array
            var count = lines.Count - 1;
            double[,] data = new double[3, count];
            for (int i = 0; i < count; i++) // i=1 to skip header
            {
                var line = lines[i].Split(new char[] { '\t' });
                for (int k = 0; k < 3; k++)
                {
                    data[k, i] = double.Parse(line[k + 1].ToString().Replace('.', ','));
                }
            }

            List<List<double>> DATA = new List<List<double>>();
            for (int k = 0; k < 3; k++)
            {
                List<double> row = GetMatrixPart(data, k, count).ToList();
                DATA.Add(row);
            }

            return DATA;
        }

        public static T[] GetMatrixPart<T>(T[,] matrix, int row, int columns)
        {
            var array = new T[columns];
            for (int i = 0; i < columns; ++i)
                array[i] = matrix[row, i];
            return array;
        }




        async private void Config_Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (MySerial.IsOpen) //Disconect
                {
                    MySerial.Close();
                    await Task.Delay(50);

                    UpdateCalibLog("Desconectado.");

                    Parallel.Invoke(() =>
                    this.BeginInvoke((Action)(() =>
                    {
                        PesoBox.Enabled = false;
                        GetValueButton.Enabled = false;
                        StopCalib_Button.Enabled = false;
                        Save_Button.Enabled = false;
                        PesoBox.Text = "0";
                        //PesoBox.Select();
                        TipoBox.Enabled = true;
                        SerialPortsBox.Enabled = true;
                        Config_Button.BackColor = Color.ForestGreen;
                        Config_Button.Text = "Conectar";
                        FyCheckBox.Enabled = false;
                        MxCheckBox.Enabled = false;
                        MzCheckBox.Enabled = false;
                    })));
                }
                else  //Conect
                {
                    isTare = false;
                    UpdateSizeBuffers("ChartBuffer", 30000);
                    ConnectarPlataforma();

                    Parallel.Invoke(() =>
                    this.BeginInvoke((Action)(() =>
                    {
                        PesoBox.Enabled = true;
                        GetValueButton.Enabled = true;
                        PesoBox.Text = "0";
                        PesoBox.Select();
                        TipoBox.Enabled = false;
                        SerialPortsBox.Enabled = false;
                        Config_Button.BackColor = Color.Red;
                        Config_Button.Text = "Desconectar";

                    })));

                    if (nSens == 3)
                    {
                        Parallel.Invoke(() => 
                        this.BeginInvoke((Action)(() =>
                        {
                            FyCheckBox.Enabled = true;
                            MxCheckBox.Enabled = true;
                            MzCheckBox.Enabled = true;
                            if (!FyCheckBox.Checked)
                            {
                                ZBox.Enabled = true;
                                XBox.Enabled = true;
                            }
                            ZBox.Text = "0";
                            XBox.Text = "0";
                        })));
                    }
                }
            }
            catch { }
        }

        async void ConnectarPlataforma()
        {
            try
            {
                MySerial.PortName = SerialPortsBox.Text;
                MySerial.BaudRate = 2000000;
                MySerial.ReadTimeout = 0;
                MySerial.WriteTimeout = -1;
                MySerial.Parity = Parity.None;
                MySerial.StopBits = StopBits.One;
                MySerial.DataBits = 8;
                MySerial.Handshake = Handshake.None;
                MySerial.ReadBufferSize = 20480;
                MySerial.DtrEnable = true;
                MySerial.ReceivedBytesThreshold = 20;//12; 1sample=20, 25*16=500
                MySerial.DiscardNull = false;
                MySerial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                MySerial.Open();
                await Task.Delay(50);
                await Task.Run(() => Clear_Serial_Buffer());

                Serial_thread = new Thread(new ThreadStart(Serial_bgWorker_DoWork));
                Serial_thread.Priority = ThreadPriority.Highest;
                Serial_thread.Start();

                if (!MySerial.IsOpen)
                    UpdateCalibLog("A conexão a porta " + SerialPortsBox.Text + " falhou.");
                else
                    UpdateCalibLog("Conectado.");
            }
            catch (Exception e1) { UpdateCalibLog("Erro: " + e1.ToString()); }
        }

        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)    //Whenever has MySerial.ReceivedBytesThreshold bytes in Serial buffer
            {
                if (MySerial.IsOpen)
                {
                    int nBytes = MySerial.BytesToRead;
                    if (nBytes >= MySerial.ReceivedBytesThreshold) //500bytes
                    {
                        DataReceived = true;
                    }
                }
            }
            if (e.EventType == SerialData.Eof)  //Whenever 0x1A is Received (end of line)
            {
                RecordTrigger = true;
            }
        }

        void Serial_bgWorker_DoWork()
        {
            while (MySerial.IsOpen)
            {
                if (DataReceived)
                {
                    int dataLength = MySerial.BytesToRead;
                    byte[] DataBytes = new byte[dataLength];
                    MySerial.Read(DataBytes, 0, dataLength);
                    if (DataBytes.Length > 0)
                    {
                        for (int i = 0; i < DataBytes.Length; i++)
                        {
                            SerialSlideBytes.Add(DataBytes[i]);
                        }
                        Data_Processing();
                    }
                    DataReceived = false;
                }
                Thread.Sleep(1);
            }
        }

        void Data_Processing()
        {
            //TextWriter tw = new StreamWriter(pathLog + @"SampleBytes.txt", append: true);

            while (SerialSlideBytes.Count() > 60)
            {
                double[] Sample = new double[3];
                //  [0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23]
                //  [\r \r \r \r y1 y2 y3 y4 x1 x2 x3 x4 z1 z2 z3 z4 t0 t1 t2 t3 \r \r \r \r]
                //  If starts with /r/r/r/r and Ends with /r/r/r/r (sample complete)
                if (byteDivisor.SequenceEqual(SerialSlideBytes.GetRange(0, 4)) && byteDivisor.SequenceEqual(SerialSlideBytes.GetRange(20, 4)))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        byte[] abc = SerialSlideBytes.GetRange((i * 4) + 4, 4).ToArray();
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(abc);
                        }
                        Sample[i] = Convert.ToDouble(BitConverter.ToInt32(abc, 0)) * resolution_mV;
                    }
                    if (!isTare) //WILL TARE IN START
                    {
                        Ch1TareCircularBuffer.Put(Sample[0]);
                        Ch2TareCircularBuffer.Put(Sample[1]);
                        Ch3TareCircularBuffer.Put(Sample[2]);
                        if (Ch1TareCircularBuffer.Size >= 1000)
                        {
                            Ch1Tara = Ch1TareCircularBuffer.Get(1000).Sum() / 1000;
                            Ch2Tara = Ch2TareCircularBuffer.Get(1000).Sum() / 1000;
                            Ch3Tara = Ch3TareCircularBuffer.Get(1000).Sum() / 1000;
                            isTare = !isTare;
                        }
                    }
                    else
                    {
                        Ch1CircularBuffer.Put(Sample[0] - Ch1Tara);
                        Ch2CircularBuffer.Put(Sample[1] - Ch2Tara);
                        Ch3CircularBuffer.Put(Sample[2] - Ch3Tara);
                        SerialSlideBytes.RemoveRange(0, 20);
                    }
                    if (isRecording)
                    {
                        Ch1RecordBuffer.Put(Sample[0] - Ch1Tara);
                        Ch2RecordBuffer.Put(Sample[1] - Ch2Tara);
                        Ch3RecordBuffer.Put(Sample[2] - Ch3Tara);
                        //RecordSamples++;
                        if (RecordWatch.ElapsedMilliseconds > RecordTime * 1000) //(RecordSamples == SamplesToRecord)
                        {
                            isRecording = false;
                            FinishGetSample();
                        }
                    }
                }
                else
                {
                    if (Ch1CircularBuffer.Size > 0)
                    {
                        Ch1CircularBuffer.Put(Ch1CircularBuffer.PeekLast());
                        Ch2CircularBuffer.Put(Ch2CircularBuffer.PeekLast());
                        Ch3CircularBuffer.Put(Ch3CircularBuffer.PeekLast());

                        if (isRecording)
                        {
                            Ch1RecordBuffer.Put(Ch1RecordBuffer.PeekLast());
                            Ch2RecordBuffer.Put(Ch2RecordBuffer.PeekLast());
                            Ch3RecordBuffer.Put(Ch3RecordBuffer.PeekLast());
                            //RecordSamples++;
                            if (RecordWatch.ElapsedMilliseconds > RecordTime * 1000)//(RecordSamples == SamplesToRecord)
                            {
                                isRecording = false;
                                FinishGetSample();
                            }
                        }
                    }
                    int index = 0;
                    for (int i = 1; i < 21; i++)
                    {
                        if (byteDivisor.SequenceEqual(SerialSlideBytes.GetRange(i, 4)))
                        {
                            index = i;
                            break;
                        }
                        else if (i == 20) //didn't find, probably next sampe is also broken
                        {
                            index = i;
                        }
                    }
                    SerialSlideBytes.RemoveRange(0, index);
                }
                nSamples++;
            }
        }

        void Clear_Serial_Buffer()
        {
            try
            {
                if (MySerial.IsOpen)
                {
                    MySerial.BaseStream.Flush();
                    if (MySerial.BytesToRead > 0)
                        MySerial.DiscardInBuffer();
                }
            }
            catch { }
        }


        #region Sample Routines

        private void GetValueButton_Click(object sender, EventArgs e)
        {
            if (!isCalibrating)
            {
                isCalibrating = true;
                CalibEquation.LinRegEq = new double[3, 100]; //100 SAMPLES AT MOST ON CALIBRATION. IS THAT ENOUGHT? haha
                CalibEquation.Readings = new double[3, 100];
                CalibEquation.Weights = new double[3, 100];
                CalibEquation.LinRegEq = new double[3, 3];  // Ax + B, r^2
                nCalibSample = 0;
                //UpdateCalibLog("SLOPES:");
                //UpdateCalibLog("Fy \t Mx \t Mz");
            }
            Parallel.Invoke(() =>
            this.BeginInvoke((Action)(() =>
            {
                FyCheckBox.Enabled = false;
                MxCheckBox.Enabled = false;
                MzCheckBox.Enabled = false;
            })));

            GetSample();
            StopCalib_Button.Enabled = true;
        }

        void GetSample()
        {
            UpdateSizeBuffers("RecordBuffer", 1000); //This is the buffer that will collect data to save

            RecordTrigger = false;
            while (RecordTrigger == false) ;

            RecordWatch.Start();
            isRecording = true;
        }

        void FinishGetSample()
        {
            isRecording = false;
            RecordWatch.Reset();
            nCalibSample++;
            UpdateCalibEquation(); 
            Parallel.Invoke(() =>
            this.BeginInvoke((Action)(() =>
            {
                Save_Button.Enabled = true;
            })));
        }

        void UpdateCalibEquation()
        {
            double coordValue = 0;
            calibPeso = double.TryParse(PesoBox.Text, out coordValue) ? coordValue : 0; // Convert.ToDouble(PesoBox.Text.Replace('.', ','));
            coordX = double.TryParse(XBox.Text, out coordValue) ? coordValue / 100 : 0;
            coordZ = double.TryParse(ZBox.Text, out coordValue) ? coordValue / 100 : 0;

            double[] 
                fyarray = Ch1RecordBuffer.ToArray(),
                mxarray = Ch2RecordBuffer.ToArray(),
                mzarray = Ch3RecordBuffer.ToArray();

            if (FyCheckBox.Checked)
            {
                CalibEquation.Readings[0, nCalibSample - 1] = fyarray.Sum() / fyarray.Length;
                CalibEquation.Weights[0, nCalibSample - 1] = double.Parse(PesoBox.Text);
                double[] targetWeight = GetMatrixPart(CalibEquation.Weights, 0, nCalibSample);
                double[] targetReadings = GetMatrixPart(CalibEquation.Readings, 0, nCalibSample);
                List<double> a = LinearRegression(targetWeight.ToList(), targetReadings.ToList());
                CalibEquation.LinRegEq[0, 0] = a[0];
                CalibEquation.LinRegEq[0, 1] = a[1];
                CalibEquation.LinRegEq[0, 2] = a[2];
                UpdateCalibLog(
                    "Fy: " + Math.Round(CalibEquation.LinRegEq[0, 0], 3).ToString("#00.000").Replace(",", ".") + "\t" +
                    "R^2: " + Math.Round(CalibEquation.LinRegEq[0, 2] * 100, 5).ToString("#00.00000").Replace(",", ".") + " [%]");
            }
            if (MxCheckBox.Checked)
            {
                CalibEquation.Readings[1, nCalibSample - 1] = mxarray.Sum() / mxarray.Length;
                CalibEquation.Weights[1, nCalibSample - 1] = calibPeso * coordZ;
                double[] targetWeight = GetMatrixPart(CalibEquation.Weights, 1, nCalibSample);
                double[] targetReadings = GetMatrixPart(CalibEquation.Readings, 1, nCalibSample);
                List<double> a = LinearRegression(targetWeight.ToList(), targetReadings.ToList());
                CalibEquation.LinRegEq[1, 0] = a[0];
                CalibEquation.LinRegEq[1, 1] = a[1];
                CalibEquation.LinRegEq[1, 2] = a[2];
                UpdateCalibLog(
                    "Mx: " + Math.Round(CalibEquation.LinRegEq[1, 0], 3).ToString("#00.000").Replace(",", ".") + "\t" +
                    "R^2: " + Math.Round(CalibEquation.LinRegEq[1, 2] * 100, 5).ToString("#00.00000").Replace(",", ".") + " [%]");
                //UpdateCalibLog("Inclinação Mx: " + Math.Round(CalibEquation.LinRegEq[1, 0], 3).ToString("#00.000").Replace(",", "."));
                //UpdateCalibLog("Precisão Mx: " + Math.Round(CalibEquation.LinRegEq[1, 2] * 100, 5).ToString("#00.00000").Replace(",", ".") + " [%]");
            }
            if (MzCheckBox.Checked)
            {
                CalibEquation.Readings[2, nCalibSample - 1] = mzarray.Sum() / mzarray.Length;
                CalibEquation.Weights[2, nCalibSample - 1] = calibPeso * coordX;
                double[] targetWeight = GetMatrixPart(CalibEquation.Weights, 2, nCalibSample);
                double[] targetReadings = GetMatrixPart(CalibEquation.Readings, 2, nCalibSample);
                List<double> a = LinearRegression(targetWeight.ToList(), targetReadings.ToList());
                CalibEquation.LinRegEq[2, 0] = a[0];
                CalibEquation.LinRegEq[2, 1] = a[1];
                CalibEquation.LinRegEq[2, 2] = a[2]; 
                UpdateCalibLog(
                     "Mz: " + Math.Round(CalibEquation.LinRegEq[2, 0], 3).ToString("#00.000").Replace(",", ".") + "\t" +
                     "R^2: " + Math.Round(CalibEquation.LinRegEq[2, 2] * 100, 5).ToString("#00.00000").Replace(",", ".") + " [%]");
                //UpdateCalibLog("Inclinação Mz: " + Math.Round(CalibEquation.LinRegEq[2, 0], 3).ToString("#00.000").Replace(",", "."));
                //UpdateCalibLog("Precisão Mz: " + Math.Round(CalibEquation.LinRegEq[2, 2] * 100, 5).ToString("#00.00000").Replace(",", ".") + " [%]");
            }

            COPFormsPlot.plt.PlotPoint(coordX * 100 , coordZ * 100, color: Color.Red, markerSize: 15, markerShape: MarkerShape.openCircle, errorY: 1, errorX: 1);
            COPFormsPlot.Render();

            //UpdateCalibLog(
            //    Math.Round(CalibEquation.LinRegEq[0, 0], 3).ToString("#00.000").Replace(",", ".") + "\t" +
            //    Math.Round(CalibEquation.LinRegEq[1, 0], 3).ToString("#00.000").Replace(",", ".") + "\t" +
            //    Math.Round(CalibEquation.LinRegEq[2, 0], 3).ToString("#00.000").Replace(",", "."));

            //Update_RichTextBox(CalibrationLogBox, "Equation updated.");
            //Update_Label_Text(Equation_Label, "Weight = " + Math.Round(CalibEquation.LinRegEq[0, 0], 5).ToString("#00.0000").Replace(",", ".") + " * mV + (" + Math.Round(CalibEquation.LinRegEq[0, 1], 5).ToString("#00.0000").Replace(",", ".") + ") [Kg]");
            //Update_Label_Text(Precision_Label, "Precision: " + Math.Round(CalibEquation.LinRegEq[0, 2] * 100, 5).ToString("#00.00000").Replace(",", ".") + " [%]");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Parallel.Invoke(() =>
                this.BeginInvoke((Action)(() =>
                {
                    if (nSens == 3)
                    {
                        FyCheckBox.Enabled = true;
                        MxCheckBox.Enabled = true;
                        MzCheckBox.Enabled = true;
                        XBox.Enabled = true;
                        ZBox.Enabled = true;
                    }
                    PesoBox.Enabled = true;
                    GetValueButton.Enabled = true;
                    StopCalib_Button.Enabled = false;
                    Save_Button.Enabled = false;
                })));

            isCalibrating = false;
            isRecording = false;
            nSample = 0;


            string[] stringSeparators = new string[] { "\r\n" };
            StreamReader reader = new StreamReader(pathCalib);
            string input = reader.ReadToEnd();
            reader.Close();

            string[] bla = input.Split(stringSeparators, StringSplitOptions.None);

            if (nSens == 1)
                bla[1] = "Fy: " + CalibEquation.LinRegEq[0, 0].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[0, 1].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[0, 2].ToString().Replace(",", ".");
            else if (nSens == 3)
            {
                if (FyCheckBox.Checked)
                    bla[3] = "Fy: " + CalibEquation.LinRegEq[0, 0].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[0, 1].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[0, 2].ToString().Replace(",", ".");
                if (MxCheckBox.Checked)
                    bla[4] = "Mx: " + CalibEquation.LinRegEq[1, 0].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[1, 1].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[1, 2].ToString().Replace(",", ".");
                if (MzCheckBox.Checked)
                    bla[5] = "Mz: " + CalibEquation.LinRegEq[2, 0].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[2, 1].ToString().Replace(",", ".") + "," + CalibEquation.LinRegEq[2, 2].ToString().Replace(",", ".");
            }

            TextWriter tw = new StreamWriter(pathCalib);
            tw.WriteLine(string.Join("\r\n", bla));
            tw.Close();

            UpdateCalibLog("Calibração salva!");
        }

        private void StopCalib_Button_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Tem certeza que deseja finalizar a calibração sem salvar os dados?",
                                                "Cancelar!",
                                                MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                Parallel.Invoke(() =>
                this.BeginInvoke((Action)(() =>
                {
                    if (nSens == 3)
                    {
                        FyCheckBox.Enabled = true;
                        MxCheckBox.Enabled = true;
                        MzCheckBox.Enabled = true;
                        XBox.Enabled = true;
                        ZBox.Enabled = true;
                    }
                    PesoBox.Enabled = true;
                    GetValueButton.Enabled = true;
                    StopCalib_Button.Enabled = false;
                    Save_Button.Enabled = false;
                })));

                isCalibrating = false;
                isRecording = false;
                nSample = 0;

                UpdateCalibLog("Calibração cancelada.");
            }
            else
            {
                UpdateCalibLog("Para salvar a calibração, clique em \"Salvar\".");
            }
        }

        #endregion

        List<double> LinearRegression(List<double> KG, List<double> MV)
        {
            List<double> linregTemp = new List<double>();

            if (KG.Count != MV.Count)
            {
                linregTemp.Add(0); linregTemp.Add(0); linregTemp.Add(0);
                return linregTemp;
            }

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;

            for (var i = 0; i < KG.Count; i++)
            {
                var x = MV[i];
                var y = KG[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = MV.Count;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            //var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            var rSquared = dblR * dblR;
            var yIntercept = meanY - ((sCo / ssX) * meanX);
            var slope = sCo / ssX;
            linregTemp.Add(slope); linregTemp.Add(yIntercept); linregTemp.Add(rSquared);
            return linregTemp;
        }








        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MySerial.IsOpen) 
                MySerial.Close();
            if (Serial_thread != null)
                if (Serial_thread.IsAlive) Serial_thread.Abort();
        }







        private void CalibLogTextBox_TextChanged(object sender, EventArgs e)
        {
            CalibLogTextBox.SelectionStart = CalibLogTextBox.Text.Length;
            CalibLogTextBox.ScrollToCaret();
        }

        private void PesoBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow two decimal point
            if (Regex.IsMatch(PesoBox.Text, @"\.\d\d\d\d"))
            {
                e.Handled = true;
            }
        }

        private void XBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && !((e.KeyChar != '-') || XBox.Text == ""))
            {
                e.Handled = true;
            }
            // only allow two decimal point
            if (Regex.IsMatch(XBox.Text, @"\.\d\d") || Regex.IsMatch(XBox.Text, @"-\.\d\d"))
            {
                e.Handled = true;
            }
        }

        private void ZBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && !((e.KeyChar != '-') || ZBox.Text == ""))
            {
                e.Handled = true;
            }
            // only allow two decimal point
            if (Regex.IsMatch(ZBox.Text, @"\.\d\d"))
            {
                e.Handled = true;
            }
        }

        void UpdateChekBoxes()
        {
            FyChart = FyCheckBox.Checked ? true : false;
            MxChart = MxCheckBox.Checked ? true : false;
            MzChart = MzCheckBox.Checked ? true : false;
        }

        private void FyCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            FyChart = FyCheckBox.Checked ? true : false;
            if (MxCheckBox.Checked || MzCheckBox.Checked)
            {
                XBox.Enabled = true;
                ZBox.Enabled = true;
            }
            else
            {
                XBox.Enabled = false;
                ZBox.Enabled = false;
            }
        }

        private void MxCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MxChart = MxCheckBox.Checked ? true : false;
            if (MxCheckBox.Checked || MzCheckBox.Checked)
            {
                XBox.Enabled = true;
                ZBox.Enabled = true;
            }
            else
            {
                XBox.Enabled = false;
                ZBox.Enabled = false;
            }
        }

        private void MzCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MzChart = MzCheckBox.Checked ? true : false;
            if (MxCheckBox.Checked || MzCheckBox.Checked)
            {
                XBox.Enabled = true;
                ZBox.Enabled = true;
            }
            else
            {
                XBox.Enabled = false;
                ZBox.Enabled = false;
            }
        }

        private void PesoBox_Click(object sender, EventArgs e)
        {
            PesoBox.Text = "";
        }

        private void XBox_Click(object sender, EventArgs e)
        {
            XBox.Text = "";
        }

        private void ZBox_Click(object sender, EventArgs e)
        {
            ZBox.Text = "";
        }

        private void TipoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TipoBox.Text != "" && SerialPortsBox.Text != "")
            {
                Config_Button.Enabled = true;
            }
            else
            {
                Config_Button.Enabled = false;
            }

            //CopPlotRaw(0, 0, 50, 50);
            if (TipoBox.Text == "1D - 500x500")
            {
                nSens = 1;
                UpdateCalibLog("ESQUEMA DE LIGAÇÃO: " + Environment.NewLine + "C\tB\tA" + Environment.NewLine + "-\t-\tFy");
            }
            else if (TipoBox.Text == "3D - 500x500")
            {
                nSens = 3;
                UpdateCalibLog("ESQUEMA DE LIGAÇÃO: " + Environment.NewLine + "C\tB\tA" + Environment.NewLine + "Mz\tMx\tFy");
            }
            //LinReg = GetCalibConfig();
        }

        private void SerialPortsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TipoBox.Text != "" && SerialPortsBox.Text != "")
            {
                Config_Button.Enabled = true;
            }
            else
            {
                Config_Button.Enabled = false;
            }
        }




        //private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        Leitura.Clear();
        //        AmostraLida.Clear();
        //        DateTime tUpdate = DateTime.Now.AddSeconds(1);

        //        BackgroundWorker worker = sender as BackgroundWorker;
        //        UpdateCalibLog("Fazendo leitura dos dados...");

        //        while (true)
        //        {
        //            if (worker.CancellationPending == true) //Assure worker will stop when CancelAsync is called
        //            {
        //                e.Cancel = true;
        //                break;
        //            }

        //            /* Após configurar a placa, o sistema começa a fazer a leitura da porta COM
        //             * e a cada (1) segundo todo o dado é decodificado e o valor médio do segundo
        //             * apresentado no ComboBox LeituraBox. Após a primeira coleta de calibração a 
        //             * variável SampleMode se responsabiliza por não atualizar mais o LeituraBox */
        //            if (!IsSampling && !SampleMode)
        //            {
        //                if (MySerial.IsOpen)
        //                {
        //                    try
        //                    {
        //                        string a = MySerial.ReadExisting();
        //                        Leitura.Add(a);
        //                    }
        //                    catch (Exception) { continue; }

        //                    if (DateTime.Now >= tUpdate)
        //                    {
        //                        string Tudo = String.Join("", Leitura);
        //                        Tudo = Tudo.Substring(Tudo.IndexOf(',') + 1); //Discarta primeira amostra pois pode vir quebrada
        //                        Tudo = Tudo.Substring(0, Tudo.LastIndexOf(',')-1); //Discarta última amostra pois pode vir quebrada

        //                        try { 
        //                        foreach (string str1 in Tudo.Split(','))
        //                        {
        //                            if (FyCheckBox.Checked && double.TryParse(str1.Split(';')[0].Replace('.', ','), out double a))
        //                            {
        //                                AmostraLida.Add(a);
        //                            }
        //                            else if (MxCheckBox.Checked && double.TryParse(str1.Split(';')[1].Replace('.', ','), out double b))
        //                            {
        //                                AmostraLida.Add(b);
        //                            }
        //                            else if (MzCheckBox.Checked && double.TryParse(str1.Split(';')[2].Replace('.', ','), out double c))
        //                            {
        //                                AmostraLida.Add(c);
        //                            }
        //                        }
        //                        }
        //                        catch(Exception ex) { MessageBox.Show("Erro: " + ex.Message + "AAAAA"); }
        //                        if (AmostraLida.Count > 0)
        //                        {
        //                            UpdateLeituraBox((AmostraLida.Sum() / AmostraLida.Count()).ToString("N4").Replace(",", "."));
        //                            AmostraLida.Clear();
        //                            tUpdate = DateTime.Now.AddSeconds(1);
        //                        }
        //                        Leitura.Clear();
        //                        AmostraLida.Clear();
        //                    }
        //                }
        //            }

        //            /* Ao clicar em "Adicionar Leitura" o sistema, em primeiro momento, cria as variáveis e indica que esta em coleta.
        //             * A coleta de dados será realizada por tSample segundos e então os dados processados. Em seguida,
        //             * a média da leitura é disponibilizada para armazenamento e pós processamento (atualização dos gráficos, 
        //             * equações, et al.). */

        //            if (IsSampling)
        //            {
        //                if (Calibrando) //CONFIGURA VARIÁVEIS DA CALIBRAÇÃO
        //                {
        //                    //tarado = false;
        //                    AmostraLida.Clear();
        //                    Leitura.Clear();
        //                    SampleMode = true;
        //                    AmostraKG.Clear();
        //                    AmostraMV.Clear();
        //                    Calibrando = false;
        //                    LinRegEq.Clear();
        //                }
        //                if (PrimeiraLeitura) //LIMPA BUFFER ANTES DE CADA COLETA
        //                {
        //                    MySerial.DiscardOutBuffer();
        //                    MySerial.DiscardInBuffer();
        //                    PrimeiraLeitura = false;
        //                }
        //                if (MySerial.IsOpen)
        //                {
        //                    try
        //                    {
        //                        string a = MySerial.ReadExisting();
        //                        Leitura.Add(a);
        //                    }
        //                    catch (Exception) { continue; }

        //                    if (DateTime.Now >= tSample)
        //                    {
        //                        string Tudo = String.Join("", Leitura);
        //                        Tudo = Tudo.Substring(Tudo.IndexOf(',') + 1); //Discarta primeira amostra pois pode vir quebrada
        //                        Tudo = Tudo.Substring(0, Tudo.LastIndexOf(',') - 1); //Discarta última amostra pois pode vir quebrada
        //                        foreach (string str1 in Tudo.Split(','))
        //                        { //double.TryParse remove linhas vazias, só aceita doubles.
        //                            if (FyCheckBox.Checked && double.TryParse(str1.Split(';')[0].Replace('.', ','), out double a))
        //                                AmostraLida.Add(a);
        //                            else if (MxCheckBox.Checked && double.TryParse(str1.Split(';')[1].Replace('.', ','), out double b))
        //                                AmostraLida.Add(b);
        //                            else if (MzCheckBox.Checked && double.TryParse(str1.Split(';')[2].Replace('.', ','), out double c))
        //                                AmostraLida.Add(c);
        //                        }

        //                        if (AmostraLida.Count > 0)
        //                        {
        //                            try
        //                            {
        //                                double KgTemp = calibPeso;
        //                                AmostraMV.Add(AmostraLida.Sum() / AmostraLida.Count());
        //                                //AmostraKG.Add(calibPeso);

        //                                if (FyCheckBox.Checked)
        //                                {
        //                                    AmostraKG.Add(KgTemp);
        //                                    LinRegEq = UpdateLinRegEquation(AmostraKG.ToArray(), AmostraMV.ToArray());
        //                                }
        //                                else if (MxCheckBox.Checked)
        //                                {
        //                                    AmostraKG.Add(KgTemp * coordX);
        //                                    LinRegEq = UpdateLinRegEquation(AmostraKG.ToArray(), AmostraMV.ToArray());
        //                                }
        //                                else if (MzCheckBox.Checked)
        //                                {
        //                                    AmostraKG.Add(KgTemp * coordZ);
        //                                    LinRegEq = UpdateLinRegEquation(AmostraKG.ToArray(), AmostraMV.ToArray());
        //                                }

        //                                UpdateLeituraBox(AmostraMV.Last().ToString("N4").Replace(",", "."));

        //                                UpdateLinRegChart(AmostraKG, AmostraMV);
        //                                UpdateMapXZChart(coordX, coordZ);
        //                                UpdateCalibLog("Amostra Nº:" 
        //                                    + AmostraMV.Count().ToString() 
        //                                    + ": mV: " + Math.Round(AmostraMV.Last(), 3).ToString() 
        //                                    + "\tPeso: " + KgTemp.ToString());

        //                                if (AmostraMV.Count() > 2)
        //                                {
        //                                    this.BeginInvoke(new Action(() =>
        //                                    {
        //                                        Save_Button.Enabled = true;
        //                                    }));
        //                                }
        //                                //}
        //                                IsSampling = false;
        //                            }
        //                            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message + " - GetValue1"); }
        //                        }
        //                        AmostraLida.Clear();
        //                        Leitura.Clear();
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message + " - SampleIN1"); }
        //}



        //}
        void Upload_Schetch()
        {
            if (MySerial != null) //Make sure MySerial port is closed
            {
                if (MySerial.IsOpen) //if open, close
                {
                    MySerial.Close();
                    Task.Delay(2000).Wait();
                    MySerial = null;
                }
            }
            //if (MySerial.IsOpen) { MySerial.Close(); }
            string tipo = "";
            //if (TipoBox.Text == "1D - 500x500") { tipo = @"calib1D.ino.eightanaloginputs.hex"; }
            //else if (TipoBox.Text == "3D - 500x500") { tipo = @"calib3D.ino.eightanaloginputs.hex"; }
            //else if (TipoBox.Text == "6D - 400x600") { tipo = @"calib3D.ino.eightanaloginputs.hex"; }
            //else if (TipoBox.Text == "") { tipo = @"calib1D.ino.eightanaloginputs.hex"; TipoBox.SelectedIndex = 0; }
            tipo = @"calib1D.ino.eightanaloginputs.hex";
            try
            {
                if (SerialPortsBox.Text == "")
                {
                    SerialPortsBox.Items.Clear();
                    SerialPortsBox.Items.AddRange(SerialPort.GetPortNames());
                    SerialPortsBox.SelectedIndex = 0;
                }

                var uploader = new ArduinoSketchUploader(
                new ArduinoSketchUploaderOptions()
                {
                    FileName = tipo,
                    PortName = SerialPortsBox.Text,
                    ArduinoModel = ArduinoModel.NanoR3
                });
                uploader.UploadSketch();

                UpdateCalibLog("Placa configurada para plataforma " + TipoBox.Text);

            }
            catch (Exception ex)
            {
                MySerial.Close();
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

    }
    public static class CalibEquation
    {
        public static string Date;
        public static double[,] SavedLinReg = new double[3, 2]; // [a, r^2] => (Y = aX) 
        public static double[,] LinRegEq;// = new List<List<double>>();
        public static double[,] Readings;// = new List<List<double>>();
        public static double[,] Weights;// = new List<List<double>>();
    }
}
