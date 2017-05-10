using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Graph = System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace FingerPrint
{
    public partial class Plots : Form
    {
        Graph.Chart chart;
        int totalFiles = 0;
        string path = "";
        public Plots(int TotalFiles,string readFrom)
        {
            InitializeComponent();
            totalFiles = TotalFiles;
            path = readFrom;
            this.ClientSize = new System.Drawing.Size(750, 900);
        }

        private void Plots_Load(object sender, EventArgs e)
        {
            const int MaxX = 1;
            // Create new Graph
            chart = new Graph.Chart();
            chart.Location = new System.Drawing.Point(10, 10);
            chart.Size = new System.Drawing.Size(700, 700);
            // Add a chartarea called "draw", add axes to it and color the area black
            chart.ChartAreas.Add("draw");
            chart.ChartAreas["draw"].AxisX.Minimum = 0;
            chart.ChartAreas["draw"].AxisX.Maximum = MaxX;
            chart.ChartAreas["draw"].AxisX.Interval = 0.1;
            chart.ChartAreas["draw"].AxisX.MajorGrid.LineColor = Color.White;
            chart.ChartAreas["draw"].AxisX.MajorGrid.LineDashStyle = Graph.ChartDashStyle.Dash;
            chart.ChartAreas["draw"].AxisY.Minimum = 0;
            chart.ChartAreas["draw"].AxisY.Maximum = 1;
            chart.ChartAreas["draw"].AxisY.Interval = 0.1;
            chart.ChartAreas["draw"].AxisY.MajorGrid.LineColor = Color.White;
            chart.ChartAreas["draw"].AxisY.MajorGrid.LineDashStyle = Graph.ChartDashStyle.Dash;

            chart.ChartAreas["draw"].BackColor = Color.White;

            // Create a new function series
            chart.Series.Add("FAR");
            chart.Series.Add("FRR");
            // Set the type to line      
            chart.Series["FAR"].ChartType = Graph.SeriesChartType.Line;
            chart.Series["FRR"].ChartType = Graph.SeriesChartType.Line;
            // Color the line of the graph light green and give it a thickness of 3
            chart.Series["FAR"].Color = Color.Blue;
            chart.Series["FAR"].BorderWidth = 3;
            chart.Series["FRR"].Color = Color.Green;
            chart.Series["FRR"].BorderWidth = 3;
            //This function cannot include zero, and we walk through it in steps of 0.1 to add coordinates to our series

            string[] lines = System.IO.File.ReadAllLines(path);
            double[] score = new double[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                score[i] = double.Parse(lines[i]);
            }
            double[] far = new double[100];
            double[] frr = new double[100];

            double threshold = 0.0;

            int k = 0;
            for (threshold = 0; threshold <= 1.0; threshold += 0.1)
            {
                far[k] = 0;
                for (int i = 0; i < score.Length; i++)
                { 
                    if (score[i] > threshold)
                    {
                        far[k]++;
                    }
                }
                far[k] = far[k] / (totalFiles * totalFiles);

                frr[k] = 0;
                for (int j = 0; j < score.Length; j += (totalFiles + 1))
                {
                    //Console.WriteLine("Iteration : " + j.ToString());
                    //Console.WriteLine("Threshold:");
                    //Console.WriteLine(threshold);

                    if (score[j] <= threshold)
                    {
                        //Console.WriteLine("\n");
                        //Console.WriteLine(score[j]);
                        //Console.WriteLine("\n");
                        //Console.WriteLine("\n");
                        frr[k]++;
                    }

                }

                frr[k] = frr[k] / (20);
                k++;
            }
            k = 0;
            for (double x = 0; x <=MaxX; x += 0.1)
            {
                chart.Series["FAR"].Points.AddXY(x, far[k]);
                chart.Series["FRR"].Points.AddXY(x, frr[k]);
                k++;
            }
            chart.Series["FAR"].LegendText = "FAR";
            chart.Series["FRR"].LegendText = "FRR";
            // Create a new legend called "MyLegend".
            chart.Legends.Add("FAR");
            chart.Legends.Add("FRR");
            chart.Legends["FAR"].BorderColor = Color.Tomato; // I like tomato juice!
            chart.Legends["FRR"].BorderColor = Color.Tomato; // I like tomato juice!
            Controls.Add(this.chart);
        }
    }
}