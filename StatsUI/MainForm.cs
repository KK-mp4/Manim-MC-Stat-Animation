namespace StatsUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        List<Stat> Stats = new List<Stat>();

        public MainForm()
        {
            InitializeComponent();

            UpdateBinding();

            comboBox1.SelectedIndex = 0;
        }

        protected override void OnPaintBackground(PaintEventArgs e) // Paints BG to gradient
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                                                               Color.FromArgb(152,158,176),
                                                               Color.FromArgb(84, 87, 107),
                                                               45F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)    // Updates BG upon resize
        {
            this.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)  // Gets data from Microsoft SQL Sever
        {
            this.Cursor = Cursors.WaitCursor;

            DataAccess db = new DataAccess();

            int stat_id = 0;
            try
            {
                stat_id = Convert.ToInt32(tBstat_id.Text);
                if (stat_id < 1)
                {
                    stat_id = 1;
                    tBstat_id.Text = "1";
                }
                else if (stat_id > 1879)
                {
                    stat_id = 1879;
                    tBstat_id.Text = "1879";
                }
            }
            catch
            {
                MessageBox.Show("Incorrect stat_id input: " + tBstat_id.Text, "Error");
                this.Cursor = Cursors.Default;
                return;
            }

            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Incorrect date input: ", "Error");
                this.Cursor = Cursors.Default;
                return;
            }

            Stats.Clear();
            Stats = db.GetStats(stat_id, dateTimePicker1.Value, dateTimePicker2.Value);

            UpdateBinding();
            this.Cursor = Cursors.Default;
        }

        private void UpdateBinding()    // Refreshes statsList
        {
            statsList.DataSource = Stats;
            statsList.DisplayMember = "FullInfo";
        }

        private void button2_Click(object sender, EventArgs e)  // Generates Manim scene.py file
        {
            if (Stats.Count < 1)
            {
                MessageBox.Show("No data.", "Error");
                return;
            }

            switch(comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        CumulativeChart();
                    }
                    break;

                case 1:
                    {
                        LineChart();
                    }
                    break;

                default:
                    {
                        return;
                    }
            }   
        }

        private void LineChart()
        {
            return;
        }

        private void CumulativeChart()
        {
            this.Cursor = Cursors.WaitCursor;

            List<StatsProcessed> ProcessedList = new List<StatsProcessed>();
            List<UuidAmount> uuidList = new List<UuidAmount>();

            while (Stats.Count > 0) 
            {
                DateTime date = Stats[0].timestamp.Date;

                for (int i = 0; i < Stats.Count; i++)
                {
                    if (date == Stats[i].timestamp.Date)
                    {
                        if (!uuidList.Exists(x => x.uuid == Stats[i].uuid))
                        {
                            uuidList.Add(new UuidAmount { uuid = Stats[i].uuid, amount = Stats[i].amount });
                        }

                        int uuidindex = uuidList.FindIndex(x => x.uuid == Stats[i].uuid);

                        if (ProcessedList.Exists(x => x.timestamp == date))
                        {
                            int index = ProcessedList.FindIndex(x => x.timestamp == date);
                            ProcessedList[index].amount += Stats[i].amount - uuidList[uuidindex].amount;
                        }
                        else
                        {
                            ProcessedList.Add(new StatsProcessed { timestamp = date, amount = Stats[i].amount - uuidList[uuidindex].amount });
                        }

                        uuidList[uuidindex].amount = Stats[i].amount;
                    }
                }

                Stats.RemoveAt(0);
            }

            int max = ProcessedList[0].amount;
            int maxindex = 0;
            var total = 0;

            for (int i = 0; i < ProcessedList.Count; i++)
            {
                if (ProcessedList[i].amount > max)
                {
                    max = ProcessedList[i].amount;
                    maxindex = i;
                }

                total += ProcessedList[i].amount;
            }

            label4.Text = "Max: " + ProcessedList[maxindex].timestamp.ToShortDateString() + ": " + ProcessedList[maxindex].amount;
            TimeSpan difference = dateTimePicker2.Value - dateTimePicker1.Value;
            label5.Text = "Average: " + Convert.ToInt32((double)total / difference.Days);

            statsList.DataSource = ProcessedList;
            statsList.DisplayMember = "FullInfo";


            using (StreamWriter writer = new StreamWriter(@"..\..\..\..\ManimStatAnim\main.py"))
            {
                TimeSpan totaldays = ProcessedList[ProcessedList.Count - 1].timestamp - dateTimePicker1.Value;
                int incX = Convert.ToInt32((double)totaldays.Days / 10) - 1;
                int incY = Convert.ToInt32((double)ProcessedList[maxindex].amount / 10) - 1;
                if (totaldays.Days < 10)
                    incX = 1;
                if (ProcessedList[maxindex].amount < 10)
                    incY = 1;

                writer.Write("from manim import *\n\n" +
                    "class Chart(Scene):\n" +
                    "\tdef construct(self):\n" +
                    "\t\tText.set_default(font_size=20)\n" +
                    "\t\taxes = Axes(\n" +
                    $"\t\t\tx_range=[1, {totaldays.Days}, {incX}],\n" +
                    $"\t\t\ty_range=[0, {ProcessedList[maxindex].amount + 1}, {incY}],\n" +
                    $"\t\t\taxis_config={{\"include_tip\": False}},\n" +
                    "\t\t\ty_axis_config={\"include_numbers\": True, \"font_size\": 20}\n" +
                    "\t\t)\n" +
                    "\t\tlabels = axes.get_axis_labels(x_label=\"Days\", y_label=\"Amount\").set_color(MAROON)\n" +
                    "\t\tline_graph = axes.plot_line_graph(\n" +
                    "\t\t\tx_values = [");

                for (int i = 1; i < ProcessedList.Count - 1; i++)
                {
                    TimeSpan difference2 = ProcessedList[i].timestamp - dateTimePicker1.Value;
                    writer.Write($"{difference2.Days},");
                }

                writer.Write($"{totaldays.Days}");

                writer.Write($"],\n" +
                    "\t\t\ty_values = [");

                for (int i = 1; i < ProcessedList.Count - 1; i++)
                {
                    writer.Write($"{ProcessedList[i].amount},");
                }

                writer.Write($"{ProcessedList[ProcessedList.Count - 1].amount}");

                writer.Write("],\n" +
                    "\t\t\tline_color=GREEN,\n" +
                    "\t\t\tadd_vertex_dots=False\n" +
                    "\t\t)\n" +
                    "\t\tnumberLine = NumberLine(\n" +
                    "\t\t\tx_range=[1, 10, 1],\n" +
                    "\t\t\tlength=12,\n" +
                    "\t\t).set_color(BLACK).shift(3 * DOWN)\n" +
                    $"\t\tlables2 = numberLine.add_labels(dict_values={{");

                int dayValuesAmount = 9;
                int incrementDays = Convert.ToInt32(totaldays.Days / dayValuesAmount);
                var incDay = dateTimePicker1.Value;
                for (int i = 0; i < dayValuesAmount; i++)
                {                  
                    writer.Write($"{ i + 1 }:\"{ incDay.ToShortDateString() }\",");
                    incDay = incDay.AddDays(incrementDays);
                }

                writer.Write($"{ dayValuesAmount + 1 }:\"{ incDay.ToShortDateString() }\"");

                writer.Write($"}}, font_size=20)\n" +
                    "\t\tself.play(Write(lables2, lag_ratio=0.01, run_time=1), Write(axes, lag_ratio=0.01, run_time=1))\n" +
                    "\t\tself.play(Write(labels, run_time=0.4), Write(line_graph, run_time=5))\n" +
                    "\t\tself.wait()");
            }

            this.Cursor = Cursors.Default;
            return;
        }
    }
}
