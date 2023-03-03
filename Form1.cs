using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using CrossbarSwitch.Properties;
using System.ComponentModel;
using System.Diagnostics;
using CrossbarSwitch.Object_Classes;

namespace CrossbarSwitch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("2");
            comboBox1.Items.Add("4");
            comboBox1.Items.Add("8");
            comboBox1.Items.Add("16");
            comboBox1.Items.Add("32");
            comboBox2.Items.Add("2");
            comboBox2.Items.Add("4");
            comboBox2.Items.Add("6");
            comboBox2.Items.Add("8");
            comboBox3.Items.Add("2");
            comboBox3.Items.Add("4");
            comboBox3.Items.Add("8");
            comboBox3.Items.Add("16");
            comboBox3.Items.Add("32");
            comboBox4.Items.Add("2");
            comboBox4.Items.Add("4");
            comboBox4.Items.Add("6");
            comboBox4.Items.Add("8");
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            chart1.Visible = false;
            comboBox1.SelectedItem = 0;
            comboBox2.SelectedItem = 0;
            comboBox3.SelectedItem = 0;
            comboBox4.SelectedItem = 0;
        }

        #region Global Variables

        Graphics drawArea;
        Bitmap image;
        Queue<int> deleteN = new Queue<int>(); // опашка за изтриване
        Queue<int> deleteM = new Queue<int>(); // опашка за изтриване
        Queue<int?> Latency = new Queue<int?>();
        Queue<int> Breadth = new Queue<int>();
        Queue<int> t = new Queue<int>();
        Rectangle R = new Rectangle();
        Requests requests;
        List<RequestLasts> lasts;
        int globalP;
        List<double> BY, LY;
        List<int> PX;

        #endregion

        #region Events

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton2.Checked = Settings.Default.AutoMode;
            radioButton1.Checked = Settings.Default.StepMode;
            trackBar1.Value = Settings.Default.GenReq;
            trackBar3.Value = Settings.Default.TactLenght;
            comboBox1.Text = Settings.Default.ComboBox1.ToString();
            comboBox2.Text = Settings.Default.ComboBox2.ToString();
            comboBox3.Text = Settings.Default.ComboBox3.ToString();
            comboBox4.Text = Settings.Default.ComboBox4.ToString();
            textBox1.Text = Settings.Default.ModTime;
            checkBox1.Checked = Settings.Default.Visualisation;
            trackBar2.Value = Settings.Default.TrackBar2;
            if (image == null)
            {
                image = new Bitmap(pictureBox2.ClientSize.Width, pictureBox2.ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            label1.Text = "Честота на генерираните заявки: " + (trackBar1.Value).ToString();
            label10.Text = "Дължина (брой тактове за обслужване) на една заявка в паметта: " + Settings.Default.TactLenght.ToString();
            label8.Text = "Време за визуализация на един такт: " + (trackBar2.Value * 0.001).ToString();
            radioButton1_CheckedChanged(sender, e);
            radioButton2_CheckedChanged(sender, e);

        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (image == null)
            {
                image = new Bitmap(pictureBox2.ClientSize.Width, pictureBox2.ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            e.Graphics.DrawImage(image, 0, 0, image.Width, image.Height);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Settings.Default.StepMode)
            {
                if (this.InvokeRequired)
                    dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Clear()));
                else dataGridView1.Rows.Clear();

                if (sender.GetType().Name != "BackgroundWorker")
                {
                    dataGridView1.Visible = false;
                    pictureBox2.Visible = true;
                }
            }
            int n = (Settings.Default.StepMode) ? Settings.Default.ComboBox1 : Settings.Default.ComboBox3;
            int m = (Settings.Default.StepMode) ? Settings.Default.ComboBox2 : Settings.Default.ComboBox4;
            List<Memory> memories = new List<Memory>();
            List<CPU> cpus = new List<CPU>();
            for (int i = 0; i < m; i++) //инициализира броя памети
            {
                memories.Add(new Memory(i + 1, 0, false, new List<int>()));
            }
            for (int i = 0; i < n; i++) //инициализира броя памети
            {
                cpus.Add(new CPU(i + 1, false, null));
            }
            requests = new Requests(cpus, memories);
            lasts = new List<RequestLasts>();
            drawArea = Graphics.FromImage(image);
            deleteN.Clear();
            deleteM.Clear(); Latency.Clear(); Breadth.Clear(); t.Clear();
            drawArea.Clear(pictureBox2.BackColor);

            Pen blackPen = new Pen(Color.Black, 1);
            Pen qPen = new Pen(Color.DarkCyan, 1);

            int cpu, mem;

            cpu = (Settings.Default.StepMode) ? Settings.Default.ComboBox1 : Settings.Default.ComboBox3;
            mem = (Settings.Default.StepMode) ? Settings.Default.ComboBox2 : Settings.Default.ComboBox4;


            if ((cpu == 0) || (mem == 0))
            {
                if (Settings.Default.AutoMode && string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Въведете размерност на мрежата и време за моделиране!", "Некоректни входни данни!");
                }
                else
                {
                    MessageBox.Show("Въведете размерност на мрежата!", "Некоректни входни данни!");
                }
            }
            else
            {
                int x = requests.CPUs.Count == 32 ? 30 : 50; // разстоянието в решетката м/у п-сорите
                int x0 = 50;
                int y0 = 50;
                int y = 100; // разстоянието в решетката м/у модулите памет
                Font drawFont = new Font("Arial Narrow", 13);
                SolidBrush drawBrush = new SolidBrush(Color.MediumVioletRed);
                for (int i = 0; i < n; i++) //рисува хоризонтални линии
                {
                    drawArea.DrawLine(blackPen, x0, y0 + x / 2 + i * x, x0 + m * y - y / 2, y0 + x / 2 + i * x);
                    drawArea.DrawString("CPU" + (i + 1).ToString(), drawFont, drawBrush, 0, y0 + i * x);
                    drawArea.DrawRectangle(qPen, x0 + m * y - y / 4, y0 + i * x, 60, 25);


                    for (int j = 0; j < m; j++) //рисува вертикални линии
                    {
                        drawArea.DrawLine(blackPen, x0 + y / 2 + j * y, y0, x0 + y / 2 + j * y, y0 + n * x - x / 2);
                        drawArea.DrawString("M" + (j + 1).ToString(), drawFont, drawBrush, y / 2 + x0 - 10 + j * y, y0 - 30);
                    }
                }
                drawFont = new Font("Arial Narrow", 13, FontStyle.Bold);
                drawArea.DrawString("CPU REQ", drawFont, drawBrush, y / 2 + x0 - 10 + (m - 1) * y + 40, y0 - 35);
                drawFont = new Font("Arial Narrow", 8, FontStyle.Bold);
                drawArea.DrawString("COUNT", drawFont, drawBrush, y / 2 + x0 - 10 + (m - 1) * y + 100, y0 - 20);
                if (Settings.Default.StepMode && sender.GetType().Name != "BackgroundWorker")
                {
                    button7.Visible = true;
                    button2.Visible = true;
                }
            }
            pictureBox2.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int cpuCount = (Settings.Default.StepMode) ? Settings.Default.ComboBox1 : Settings.Default.ComboBox3;
            int memCount = (Settings.Default.StepMode) ? Settings.Default.ComboBox2 : Settings.Default.ComboBox4;


            Random rnd = new Random();

            for (int i = 0; i < cpuCount; i++)
            {
                if (!requests.CPUs[i].Busy)
                {
                    requests.CPUs[i].MemoryId = (requests.CPUs[i].Busy = rnd.Next(100) < ((!Settings.Default.Visualisation && Settings.Default.AutoMode) ? globalP : Settings.Default.GenReq) ? true : false) ? rnd.Next(0, memCount) : default(int);
                    if (requests.CPUs[i].Busy)
                    {
                        int memId = requests.CPUs[i].MemoryId.GetValueOrDefault();
                        lasts.Add(new RequestLasts(lasts.Count + 1, 0, i, false));
                        requests.Memories[memId].Busy = true;
                        if (requests.Memories[memId].TactsLeftToRelease == default(int)
                            && requests.Memories[memId].WaitingProcessors.Count == default(int))
                        {
                            requests.Memories[memId].TactsLeftToRelease = Settings.Default.TactLenght;
                        }
                        requests.Memories[memId].WaitingProcessors.Add(i);
                    }
                }
                else
                {
                    lasts.AsQueryable().Where(l => l.CPUId == i && l.Done == false).Single().TactsLast++;
                }
            }



            double p = ((!Settings.Default.Visualisation && Settings.Default.AutoMode) ? globalP : Settings.Default.GenReq) * 0.1;
            int B;

            B = requests.Memories.AsQueryable().Where(m => m.Busy == true).Count();

            //L = Convert.ToInt32((1 - pA) / pA);
            Breadth.Enqueue(B);
            //Latency.Enqueue(L);
            t.Enqueue(getSteps() + 1);

            for (int i = 0; i < memCount; i++)
            {
                if (requests.Memories[i].WaitingProcessors.Count > default(int))
                {
                    if (requests.Memories[i].TactsLeftToRelease != default(int))
                    {
                        procedureBeforeGeneratingRequest(i);
                    }
                    else
                    {
                        requests.Memories[i].Busy = false;
                        int cpuId = requests.Memories[i].WaitingProcessors[0];
                        requests.CPUs[cpuId].Busy = false;
                        lasts.AsQueryable().Where(l => l.CPUId == cpuId && l.Done == false).Single().Done = true;
                        if (Settings.Default.Visualisation)
                        {
                            deleteRequests(i);
                        }
                        requests.Memories[i].WaitingProcessors.RemoveAt(0);
                        requests.Memories[i].TactsLeftToRelease = Settings.Default.TactLenght;

                        if (requests.Memories[i].WaitingProcessors.Count > default(int))
                        {
                            procedureBeforeGeneratingRequest(i);
                        }
                    }
                }

            }
            requests.Tact++;
            if (InvokeRequired)
                label11.Invoke((MethodInvoker)(() => label11.Text = requests.Tact.ToString() + ". такт"));
            else label11.Text = requests.Tact.ToString() + ". такт";



        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            Breadth.Clear();
            t.Clear();
            chart1.Invalidate();
            dataGridView1.Rows.Clear();
            dataGridView1.Visible = false;
            int x;
            if (Int32.TryParse(textBox1.Text, out x))
            {
                if (Settings.Default.Visualisation)
                {
                    if ((x > 99) && (x < 50001))
                    {
                        button1_Click(sender, e);
                        int stepsCounter = 0;
                        trackBar1.Enabled = false;
                        trackBar3.Enabled = false;
                        pictureBox2.Visible = true;
                        bool isStopped = false;
                        button9.Visible = true;
                        do
                        {
                            Thread.Sleep(trackBar2.Value);
                            button2_Click(sender, e);
                            stepsCounter++;
                            pictureBox2.Invalidate();
                            Application.DoEvents();
                            label2.Text = stepsCounter.ToString() + " /";
                            isStopped = button9.Visible ? false : true;
                        } while (stepsCounter < x && !isStopped);
                        drawArea.Clear(pictureBox2.BackColor); pictureBox2.Invalidate();
                    }
                    else { MessageBox.Show("Въведете време за моделиране!", "Некоректни входни данни!"); }
                }
                else
                {
                    button4_Click_1(sender, e);
                }
                trackBar1.Enabled = true;
                trackBar3.Enabled = true;
                label2.Text = "0 /";

            }
            else { MessageBox.Show("Въведете време за моделиране!", "Некоректни входни данни!"); }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            button7_Click_1(sender, e);
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
            dataGridView1.Rows.Clear();
            panel2.Enabled = false;
            panel3.Enabled = true;
            textBox1.Enabled = false;
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;
            checkBox1.Enabled = false;
            trackBar2.Enabled = false;
            button3.Enabled = false;
            button7.Enabled = false;
            button9.Enabled = true;
            if (Settings.Default.StepMode || (Settings.Default.AutoMode && Settings.Default.Visualisation) && !backgroundWorker2.IsBusy)
            {
                backgroundWorker2.RunWorkerAsync();
            }
            else if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            button7.Visible = true;
            textBox1.Enabled = true;
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;
            checkBox1.Enabled = true;
            trackBar2.Enabled = true;
            button3.Enabled = true;
            button7.Enabled = true;
            dataGridView1.Enabled = false;
            dataGridView1.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e) // изход
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("© 2022 Георги Георгиев.\n\nmaymi5@abv.bg\n___________________________\n ТУ-Варна", "Product licence information");
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            drawArea = Graphics.FromImage(image);
            deleteN.Clear();
            deleteM.Clear(); Latency.Clear(); Breadth.Clear(); t.Clear();
            drawArea.Clear(pictureBox2.BackColor);
            button7.Visible = false;
            button2.Visible = false;
            label11.Text = " ";
            dataGridView1.Visible = false;
            Refresh();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.ShowDialog();


        }

        private void button9_Click(object sender, EventArgs e)
        {
            button9.Visible = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Settings.Default.GenReq = trackBar1.Value;
            Settings.Default.Save();
            Settings.Default.Upgrade();

            label1.Text = "Честота на генерираните заявки: " + (Settings.Default.GenReq).ToString();
        }

        private void trackBar3_Scroll_1(object sender, EventArgs e)
        {
            Settings.Default.TactLenght = trackBar3.Value;
            Settings.Default.Save();
            Settings.Default.Upgrade();
            label10.Text = "Дължина (брой тактове за обслужване) на една заявка в паметта: " + Settings.Default.TactLenght.ToString();
        }

        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            if (trackBar2.Value <= 750)
            {
                trackBar2.Value = 500;
            }
            else if (trackBar2.Value <= 1250)
            {
                trackBar2.Value = 1000;
            }
            else if (trackBar2.Value <= 1750)
            {
                trackBar2.Value = 1500;
            }
            else
            {
                trackBar2.Value = 2000;
            }
            double x = trackBar2.Value;
            Settings.Default.TrackBar2 = trackBar2.Value;
            Settings.Default.Save();
            Settings.Default.Upgrade();
            label8.Text = "Време за визуализация на един такт: " + (x / 1000).ToString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Settings.Default.StepMode = true;
                groupBox1.Visible = true;
                groupBox2.Visible = true;
                groupBox3.Visible = false;
                trackBar1.Enabled = true;
            }
            else
            {
                Settings.Default.StepMode = false;
            }
            label11.Text = " ";
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                Settings.Default.AutoMode = true;
                groupBox1.Visible = true;
                groupBox2.Visible = false;
                groupBox3.Visible = true;
                if (Settings.Default.Visualisation)
                {
                    trackBar1.Enabled = true;
                }
                else
                {
                    trackBar1.Enabled = false;
                }

            }
            else
            {
                Settings.Default.AutoMode = false;
            }
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.ComboBox1 = int.Parse(comboBox1.Text);
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.ComboBox2 = int.Parse(comboBox2.Text);
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.ComboBox3 = int.Parse(comboBox3.Text);
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.ComboBox4 = int.Parse(comboBox4.Text);
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.ModTime = textBox1.Text;
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                trackBar1.Enabled = true;
                trackBar2.Enabled = true;
            }
            else
            {
                trackBar1.Enabled = false;
                trackBar2.Enabled = false;
            }
            Settings.Default.Visualisation = checkBox1.Checked;
            Settings.Default.Save();
            Settings.Default.Upgrade();
        }

        #endregion

        #region Basic Functions

        private void procedureBeforeGeneratingRequest(int memoryId)
        {
            if (Settings.Default.Visualisation)
            {
                generateRequests(memoryId);
            }
            requests.Memories[memoryId].Busy = true;
            requests.Memories[memoryId].TactsLeftToRelease--;
        }

        private int getSteps()
        {
            return requests.Tact;
        }

        private void DisplayChart()
        {
            if (PX.Count > default(int))
            {
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                    series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    series.BorderWidth = 3;
                    series.Name = "Брой заети памети спрямо честота на генерираните заявки";
                }


                //chart1.ChartAreas[0].AxisY.Minimum = -2;
                this.chart1.Series["Брой заети памети спрямо честота на генерираните заявки"].Points.DataBindXY(PX, BY);

                chart1.Legends[0].Title = " ";
                chart1.ChartAreas[0].AxisX.Title = "Честота(p)";
                chart1.ChartAreas[0].AxisY.Title = "Памети";
                chart1.SaveImage("Графика средна широчина на лентата на пропускане.png", System.Drawing.Imaging.ImageFormat.Png);


                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                    series.Name = "Брой тактове нужни за ослужване на една заявка спрямо честота на генерираните заявки";
                }
                this.chart1.Series["Брой тактове нужни за ослужване на една заявка спрямо честота на генерираните заявки"].Points.DataBindXY(PX, LY);

                chart1.Legends[0].Title = " ";
                chart1.ChartAreas[0].AxisX.Title = "Честота(p)";
                chart1.ChartAreas[0].AxisY.Title = "Тактове";
                chart1.SaveImage("Графика средна латентност.png", System.Drawing.Imaging.ImageFormat.Png);
                //Results();
                PX = new List<int>();

                if (MessageBox.Show("Графиките бяха създадени, искате ли да бъдат отворени?", "Изходни резултати", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    //System.Diagnostics.Process.Start(@"Изходни резултати.txt");
                    System.Diagnostics.Process.Start(@"Графика средна широчина на лентата на пропускане.png");
                    System.Diagnostics.Process.Start(@"Графика средна латентност.png");
                }
            }
        }

        private void generateRequests(int memoryId) // процесор n генерира заявка
        {
            Pen redPen = new Pen(Color.Red, 3);
            Pen greenPen = new Pen(Color.Green, 3);
            Font drawFont = new Font("Arial Narrow", 13);
            SolidBrush drawBrush = new SolidBrush(Color.DarkCyan);
            int n1, n2;

            n1 = (Settings.Default.StepMode) ? Settings.Default.ComboBox1 : Settings.Default.ComboBox3;

            int x0 = 50;
            int y0 = 50;
            int x = requests.CPUs.Count == 32 ? 30 : 50; // разстоянието в решетката м/у п-сорите
            int y = 100; // разстоянието в решетката м/у модулите памет

            int n = requests.Memories[memoryId].WaitingProcessors[0];
            drawArea.DrawLine(greenPen, x0, y0 + x / 2 + n * x, x0 + memoryId * y + y / 2, y0 + x / 2 + n * x);
            drawArea.DrawLine(greenPen, x0 + memoryId * y + y / 2, y0 + 0, x0 + memoryId * y + y / 2, y0 + x / 2 + n * x);
            drawArea.DrawString("CPU" + (n + 1).ToString(), drawFont, drawBrush, y / 2 + x0 + memoryId * y, 20 + y0 + n1 * x);
            R.X = x0 + memoryId * y + y / 2 - 3;
            R.Y = y0 + x / 2 + n * x - 3;
            R.Width = 6;
            R.Height = 6;
            drawArea.DrawPie(greenPen, R, 0, 360);
            deleteM.Enqueue(memoryId);
            deleteN.Enqueue(n);

            if (requests.Memories[memoryId].WaitingProcessors.Count > 1) // ако има конфликт
            {
                n = requests.Memories[memoryId].WaitingProcessors[requests.Memories[memoryId].WaitingProcessors.Count - 1];

                drawArea.DrawLine(redPen, x0, y0 + x / 2 + n * x, x0 + memoryId * y + y / 2, y0 + x / 2 + n * x);
                R.X = x0 + memoryId * y + y / 2 - 3;
                R.Y = y0 + x / 2 + n * x - 3;
                R.Width = 6;
                R.Height = 6;
                drawArea.DrawPie(redPen, R, 0, 360);
            }

            if (lasts.Count > default(int))
            {

                List<Tuple<int, int>> counts = new List<Tuple<int, int>>();
                int m = requests.Memories.Count;
                foreach (var el in lasts.AsQueryable().GroupBy(l => l.CPUId))
                {
                    lasts.AsQueryable().Where(l => l.CPUId == el.Key).Count();
                    drawArea.FillRectangle(new SolidBrush(Color.White), new Rectangle(x0 + m * y - y / 4 + 1, y0 + el.Key * x + 1, 59, 24));
                    drawArea.DrawString(lasts.AsQueryable().Where(l => l.CPUId == el.Key).Count().ToString(), drawFont, drawBrush, x0 + m * y - y / 4, y0 + el.Key * x);
                }

            }
            pictureBox2.Invalidate();
        }

        private void deleteRequests(int memoryId)
        {
            Pen blackPen = new Pen(Color.Black, 1);
            Pen redPen = new Pen(Color.Red, 3);
            Pen delPen = new Pen(Color.White, 3);
            SolidBrush delBrush = new SolidBrush(Color.White);

            int m = memoryId;
            int n = requests.Memories[memoryId].WaitingProcessors[0];
            int n1;

            n1 = (Settings.Default.StepMode) ? Settings.Default.ComboBox1 : Settings.Default.ComboBox3;

            int x0 = 50;
            int y0 = 50;
            int x = requests.CPUs.Count == 32 ? 30 : 50; // разстоянието в решетката м/у п-сорите
            int y = 100; // разстоянието в решетката м/у модулите памет
            drawArea.DrawLine(delPen, x0, y0 + x / 2 + n * x, x0 + m * y + y / 2, y0 + x / 2 + n * x);
            drawArea.DrawLine(delPen, x0 + m * y + y / 2, y0 + 0, x0 + m * y + y / 2, y0 + x / 2 + n * x);
            R.X = x0 + m * y + y / 2 - 3;
            R.Y = y0 + x / 2 + n * x - 3;
            R.Width = 6;
            R.Height = 6;
            drawArea.DrawPie(delPen, R, 0, 360);
            drawArea.FillRectangle(delBrush, new Rectangle(x0 + m * y, 20 + y0 + n1 * x, 100, 20)); // изтрива приета заявка (граф.)
            drawArea.DrawLine(blackPen, x0, y0 + x / 2 + n * x, x0 + m * y + y / 2 + (m + 1 == requests.Memories.Count ? 0 : 4), y0 + x / 2 + n * x);
            drawArea.DrawLine(blackPen, x0 + m * y + y / 2, y0 + 0, x0 + m * y + y / 2, y0 + x / 2 + n * x + (n + 1 == n1 ? 0 : 4));

            for (int i = 1; i < requests.Memories[memoryId].WaitingProcessors.Count; i++)
            {
                n = requests.Memories[memoryId].WaitingProcessors[i];
                drawArea.DrawLine(redPen, x0, y0 + x / 2 + n * x, x0 + memoryId * y + y / 2, y0 + x / 2 + n * x);
                R.X = x0 + memoryId * y + y / 2 - 3;
                R.Y = y0 + x / 2 + n * x - 3;
                R.Width = 6;
                R.Height = 6;
                drawArea.DrawPie(redPen, R, 0, 360);
            }
            pictureBox2.Invalidate();
        }

        #endregion

        #region BackGroundWorkers

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int p = 0, stepsCounter = 0;
            int n = (Settings.Default.StepMode) ? Settings.Default.ComboBox1 : Settings.Default.ComboBox3;
            int m = (Settings.Default.StepMode) ? Settings.Default.ComboBox2 : Settings.Default.ComboBox4;
            List<Memory> memories = new List<Memory>();
            List<CPU> cpus = new List<CPU>();
            for (int i = 0; i < m; i++) //initialize memory count
            {
                memories.Add(new Memory(i + 1, 0, false, new List<int>()));
            }
            for (int i = 0; i < n; i++) // initialize processor count
            {
                cpus.Add(new CPU(i + 1, false, null));
            }
            lasts = new List<RequestLasts>();
            requests = new Requests(cpus, memories);
            BY = new List<double>();
            LY = new List<double>();
            PX = new List<int>();
            backgroundWorker1.ReportProgress(p + 1);// passes out the progress about the simulation

            button1_Click(sender, e);
            bool isStopped = false;
            if (InvokeRequired)
                button9.Invoke((MethodInvoker)(() => button9.Visible = true));
            else button9.Visible = true;
            do
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    globalP = p + 1;
                    if (InvokeRequired)
                        button9.Invoke((MethodInvoker)(() => isStopped = button9.Visible ? false : true));
                    else isStopped = button9.Visible ? false : true;
                    button2_Click(sender, e);
                    stepsCounter++;
                    int time = int.Parse(Settings.Default.ModTime);
                    if (stepsCounter == time)
                    {
                        p++;
                        backgroundWorker1.ReportProgress(p + 1);
                        stepsCounter = 0;
                        lasts.Sort((u, y) => u.TactsLast.CompareTo(y.TactsLast));
                        double bAvr = Math.Round((double)Breadth.AsQueryable().Average(), 2);
                        double lAvr;
                        if (lasts.Count <= 0)
                        {
                            lAvr = 0.0;
                        }
                        else
                        {
                            lAvr = Math.Round((double)lasts?.AsQueryable().Average(item => item.TactsLast), 2);
                        }
                        PX.Add(p);
                        BY.Add(bAvr);
                        LY.Add(lAvr);

                        var input = new object[] { p,
                                bAvr,
                                lasts.AsQueryable().Count(),
                                lasts.AsQueryable().Where(l => l.Done).Count(),
                                (lasts.Count <= 0) ? 0 :lasts.AsQueryable().Where(l => l.Done).First().TactsLast,
                                (lasts.Count <= 0) ? 0 :lasts.AsQueryable().Where(l => l.Done).Last().TactsLast,
                                lAvr };
                        if (this.InvokeRequired)
                            dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add(input)));
                        else dataGridView1.Rows.Add(input);
                        memories = new List<Memory>();
                        cpus = new List<CPU>();
                        for (int i = 0; i < m; i++) //инициализира броя памети
                        {
                            memories.Add(new Memory(i + 1, 0, false, new List<int>()));
                        }
                        for (int i = 0; i < n; i++) //инициализира броя памети
                        {
                            cpus.Add(new CPU(i + 1, false, null));
                        }
                        lasts = new List<RequestLasts>();
                        requests = new Requests(cpus, memories);
                        Breadth.Clear();
                    }
                }
                finally
                {
                    sw.Stop();
                    Debug.WriteLine($"Took {sw.ElapsedMilliseconds}");
                }
            } while (p < 10 && !isStopped);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int stepsCounter = 0;
            int n = (Settings.Default.StepMode) ? Settings.Default.ComboBox1 : Settings.Default.ComboBox3;
            int m = (Settings.Default.StepMode) ? Settings.Default.ComboBox2 : Settings.Default.ComboBox4;
            List<Memory> memories = new List<Memory>();
            List<CPU> cpus = new List<CPU>();
            for (int i = 0; i < m; i++) //инициализира броя памети
            {
                memories.Add(new Memory(i + 1, 0, false, new List<int>()));
            }
            for (int i = 0; i < n; i++) //инициализира броя памети
            {
                cpus.Add(new CPU(i + 1, false, null));
            }
            lasts = new List<RequestLasts>();
            backgroundWorker2.ReportProgress(Settings.Default.GenReq);
            int time = Settings.Default.StepMode ? requests.Tact : int.Parse(Settings.Default.ModTime);
            button1_Click(sender, e);
            requests = new Requests(cpus, memories);
            while (stepsCounter < time)
            {
                button2_Click(sender, e);
                stepsCounter++;
                if (stepsCounter == time)
                {
                    lasts.Sort((u, y) => u.TactsLast.CompareTo(y.TactsLast));
                    var input = new object[] { Settings.Default.GenReq,
                            Math.Round((double)Breadth.AsQueryable().Average(), 2),
                            lasts.AsQueryable().Count(),
                            lasts.AsQueryable().Where(l => l.Done).Count(),
                            (lasts.Count <= 0) ? 0 :lasts.AsQueryable().Where(l => l.Done).First().TactsLast,
                            (lasts.Count <= 0) ? 0 :lasts.AsQueryable().Where(l => l.Done).Last().TactsLast,
                            (lasts.Count <= 0) ? 0 : Math.Round((double)lasts.AsQueryable().Average(item => item.TactsLast), 2) };
                    if (this.InvokeRequired)
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add(input)));
                    else dataGridView1.Rows.Add(input);
                    lasts = new List<RequestLasts>();
                    requests = new Requests(cpus, memories);
                    Breadth.Clear();
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label16.Visible = true;
            if (e.ProgressPercentage == 11)
            {
                label16.Visible = false;
            }
            label16.Text = "Правят се изчисления с честота на генерираните заявки: " + e.ProgressPercentage.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                panel2.Enabled = true;
                panel3.Enabled = true;
                dataGridView1.Enabled = true;
                pictureBox1.Visible = false;
                label16.Visible = false;
                DisplayChart();
            }

        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                panel2.Enabled = true;
                panel3.Enabled = true;
                dataGridView1.Enabled = true;
                pictureBox1.Visible = false;
                label16.Visible = false;
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label16.Visible = true;
            label16.Text = "Правят се изчисления с честота на генерираните заявки: " + e.ProgressPercentage.ToString();
        }

        #endregion

    }
}