using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using ArduinoUploader;
using ArduinoUploader.Hardware;
using System.Text.RegularExpressions;
using BioMensurae;
using Microsoft.Win32;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using ScottPlot;
using Cyotek.Collections.Generic;

namespace WindowsFormsApp4
{
    public partial class BioMensurae : Form
    {
        #region Variables

        private SerialPort 
            MySerial = new SerialPort();
        
        TimeSpan
            ElapsedTime;

        Stopwatch
            RecordWatch = new Stopwatch(),
            BeepWatch = new Stopwatch();

        List<List<double>> 
            dados = new List<List<double>>();

        System.Windows.Forms.Timer aTimer;
            //error_timer;

        Thread Serial_thread; //Made it global to finish tread on close app event

        static readonly double VREF = 2.5;
        static readonly int bit = 32;
        static readonly double resolution_mV = (VREF / Math.Pow(2, (bit - 1))) * 1000;

        readonly string 
            pathTxt = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BioMensurae DAQ Coletas\txt\",
            pathSad = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BioMensurae DAQ Coletas\sad\",
            pathLog = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BioMensurae DAQ Coletas\log\";

        List<List<double>> LinReg = new List<List<double>>();
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
        //CircularBuffer<byte>
        //    SerialSlidingWindow;

        List<byte>
            SerialSlideBytes = new List<byte>(),
            byteDivisor = new List<byte>() { 13, 13, 13, 13 }; //uControler /r /r /r /r;

        bool
            first = true,
            DataReceived = false,
            isRecording = false,
            Ch1Chart,
            Ch2Chart,
            Ch3Chart,
            isTare = false,
            isFirstSample = true,
            isMoved = false;

        volatile bool
            RecordTrigger = false;

        int 
            nSens = 3,
            nSamples = 0,
            //RecordSamples,
            nBlink = 0,
            RecordTime = 30, //time to record in seconds
            ChartXSize = 30000;

        double
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

        string[] Ports_avaliable = SerialPort.GetPortNames();
        int Numer_of_COMPorts = 0;

        #endregion

        #region initialization

        public BioMensurae()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            CreateFolders();
            UpdateSizeBuffers("ChartBuffer", 30000);
            ScottPlotRaw();
            CopPlotRaw(0, 0, 50, 50);
            Timer_config();
        }

        void updateLogFile()
        {
            TextWriter tw = new StreamWriter(pathLog + @"SavedList.txt");
            //foreach (double s in ReadValues)
            //    tw.WriteLine(s);
            tw.Close();
        }

