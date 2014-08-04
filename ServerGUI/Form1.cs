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
        public Hypercube.Hypercube ServerCore;
        bool Saved = true;

        public mainForm() {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e) {
            Console.SetOut(new ControlWriter(txtAllConsole));

            ServerCore = new Hypercube.Hypercube();

            #region Text Events
            ServerCore.Logger.ChatMessage += (message) => {
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.AppendText(message + Environment.NewLine)));
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.Select(txtChatbox.Text.Length, 1)));
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.ScrollToCaret()));
            };

            ServerCore.Logger.CommandMessage += (message) => {
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.AppendText(message + Environment.NewLine)));
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.Select(txtCommandbox.Text.Length, 1)));
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.ScrollToCaret()));
            };

            ServerCore.Logger.CriticalMessage += (message) => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };

            ServerCore.Logger.DebugMessage += (message) => {
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.AppendText(message + Environment.NewLine)));
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.Select(txtDebugBox.Text.Length, 1)));
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.ScrollToCaret()));
            };

            ServerCore.Logger.ErrorMessage += (message) => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };

            ServerCore.Logger.InfoMessage += (message) => {
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.AppendText(message + Environment.NewLine)));
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.Select(txtInfobox.Text.Length, 1)));
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.ScrollToCaret()));
            };

            ServerCore.Logger.WarningMessage += (message) => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };
            #endregion

            foreach (HypercubeMap m in ServerCore.Maps) 
                lstMaps.Items.Add(m.CWMap.MapName);
            
        }

        private void LoadSettingsMenu() {
            txtSrvName.Text = ServerCore.ServerName;
            txtSrvMotd.Text = ServerCore.MOTD;
            txtWelcomeMess.Text = ServerCore.WelcomeMessage;

            chkRotLogs.Checked = ServerCore.RotateLogs;
            chkLogs.Checked = ServerCore.LogOutput;
            chkComArgs.Checked = ServerCore.LogArguments;
            chkMaphistory.Checked = ServerCore.CompressHistory;

            numMaxBlocks.Value = ServerCore.MaxBlockChanges;
            numHistory.Value = ServerCore.MaxHistoryEntries;

            // -- Network
            numPort.Value = ServerCore.nh.Port;
            numMaxPlayers.Value = ServerCore.nh.MaxPlayers;

            chkVerifyNames.Checked = ServerCore.nh.VerifyNames;
            chkPub.Checked = ServerCore.nh.Public;

            // -- Chat
            txtChatError.Text = ServerCore.TextFormats.ErrorMessage;
            txtChatDivider.Text = ServerCore.TextFormats.Divider;
            txtChatSystem.Text = ServerCore.TextFormats.SystemMessage;
            txtPlayerlist.Text = ServerCore.TextFormats.ExtPlayerList;
        }

        private void SaveSettingsMenu() {
            ServerCore.ServerName = txtSrvName.Text;
            ServerCore.MOTD = txtSrvMotd.Text;
            ServerCore.WelcomeMessage = txtWelcomeMess.Text;

            ServerCore.RotateLogs = chkRotLogs.Checked;
            ServerCore.LogOutput = chkLogs.Checked;
            ServerCore.LogArguments = chkComArgs.Checked;
            ServerCore.CompressHistory = chkMaphistory.Checked;

            ServerCore.MaxBlockChanges = (int)numMaxBlocks.Value;
            ServerCore.MaxHistoryEntries = (int)numHistory.Value;
            ServerCore.SaveSystemSettings();

            // -- Network
            ServerCore.nh.Port = (int)numPort.Value;
            ServerCore.nh.MaxPlayers = (int)numMaxPlayers.Value;
            ServerCore.nh.VerifyNames = chkVerifyNames.Checked;
            ServerCore.nh.Public = chkPub.Checked;
            ServerCore.nh.SaveSettings();

            // -- Chat
            ServerCore.TextFormats.ErrorMessage = txtChatError.Text;
            ServerCore.TextFormats.Divider = txtChatDivider.Text;
            ServerCore.TextFormats.SystemMessage = txtChatSystem.Text;
            ServerCore.TextFormats.ExtPlayerList = txtPlayerlist.Text;
            ServerCore.TextFormats.SaveTextSettings();
            Saved = true;
        }

        private void startServerToolStripMenuItem_Click(object sender, EventArgs e) {
            ServerCore.Start();
        }

        private void stopServerToolStripMenuItem_Click(object sender, EventArgs e) {
            ServerCore.Stop();
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

