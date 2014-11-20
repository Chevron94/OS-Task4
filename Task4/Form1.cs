using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Task4
{
    public partial class Form1 : Form
    {
        Simulate s;
        Thread Thread;
        public Form1()
        {
            InitializeComponent();
            s = new Simulate(ref RATABLE, ref PWTABLE, ref Logs);
            Thread = new Thread(new ParameterizedThreadStart(MyThread));
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            Application.Idle += MyIdle;

        }

        void MyIdle(object sender, EventArgs e)
        {
            Logs.SelectedIndex = Logs.Items.Count - 1;
            Logs.SelectedIndex = -1;
            stopToolStripMenuItem.Enabled = Thread.IsAlive;
            startToolStripMenuItem.Enabled = !Thread.IsAlive;
        }

        void MyThread(object random)
        {
                s.Run();
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Thread.IsAlive)
            {
                s.Stop = true;
                Thread.Abort();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Thread.IsAlive)
            {
                s.Stop = true;
                Thread.Abort();
            }
        }

        private void startRandomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s.Stop = false;
            Logs.Items.Clear();
            Thread = new Thread(new ParameterizedThreadStart(MyThread));
            Thread.Start(true);
        }
    }
}
