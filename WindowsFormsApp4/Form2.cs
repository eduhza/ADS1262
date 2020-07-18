using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp4
{
    public partial class Form2 : Form
    {
        public Form2(List<List<double>> dados, int nSens)
        {
            InitializeComponent();
            PlotScottPlot(formsPlot1 , dados, nSens);
        }

        void PlotScottPlot(FormsPlot chart, List<List<double>> dados, int nSens)
        {
            chart.plt.Clear();
            Color[] colors = { Color.FromArgb(51, 205, 117), Color.Blue, Color.Magenta };
            double maxY = 0;
            double minY = 0;

            for (int i = 0; i < nSens; i++)
            {
                chart.plt.PlotSignal(dados[i].ToArray(), sampleRate: 1, markerSize: 1, color: colors[i]);
                double maxYS = dados[i].Max();
                double minYS = dados[i].Min();
                if (maxYS > maxY)
                    maxY = maxYS;
                if (minYS < minY)
                    minY = minYS;
            }

            chart.plt.Ticks(
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
            chart.Configure(
                enablePanning: true,
                enableZooming: true,
                enableRightClickMenu: true,
                lowQualityWhileDragging: false,
                enableDoubleClickBenchmark: true,
                lockVerticalAxis: false,
                lockHorizontalAxis: false,
                equalAxes: false);
            
            chart.plt.YLabel("Kg", bold:true);
            chart.plt.XLabel("Segundos", bold: true);
            chart.plt.Title(enable: true);

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
            chart.plt.YTicks(YtickPositions, YtickLabels);

            double Xmax = dados[0].Count;
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
                    (Xmax/1000).ToString(),
                    ((Xmax - 1.0 * Xtick)/1000).ToString(),
                    ((Xmax - 2.0 * Xtick)/1000).ToString(),
                    ((Xmax - 3.0 * Xtick)/1000).ToString(),
                    ((Xmax - 4.0 * Xtick)/1000).ToString() };
            chart.plt.XTicks(XtickPositions, XtickLabels);
            chart.plt.Axis(0, Xmax, minY, maxY);

            if (nSens == 1)
                chart.plt.Title("Plataforma Vertical");
            else if (nSens == 2)
                chart.plt.Title("Plataforma Bilatera");
            else if (nSens == 3)
                chart.plt.Title("Plataforma Tridimensional");


            chart.plt.TightenLayout(padding: 0, render: true);
            chart.plt.Frame(drawFrame: true, left: true, right: true, top: true, bottom: true, frameColor: null);
            chart.plt.Style(figBg: Color.Transparent);//FromArgb(246, 249, 0));
            chart.BackColor = Color.Transparent;//FromArgb(246, 249, 0));
            chart.Render();
        }
    }
}