        void CreateFolders()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BioMensurae DAQ Coletas")) //"\\BioMensurae 2ADS Coletas"
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BioMensurae DAQ Coletas");
                InstallDriver();
                Update_EventLog_TextBox("As coletas serão salvas em: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BioMensurae DAQ Coletas");
            }
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + pathTxt))
                Directory.CreateDirectory(pathTxt);
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + pathSad))
                Directory.CreateDirectory(pathSad);
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + pathLog))
                Directory.CreateDirectory(pathLog);
        }

        private void InstallDriver()
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo("CH34x_Install_Windows_v3_4.exe")
            {
                //UseShellExecute = false,
                //Arguments = @"C:\Users\10522\Desktop\spssJob1.spj -production",
                //WorkingDirectory = @"C:\Program Files\IBM\SPSS\Statistics\22",
            };
            p.Start();
            p.WaitForExit();
            Thread.Sleep(1000);
        }

        void Timer_config()
        {
            aTimer = new System.Windows.Forms.Timer
            { Interval = 100 };
            aTimer.Tick += new EventHandler(OnTimedEvent);
            //aTimer.Elapsed += OnTimedEvent;
            //aTimer.AutoReset = true;
            aTimer.Start();
            //    error_timer = new System.Windows.Forms.Timer
            //    { Interval = 1000 };
            //    error_timer.Tick += new EventHandler(ErrorTick);
            //    error_timer.Start();
        }

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


        #endregion

        #region Main Form Control

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MySerial.IsOpen) MySerial.Close();
            if (Serial_thread != null)
                if (Serial_thread.IsAlive) Serial_thread.Abort();
            Application.Exit();
        }

        private void RecordTime_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) //Only integer
                e.Handled = true;
        }

        #endregion

        #region Custom Forms Settings

        private void EventLogTextBox_TextChanged(object sender, EventArgs e)
        {
            EventLogTextBox.SelectionStart = EventLogTextBox.Text.Length;
            EventLogTextBox.ScrollToCaret();
        }

        private delegate void SetTextDelegate(Label tb, string Text); //protected delegate void
        private void SetText(Label tb, string Text) //protected void
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

        //private delegate void SetProgressBarDelegate(ProgressBar tb, int value);
        //private void SetProgressBar(ProgressBar tb, int value) //protected void
        //{
        //    if (tb.InvokeRequired)
        //    {
        //        tb.Invoke(new SetProgressBarDelegate(SetProgressBar), tb, value);
        //        return;
        //    }
        //    tb.Value = value;
        //}

        private void Blink_Label(Label lb)
        {
            switch (nBlink)
            {
                case 0:
                    lb.ForeColor = Color.Red;
                    nBlink++;
                    break;
                case 1:
                    lb.ForeColor = Color.FromArgb(255, 128, 128);
                    nBlink++;
                    break;
                case 2:
                    lb.ForeColor = Color.FromArgb(255, 192, 192);
                    nBlink++;
                    break;
                case 3:
                    lb.ForeColor = Color.FromArgb(246, 249, 250);
                    nBlink++;
                    break;
                case 4:
                    lb.ForeColor = Color.FromArgb(255, 192, 192);
                    nBlink++;
                    break;
                case 5:
                    lb.ForeColor = Color.FromArgb(255, 128, 128);
                    nBlink++;
                    break;
            }
            if (nBlink >= 6)
                nBlink = 0;
        }

        private void Update_EventLog_TextBox(string v)
        {
            try
            {
                this.BeginInvoke((Action)(() =>
                {
                    EventLogTextBox.AppendText(v + Environment.NewLine);
                }));
            }
            catch { }
        }

        #endregion

        #region User Interface Control

        async void Connect_Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (MySerial.IsOpen) //Disconect
                {
                    MySerial.Close();
                    await Task.Delay(50);

                    Update_EventLog_TextBox("Desconectado.");

                    Parallel.Invoke(() =>
                    this.BeginInvoke((Action)(() =>
                    {
                        Connect_Button.BackColor = Color.ForestGreen;
                        connect_label.Text = "Conectar:";
                        Record_Button.Enabled = false;
                        Stop_Button.Enabled = false;
                        OpenFile_Button.Enabled = true;
                        Calib_Button.Enabled = true;
                        TipoBox.Enabled = true;
                        SerialPortsBox.Enabled = true;
                        StatusText.Text = "OFF";
                        StatusText.ForeColor = Color.Red;

                    })));
                }
                else  //Conect
                {
                    UpdateSizeBuffers("ChartBuffer", 30000);
                    Upload_Schetch();
                    isTare = false;
                    isMoved = false;
                    Connect();

                    if (MySerial.IsOpen)
                    {
                        SerialPortsBox.Enabled = false;
                        TipoBox.Enabled = false;
                        Record_Button.Enabled = true;
                        Stop_Button.Enabled = false;
                        OpenFile_Button.Enabled = false;
                        Calib_Button.Enabled = false;
                        Connect_Button.BackColor = Color.Red;
                        connect_label.Text = "Desconectar:";
                        StatusText.Text = "ON";
                        StatusText.ForeColor = Color.Green;
                    }
                }
            }
            catch { }
        }


        private void SerialPortsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TipoBox.Text != "" && SerialPortsBox.Text != "")
            {
                Connect_Button.Enabled = true;
            }
            else
            {
                Connect_Button.Enabled = false;
            }
        }

        void Check_CheckBox_Status()
        {
            Ch1Chart = Ch1CheckBox.Checked ? true : false;

            Ch2Chart = Ch2CheckBox.Checked ? true : false;

            Ch3Chart = Ch3CheckBox.Checked ? true : false;
        }

        private void FyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Ch1Chart = Ch1CheckBox.Checked ? true : false;
            if (!MySerial.IsOpen)
                ScottPlotRaw();
        }

        private void MxCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Ch2Chart = Ch2CheckBox.Checked ? true : false;
            if (!MySerial.IsOpen)
                ScottPlotRaw();
        }

        private void MzCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Ch3Chart = Ch3CheckBox.Checked ? true : false;
            if (!MySerial.IsOpen)
                ScottPlotRaw();
        }

        private void TipoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TipoBox.Text != "" && SerialPortsBox.Text != "")
            {
                Connect_Button.Enabled = true;
            }
            else
            {
                Connect_Button.Enabled = false;
            }

            nSens = TipoBox.Text == "Plataforma 1D" ? 1 : TipoBox.Text == "Bilateral" ? 2 : 3;
            CopPlotRaw(0, 0, 50, 50);
            if (nSens == 1)
            {
                Ch1CheckBox.Enabled = true;  Ch1CheckBox.Checked = true;  Ch1CheckBox.Visible = true;
                Ch2CheckBox.Enabled = false; Ch2CheckBox.Checked = false; Ch2CheckBox.Visible = false;
                Ch3CheckBox.Enabled = false; Ch3CheckBox.Checked = false; Ch3CheckBox.Visible = false;
                Update_EventLog_TextBox("ESQUEMA DE LIGAÇÃO: " + Environment.NewLine + "C\tB\tA" + Environment.NewLine + "-\t-\tFy");
            }
            else if (nSens == 2)
            {
                Ch1CheckBox.Enabled = true; Ch1CheckBox.Checked = true; Ch1CheckBox.Visible = true;
                Ch2CheckBox.Enabled = true; Ch2CheckBox.Checked = true; Ch2CheckBox.Visible = true;
                Ch3CheckBox.Enabled = false; Ch3CheckBox.Checked = false; Ch3CheckBox.Visible = false;
                Update_EventLog_TextBox("ESQUEMA DE LIGAÇÃO: " + Environment.NewLine + "C\tB\tA" + Environment.NewLine + "-\t1D\t3D");
            }
            else if (nSens == 3)
            {
                Ch1CheckBox.Enabled = true; Ch1CheckBox.Checked = true; Ch1CheckBox.Visible = true;
                Ch2CheckBox.Enabled = true; Ch2CheckBox.Checked = true; Ch2CheckBox.Visible = true;
                Ch3CheckBox.Enabled = true; Ch3CheckBox.Checked = true; Ch3CheckBox.Visible = true;
                Update_EventLog_TextBox("ESQUEMA DE LIGAÇÃO: " + Environment.NewLine + "C\tB\tA" + Environment.NewLine + "Mz\tMx\tFy");
            }
            LinReg = GetCalibConfig();
        }

        private void Record_Button_Click(object sender, EventArgs e)
        {
            if (!isRecording)
                InitRecording();
            else
                FinishRecording();
        }
        private void Stop_Button_Click(object sender, EventArgs e) { FinishRecording(); }
        private void Plot_Button_Click(object sender, EventArgs e) 
        {
            Form2 Graph_Window = new Form2(dados, nSens);
            Graph_Window.Show();
        }
        private void OpenFile_Button_Click(object sender, EventArgs e) => Open_file();
        private void Calib_Button_Click(object sender, EventArgs e)
        {
            if (MySerial.IsOpen)
            {
                MySerial.Close();
                Update_EventLog_TextBox("Abrindo Janela de Calibração" + Environment.NewLine);
            }
            Form3 Calib_Window = new Form3();
            Calib_Window.Show();
        }
        private void SerialPortsBox_Click(object sender, EventArgs e)
        {
            SerialPortsBox.Items.Clear();
            SerialPortsBox.Items.AddRange(SerialPort.GetPortNames());
        }
        private void OnTimedEvent(object source, EventArgs e)
        {
            SetText(clock_display, DateTime.Now.ToString("HH:mm:ss"));
            if (MySerial.IsOpen)
            {
                if (isRecording)
                {
                    Blink_Label(StatusText);
                }
                if (Ch1CircularBuffer.Size > 200 && LinReg.Count > 0)
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
                            Connect_Button.Enabled = true;
                        }
                    }
                    else
                    {
                        SerialPortsBox.Text = null;
                        Connect_Button.Enabled = false;
                    }
                    SerialPortsBox.Enabled = true;
                }

                Parallel.Invoke(() =>
                this.BeginInvoke((Action)(() =>
                {
                    Connect_Button.BackColor = Color.ForestGreen;
                    connect_label.Text = "Conectar:";
                    Record_Button.Enabled = false;
                    Stop_Button.Enabled = false;
                    OpenFile_Button.Enabled = true;
                    Calib_Button.Enabled = true;
                    TipoBox.Enabled = true;
                    SerialPortsBox.Enabled = true;
                    StatusText.Text = "OFF";
                    StatusText.ForeColor = Color.Red;

                })));
            }
        }


        #endregion

        #region Hardware Control Routines
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

        void Relay_on_off(string stat)
        {
            switch (stat)
            {
                case "OFF":
                    MySerial.Write("0");  //TURN OFF RELAY - Turn on lights
                    break;
                case "ON":
                    MySerial.Write("1");  //TURN ON RELAY - Turn off lights
                    BeepWatch.Start();
                    break;
            }
        }

        void Upload_Schetch()
        {
            //if (TipoBox.Text == "Plataforma 1D") 
            //    tipo = @"singleCell1262.ino.eightanaloginputs.hex";
            //else if (TipoBox.Text == "Bilateral" || TipoBox.Text == "Plataforma 3D")
            //    tipo = @"3D.ino.eightanaloginputs.hex";

            string tipo = @"3D.ino.eightanaloginputs.hex";
            try
            {
                var uploader = new ArduinoSketchUploader(
                new ArduinoSketchUploaderOptions()
                {
                    FileName = tipo,
                    PortName = SerialPortsBox.Text,
                    ArduinoModel = ArduinoModel.NanoR3
                });
                uploader.UploadSketch();
                Update_EventLog_TextBox("Configurado para: " + TipoBox.Text + ".");
            }
            catch { Update_EventLog_TextBox("Não foi possível configurar a placa, tente novamente."); }
        }

        async void Connect()
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
                    Update_EventLog_TextBox("A conexão a porta " + SerialPortsBox.Text + " falhou.");
                else
                    Update_EventLog_TextBox("Conectado.");
            }
            catch (Exception e1) { Update_EventLog_TextBox("Erro: " + e1.ToString()); }
        }

        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (MySerial.IsOpen)
            {
                if (e.EventType == SerialData.Chars)    //Whenever has MySerial.ReceivedBytesThreshold bytes in Serial buffer
                {
                    int nBytes = MySerial.BytesToRead;

                    if (nBytes >= MySerial.ReceivedBytesThreshold) //500bytes
                    {
                        DataReceived = true;
                    }
                }
                if (e.EventType == SerialData.Eof)  //Whenever 0x1A is Received (end of line)
                {
                    RecordTrigger = true;
                }
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

        //private void ErrorTick(object source, EventArgs e)
        //{
        //    if (data_error > 0)
        //    {
        //        Update_EventLog_TextBox(data_error.ToString());
        //        data_error = 0;
        //    }
        //}

        //int data_error = 0;
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
                        byte[] abc =  SerialSlideBytes.GetRange((i * 4) + 4, 4).ToArray();
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
                            FinishRecording();
                        }

                        if (BeepWatch.ElapsedMilliseconds > 3000)
                        {
                            Relay_on_off("OFF"); //VERIFY IF IS CORRECT
                            BeepWatch.Reset();
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
                        //data_error++;
                        if (isRecording)
                        {
                            Ch1RecordBuffer.Put(Ch1RecordBuffer.PeekLast());
                            Ch2RecordBuffer.Put(Ch2RecordBuffer.PeekLast());
                            Ch3RecordBuffer.Put(Ch3RecordBuffer.PeekLast());
                            //RecordSamples++;
                            if (RecordWatch.ElapsedMilliseconds > RecordTime * 1000)//(RecordSamples == SamplesToRecord)
                            {
                                isRecording = false;
                                FinishRecording();
                            }
                            if (BeepWatch.ElapsedMilliseconds > 3000)
                            {
                                Relay_on_off("OFF");
                                BeepWatch.Reset();
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

        #endregion

        #region Chart Controls

        void ScottPlotRaw()
        {
            formsPlot1.plt.Clear();
            Color[] colors = { Color.FromArgb(51, 205, 117), Color.Black, Color.Red, Color.Blue, Color.FromArgb(255, 128, 0), Color.Magenta };
            double maxY = 0;
            double minY = 0;
            Check_CheckBox_Status();
            bool[] toPlotIndex = { Ch1Chart, Ch2Chart, Ch3Chart };

            List<List<double>> rawValue = ReadRawInitFile();
            int n = rawValue[0].Count;

            for (int i = 0; i < 3; i++)
            {
                if (toPlotIndex[i])
                {
                    formsPlot1.plt.PlotSignal(rawValue[i].ToArray(), color: colors[i]);
                    double maxYS = rawValue[i].Max();
                    double minYS = rawValue[i].Min();
                    if (maxYS > maxY)
                        maxY = maxYS;
                    if (minYS < minY)
                        minY = minYS;
                }
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
            formsPlot1.plt.YLabel("");
            formsPlot1.plt.XLabel("Time (s)");
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
                double chartXsizeSec = ChartXSize / SampleRate;
                xMin = isFirstSample ? 0 : formsPlot1.plt.GetSettings().axes.x.min;
                xMax = isFirstSample ? chartXsizeSec : formsPlot1.plt.GetSettings().axes.x.max;
                double xSize = xMax - xMin;

                yMin = isFirstSample ? -50 : formsPlot1.plt.GetSettings().axes.y.min;
                yMax = isFirstSample ? 100 : formsPlot1.plt.GetSettings().axes.y.max;
                double ySize = yMax - yMin;

                formsPlot1.plt.GetPlottables().Clear();

                Check_CheckBox_Status();
                if (Ch1Chart) formsPlot1.plt.PlotSignal(Ch1array, sampleRate: 1000, markerSize: 1, color: Color.FromArgb(51, 205, 117));
                if (Ch2Chart) formsPlot1.plt.PlotSignal(Ch2array, sampleRate: 1000, markerSize: 1, color: Color.Blue);
                if (Ch3Chart) formsPlot1.plt.PlotSignal(Ch3array, sampleRate: 1000, markerSize: 1, color: Color.Magenta);

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
            //COPFormsPlot.plt.PlotVLine(-(X / 2), lineWidth: 2, color: Color.DarkBlue);
            //COPFormsPlot.plt.PlotVLine(X / 2, lineWidth: 2, color: Color.DarkBlue);
            //COPFormsPlot.plt.PlotHLine(-(Z / 2), lineWidth: 2, color: Color.DarkBlue);
            //COPFormsPlot.plt.PlotHLine(Z / 2, lineWidth: 2, color: Color.DarkBlue);
            if (COPFormsPlot.plt.GetPlottables().Count > 0)
                COPFormsPlot.plt.Clear();

            if (nSens == 1)
            {
                COPFormsPlot.plt.PlotHLine(SizeZ / 2, lineWidth: 3, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotHLine(-SizeZ / 2, lineWidth: 5, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(SizeX / 2, lineWidth: 5, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(-SizeX / 2, lineWidth: 3, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotLine(-SizeX, SizeZ, SizeX, -SizeZ, lineWidth: 1, color: Color.DarkGray, lineStyle: LineStyle.Dash);
                COPFormsPlot.plt.PlotLine(-SizeX, -SizeZ, SizeX, SizeZ, lineWidth: 1, color: Color.DarkGray, lineStyle: LineStyle.Dash);
                COPFormsPlot.plt.Style(dataBg: Color.LightGray);
            }
            if (nSens == 2)
            {
                COPFormsPlot.plt.PlotHLine(SizeZ / 2, lineWidth: 3, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotHLine(-SizeZ / 2, lineWidth: 5, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(SizeX / 2, lineWidth: 5, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(-SizeX / 2, lineWidth: 3, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(X / 2, lineWidth: 1, color: Color.DarkGray, lineStyle: LineStyle.Dash);
                COPFormsPlot.plt.PlotPoint(Z, X, color: Color.Red, markerSize: 10);
                COPFormsPlot.plt.Style(dataBg: Color.White);
            }
            if (nSens == 3)
            {
                COPFormsPlot.plt.PlotHLine(SizeZ / 2, lineWidth: 3, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotHLine(-SizeZ / 2, lineWidth: 5, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(SizeX / 2, lineWidth: 5, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(-SizeX / 2, lineWidth: 3, color: Color.DarkBlue);
                COPFormsPlot.plt.PlotVLine(X / 2, lineWidth: 1, color: Color.DarkGray, lineStyle: LineStyle.Dash);
                COPFormsPlot.plt.PlotHLine(Z / 2, lineWidth: 1, color: Color.DarkGray, lineStyle: LineStyle.Dash);
                COPFormsPlot.plt.PlotPoint(Z, X, color: Color.Red, markerSize: 10);
                COPFormsPlot.plt.Style(dataBg: Color.White);
            }

            COPFormsPlot.plt.Grid(enable: false, xSpacing: 10, ySpacing: 10);
            COPFormsPlot.plt.Axis(-(SizeZ / 2), SizeZ / 2, -(SizeX / 2), SizeX / 2);

            COPFormsPlot.plt.YLabel(enable: false);
            COPFormsPlot.plt.XLabel(enable: false);
            COPFormsPlot.plt.Title(enable: false);
            COPFormsPlot.plt.Ticks(
                    displayTicksX: false,
                    displayTicksXminor: false,
                    displayTickLabelsX: false,
                    displayTicksY: false,
                    displayTicksYminor: false,
                    displayTickLabelsY: false,
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
            COPFormsPlot.plt.Title(enable: false);
            COPFormsPlot.plt.TightenLayout(padding: 0, render: true);
            COPFormsPlot.plt.Frame(drawFrame: true, left: true, right: true, top: true, bottom: true, frameColor: null);
            COPFormsPlot.plt.Style(figBg: Color.Transparent);//FromArgb(246, 249, 0));
            COPFormsPlot.BackColor = Color.Transparent;//FromArgb(246, 249, 0));
            COPFormsPlot.Render();
        }

        void UpdateCopPlot(double x, double z)
        {

            if(nSens == 2)
            {
                COPFormsPlot.plt.GetPlottables().RemoveAt(5); // removes only COP DOT
            }
            if (nSens == 3)
            {
                COPFormsPlot.plt.GetPlottables().RemoveAt(6); // removes only COP DOT
            }

            COPFormsPlot.plt.PlotPoint(x, z, color: Color.Red, markerSize: 10);
            COPFormsPlot.plt.Grid(enable: false, xSpacing: 10, ySpacing: 10);
            COPFormsPlot.plt.Axis(-(50 / 2), 50 / 2, -(50 / 2), 50 / 2);
            COPFormsPlot.Render(skipIfCurrentlyRendering: true);
        }

        double[] Data_Conversion(double[] dataBits, int ch)
        {
            for (int i = 0; i < dataBits.Count(); i++)
            {
                //if(dataBits.Count() >= 10000)
                //{
                //    MySerial.Close();
                //}
                dataBits[i] *= LinReg[ch][0]; // [Kg] or [Kg*m]
            }
            return dataBits;
        }

        void UpdateUI_Live_Chart()
        {
            double[] Ch1array = Data_Conversion(Ch1CircularBuffer.ToArray(), 0); // [bits] to [Kg]
            double[] Ch2array = Data_Conversion(Ch2CircularBuffer.ToArray(), 1); // [bits] to [Kg * m]
            double[] Ch3array = Data_Conversion(Ch3CircularBuffer.ToArray(), 2); // [bits] to [Kg]

            //double[] Ch1array = Ch1CircularBuffer.ToArray();
            //double[] Ch2array = Ch2CircularBuffer.ToArray();
            //double[] Ch3array = Ch3CircularBuffer.ToArray();


            UpdateScottPlot(Ch1array, Ch2array, Ch3array);

            int MeanSize = 500;
            if (Ch1array.Length > MeanSize)
            {
                Ch1Mean = Ch1array.Skip(Ch1array.Length - MeanSize).Sum() / MeanSize; // [kg]
                Ch2Mean = Ch2array.Skip(Ch2array.Length - MeanSize).Sum() / MeanSize; // [k] when 2D or [kg*m] when 3D
                Ch3Mean = Ch3array.Skip(Ch3array.Length - MeanSize).Sum() / MeanSize; // [kg*m]
            }

            double X = (Ch2Mean / Ch1Mean) * 100; //[m] to [cm]
            double Z = (Ch3Mean / Ch1Mean) * 100; //[m] to [cm]

            if (nSens == 1)
            {
                SetText(PesoText, Ch1Mean.ToString("#00.000"));
                SetText(NewtonText, (Ch1Mean * Gravity).ToString("#00.000"));
            }
            else if (nSens == 2)
            {
                SetText(PesoText, (Ch1Mean + Ch2Mean).ToString("#00.000"));
                SetText(NewtonText, ((Ch1Mean + Ch2Mean) * Gravity).ToString("#00.000"));

                UpdateCopPlot((50 * (Ch1Mean / (Ch1Mean + Ch2Mean))) - 25,0);
            }
            else
            {
                SetText(PesoText, (Ch1Mean).ToString("#00.000"));
                SetText(NewtonText, (Ch1Mean * Gravity).ToString("#00.000"));

                UpdateCopPlot(Z, X);
            }

        }


        #endregion

        #region Sample Routines
        void InitRecording()
        {
            Update_EventLog_TextBox("Coleta iniciada.");

            Parallel.Invoke(() =>
                this.BeginInvoke((Action)(() =>
                {
                    Record_Button.Enabled = false;
                    Stop_Button.Enabled = true;
                    Plot_Button.Enabled = false;
                    Plot_Button.BackColor = Color.DarkGray;
                    StatusText.Text = "REC";
                })));

            if (int.TryParse(RecordTime_TextBox.Text, out int a))
            { RecordTime = a; }
            else { RecordTime = 30; }
            int SamplesToRecord = RecordTime * 1200;

            UpdateSizeBuffers("ChartBuffer", ChartXSize); //just to reset live chart data
            UpdateSizeBuffers("RecordBuffer", SamplesToRecord); //This is the buffer that will collect data to save

            RecordTrigger = false;
            while (RecordTrigger == false) ;
            RecordWatch.Start();
            isRecording = true; //Initializa data recording to RecordBuffers
            Relay_on_off("ON");
            new Thread(() => Console.Beep(2000, 500)).Start(); //just a small beep to say it istarted
        }

        void FinishRecording()
        {
            isRecording = false;
            ElapsedTime = RecordWatch.Elapsed;
            RecordWatch.Reset();
            BeepWatch.Reset();
            RecordWatch.Reset();

            Update_EventLog_TextBox("Coleta finalizada, processando dados...");
            List<List<double>> dataTemp = new List<List<double>>
            {
                Ch1RecordBuffer.ToArray().ToList(),
                Ch2RecordBuffer.ToArray().ToList(),
                Ch3RecordBuffer.ToArray().ToList()
            };
            dados.Clear();
            dados = RemodeAddRemaining(dataTemp, ElapsedTime); //removes data with predetermined interval to fit 1kHz

            string fileName = GetFileName();
            Save_txt(dados, DateTime.Now - ElapsedTime, ElapsedTime, fileName);
            Save_sad(dados, fileName);
            Update_EventLog_TextBox("Amostra: " + fileName);

            Parallel.Invoke(() =>
                this.BeginInvoke((Action)(() =>
                {
                    Record_Button.Enabled = true;
                    Stop_Button.Enabled = false;
                    Plot_Button.Enabled = true;
                    Plot_Button.BackColor = Color.Blue;
                    if (MySerial.IsOpen)
                    {
                        StatusText.ForeColor = Color.Green;
                        StatusText.Text = "ON";
                    }
                    else
                    {
                        StatusText.ForeColor = Color.Red;
                        StatusText.Text = "OFF";
                    }
                })));
        }

        //string GetFileName()
        //{
        //    List<string> file_pathName = new List<string>();
        //    List<string> file_pathFull = Directory.GetFiles(pathTxt).ToList();

        //    foreach (string v in file_pathFull) //Pega do nome a parte com a data + número
        //        file_pathName.Add(v.Remove(0, v.LastIndexOf('\\') + 1).Remove(11));

        //    List<double> mesma_data = new List<double>();
        //    foreach (string x in file_pathName) //Pega o maior valor dos números de arquivos com mesmo nome
        //        if (x.Contains(DateTime.Now.Date.ToString("dd/MM/yyyy").Replace("/", "")))
        //            mesma_data.Add(Convert.ToInt16(x.Remove(0, 7)));
        //    if (mesma_data.Count > 0)
        //        return DateTime.Now.Date.ToString("dd/MM/yyyy").Replace("/", "") + (mesma_data.Max() + 1).ToString().PadLeft(3, '0');
        //    else
        //        return DateTime.Now.Date.ToString("dd/MM/yyyy").Replace("/", "") + 0.ToString().PadLeft(3, '0');
        //}

        string GetFileName() //formato: NOME-DATA-NUMERO ex:Eduardo-26012020-001 //AINDA NÃO VERIFIQUEI SE ESTA CERTO
        {
            List<string> file_pathFull = Directory.GetFiles(pathTxt).ToList();
            int count = 0;

            string SubjectName = Name_TextBox.Text == "" ? "Sample" : Name_TextBox.Text;

            string data = DateTime.Now.Date.ToString("dd/MM/yyyy").Replace("/", "");

            foreach (string v in file_pathFull) //Verifica se já mesmo nome e mesma data
                if (v.Contains(SubjectName + "-" + data))
                    count++;

            return SubjectName + "-" + data + "-" + count.ToString().PadLeft(3, '0');
        }

        void Save_txt(List<List<double>> saveData, DateTime tInit, TimeSpan tEnd, string fileName)
        {
            try
            {
                TextWriter twTXT = new StreamWriter(pathTxt + fileName + ".txt");
                twTXT.WriteLine("Data da coleta: " + tInit.ToString("dd/MM/yyyy - H:mm:ss"));
                twTXT.WriteLine("Sensores: " + nSens.ToString());
                twTXT.WriteLine("Tempo de coleta: " + tEnd.TotalSeconds.ToString().Replace(",", ".") + " segundos.");
                twTXT.WriteLine("Amostras: " + saveData[0].Count);
                twTXT.WriteLine("Taxa de aquisição: " + (saveData[0].Count / tEnd.TotalSeconds).ToString("#0.00"));
                //twTXT.WriteLine(("Taxa de aquisição: " + saveData[0].Count() / saveData[saveData.Count() - 1].Last() + "Hz").Replace(",", "."));
                if (nSens == 1) { twTXT.WriteLine("Tempo\t\tSensor 1"); }
                if (nSens == 2) { twTXT.WriteLine("Tempo\t\tSensor 1\tSensor 2"); }
                if (nSens == 3) { twTXT.WriteLine("Tempo\t\tSensor 1\tSensor 2\tSensor 3"); }
                //twTXT.WriteLine("Tempo\t\tSensor 1\tSensor 2\tSensor 3");

                for (int i = 0; i < saveData[0].Count; i++)
                {
                    string f = saveData[3][i].ToString("#0.0000000000").Replace(",", ".");
                    for (int k = 0; k < nSens; k++)
                        f += "\t " + saveData[k][i].ToString("#0.0000000000").Replace(",", ".");
                    twTXT.WriteLine(f);
                }
                twTXT.Close();
            }
            catch (Exception e)
            {
                Update_EventLog_TextBox("Error sTXT01: " + e.Message);
            }
        }

        void Save_sad(List<List<double>> saveData, string fileName)
        {
            try
            {
                var SAD = new StringBuilder();
                SAD.AppendLine("SADv2"); SAD.AppendLine("Arquivo tipo AQDADOS importado"); SAD.AppendLine(""); SAD.AppendLine(""); SAD.AppendLine("");
                if (nSens == 1) { SAD.AppendLine("1"); }
                if (nSens == 2) { SAD.AppendLine("2"); }
                if (nSens == 3) { SAD.AppendLine("3"); }
                SAD.AppendLine("1"); SAD.AppendLine("0 0 1 0 0 0 0"); SAD.AppendLine("0 0 0 0 0 0 0");

                for (int i = 0; i < nSens; i++)
                {
                    SAD.AppendLine("Sensor " + (i + 1).ToString());
                    SAD.AppendLine((i + 1).ToString());
                    SAD.AppendLine("0");
                    SAD.AppendLine(saveData[0].Count().ToString());
                    for (int j = 0; j < saveData[0].Count(); j++)
                    {
                        string[] f = { saveData[3][j].ToString("#0.0000000000").Replace(",", "."), saveData[i][j].ToString("#0.0000000000").Replace(",", ".") };
                        SAD.AppendLine(string.Join(" ", f));
                    }
                }
                File.WriteAllText(pathSad + fileName + ".SAD", SAD.ToString());
            }
            catch (Exception e)
            {
                Update_EventLog_TextBox("Error sSAD01: " + e.Message);
            }
        }

        List<List<double>> RemodeAddRemaining(List<List<double>> saveData, TimeSpan tEnd)
        {
            double nSize = saveData[0].Count;
            double nMilis = tEnd.TotalMilliseconds;
            double nToRemove = nSize - nMilis;
            double interval = nSize / nToRemove;

            List<int> sensRemove = Enumerable.Range(0, Convert.ToInt32(Math.Round(Math.Abs(nToRemove)))).Select(x => Convert.ToInt32(x * (nSize / Math.Abs(nToRemove)))).ToList();
            var sensRemoveOrdered = sensRemove.OrderByDescending(i => i);

            if (nToRemove > 0) //If sps > 1000Hz, remove samples at regular time
            {
                foreach (int index in sensRemoveOrdered)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        saveData[i].RemoveAt(index);
                    }
                }
            }
            else //If sps < 1000Hz, duplicate samples at regular time
            {
                sensRemove[0] = 1;
                foreach (int index in sensRemove)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        saveData[i].Insert(index, saveData[i][index - 1]);
                    }
                }
            }

            //Criando o vetor de tempo
            List<double> Timestamp = new List<double>();
            double sum = 0;
            int size = saveData[0].Count;
            for (int i = 0; i < size; i++)
            {
                Timestamp.Add(sum);
                sum += 0.001;
            }
            saveData.Add(Timestamp);

            return saveData;
        }

        void Open_file()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = pathTxt,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            dados.Clear();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.SafeFileName.Remove(openFileDialog1.SafeFileName.Length - 4);
                Update_EventLog_TextBox("Processando coleta " + fileName + "...");
                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    {
                        sr.ReadLine();
                        nSens = Convert.ToInt32(Regex.Split(sr.ReadLine(), ": ").Last());
                        sr.ReadLine();
                        int nSamples = Convert.ToInt32(Regex.Split(sr.ReadLine(), ": ").Last());
                        sr.ReadLine(); sr.ReadLine();

                        for (int i = 0; i <= nSens; i++)
                            dados.Add(new List<double>(new double[nSamples]));

                        int k = 0;
                        for (int i = 0; i < nSamples; i++)
                        {
                            string[] result = Regex.Split(sr.ReadLine(), "\t");
                            for (int j = 0; j < nSens; j++)
                                dados[j][k] = Convert.ToDouble(result[j + 1].Replace(".", ","));

                            dados[dados.Count() - 1][k] = Convert.ToDouble(result[0].Replace(".", ",")); //tempo (ultimo)
                            k++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
            Update_EventLog_TextBox("Coleta processada.");
            Plot_Button.Enabled = true;
            Plot_Button.BackColor = Color.Blue;
        }

        #endregion

        #region Overall Functions

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

        List<List<double>> GetCalibConfig()
        {
            List<List<double>> configs = new List<List<double>>();
            TextReader tw = new StreamReader(@"CalibConfig.txt");
            string a = tw.ReadToEnd();
            tw.Close();

            List<double> configA = new List<double>();
            List<double> configB = new List<double>();
            List<double> configC = new List<double>();

            if (nSens == 1)
            {
                configA.Add(Convert.ToDouble(a.Split('\r')[1].Substring(5).Split(',')[0].Replace('.', ',')));
                configA.Add(Convert.ToDouble(a.Split('\r')[1].Substring(5).Split(',')[1].Replace('.', ',')));
                configA.Add(Convert.ToDouble(a.Split('\r')[1].Substring(5).Split(',')[2].Replace('.', ',')));
                
                configB.Add(0); configB.Add(0); configB.Add(0);
                
                configC.Add(0); configC.Add(0); configC.Add(0);
            }

            else if (nSens == 2)
            {
                configA.Add(Convert.ToDouble(a.Split('\r')[3].Substring(5).Split(',')[0].Replace('.', ',')));
                configA.Add(Convert.ToDouble(a.Split('\r')[3].Substring(5).Split(',')[1].Replace('.', ',')));
                configA.Add(Convert.ToDouble(a.Split('\r')[3].Substring(5).Split(',')[2].Replace('.', ',')));

                configB.Add(Convert.ToDouble(a.Split('\r')[1].Substring(5).Split(',')[0].Replace('.', ',')));
                configB.Add(Convert.ToDouble(a.Split('\r')[1].Substring(5).Split(',')[1].Replace('.', ',')));
                configB.Add(Convert.ToDouble(a.Split('\r')[1].Substring(5).Split(',')[2].Replace('.', ',')));
                
                configC.Add(0); configC.Add(0); configC.Add(0);
            }

            else if (nSens == 3)
            {
                configA.Add(Convert.ToDouble(a.Split('\r')[3].Substring(5).Split(',')[0].Replace('.', ',')));
                configA.Add(Convert.ToDouble(a.Split('\r')[3].Substring(5).Split(',')[1].Replace('.', ',')));
                configA.Add(Convert.ToDouble(a.Split('\r')[3].Substring(5).Split(',')[2].Replace('.', ',')));

                configB.Add(Convert.ToDouble(a.Split('\r')[4].Substring(5).Split(',')[0].Replace('.', ',')));
                configB.Add(Convert.ToDouble(a.Split('\r')[4].Substring(5).Split(',')[1].Replace('.', ',')));
                configB.Add(Convert.ToDouble(a.Split('\r')[4].Substring(5).Split(',')[2].Replace('.', ',')));

                configC.Add(Convert.ToDouble(a.Split('\r')[5].Substring(5).Split(',')[0].Replace('.', ',')));
                configC.Add(Convert.ToDouble(a.Split('\r')[5].Substring(5).Split(',')[1].Replace('.', ',')));
                configC.Add(Convert.ToDouble(a.Split('\r')[5].Substring(5).Split(',')[2].Replace('.', ',')));
            }

            configs.Add(configA); configs.Add(configB); configs.Add(configC);
            return configs;
        }

        public static T[] GetMatrixPart<T>(T[,] matrix, int row, int columns)
        {
            var array = new T[columns];
            for (int i = 0; i < columns; ++i)
                array[i] = matrix[row, i];
            return array;
        }
        #endregion

        #region BackgroundWorker2_DoWork

        //private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    List<byte>  SerialSlidingWindow = new List<byte>();
        //    dados.Clear();
        //    mvDataA.Clear();
        //    SensorA.Clear();
        //    mvDataB.Clear();
        //    SensorB.Clear();
        //    mvDataC.Clear();
        //    SensorC.Clear();
        //    Tempo.Clear();
        //    first = true;
        //    MySerial.Write("0");  //TURN OFF RELAY

        //    BackgroundWorker worker = sender as BackgroundWorker;

        //    if (MySerial.IsOpen) //Clear buffer
        //    {
        //        //BackgroundBeep.Beep(); //Console.Beep(1000, 100); 
        //        int dt = MySerial.BytesToRead;
        //        byte[] dR = new byte[dt];
        //        MySerial.Read(dR, 0, dt);
        //    }
        //    else //if not opened (not supposed to), open port and clear buffer
        //    {
        //        MessageBox.Show("Erro: Porta " + MySerial.PortName + " estava fechada.");
        //        MySerial.Open();
        //        Task.Delay(1500).Wait();
        //        //Console.Beep(1000, 100); //Console.Beep(5000, 1000);
        //        int dt = MySerial.BytesToRead;
        //        byte[] dR = new byte[dt];
        //        MySerial.Read(dR, 0, dt);
        //    }

        //    var stopwatch = Stopwatch.StartNew();
        //    bool sincronizador = false;
        //    //aTimer.Stop();
        //    //aTimer.Start();

        //    while (true)
        //    {
        //        if (worker.CancellationPending)
        //        {
        //            e.Cancel = true;
        //            break;
        //        }
        //        else
        //        {
        //            if (MySerial.IsOpen)
        //            {
        //                int dataLength = MySerial.BytesToRead;
        //                byte[] dataRecevied = new byte[dataLength];
        //                MySerial.Read(dataRecevied, 0, dataLength);

        //                if (dataRecevied.Length > 0)
        //                {
        //                    for (int i = 0; i < dataRecevied.Length; i++)
        //                         SerialSlidingWindow.Add(dataRecevied[i]);

        //                    //TextWriter asdasd = new StreamWriter(pathLog + @"SavedListA.txt");

        //                    //Remove 1ra amostra se quebrada
        //                    //Verifica se a placa esta configurada corretamente
        //                    if (first)
        //                    {
        //                        first = false;
        //                        while (true)
        //                        {
        //                            try
        //                            {
        //                                if (!byteDivisor.SequenceEqual( SerialSlidingWindow.GetRange(0, 4)))
        //                                     SerialSlidingWindow.RemoveAt(0);
        //                                else if (!byteDivisor.SequenceEqual( SerialSlidingWindow.GetRange(div1, 4)))
        //                                {
        //                                    errorToColect();
        //                                    break;
        //                                }
        //                                else { break; }
        //                            }
        //                            catch (Exception)
        //                            {
        //                                errorToColect();
        //                                break;
        //                            }
        //                        }
        //                        startTime = DateTime.Now;
        //                        stopwatch = Stopwatch.StartNew();
        //                        Task.Run(() => BackgroundBeep.Beep());
        //                    }

        //                    while ( SerialSlidingWindow.Count() > 60)
        //                    {
        //                        if (byteDivisor.SequenceEqual( SerialSlidingWindow.GetRange(0, 4)) && byteDivisor.SequenceEqual( SerialSlidingWindow.GetRange(div1, 4)))
        //                        { //Verifica se a amostra inicia com \r\r\r\r e esta completa ->(\r\r\r\rb0b1b2b3\t0t1t2t3\r\r\r\r....)
        //                            List<byte[]> UnsignedByteArray = new List<byte[]>();
        //                            for (int sn = 1; sn <= nSens; sn++)
        //                            {
        //                                UnsignedByteArray.Add( SerialSlidingWindow.GetRange(4*sn, 4).ToArray());
        //                            }
        //                            if (div1 == 20 && nSens == 2) //Para o caso de 2 sensores
        //                                UnsignedByteArray.Add( SerialSlidingWindow.GetRange(16, 4).ToArray());
        //                            else
        //                                UnsignedByteArray.Add( SerialSlidingWindow.GetRange(4 * (nSens + 1), 4).ToArray());

        //                            //byte[] timeTemp =  SerialSlidingWindow.GetRange(4 * (nSens + 1), 4).ToArray();

        //                            if (BitConverter.IsLittleEndian)
        //                            {
        //                                for (int sn = 0; sn < UnsignedByteArray.Count(); sn++)
        //                                    Array.Reverse(UnsignedByteArray[sn]);
        //                                //Array.Reverse(timeTemp);
        //                            }

        //                            SetDataValues(UnsignedByteArray);

        //                             SerialSlidingWindow.RemoveRange(0, div1);
        //                        }
        //                        else
        //                        { //Se amostra não ta completa, descartar ela -> igual a amostra anterior
        //                            if (SensorA.Count > 0)
        //                            {
        //                                if (nSens == 1) 
        //                                { 
        //                                    SensorA.Add(SensorA.Last());
        //                                    Ch1CircularBuffer.Put(SensorA.Last());
        //                                }
        //                                if (nSens == 2)
        //                                {
        //                                    SensorA.Add(SensorA.Last());
        //                                    SensorB.Add(SensorB.Last());
        //                                    Ch1CircularBuffer.Put(SensorA.Last());
        //                                    Ch2CircularBuffer.Put(SensorB.Last());
        //                                }
        //                                if (nSens == 3)
        //                                {
        //                                    SensorA.Add(SensorA.Last());
        //                                    SensorB.Add(SensorB.Last());
        //                                    SensorC.Add(SensorC.Last());
        //                                    Ch1CircularBuffer.Put(SensorA.Last());
        //                                    Ch2CircularBuffer.Put(SensorB.Last());
        //                                    Ch3CircularBuffer.Put(SensorC.Last());
        //                                }
        //                                Tempo.Add(Tempo.Last());
        //                            }
        //                            bool b = true; int i = 1;
        //                            while (b)
        //                            {
        //                                //i++;
        //                                try
        //                                {
        //                                    if (byteDivisor.SequenceEqual( SerialSlidingWindow.GetRange(i, 4)))
        //                                    {
        //                                         SerialSlidingWindow.RemoveRange(0, i);
        //                                        b = false;
        //                                    }
        //                                    i++;
        //                                }
        //                                catch { b = false; }
        //                            }
        //                        }
        //                    }
        //                    if (SensorA.Count() >  0)
        //                    {
        //                        if (nSens == 1)
        //                        {
        //                            double[] charPlot = { sensorAmm };
        //                            //UpdateLiveChart(LiveChart, charPlot);// wSensorA.Last() + mmSensorB.Last());
        //                        }
        //                        else if (nSens == 2)
        //                        {
        //                            double[] charPlot = { sensorAmm, sensorBmm };
        //                            //UpdateLiveChart(LiveChart, charPlot);// wSensorA.Last() + mmSensorB.Last());
        //                            UpdateCOPChart(COPChart, (50 * (sensorAmm / (sensorAmm + sensorBmm))) - 25, 0);
        //                        }
        //                        else if (nSens == 3)
        //                        {
        //                            double[] charPlot = { sensorAmm, sensorBmm, sensorCmm };
        //                            //double[] charPlot = { sensorAmm * gravidade, sensorBmm * gravidade, sensorCmm * gravidade };
        //                            //UpdateLiveChart(LiveChart, charPlot);//wSensorA.Last());
        //                            UpdateCOPChart(COPChart, (sensorCmm / sensorAmm)*100, (sensorBmm / sensorAmm)*100);
        //                        }
        //                    }

        //                    //asdasd.Close();
        //                }

        //                if (stopFlag)
        //                {
        //                    var stopTime = startTime + stopwatch.Elapsed;
        //                    stopwatch.Stop();
        //                    //SetProgressBar(ProgressBar, 100);
        //                    //break; //STOP DATA ACQUISITION
        //                }

        //                if (!sincronizador && stopwatch.Elapsed.TotalMilliseconds > 3000)
        //                {
        //                    MySerial.Write("1");  //TURN ON RELAY
        //                    sincronizador = true;
        //                }

        //                if (sincronizador && stopwatch.Elapsed.TotalMilliseconds > 5000)
        //                {
        //                    MySerial.Write("0");  //TURN OFF RELAY
        //                }

        //                var cronometro = DateTime.Now.Subtract(startTime);
        //                SetText(clock_display, cronometro.Hours.ToString().PadLeft(2, '0') + ":" +
        //                    cronometro.Minutes.ToString().PadLeft(2, '0') + ":" +
        //                    cronometro.Seconds.ToString().PadLeft(2, '0'));
        //            }
        //            else
        //            {
        //                /* DECODIFICAR O RESTO DOS DADOS NA JANELA 
        //                break;
        //            }
        //        }
        //    }
        //    MySerial.Close();
        //    //dados.Add(ReadValues);
        //    ////dados.Add(ReadTimes);
        //    //TextWriter tw = new StreamWriter(pathLog + @"SavedList.txt");
        //    //foreach (double s in ReadValues)
        //    //    tw.WriteLine(s);
        //    //tw.Close();
        //}

        //private void errorToColect()
        //{
        //    if (MySerial.IsOpen)
        //        MySerial.Close();
        //    //Stop_Button_Click(sender, e);
        //    Update_EventLog_TextBox("Placa não configurada corretamente.");
        //    stopFlag = true;
        //    this.BeginInvoke((Action)(() =>
        //    {
        //        Stop_Button.Enabled = false;
        //        Record_Button.Enabled = true;
        //        Plot_Button.Enabled = false;
        //        OpenFile_Button.Enabled = true;
        //        Calib_Button.Enabled = true;
        //        SerialPortsBox.Enabled = true;
        //        StatusText.Text = "OFF";
        //        StatusText.ForeColor = System.Drawing.Color.Red;
        //    }));
        //}

        //readonly int mmSize = 50;
        //void SetDataValues(List<byte[]> mvRead)
        //{
        //    if(nSens == 1)
        //    {
        //        mvDataA.Add(BitConverter.ToInt32(mvRead[0], 0) * resolution_mV);
        //        if (InitRecordFlag)
        //        {
        //            SensorA.Add((mvDataA.Last()) * LinReg[0][0] + LinReg[0][1]);
        //        }
        //        nSamples++;

        //        wSensorA.Add((mvDataA.Last()) * LinReg[0][0] + LinReg[0][1]);
        //        if (wSensorA.Count() > mmSize)
        //            wSensorA.RemoveAt(0);
        //        sensorAmm = wSensorA.Sum() / wSensorA.Count();
        //        Ch1CircularBuffer.Put(sensorAmm); 
        //    }
        //    else if (nSens == 2)
        //    {
        //        mvDataA.Add(BitConverter.ToInt32(mvRead[0], 0) * resolution_mV);
        //        mvDataB.Add(BitConverter.ToInt32(mvRead[1], 0) * resolution_mV);

        //        if (InitRecordFlag)
        //        {
        //            SensorA.Add(mvDataA.Last() * LinReg[0][0] + LinReg[0][1]);
        //            SensorB.Add(mvDataB.Last() * LinReg[1][0] + LinReg[1][1]);
        //        }
        //        nSamples++;

        //        wSensorA.Add(mvDataA.Last() * LinReg[0][0] + LinReg[0][1]);
        //        wSensorB.Add(mvDataB.Last() * LinReg[1][0] + LinReg[1][1]);
        //        if (wSensorA.Count() > mmSize)
        //        {
        //            wSensorA.RemoveAt(0);
        //            wSensorB.RemoveAt(0);
        //        }
        //        sensorAmm = wSensorA.Sum() / wSensorA.Count();
        //        sensorBmm = wSensorB.Sum() / wSensorB.Count();
        //        Ch1CircularBuffer.Put(sensorAmm);
        //        Ch2CircularBuffer.Put(sensorBmm);
        //    }
        //    else if (nSens == 3)
        //    {
        //        mvDataA.Add(BitConverter.ToInt32(mvRead[0], 0) * resolution_mV);
        //        mvDataB.Add(BitConverter.ToInt32(mvRead[1], 0) * resolution_mV);
        //        mvDataC.Add(BitConverter.ToInt32(mvRead[2], 0) * resolution_mV);
        //        nSamples++;

        //        if (InitRecordFlag)
        //        {
        //            SensorA.Add(mvDataA.Last() * LinReg[0][0] + LinReg[0][1]);
        //            SensorB.Add(mvDataB.Last() * LinReg[1][0] + LinReg[1][1]);
        //            SensorC.Add(mvDataC.Last() * LinReg[2][0] + LinReg[2][1]);
        //        }

        //        wSensorA.Add(mvDataA.Last() * LinReg[0][0] + LinReg[0][1]);
        //        wSensorB.Add(mvDataB.Last() * LinReg[1][0] + LinReg[1][1]);
        //        wSensorC.Add(mvDataC.Last() * LinReg[2][0] + LinReg[2][1]);
        //        if (wSensorA.Count() > mmSize)
        //        {
        //            wSensorA.RemoveAt(0);
        //            wSensorB.RemoveAt(0);
        //            wSensorC.RemoveAt(0);
        //        }
        //        sensorAmm = wSensorA.Sum() / wSensorA.Count();
        //        sensorBmm = wSensorB.Sum() / wSensorB.Count();
        //        sensorCmm = wSensorC.Sum() / wSensorC.Count();
        //        Ch1CircularBuffer.Put(sensorAmm);
        //        Ch2CircularBuffer.Put(sensorBmm);
        //        Ch3CircularBuffer.Put(sensorCmm);
        //    }


        //    Tempo.Add(Convert.ToDouble(BitConverter.ToUInt32(mvRead.Last(), 0)) / 1000);

        //    //mvData.Add((BitConverter.ToUInt32(mvRead, 0) * resolution_mV) - LinReg[0][3]);// = SensorA.Last() - LinReg[0][3];
        //    //SensorA.Add(mvData.Last() * LinReg[0][0] + LinReg[0][1]);
        //    //Tempo.Add(Convert.ToDouble(BitConverter.ToUInt32(tRead, 0)) / 1000);
        //}

        #endregion
    }

}
