using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Hypercube;
using Hypercube.Core;
using Hypercube.Map;

namespace ServerGUI {
    public partial class MainForm : Form {
        bool _saved = true;

        public MainForm() {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e) {
            Console.SetOut(new ControlWriter(txtAllConsole));

            #region Text Events
            ServerCore.Logger.ChatMessage += message => {
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.AppendText(message + Environment.NewLine)));
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.Select(txtChatbox.Text.Length, 1)));
                txtChatbox.Invoke(new MethodInvoker(() => txtChatbox.ScrollToCaret()));
            };

            ServerCore.Logger.CommandMessage += message => {
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.AppendText(message + Environment.NewLine)));
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.Select(txtCommandbox.Text.Length, 1)));
                txtCommandbox.Invoke(new MethodInvoker(() => txtCommandbox.ScrollToCaret()));
            };

            ServerCore.Logger.CriticalMessage += message => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };

            ServerCore.Logger.DebugMessage += message => {
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.AppendText(message + Environment.NewLine)));
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.Select(txtDebugBox.Text.Length, 1)));
                txtDebugBox.Invoke(new MethodInvoker(() => txtDebugBox.ScrollToCaret()));
            };

            ServerCore.Logger.ErrorMessage += message => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };

            ServerCore.Logger.InfoMessage += message => {
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.AppendText(message + Environment.NewLine)));
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.Select(txtInfobox.Text.Length, 1)));
                txtInfobox.Invoke(new MethodInvoker(() => txtInfobox.ScrollToCaret()));
            };

            ServerCore.Logger.WarningMessage += message => {
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.AppendText(message + Environment.NewLine)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.Select(txtErrorbox.Text.Length, 1)));
                txtErrorbox.Invoke(new MethodInvoker(() => txtErrorbox.ScrollToCaret()));
            };
            #endregion
            ServerCore.Setup();

            foreach (var m in ServerCore.Maps.Values) 
                lstMaps.Items.Add(m.CWMap.MapName);

            foreach (var block in ServerCore.Blockholder.NumberList) {
                if (block.Name == "Unknown")
                    continue;
                lstBlocks.Items.Add(block.Name);
            }

        }

        private void LoadSettingsMenu() {
            txtSrvName.Text = ServerCore.ServerName;
            txtSrvMotd.Text = ServerCore.Motd;
            txtWelcomeMess.Text = ServerCore.WelcomeMessage;

            chkRotLogs.Checked = ServerCore.RotateLogs;
            chkLogs.Checked = ServerCore.LogOutput;
            chkComArgs.Checked = ServerCore.LogArguments;
            chkMaphistory.Checked = ServerCore.CompressHistory;

            numMaxBlocks.Value = ServerCore.MaxBlockChanges;
            numHistory.Value = ServerCore.MaxHistoryEntries;

            // -- Network
            numPort.Value = ServerCore.Nh.Port;
            numMaxPlayers.Value = ServerCore.Nh.MaxPlayers;

            chkVerifyNames.Checked = ServerCore.Nh.VerifyNames;
            chkPub.Checked = ServerCore.Nh.Public;

            // -- Chat
            txtChatError.Text = ServerCore.TextFormats.ErrorMessage;
            txtChatDivider.Text = ServerCore.TextFormats.Divider;
            txtChatSystem.Text = ServerCore.TextFormats.SystemMessage;
            txtPlayerlist.Text = ServerCore.TextFormats.ExtPlayerList;
        }

        private void SaveSettingsMenu() {
            ServerCore.ServerName = txtSrvName.Text;
            ServerCore.Motd = txtSrvMotd.Text;
            ServerCore.WelcomeMessage = txtWelcomeMess.Text;

            ServerCore.RotateLogs = chkRotLogs.Checked;
            ServerCore.LogOutput = chkLogs.Checked;
            ServerCore.LogArguments = chkComArgs.Checked;
            ServerCore.CompressHistory = chkMaphistory.Checked;

            ServerCore.MaxBlockChanges = (int)numMaxBlocks.Value;
            ServerCore.MaxHistoryEntries = (int)numHistory.Value;
            ServerCore.SaveSystemSettings();

            // -- Network
            ServerCore.Nh.Port = (int)numPort.Value;
            ServerCore.Nh.MaxPlayers = (int)numMaxPlayers.Value;
            ServerCore.Nh.VerifyNames = chkVerifyNames.Checked;
            ServerCore.Nh.Public = chkPub.Checked;
            ServerCore.Nh.SaveSettings();

            // -- Chat
            ServerCore.TextFormats.ErrorMessage = txtChatError.Text;
            ServerCore.TextFormats.Divider = txtChatDivider.Text;
            ServerCore.TextFormats.SystemMessage = txtChatSystem.Text;
            ServerCore.TextFormats.ExtPlayerList = txtPlayerlist.Text;
            ServerCore.TextFormats.SaveTextSettings();
            _saved = true;
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
                _saved = false;
            } else {
                if (!_saved)
                    SaveSettingsMenu();
            }
        }

        

        #region Button Presses
        #region Blocks Tab
        private void lstBlocks_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstBlocks.SelectedIndices.Count == 0) 
                return;

            var thisBlock = ServerCore.Blockholder.GetBlock((string)lstBlocks.SelectedItem);
            numBlockId.Value = thisBlock.Id;
            txtBlockName.Text = thisBlock.Name;
            numBlockCId.Value = thisBlock.OnClient;

            var hexValue = thisBlock.Color.ToString("X"); // Swap last, with the center.
            hexValue = hexValue.PadLeft(6, '0');
            hexValue = hexValue.Substring(4, 2) + hexValue.Substring(2, 2) + hexValue.Substring(0, 2);

            var oColor = ColorTranslator.FromHtml("#" + hexValue);
            picBlockColor.BackColor = oColor;

            selectBlockKills.SelectedIndex = thisBlock.Kills ? 0 : 1;
            selectBlockSpecial.SelectedIndex = thisBlock.Special ? 0 : 1;
            selectRepPhysics.SelectedIndex = thisBlock.RepeatPhysics ? 0 : 1;
           // selectPhysMapLoad.SelectedIndex = thisBlock
        }

        private void btnCreateBlock_Click(object sender, EventArgs e) {

        }

        private void btnDeleteBlock_Click(object sender, EventArgs e) {

        }

        private void btnSaveBlocks_Click(object sender, EventArgs e) {

        }

        private void btnReloadBlocks_Click(object sender, EventArgs e) {

        }
        #endregion
        #region Maps Tab
        private void lstMaps_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string) lstMaps.SelectedItem, out m);

            if (m == null)
                return;

            txtMapName.Text = m.CWMap.MapName;
            txtMapMotd.Text = m.HCSettings.Motd;
            
            lstJoinPerms.Items.Clear();
            lstShowPerms.Items.Clear();
            lstBuildPerms.Items.Clear();

            foreach (var perm in m.Buildperms) 
                lstBuildPerms.Items.Add(perm.Key);

            foreach (var perm in m.Showperms) 
                lstShowPerms.Items.Add(perm.Key);

            foreach (var perm in m.Joinperms) 
                lstJoinPerms.Items.Add(perm.Key);

            lblMapClients.Text = "Clients: " + m.Clients.Count;
            lblMapPhys.Text = "Physics: " + (m.HCSettings.Physics ? "On" : "Off");
            lblMapBuilding.Text = "Building: " + (m.HCSettings.Building ? "On" : "Off");
            lblMapHist.Text = "History: " + (m.HCSettings.History ? "On" : "Off");
            lblMapBlockqueue.Text = "Blocksend Queue: " + m.BlockchangeQueue.Count;
            lblMapPhysqueue.Text = "Physics Queue: " + m.PhysicsQueue.Count;
            lblLoadStatus.Text = "Loaded: " + (m.Loaded ? "Loaded" : "Unloaded");
            lblMapGen.Text = "Generator: " + m.CWMap.GeneratorName;
            lblMapSize.Text = "Size: " + m.CWMap.SizeX + " x " + m.CWMap.SizeZ + " x " + m.CWMap.SizeY;
            lblMapSpawn.Text = "Spawn: " + m.CWMap.SpawnX + ", " + m.CWMap.SpawnZ + ", " + m.CWMap.SpawnY;
        }
        private void btnMapResize_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;
        }

        private void btnMapResend_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;
        }

        private void btnMapFill_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;
        }

        private void btnMapHistoryOn_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;
        }

        private void btnMapBuilding_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;

            m.HCSettings.Building = !m.HCSettings.Building;
            lblMapBuilding.Text = "Building: " + (m.HCSettings.Building ? "On" : "Off");
        }

        private void btnMapClearQueue_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;

            m.PhysicsQueue = new ConcurrentQueue<QueueItem>();
            m.BlockchangeQueue = new ConcurrentQueue<QueueItem>();
            lblMapBlockqueue.Text = "Blocksend Queue: " + m.BlockchangeQueue.Count;
            lblMapPhysqueue.Text = "Physics Queue: " + m.PhysicsQueue.Count;
        }

        private void btnMapPhysics_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;

            m.HCSettings.Physics = (!m.HCSettings.Physics);
            lblMapPhys.Text = "Physics: " + (m.HCSettings.Physics ? "On" : "Off");

            if (m.HCSettings.Physics == false)
                m.PhysicsQueue = new ConcurrentQueue<QueueItem>();

            lblMapPhysqueue.Text = "Physics Queue: " + m.PhysicsQueue.Count;
        }

        private void btnMapDefault_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;

            ServerCore.MapMain = m.CWMap.MapName;
            ServerCore.SaveSystemSettings();
        }

        private void btnMapUpdate_Click(object sender, EventArgs e) {
            if (lstMaps.SelectedIndices.Count == 0)
                return;

            HypercubeMap m;
            ServerCore.Maps.TryGetValue((string)lstMaps.SelectedItem, out m);

            if (m == null)
                return;

            if (ServerCore.MapMain == m.CWMap.MapName) {
                ServerCore.MapMain = txtMapName.Text;
                ServerCore.SaveSystemSettings();
            }

            m.CWMap.MapName = txtMapName.Text;
            m.HCSettings.Motd = txtMapMotd.Text;

            lstMaps.Items.Clear();

            foreach (var c in ServerCore.Maps.Values)
                lstMaps.Items.Add(c.CWMap.MapName);

            lstMaps.SelectedItem = m.CWMap.MapName;
            ServerCore.ActionQueue.Enqueue(new MapAction {Action = MapActions.Save, Map = m});
        }
        #endregion

        #endregion


    }

    public class ControlWriter : TextWriter {
        private readonly TextBox _infoTb;

        public ControlWriter(TextBox tb) {
            _infoTb = tb;
        }

        public override void Write(char value) {
            //infoTB.Text += value;
            _infoTb.Invoke(new MethodInvoker(() => _infoTb.Text += value));
            _infoTb.Invoke(new MethodInvoker(() => _infoTb.Select(_infoTb.Text.Length, 1)));
            _infoTb.Invoke(new MethodInvoker(() => _infoTb.ScrollToCaret()));
        }

        public override void Write(string value) {
            _infoTb.Invoke(new MethodInvoker(() => _infoTb.Text += value));
            //infoTB.Text += value;
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}

