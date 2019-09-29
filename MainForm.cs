using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AwsEc2Manager
{
    public partial class MainForm : Form
    {
        private List<Server> servers = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.workingDirTextBox.Text = Properties.Settings.Default.working_dir;
            this.pemTextBox.Text = Properties.Settings.Default.pem_file;
            //if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.working_dir) && !string.IsNullOrWhiteSpace(Properties.Settings.Default.pem_file)) 
            //{
                
                
            //}
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
                        copy_cell.Value = "Copied";
                        //(senderGrid.Columns[e.ColumnIndex] as DataGridViewButtonColumn).Text = "Copied";
                    }
                }

                if (senderGrid.Columns[e.ColumnIndex].Name == "Start")
                {
                    string command = "ec2 start-instances --instance-ids " + servers[e.RowIndex].Id;
                    string result = AwsCommandUtility.Call(command);
                    resultTextBox.Text += "\r\n" + result;
                    Refresh();
                    copy_cell.Value = "";
                }

                if (senderGrid.Columns[e.ColumnIndex].Name == "Stop")
                {
                    string command = "ec2 stop-instances --instance-ids " + servers[e.RowIndex].Id;
                    string result = AwsCommandUtility.Call(command);
                    resultTextBox.Text = result;
                    Refresh();
                    copy_cell.Value = "";
                }

                if (senderGrid.Columns[e.ColumnIndex].Name == "SSH")
                {
                    string result = AwsCommandUtility.Ssh(servers[e.RowIndex].PublicDns);
                    resultTextBox.Text = result;
                }
                //TODO - Button Clicked - Execute Code Here
            }
        }

        private void Refresh()
        {
            servers = AwsCommandUtility.GetServers();
            bindingSource1.DataSource = servers;
            lastRefreshToolStripLabel.Text = "Last Refreshed: " + DateTime.Now.ToString();
        }

        private void RefreshToolStripButton_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void AutoRefreshToolStripDropDownButton_Click(object sender, EventArgs e)
        {

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
            //if (string.IsNullOrWhiteSpace(Properties.Settings.Default.working_dir) || string.IsNullOrWhiteSpace(Properties.Settings.Default.pem_file))
            //{
            //    return;
            //}
            Refresh();
        }

        private void CopyIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem contextMenu = (ToolStripMenuItem)sender;
            ContextMenuStrip context = (ContextMenuStrip)contextMenu.Owner;
            DataGridView gridView = (DataGridView)context.SourceControl;
            var cell = (DataGridViewTextBoxCell)gridView.CurrentCell;
            cell.Value = "Copied";
            Clipboard.SetText(cell.Value.ToString());
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
