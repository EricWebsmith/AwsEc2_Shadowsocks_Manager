using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AwsEc2Manager
{
    public partial class MainForm : Form
    {
        private List<Server> servers = null;
        private List<string> copyStates = new List<string>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.workingDirTextBox.Text = Properties.Settings.Default.working_dir;
            this.pemTextBox.Text = Properties.Settings.Default.pem_file;
            Refresh();
            timer1.Start();
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                var copy_cell = senderGrid.Rows[e.RowIndex].Cells[3] as DataGridViewButtonCell;
                
                if (senderGrid.Columns[e.ColumnIndex].Name == "CopyIP")
                {
                    if (!string.IsNullOrWhiteSpace(servers[e.RowIndex].IP))
                    {
                        Clipboard.SetText(servers[e.RowIndex].IP);
                        copyStates[e.RowIndex] = "Copied";
                        copy_cell.Value = "Copied";
                    }
                }

                if (senderGrid.Columns[e.ColumnIndex].Name == "Start")
                {
                    string command = "ec2 start-instances --instance-ids " + servers[e.RowIndex].Id;
                    string result = AwsCommandUtility.Call(command);
                    resultTextBox.Text += "\r\n" + result;
                    Refresh();
                    copyStates[e.RowIndex] = "";
                    copy_cell.Value = "";
                }

                if (senderGrid.Columns[e.ColumnIndex].Name == "Stop")
                {
                    string command = "ec2 stop-instances --instance-ids " + servers[e.RowIndex].Id;
                    string result = AwsCommandUtility.Call(command);
                    resultTextBox.Text = result;
                    Refresh();
                    copyStates[e.RowIndex] = "";
                    copy_cell.Value = "";
                }

                if (senderGrid.Columns[e.ColumnIndex].Name == "SSH")
                {
                    string result = AwsCommandUtility.Ssh(servers[e.RowIndex].PublicDns);
                    resultTextBox.Text = result;
                }
            }
        }

        private void Refresh()
        {
            servers = AwsCommandUtility.GetServers();
            bindingSource1.DataSource = servers;
            lastRefreshToolStripLabel.Text = "Last Refreshed: " + DateTime.Now.ToString();
            for(int i=0;i<copyStates.Count;i++)
            {
                var copy_cell = dataGridView1.Rows[i].Cells[3] as DataGridViewButtonCell;
                copy_cell.Value = copyStates[i];
            }
            if(copyStates.Count==0)
            {
                for(int i=0;i<servers.Count;i++)
                {
                    copyStates.Add("");
                }
            }
        }

        private void RefreshToolStripButton_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Every30SecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Interval = 30000;
            timer1.Start();
            AutoRefreshToolStripDropDownButton.Text = "Every 30 Seconds";
        }

        private void EveryMinutetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Interval = 60000;
            timer1.Start();
            AutoRefreshToolStripDropDownButton.Text = "Every Minute";
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            AutoRefreshToolStripDropDownButton.Text = "Stop";
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.pem_file = this.pemTextBox.Text;
            Properties.Settings.Default.working_dir = this.workingDirTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void pemTextBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pem_file = this.pemTextBox.Text;
        }

        private void workingDirTextBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.working_dir = this.workingDirTextBox.Text;
        }
    }
}
