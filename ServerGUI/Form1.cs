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

using Hypercube;
using Hypercube.Core;
using Hypercube.Map;

namespace ServerGUI {
    public partial class mainForm : Form {
        bool Saved = true;

        public mainForm() {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e) {
            Console.SetOut(new ControlWriter(txtAllConsole));

            ServerCore = new Hypercube.ServerCore();

            #region Text Events
            this.ServerCore.Logger.ChatMessage += (message) => {
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.AppendText(message + Environment.NewLine)));
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.Select(txtChatbox.Text.Length, 1)));
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.ScrollToCaret()));
            };

            this.ServerCore.Logger.CommandMessage += (message) => {
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.AppendText(message + Environment.NewLine)));
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.Select(txtCommandbox.Text.Length, 1)));
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.ScrollToCaret()));
            };

            this.ServerCore.Logger.CriticalMessage += (message) => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };

            this.ServerCore.Logger.DebugMessage += (message) => {
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.AppendText(message + Environment.NewLine)));
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.Select(txtDebugBox.Text.Length, 1)));
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.ScrollToCaret()));
            };

            this.ServerCore.Logger.ErrorMessage += (message) => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };

            this.ServerCore.Logger.InfoMessage += (message) => {
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.AppendText(message + Environment.NewLine)));
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.Select(txtInfobox.Text.Length, 1)));
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.ScrollToCaret()));
            };

            this.ServerCore.Logger.WarningMessage += (message) => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };
            #endregion

            foreach (var m in this.ServerCore.Maps) 
                lstMaps.Items.Add(m.CWMap.MapName);
            
        }

        private void LoadSettingsMenu() {
            txtSrvName.Text = this.ServerCore.ServerName;
            txtSrvMotd.Text = this.ServerCore.Motd;
            txtWelcomeMess.Text = this.ServerCore.WelcomeMessage;

            chkRotLogs.Checked = this.ServerCore.RotateLogs;
            chkLogs.Checked = this.ServerCore.LogOutput;
            chkComArgs.Checked = this.ServerCore.LogArguments;
            chkMaphistory.Checked = this.ServerCore.CompressHistory;

            numMaxBlocks.Value = this.ServerCore.MaxBlockChanges;
            numHistory.Value = this.ServerCore.MaxHistoryEntries;

            // -- Network
            numPort.Value = this.ServerCore.Nh.Port;
            numMaxPlayers.Value = this.ServerCore.Nh.MaxPlayers;

            chkVerifyNames.Checked = this.ServerCore.Nh.VerifyNames;
            chkPub.Checked = this.ServerCore.Nh.Public;

            // -- Chat
            txtChatError.Text = this.ServerCore.TextFormats.ErrorMessage;
            txtChatDivider.Text = this.ServerCore.TextFormats.Divider;
            txtChatSystem.Text = this.ServerCore.TextFormats.SystemMessage;
            txtPlayerlist.Text = this.ServerCore.TextFormats.ExtPlayerList;
        }

        private void SaveSettingsMenu() {
            this.ServerCore.ServerName = txtSrvName.Text;
            this.ServerCore.Motd = txtSrvMotd.Text;
            this.ServerCore.WelcomeMessage = txtWelcomeMess.Text;

            this.ServerCore.RotateLogs = chkRotLogs.Checked;
            this.ServerCore.LogOutput = chkLogs.Checked;
            this.ServerCore.LogArguments = chkComArgs.Checked;
            this.ServerCore.CompressHistory = chkMaphistory.Checked;

            this.ServerCore.MaxBlockChanges = (int)numMaxBlocks.Value;
            this.ServerCore.MaxHistoryEntries = (int)numHistory.Value;
            this.ServerCore.SaveSystemSettings();

            // -- Network
            this.ServerCore.Nh.Port = (int)numPort.Value;
            this.ServerCore.Nh.MaxPlayers = (int)numMaxPlayers.Value;
            this.ServerCore.Nh.VerifyNames = chkVerifyNames.Checked;
            this.ServerCore.Nh.Public = chkPub.Checked;
            this.ServerCore.Nh.SaveSettings();

            // -- Chat
            this.ServerCore.TextFormats.ErrorMessage = txtChatError.Text;
            this.ServerCore.TextFormats.Divider = txtChatDivider.Text;
            this.ServerCore.TextFormats.SystemMessage = txtChatSystem.Text;
            this.ServerCore.TextFormats.ExtPlayerList = txtPlayerlist.Text;
            this.ServerCore.TextFormats.SaveTextSettings();
            Saved = true;
        }

        private void startServerToolStripMenuItem_Click(object sender, EventArgs e) {
            this.ServerCore.Start();
        }

        private void stopServerToolStripMenuItem_Click(object sender, EventArgs e) {
            this.ServerCore.Stop();
        }

        private void mainTabs_SelectedIndexChanged(object sender, EventArgs e) {
            if (mainTabs.SelectedTab == settingsTab) {
                LoadSettingsMenu();
                Saved = false;
            } else {
                if (!Saved)
                    SaveSettingsMenu();
            }
        }
    }

    public class ControlWriter : TextWriter {
        private TextBox infoTB;

        public ControlWriter(TextBox TB) {
            this.infoTB = TB;
        }

        public override void Write(char value) {
            //infoTB.Text += value;
            infoTB.Invoke(new MethodInvoker(() => infoTB.Text += value));
            infoTB.Invoke(new MethodInvoker(() => infoTB.Select(infoTB.Text.Length, 1)));
            infoTB.Invoke(new MethodInvoker(() => infoTB.ScrollToCaret()));
        }

        public override void Write(string value) {
            infoTB.Invoke(new MethodInvoker(() => infoTB.Text += value));
            //infoTB.Text += value;
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}

