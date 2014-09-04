namespace ServerGUI {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mainTabs = new System.Windows.Forms.TabControl();
            this.consoleTab = new System.Windows.Forms.TabPage();
            this.consoletabs = new System.Windows.Forms.TabControl();
            this.Alltab = new System.Windows.Forms.TabPage();
            this.txtAllConsole = new System.Windows.Forms.TextBox();
            this.infotab = new System.Windows.Forms.TabPage();
            this.txtInfobox = new System.Windows.Forms.RichTextBox();
            this.chatTab = new System.Windows.Forms.TabPage();
            this.txtChatbox = new System.Windows.Forms.RichTextBox();
            this.btnSendChat = new System.Windows.Forms.Button();
            this.txtChat = new System.Windows.Forms.TextBox();
            this.cTab = new System.Windows.Forms.TabPage();
            this.txtCommandbox = new System.Windows.Forms.RichTextBox();
            this.ErrorsTab = new System.Windows.Forms.TabPage();
            this.txtErrorbox = new System.Windows.Forms.RichTextBox();
            this.debugTab = new System.Windows.Forms.TabPage();
            this.txtDebugBox = new System.Windows.Forms.RichTextBox();
            this.settingsTab = new System.Windows.Forms.TabPage();
            this.btnRuleEdit = new System.Windows.Forms.Button();
            this.grpSettingsChat = new System.Windows.Forms.GroupBox();
            this.txtChatDivider = new System.Windows.Forms.TextBox();
            this.lblDivider = new System.Windows.Forms.Label();
            this.txtPlayerlist = new System.Windows.Forms.TextBox();
            this.lblPlayerList = new System.Windows.Forms.Label();
            this.lblSystem = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.txtChatSystem = new System.Windows.Forms.TextBox();
            this.txtChatError = new System.Windows.Forms.TextBox();
            this.grpNetwork = new System.Windows.Forms.GroupBox();
            this.numMaxPlayers = new System.Windows.Forms.NumericUpDown();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.chkPub = new System.Windows.Forms.CheckBox();
            this.chkVerifyNames = new System.Windows.Forms.CheckBox();
            this.lblMaxPlayers = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.grpSystem = new System.Windows.Forms.GroupBox();
            this.lblHistory = new System.Windows.Forms.Label();
            this.numHistory = new System.Windows.Forms.NumericUpDown();
            this.lblBlockChn = new System.Windows.Forms.Label();
            this.numMaxBlocks = new System.Windows.Forms.NumericUpDown();
            this.chkComArgs = new System.Windows.Forms.CheckBox();
            this.chkMaphistory = new System.Windows.Forms.CheckBox();
            this.chkLogs = new System.Windows.Forms.CheckBox();
            this.chkRotLogs = new System.Windows.Forms.CheckBox();
            this.txtWelcomeMess = new System.Windows.Forms.TextBox();
            this.lblWelcmess = new System.Windows.Forms.Label();
            this.txtSrvMotd = new System.Windows.Forms.TextBox();
            this.lblSrvMotd = new System.Windows.Forms.Label();
            this.txtSrvName = new System.Windows.Forms.TextBox();
            this.lblSrvName = new System.Windows.Forms.Label();
            this.mapsTab = new System.Windows.Forms.TabPage();
            this.lblMapBPerms = new System.Windows.Forms.Label();
            this.lblMapSPerms = new System.Windows.Forms.Label();
            this.lblMapJperms = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.lstBuildPerms = new System.Windows.Forms.ListBox();
            this.lstShowPerms = new System.Windows.Forms.ListBox();
            this.lstJoinPerms = new System.Windows.Forms.ListBox();
            this.grpMapControl = new System.Windows.Forms.GroupBox();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtMapMotd = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMapPhysqueue = new System.Windows.Forms.Label();
            this.lblMapSpawn = new System.Windows.Forms.Label();
            this.lblMapBlockqueue = new System.Windows.Forms.Label();
            this.lblMapSize = new System.Windows.Forms.Label();
            this.lblMapGen = new System.Windows.Forms.Label();
            this.lblMapBuilding = new System.Windows.Forms.Label();
            this.lblMapHist = new System.Windows.Forms.Label();
            this.lblLoadStatus = new System.Windows.Forms.Label();
            this.lblMapPhys = new System.Windows.Forms.Label();
            this.lblMapClients = new System.Windows.Forms.Label();
            this.lblMapMotd = new System.Windows.Forms.Label();
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.lblMapName = new System.Windows.Forms.Label();
            this.lblMaps = new System.Windows.Forms.Label();
            this.lstMaps = new System.Windows.Forms.ListBox();
            this.contextMapStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandsTab = new System.Windows.Forms.TabPage();
            this.blocksTab = new System.Windows.Forms.TabPage();
            this.ranksTab = new System.Windows.Forms.TabPage();
            this.dbTab = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miniModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstBlocks = new System.Windows.Forms.ListBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.mainTabs.SuspendLayout();
            this.consoleTab.SuspendLayout();
            this.consoletabs.SuspendLayout();
            this.Alltab.SuspendLayout();
            this.infotab.SuspendLayout();
            this.chatTab.SuspendLayout();
            this.cTab.SuspendLayout();
            this.ErrorsTab.SuspendLayout();
            this.debugTab.SuspendLayout();
            this.settingsTab.SuspendLayout();
            this.grpSettingsChat.SuspendLayout();
            this.grpNetwork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.grpSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBlocks)).BeginInit();
            this.mapsTab.SuspendLayout();
            this.grpMapControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.contextMapStrip.SuspendLayout();
            this.blocksTab.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabs
            // 
            this.mainTabs.Controls.Add(this.consoleTab);
            this.mainTabs.Controls.Add(this.settingsTab);
            this.mainTabs.Controls.Add(this.mapsTab);
            this.mainTabs.Controls.Add(this.commandsTab);
            this.mainTabs.Controls.Add(this.blocksTab);
            this.mainTabs.Controls.Add(this.ranksTab);
            this.mainTabs.Controls.Add(this.dbTab);
            this.mainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabs.Location = new System.Drawing.Point(0, 24);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.Size = new System.Drawing.Size(989, 304);
            this.mainTabs.TabIndex = 0;
            this.mainTabs.SelectedIndexChanged += new System.EventHandler(this.mainTabs_SelectedIndexChanged);
            // 
            // consoleTab
            // 
            this.consoleTab.Controls.Add(this.consoletabs);
            this.consoleTab.Location = new System.Drawing.Point(4, 22);
            this.consoleTab.Name = "consoleTab";
            this.consoleTab.Padding = new System.Windows.Forms.Padding(3);
            this.consoleTab.Size = new System.Drawing.Size(981, 278);
            this.consoleTab.TabIndex = 0;
            this.consoleTab.Text = "Console";
            this.consoleTab.UseVisualStyleBackColor = true;
            // 
            // consoletabs
            // 
            this.consoletabs.Controls.Add(this.Alltab);
            this.consoletabs.Controls.Add(this.infotab);
            this.consoletabs.Controls.Add(this.chatTab);
            this.consoletabs.Controls.Add(this.cTab);
            this.consoletabs.Controls.Add(this.ErrorsTab);
            this.consoletabs.Controls.Add(this.debugTab);
            this.consoletabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoletabs.Location = new System.Drawing.Point(3, 3);
            this.consoletabs.Name = "consoletabs";
            this.consoletabs.SelectedIndex = 0;
            this.consoletabs.Size = new System.Drawing.Size(975, 272);
            this.consoletabs.TabIndex = 2;
            // 
            // Alltab
            // 
            this.Alltab.Controls.Add(this.txtAllConsole);
            this.Alltab.Location = new System.Drawing.Point(4, 22);
            this.Alltab.Name = "Alltab";
            this.Alltab.Padding = new System.Windows.Forms.Padding(3);
            this.Alltab.Size = new System.Drawing.Size(967, 246);
            this.Alltab.TabIndex = 0;
            this.Alltab.Text = "All";
            this.Alltab.UseVisualStyleBackColor = true;
            // 
            // txtAllConsole
            // 
            this.txtAllConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAllConsole.Location = new System.Drawing.Point(3, 3);
            this.txtAllConsole.Multiline = true;
            this.txtAllConsole.Name = "txtAllConsole";
            this.txtAllConsole.ReadOnly = true;
            this.txtAllConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAllConsole.Size = new System.Drawing.Size(961, 240);
            this.txtAllConsole.TabIndex = 0;
            // 
            // infotab
            // 
            this.infotab.Controls.Add(this.txtInfobox);
            this.infotab.Location = new System.Drawing.Point(4, 22);
            this.infotab.Name = "infotab";
            this.infotab.Padding = new System.Windows.Forms.Padding(3);
            this.infotab.Size = new System.Drawing.Size(967, 246);
            this.infotab.TabIndex = 1;
            this.infotab.Text = "Info";
            this.infotab.UseVisualStyleBackColor = true;
            // 
            // txtInfobox
            // 
            this.txtInfobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfobox.Location = new System.Drawing.Point(3, 3);
            this.txtInfobox.Name = "txtInfobox";
            this.txtInfobox.ReadOnly = true;
            this.txtInfobox.Size = new System.Drawing.Size(961, 240);
            this.txtInfobox.TabIndex = 0;
            this.txtInfobox.Text = "";
            // 
            // chatTab
            // 
            this.chatTab.Controls.Add(this.txtChatbox);
            this.chatTab.Controls.Add(this.btnSendChat);
            this.chatTab.Controls.Add(this.txtChat);
            this.chatTab.Location = new System.Drawing.Point(4, 22);
            this.chatTab.Name = "chatTab";
            this.chatTab.Size = new System.Drawing.Size(967, 246);
            this.chatTab.TabIndex = 2;
            this.chatTab.Text = "Chat";
            this.chatTab.UseVisualStyleBackColor = true;
            // 
            // txtChatbox
            // 
            this.txtChatbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChatbox.Location = new System.Drawing.Point(0, 0);
            this.txtChatbox.Name = "txtChatbox";
            this.txtChatbox.ReadOnly = true;
            this.txtChatbox.Size = new System.Drawing.Size(964, 214);
            this.txtChatbox.TabIndex = 2;
            this.txtChatbox.Text = "";
            // 
            // btnSendChat
            // 
            this.btnSendChat.Location = new System.Drawing.Point(883, 220);
            this.btnSendChat.Name = "btnSendChat";
            this.btnSendChat.Size = new System.Drawing.Size(75, 23);
            this.btnSendChat.TabIndex = 1;
            this.btnSendChat.Text = "Send";
            this.btnSendChat.UseVisualStyleBackColor = true;
            // 
            // txtChat
            // 
            this.txtChat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChat.Location = new System.Drawing.Point(3, 223);
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(874, 20);
            this.txtChat.TabIndex = 0;
            // 
            // cTab
            // 
            this.cTab.Controls.Add(this.txtCommandbox);
            this.cTab.Location = new System.Drawing.Point(4, 22);
            this.cTab.Name = "cTab";
            this.cTab.Size = new System.Drawing.Size(967, 246);
            this.cTab.TabIndex = 3;
            this.cTab.Text = "Commands";
            this.cTab.UseVisualStyleBackColor = true;
            // 
            // txtCommandbox
            // 
            this.txtCommandbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommandbox.Location = new System.Drawing.Point(0, 0);
            this.txtCommandbox.Name = "txtCommandbox";
            this.txtCommandbox.Size = new System.Drawing.Size(967, 246);
            this.txtCommandbox.TabIndex = 0;
            this.txtCommandbox.Text = "";
            // 
            // ErrorsTab
            // 
            this.ErrorsTab.Controls.Add(this.txtErrorbox);
            this.ErrorsTab.Location = new System.Drawing.Point(4, 22);
            this.ErrorsTab.Name = "ErrorsTab";
            this.ErrorsTab.Size = new System.Drawing.Size(967, 246);
            this.ErrorsTab.TabIndex = 4;
            this.ErrorsTab.Text = "Errors";
            this.ErrorsTab.UseVisualStyleBackColor = true;
            // 
            // txtErrorbox
            // 
            this.txtErrorbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorbox.Location = new System.Drawing.Point(0, 0);
            this.txtErrorbox.Name = "txtErrorbox";
            this.txtErrorbox.Size = new System.Drawing.Size(967, 246);
            this.txtErrorbox.TabIndex = 0;
            this.txtErrorbox.Text = "";
            // 
            // debugTab
            // 
            this.debugTab.Controls.Add(this.txtDebugBox);
            this.debugTab.Location = new System.Drawing.Point(4, 22);
            this.debugTab.Name = "debugTab";
            this.debugTab.Size = new System.Drawing.Size(967, 246);
            this.debugTab.TabIndex = 5;
            this.debugTab.Text = "Debug";
            this.debugTab.UseVisualStyleBackColor = true;
            // 
            // txtDebugBox
            // 
            this.txtDebugBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDebugBox.Location = new System.Drawing.Point(0, 0);
            this.txtDebugBox.Name = "txtDebugBox";
            this.txtDebugBox.Size = new System.Drawing.Size(967, 246);
            this.txtDebugBox.TabIndex = 0;
            this.txtDebugBox.Text = "";
            // 
            // settingsTab
            // 
            this.settingsTab.Controls.Add(this.btnRuleEdit);
            this.settingsTab.Controls.Add(this.grpSettingsChat);
            this.settingsTab.Controls.Add(this.grpNetwork);
            this.settingsTab.Controls.Add(this.grpSystem);
            this.settingsTab.Location = new System.Drawing.Point(4, 22);
            this.settingsTab.Name = "settingsTab";
            this.settingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.settingsTab.Size = new System.Drawing.Size(981, 278);
            this.settingsTab.TabIndex = 1;
            this.settingsTab.Text = "Settings";
            this.settingsTab.UseVisualStyleBackColor = true;
            // 
            // btnRuleEdit
            // 
            this.btnRuleEdit.Location = new System.Drawing.Point(447, 12);
            this.btnRuleEdit.Name = "btnRuleEdit";
            this.btnRuleEdit.Size = new System.Drawing.Size(200, 23);
            this.btnRuleEdit.TabIndex = 3;
            this.btnRuleEdit.Text = "Edit rules";
            this.btnRuleEdit.UseVisualStyleBackColor = true;
            // 
            // grpSettingsChat
            // 
            this.grpSettingsChat.Controls.Add(this.txtChatDivider);
            this.grpSettingsChat.Controls.Add(this.lblDivider);
            this.grpSettingsChat.Controls.Add(this.txtPlayerlist);
            this.grpSettingsChat.Controls.Add(this.lblPlayerList);
            this.grpSettingsChat.Controls.Add(this.lblSystem);
            this.grpSettingsChat.Controls.Add(this.lblError);
            this.grpSettingsChat.Controls.Add(this.txtChatSystem);
            this.grpSettingsChat.Controls.Add(this.txtChatError);
            this.grpSettingsChat.Location = new System.Drawing.Point(241, 123);
            this.grpSettingsChat.Name = "grpSettingsChat";
            this.grpSettingsChat.Size = new System.Drawing.Size(200, 136);
            this.grpSettingsChat.TabIndex = 2;
            this.grpSettingsChat.TabStop = false;
            this.grpSettingsChat.Text = "Chat";
            // 
            // txtChatDivider
            // 
            this.txtChatDivider.Location = new System.Drawing.Point(58, 39);
            this.txtChatDivider.Name = "txtChatDivider";
            this.txtChatDivider.Size = new System.Drawing.Size(135, 20);
            this.txtChatDivider.TabIndex = 7;
            // 
            // lblDivider
            // 
            this.lblDivider.AutoSize = true;
            this.lblDivider.Location = new System.Drawing.Point(9, 42);
            this.lblDivider.Name = "lblDivider";
            this.lblDivider.Size = new System.Drawing.Size(43, 13);
            this.lblDivider.TabIndex = 6;
            this.lblDivider.Text = "Divider:";
            // 
            // txtPlayerlist
            // 
            this.txtPlayerlist.Location = new System.Drawing.Point(58, 91);
            this.txtPlayerlist.Name = "txtPlayerlist";
            this.txtPlayerlist.Size = new System.Drawing.Size(135, 20);
            this.txtPlayerlist.TabIndex = 5;
            // 
            // lblPlayerList
            // 
            this.lblPlayerList.AutoSize = true;
            this.lblPlayerList.Location = new System.Drawing.Point(0, 94);
            this.lblPlayerList.Name = "lblPlayerList";
            this.lblPlayerList.Size = new System.Drawing.Size(52, 13);
            this.lblPlayerList.TabIndex = 4;
            this.lblPlayerList.Text = "PlayerList";
            // 
            // lblSystem
            // 
            this.lblSystem.AutoSize = true;
            this.lblSystem.Location = new System.Drawing.Point(8, 68);
            this.lblSystem.Name = "lblSystem";
            this.lblSystem.Size = new System.Drawing.Size(44, 13);
            this.lblSystem.TabIndex = 3;
            this.lblSystem.Text = "System:";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(20, 16);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(32, 13);
            this.lblError.TabIndex = 2;
            this.lblError.Text = "Error:";
            // 
            // txtChatSystem
            // 
            this.txtChatSystem.Location = new System.Drawing.Point(58, 65);
            this.txtChatSystem.Name = "txtChatSystem";
            this.txtChatSystem.Size = new System.Drawing.Size(135, 20);
            this.txtChatSystem.TabIndex = 1;
            // 
            // txtChatError
            // 
            this.txtChatError.Location = new System.Drawing.Point(58, 13);
            this.txtChatError.Name = "txtChatError";
            this.txtChatError.Size = new System.Drawing.Size(135, 20);
            this.txtChatError.TabIndex = 0;
            // 
            // grpNetwork
            // 
            this.grpNetwork.Controls.Add(this.numMaxPlayers);
            this.grpNetwork.Controls.Add(this.numPort);
            this.grpNetwork.Controls.Add(this.chkPub);
            this.grpNetwork.Controls.Add(this.chkVerifyNames);
            this.grpNetwork.Controls.Add(this.lblMaxPlayers);
            this.grpNetwork.Controls.Add(this.lblPort);
            this.grpNetwork.Location = new System.Drawing.Point(241, 6);
            this.grpNetwork.Name = "grpNetwork";
            this.grpNetwork.Size = new System.Drawing.Size(200, 111);
            this.grpNetwork.TabIndex = 1;
            this.grpNetwork.TabStop = false;
            this.grpNetwork.Text = "Network";
            // 
            // numMaxPlayers
            // 
            this.numMaxPlayers.Location = new System.Drawing.Point(76, 40);
            this.numMaxPlayers.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numMaxPlayers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxPlayers.Name = "numMaxPlayers";
            this.numMaxPlayers.Size = new System.Drawing.Size(120, 20);
            this.numMaxPlayers.TabIndex = 5;
            this.numMaxPlayers.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(76, 14);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(120, 20);
            this.numPort.TabIndex = 4;
            this.numPort.Value = new decimal(new int[] {
            25565,
            0,
            0,
            0});
            // 
            // chkPub
            // 
            this.chkPub.AutoSize = true;
            this.chkPub.Location = new System.Drawing.Point(139, 89);
            this.chkPub.Name = "chkPub";
            this.chkPub.Size = new System.Drawing.Size(55, 17);
            this.chkPub.TabIndex = 3;
            this.chkPub.Text = "Public";
            this.chkPub.UseVisualStyleBackColor = true;
            // 
            // chkVerifyNames
            // 
            this.chkVerifyNames.AutoSize = true;
            this.chkVerifyNames.Location = new System.Drawing.Point(106, 66);
            this.chkVerifyNames.Name = "chkVerifyNames";
            this.chkVerifyNames.Size = new System.Drawing.Size(88, 17);
            this.chkVerifyNames.TabIndex = 2;
            this.chkVerifyNames.Text = "Verify Names";
            this.chkVerifyNames.UseVisualStyleBackColor = true;
            // 
            // lblMaxPlayers
            // 
            this.lblMaxPlayers.AutoSize = true;
            this.lblMaxPlayers.Location = new System.Drawing.Point(6, 42);
            this.lblMaxPlayers.Name = "lblMaxPlayers";
            this.lblMaxPlayers.Size = new System.Drawing.Size(64, 13);
            this.lblMaxPlayers.TabIndex = 1;
            this.lblMaxPlayers.Text = "Max Players";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(44, 16);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port";
            // 
            // grpSystem
            // 
            this.grpSystem.Controls.Add(this.lblHistory);
            this.grpSystem.Controls.Add(this.numHistory);
            this.grpSystem.Controls.Add(this.lblBlockChn);
            this.grpSystem.Controls.Add(this.numMaxBlocks);
            this.grpSystem.Controls.Add(this.chkComArgs);
            this.grpSystem.Controls.Add(this.chkMaphistory);
            this.grpSystem.Controls.Add(this.chkLogs);
            this.grpSystem.Controls.Add(this.chkRotLogs);
            this.grpSystem.Controls.Add(this.txtWelcomeMess);
            this.grpSystem.Controls.Add(this.lblWelcmess);
            this.grpSystem.Controls.Add(this.txtSrvMotd);
            this.grpSystem.Controls.Add(this.lblSrvMotd);
            this.grpSystem.Controls.Add(this.txtSrvName);
            this.grpSystem.Controls.Add(this.lblSrvName);
            this.grpSystem.Location = new System.Drawing.Point(8, 6);
            this.grpSystem.Name = "grpSystem";
            this.grpSystem.Size = new System.Drawing.Size(227, 264);
            this.grpSystem.TabIndex = 0;
            this.grpSystem.TabStop = false;
            this.grpSystem.Text = "System";
            // 
            // lblHistory
            // 
            this.lblHistory.AutoSize = true;
            this.lblHistory.Location = new System.Drawing.Point(22, 235);
            this.lblHistory.Name = "lblHistory";
            this.lblHistory.Size = new System.Drawing.Size(94, 13);
            this.lblHistory.TabIndex = 13;
            this.lblHistory.Text = "Max history entries";
            // 
            // numHistory
            // 
            this.numHistory.Location = new System.Drawing.Point(122, 233);
            this.numHistory.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numHistory.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHistory.Name = "numHistory";
            this.numHistory.Size = new System.Drawing.Size(99, 20);
            this.numHistory.TabIndex = 12;
            this.numHistory.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // lblBlockChn
            // 
            this.lblBlockChn.AutoSize = true;
            this.lblBlockChn.Location = new System.Drawing.Point(3, 209);
            this.lblBlockChn.Name = "lblBlockChn";
            this.lblBlockChn.Size = new System.Drawing.Size(113, 13);
            this.lblBlockChn.TabIndex = 11;
            this.lblBlockChn.Text = "Max block changes /s";
            // 
            // numMaxBlocks
            // 
            this.numMaxBlocks.Location = new System.Drawing.Point(122, 207);
            this.numMaxBlocks.Maximum = new decimal(new int[] {
            900000,
            0,
            0,
            0});
            this.numMaxBlocks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxBlocks.Name = "numMaxBlocks";
            this.numMaxBlocks.Size = new System.Drawing.Size(99, 20);
            this.numMaxBlocks.TabIndex = 10;
            this.numMaxBlocks.Value = new decimal(new int[] {
            33000,
            0,
            0,
            0});
            // 
            // chkComArgs
            // 
            this.chkComArgs.AutoSize = true;
            this.chkComArgs.Location = new System.Drawing.Point(6, 184);
            this.chkComArgs.Name = "chkComArgs";
            this.chkComArgs.Size = new System.Drawing.Size(154, 17);
            this.chkComArgs.TabIndex = 9;
            this.chkComArgs.Text = "Show command arguments";
            this.chkComArgs.UseVisualStyleBackColor = true;
            // 
            // chkMaphistory
            // 
            this.chkMaphistory.AutoSize = true;
            this.chkMaphistory.Location = new System.Drawing.Point(6, 161);
            this.chkMaphistory.Name = "chkMaphistory";
            this.chkMaphistory.Size = new System.Drawing.Size(126, 17);
            this.chkMaphistory.TabIndex = 8;
            this.chkMaphistory.Text = "Compress Maphistory";
            this.chkMaphistory.UseVisualStyleBackColor = true;
            // 
            // chkLogs
            // 
            this.chkLogs.AutoSize = true;
            this.chkLogs.Location = new System.Drawing.Point(92, 138);
            this.chkLogs.Name = "chkLogs";
            this.chkLogs.Size = new System.Drawing.Size(72, 17);
            this.chkLogs.TabIndex = 7;
            this.chkLogs.Text = "Log to file";
            this.chkLogs.UseVisualStyleBackColor = true;
            // 
            // chkRotLogs
            // 
            this.chkRotLogs.AutoSize = true;
            this.chkRotLogs.Location = new System.Drawing.Point(6, 138);
            this.chkRotLogs.Name = "chkRotLogs";
            this.chkRotLogs.Size = new System.Drawing.Size(80, 17);
            this.chkRotLogs.TabIndex = 6;
            this.chkRotLogs.Text = "Rotate logs";
            this.chkRotLogs.UseVisualStyleBackColor = true;
            // 
            // txtWelcomeMess
            // 
            this.txtWelcomeMess.Location = new System.Drawing.Point(6, 112);
            this.txtWelcomeMess.Name = "txtWelcomeMess";
            this.txtWelcomeMess.Size = new System.Drawing.Size(215, 20);
            this.txtWelcomeMess.TabIndex = 5;
            // 
            // lblWelcmess
            // 
            this.lblWelcmess.AutoSize = true;
            this.lblWelcmess.Location = new System.Drawing.Point(3, 94);
            this.lblWelcmess.Name = "lblWelcmess";
            this.lblWelcmess.Size = new System.Drawing.Size(100, 13);
            this.lblWelcmess.TabIndex = 4;
            this.lblWelcmess.Text = "Welcome message:";
            // 
            // txtSrvMotd
            // 
            this.txtSrvMotd.Location = new System.Drawing.Point(6, 71);
            this.txtSrvMotd.Name = "txtSrvMotd";
            this.txtSrvMotd.Size = new System.Drawing.Size(215, 20);
            this.txtSrvMotd.TabIndex = 3;
            // 
            // lblSrvMotd
            // 
            this.lblSrvMotd.AutoSize = true;
            this.lblSrvMotd.Location = new System.Drawing.Point(3, 55);
            this.lblSrvMotd.Name = "lblSrvMotd";
            this.lblSrvMotd.Size = new System.Drawing.Size(76, 13);
            this.lblSrvMotd.TabIndex = 2;
            this.lblSrvMotd.Text = "Server MOTD:";
            // 
            // txtSrvName
            // 
            this.txtSrvName.Location = new System.Drawing.Point(6, 32);
            this.txtSrvName.Name = "txtSrvName";
            this.txtSrvName.Size = new System.Drawing.Size(215, 20);
            this.txtSrvName.TabIndex = 1;
            // 
            // lblSrvName
            // 
            this.lblSrvName.AutoSize = true;
            this.lblSrvName.Location = new System.Drawing.Point(3, 16);
            this.lblSrvName.Name = "lblSrvName";
            this.lblSrvName.Size = new System.Drawing.Size(70, 13);
            this.lblSrvName.TabIndex = 0;
            this.lblSrvName.Text = "Server name:";
            // 
            // mapsTab
            // 
            this.mapsTab.Controls.Add(this.lblMapBPerms);
            this.mapsTab.Controls.Add(this.lblMapSPerms);
            this.mapsTab.Controls.Add(this.lblMapJperms);
            this.mapsTab.Controls.Add(this.button9);
            this.mapsTab.Controls.Add(this.lstBuildPerms);
            this.mapsTab.Controls.Add(this.lstShowPerms);
            this.mapsTab.Controls.Add(this.lstJoinPerms);
            this.mapsTab.Controls.Add(this.grpMapControl);
            this.mapsTab.Controls.Add(this.txtMapMotd);
            this.mapsTab.Controls.Add(this.groupBox1);
            this.mapsTab.Controls.Add(this.lblMapMotd);
            this.mapsTab.Controls.Add(this.txtMapName);
            this.mapsTab.Controls.Add(this.lblMapName);
            this.mapsTab.Controls.Add(this.lblMaps);
            this.mapsTab.Controls.Add(this.lstMaps);
            this.mapsTab.Location = new System.Drawing.Point(4, 22);
            this.mapsTab.Name = "mapsTab";
            this.mapsTab.Size = new System.Drawing.Size(981, 278);
            this.mapsTab.TabIndex = 2;
            this.mapsTab.Text = "Maps";
            this.mapsTab.UseVisualStyleBackColor = true;
            // 
            // lblMapBPerms
            // 
            this.lblMapBPerms.AutoSize = true;
            this.lblMapBPerms.Location = new System.Drawing.Point(398, 51);
            this.lblMapBPerms.Name = "lblMapBPerms";
            this.lblMapBPerms.Size = new System.Drawing.Size(88, 13);
            this.lblMapBPerms.TabIndex = 17;
            this.lblMapBPerms.Text = "Build Permissions";
            // 
            // lblMapSPerms
            // 
            this.lblMapSPerms.AutoSize = true;
            this.lblMapSPerms.Location = new System.Drawing.Point(272, 51);
            this.lblMapSPerms.Name = "lblMapSPerms";
            this.lblMapSPerms.Size = new System.Drawing.Size(92, 13);
            this.lblMapSPerms.TabIndex = 16;
            this.lblMapSPerms.Text = "Show Permissions";
            // 
            // lblMapJperms
            // 
            this.lblMapJperms.AutoSize = true;
            this.lblMapJperms.Location = new System.Drawing.Point(143, 51);
            this.lblMapJperms.Name = "lblMapJperms";
            this.lblMapJperms.Size = new System.Drawing.Size(84, 13);
            this.lblMapJperms.TabIndex = 15;
            this.lblMapJperms.Text = "Join Permissions";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(447, 14);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 14;
            this.button9.Text = "Update";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // lstBuildPerms
            // 
            this.lstBuildPerms.FormattingEnabled = true;
            this.lstBuildPerms.Location = new System.Drawing.Point(398, 67);
            this.lstBuildPerms.Name = "lstBuildPerms";
            this.lstBuildPerms.Size = new System.Drawing.Size(120, 199);
            this.lstBuildPerms.TabIndex = 13;
            // 
            // lstShowPerms
            // 
            this.lstShowPerms.FormattingEnabled = true;
            this.lstShowPerms.Location = new System.Drawing.Point(272, 67);
            this.lstShowPerms.Name = "lstShowPerms";
            this.lstShowPerms.Size = new System.Drawing.Size(120, 199);
            this.lstShowPerms.TabIndex = 12;
            // 
            // lstJoinPerms
            // 
            this.lstJoinPerms.FormattingEnabled = true;
            this.lstJoinPerms.Location = new System.Drawing.Point(146, 67);
            this.lstJoinPerms.Name = "lstJoinPerms";
            this.lstJoinPerms.Size = new System.Drawing.Size(120, 199);
            this.lstJoinPerms.TabIndex = 11;
            // 
            // grpMapControl
            // 
            this.grpMapControl.Controls.Add(this.button8);
            this.grpMapControl.Controls.Add(this.button7);
            this.grpMapControl.Controls.Add(this.button6);
            this.grpMapControl.Controls.Add(this.button5);
            this.grpMapControl.Controls.Add(this.button4);
            this.grpMapControl.Controls.Add(this.button3);
            this.grpMapControl.Controls.Add(this.button2);
            this.grpMapControl.Controls.Add(this.button1);
            this.grpMapControl.Location = new System.Drawing.Point(773, 130);
            this.grpMapControl.Name = "grpMapControl";
            this.grpMapControl.Size = new System.Drawing.Size(200, 145);
            this.grpMapControl.TabIndex = 10;
            this.grpMapControl.TabStop = false;
            this.grpMapControl.Text = "Control";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(90, 106);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(84, 23);
            this.button8.TabIndex = 18;
            this.button8.Text = "Make Default";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(90, 77);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(84, 23);
            this.button7.TabIndex = 17;
            this.button7.Text = "Clear Queue";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(90, 19);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(84, 23);
            this.button6.TabIndex = 16;
            this.button6.Text = "Resend";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(9, 77);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 15;
            this.button5.Text = "Building";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(90, 48);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(84, 23);
            this.button4.TabIndex = 14;
            this.button4.Text = "History";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(9, 106);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "Physics";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(9, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Fill";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Resize";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtMapMotd
            // 
            this.txtMapMotd.Location = new System.Drawing.Point(341, 16);
            this.txtMapMotd.Name = "txtMapMotd";
            this.txtMapMotd.Size = new System.Drawing.Size(100, 20);
            this.txtMapMotd.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblMapPhysqueue);
            this.groupBox1.Controls.Add(this.lblMapSpawn);
            this.groupBox1.Controls.Add(this.lblMapBlockqueue);
            this.groupBox1.Controls.Add(this.lblMapSize);
            this.groupBox1.Controls.Add(this.lblMapGen);
            this.groupBox1.Controls.Add(this.lblMapBuilding);
            this.groupBox1.Controls.Add(this.lblMapHist);
            this.groupBox1.Controls.Add(this.lblLoadStatus);
            this.groupBox1.Controls.Add(this.lblMapPhys);
            this.groupBox1.Controls.Add(this.lblMapClients);
            this.groupBox1.Location = new System.Drawing.Point(773, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 121);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // lblMapPhysqueue
            // 
            this.lblMapPhysqueue.AutoSize = true;
            this.lblMapPhysqueue.Location = new System.Drawing.Point(6, 96);
            this.lblMapPhysqueue.Name = "lblMapPhysqueue";
            this.lblMapPhysqueue.Size = new System.Drawing.Size(90, 13);
            this.lblMapPhysqueue.TabIndex = 6;
            this.lblMapPhysqueue.Text = "Physics Queue: 0";
            // 
            // lblMapSpawn
            // 
            this.lblMapSpawn.AutoSize = true;
            this.lblMapSpawn.Location = new System.Drawing.Point(113, 64);
            this.lblMapSpawn.Name = "lblMapSpawn";
            this.lblMapSpawn.Size = new System.Drawing.Size(76, 13);
            this.lblMapSpawn.TabIndex = 9;
            this.lblMapSpawn.Text = "Spawn: 0, 0, 0";
            // 
            // lblMapBlockqueue
            // 
            this.lblMapBlockqueue.AutoSize = true;
            this.lblMapBlockqueue.Location = new System.Drawing.Point(6, 80);
            this.lblMapBlockqueue.Name = "lblMapBlockqueue";
            this.lblMapBlockqueue.Size = new System.Drawing.Size(104, 13);
            this.lblMapBlockqueue.TabIndex = 5;
            this.lblMapBlockqueue.Text = "Blocksend Queue: 0";
            // 
            // lblMapSize
            // 
            this.lblMapSize.AutoSize = true;
            this.lblMapSize.Location = new System.Drawing.Point(98, 48);
            this.lblMapSize.Name = "lblMapSize";
            this.lblMapSize.Size = new System.Drawing.Size(91, 13);
            this.lblMapSize.TabIndex = 7;
            this.lblMapSize.Text = "Size: 64 x 64 x 64";
            // 
            // lblMapGen
            // 
            this.lblMapGen.AutoSize = true;
            this.lblMapGen.Location = new System.Drawing.Point(87, 32);
            this.lblMapGen.Name = "lblMapGen";
            this.lblMapGen.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblMapGen.Size = new System.Drawing.Size(102, 13);
            this.lblMapGen.TabIndex = 8;
            this.lblMapGen.Text = "Generator: Flatgrass";
            // 
            // lblMapBuilding
            // 
            this.lblMapBuilding.AutoSize = true;
            this.lblMapBuilding.Location = new System.Drawing.Point(6, 64);
            this.lblMapBuilding.Name = "lblMapBuilding";
            this.lblMapBuilding.Size = new System.Drawing.Size(64, 13);
            this.lblMapBuilding.TabIndex = 4;
            this.lblMapBuilding.Text = "Building: On";
            // 
            // lblMapHist
            // 
            this.lblMapHist.AutoSize = true;
            this.lblMapHist.Location = new System.Drawing.Point(6, 48);
            this.lblMapHist.Name = "lblMapHist";
            this.lblMapHist.Size = new System.Drawing.Size(59, 13);
            this.lblMapHist.TabIndex = 3;
            this.lblMapHist.Text = "History: On";
            // 
            // lblLoadStatus
            // 
            this.lblLoadStatus.AutoSize = true;
            this.lblLoadStatus.Location = new System.Drawing.Point(110, 16);
            this.lblLoadStatus.Name = "lblLoadStatus";
            this.lblLoadStatus.Size = new System.Drawing.Size(79, 13);
            this.lblLoadStatus.TabIndex = 2;
            this.lblLoadStatus.Text = "Status: Loaded";
            // 
            // lblMapPhys
            // 
            this.lblMapPhys.AutoSize = true;
            this.lblMapPhys.Location = new System.Drawing.Point(6, 32);
            this.lblMapPhys.Name = "lblMapPhys";
            this.lblMapPhys.Size = new System.Drawing.Size(63, 13);
            this.lblMapPhys.TabIndex = 1;
            this.lblMapPhys.Text = "Physics: On";
            // 
            // lblMapClients
            // 
            this.lblMapClients.AutoSize = true;
            this.lblMapClients.Location = new System.Drawing.Point(6, 16);
            this.lblMapClients.Name = "lblMapClients";
            this.lblMapClients.Size = new System.Drawing.Size(50, 13);
            this.lblMapClients.TabIndex = 0;
            this.lblMapClients.Text = "Clients: 0";
            // 
            // lblMapMotd
            // 
            this.lblMapMotd.AutoSize = true;
            this.lblMapMotd.Location = new System.Drawing.Point(293, 19);
            this.lblMapMotd.Name = "lblMapMotd";
            this.lblMapMotd.Size = new System.Drawing.Size(42, 13);
            this.lblMapMotd.TabIndex = 4;
            this.lblMapMotd.Text = "MOTD:";
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(187, 16);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.Size = new System.Drawing.Size(100, 20);
            this.txtMapName.TabIndex = 3;
            // 
            // lblMapName
            // 
            this.lblMapName.AutoSize = true;
            this.lblMapName.Location = new System.Drawing.Point(143, 19);
            this.lblMapName.Name = "lblMapName";
            this.lblMapName.Size = new System.Drawing.Size(38, 13);
            this.lblMapName.TabIndex = 2;
            this.lblMapName.Text = "Name:";
            // 
            // lblMaps
            // 
            this.lblMaps.AutoSize = true;
            this.lblMaps.Location = new System.Drawing.Point(0, 0);
            this.lblMaps.Name = "lblMaps";
            this.lblMaps.Size = new System.Drawing.Size(36, 13);
            this.lblMaps.TabIndex = 1;
            this.lblMaps.Text = "Maps:";
            // 
            // lstMaps
            // 
            this.lstMaps.ContextMenuStrip = this.contextMapStrip;
            this.lstMaps.FormattingEnabled = true;
            this.lstMaps.Location = new System.Drawing.Point(8, 19);
            this.lstMaps.Name = "lstMaps";
            this.lstMaps.Size = new System.Drawing.Size(120, 251);
            this.lstMaps.TabIndex = 0;
            // 
            // contextMapStrip
            // 
            this.contextMapStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMapToolStripMenuItem,
            this.deleteMapToolStripMenuItem});
            this.contextMapStrip.Name = "contextMapStrip";
            this.contextMapStrip.Size = new System.Drawing.Size(135, 48);
            // 
            // addMapToolStripMenuItem
            // 
            this.addMapToolStripMenuItem.Name = "addMapToolStripMenuItem";
            this.addMapToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.addMapToolStripMenuItem.Text = "Add Map";
            // 
            // deleteMapToolStripMenuItem
            // 
            this.deleteMapToolStripMenuItem.Name = "deleteMapToolStripMenuItem";
            this.deleteMapToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.deleteMapToolStripMenuItem.Text = "Delete Map";
            // 
            // commandsTab
            // 
            this.commandsTab.Location = new System.Drawing.Point(4, 22);
            this.commandsTab.Name = "commandsTab";
            this.commandsTab.Size = new System.Drawing.Size(981, 278);
            this.commandsTab.TabIndex = 3;
            this.commandsTab.Text = "Commands";
            this.commandsTab.UseVisualStyleBackColor = true;
            // 
            // blocksTab
            // 
            this.blocksTab.Controls.Add(this.button12);
            this.blocksTab.Controls.Add(this.button11);
            this.blocksTab.Controls.Add(this.button10);
            this.blocksTab.Controls.Add(this.groupBox4);
            this.blocksTab.Controls.Add(this.label24);
            this.blocksTab.Controls.Add(this.label23);
            this.blocksTab.Controls.Add(this.label22);
            this.blocksTab.Controls.Add(this.label21);
            this.blocksTab.Controls.Add(this.label20);
            this.blocksTab.Controls.Add(this.label19);
            this.blocksTab.Controls.Add(this.label18);
            this.blocksTab.Controls.Add(this.lstBlocks);
            this.blocksTab.Location = new System.Drawing.Point(4, 22);
            this.blocksTab.Name = "blocksTab";
            this.blocksTab.Size = new System.Drawing.Size(981, 278);
            this.blocksTab.TabIndex = 4;
            this.blocksTab.Text = "Blocks";
            this.blocksTab.UseVisualStyleBackColor = true;
            // 
            // ranksTab
            // 
            this.ranksTab.Location = new System.Drawing.Point(4, 22);
            this.ranksTab.Name = "ranksTab";
            this.ranksTab.Size = new System.Drawing.Size(981, 278);
            this.ranksTab.TabIndex = 5;
            this.ranksTab.Text = "Ranks";
            this.ranksTab.UseVisualStyleBackColor = true;
            // 
            // dbTab
            // 
            this.dbTab.Location = new System.Drawing.Point(4, 22);
            this.dbTab.Name = "dbTab";
            this.dbTab.Size = new System.Drawing.Size(981, 278);
            this.dbTab.TabIndex = 6;
            this.dbTab.Text = "Database";
            this.dbTab.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlToolStripMenuItem,
            this.gUIToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(989, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startServerToolStripMenuItem,
            this.stopServerToolStripMenuItem});
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.controlToolStripMenuItem.Text = "&Control";
            // 
            // startServerToolStripMenuItem
            // 
            this.startServerToolStripMenuItem.Name = "startServerToolStripMenuItem";
            this.startServerToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.startServerToolStripMenuItem.Text = "&Start Server";
            this.startServerToolStripMenuItem.Click += new System.EventHandler(this.startServerToolStripMenuItem_Click);
            // 
            // stopServerToolStripMenuItem
            // 
            this.stopServerToolStripMenuItem.Name = "stopServerToolStripMenuItem";
            this.stopServerToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.stopServerToolStripMenuItem.Text = "S&top Server";
            this.stopServerToolStripMenuItem.Click += new System.EventHandler(this.stopServerToolStripMenuItem_Click);
            // 
            // gUIToolStripMenuItem
            // 
            this.gUIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendToTrayToolStripMenuItem,
            this.miniModeToolStripMenuItem});
            this.gUIToolStripMenuItem.Name = "gUIToolStripMenuItem";
            this.gUIToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.gUIToolStripMenuItem.Text = "&GUI";
            // 
            // sendToTrayToolStripMenuItem
            // 
            this.sendToTrayToolStripMenuItem.Name = "sendToTrayToolStripMenuItem";
            this.sendToTrayToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.sendToTrayToolStripMenuItem.Text = "&Send to Tray";
            // 
            // miniModeToolStripMenuItem
            // 
            this.miniModeToolStripMenuItem.Name = "miniModeToolStripMenuItem";
            this.miniModeToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.miniModeToolStripMenuItem.Text = "&Mini mode";
            // 
            // lstBlocks
            // 
            this.lstBlocks.FormattingEnabled = true;
            this.lstBlocks.Location = new System.Drawing.Point(3, 3);
            this.lstBlocks.Name = "lstBlocks";
            this.lstBlocks.Size = new System.Drawing.Size(120, 251);
            this.lstBlocks.TabIndex = 0;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(167, 3);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(19, 13);
            this.label18.TabIndex = 1;
            this.label18.Text = "Id:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(148, 29);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(38, 13);
            this.label19.TabIndex = 2;
            this.label19.Text = "Name:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(129, 51);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(65, 13);
            this.label20.TabIndex = 3;
            this.label20.Text = "ID on Client:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(160, 73);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(34, 13);
            this.label21.TabIndex = 4;
            this.label21.Text = "Color:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(166, 86);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(28, 13);
            this.label22.TabIndex = 5;
            this.label22.Text = "Kills:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(148, 99);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(45, 13);
            this.label23.TabIndex = 6;
            this.label23.Text = "Special:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(149, 132);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(45, 13);
            this.label24.TabIndex = 7;
            this.label24.Text = "Special:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label30);
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.label28);
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Location = new System.Drawing.Point(608, 20);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(325, 177);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Physics";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(18, 16);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(46, 13);
            this.label25.TabIndex = 8;
            this.label25.Text = "Physics:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(18, 41);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(76, 13);
            this.label26.TabIndex = 9;
            this.label26.Text = "Physics Delay:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(18, 66);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(89, 13);
            this.label27.TabIndex = 10;
            this.label27.Text = "Physics Random:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(18, 90);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(84, 13);
            this.label28.TabIndex = 11;
            this.label28.Text = "Repeat Physics:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(18, 112);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(76, 13);
            this.label29.TabIndex = 12;
            this.label29.Text = "Physics Script:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(18, 147);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(107, 13);
            this.label30.TabIndex = 13;
            this.label30.Text = "Physics on map load:";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(132, 231);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(96, 23);
            this.button10.TabIndex = 9;
            this.button10.Text = "Create Block";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(234, 231);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(85, 23);
            this.button11.TabIndex = 10;
            this.button11.Text = "Delete Block";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(325, 231);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(85, 23);
            this.button12.TabIndex = 11;
            this.button12.Text = "Save Blocks";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // mainForm
            // 
            this.AcceptButton = this.btnSendChat;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 328);
            this.Controls.Add(this.mainTabs);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "ServerCore";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.mainTabs.ResumeLayout(false);
            this.consoleTab.ResumeLayout(false);
            this.consoletabs.ResumeLayout(false);
            this.Alltab.ResumeLayout(false);
            this.Alltab.PerformLayout();
            this.infotab.ResumeLayout(false);
            this.chatTab.ResumeLayout(false);
            this.chatTab.PerformLayout();
            this.cTab.ResumeLayout(false);
            this.ErrorsTab.ResumeLayout(false);
            this.debugTab.ResumeLayout(false);
            this.settingsTab.ResumeLayout(false);
            this.grpSettingsChat.ResumeLayout(false);
            this.grpSettingsChat.PerformLayout();
            this.grpNetwork.ResumeLayout(false);
            this.grpNetwork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.grpSystem.ResumeLayout(false);
            this.grpSystem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBlocks)).EndInit();
            this.mapsTab.ResumeLayout(false);
            this.mapsTab.PerformLayout();
            this.grpMapControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMapStrip.ResumeLayout(false);
            this.blocksTab.ResumeLayout(false);
            this.blocksTab.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabs;
        private System.Windows.Forms.TabPage consoleTab;
        private System.Windows.Forms.TabControl consoletabs;
        private System.Windows.Forms.TabPage Alltab;
        private System.Windows.Forms.TextBox txtAllConsole;
        private System.Windows.Forms.TabPage infotab;
        private System.Windows.Forms.RichTextBox txtInfobox;
        private System.Windows.Forms.TabPage chatTab;
        private System.Windows.Forms.RichTextBox txtChatbox;
        private System.Windows.Forms.Button btnSendChat;
        private System.Windows.Forms.TextBox txtChat;
        private System.Windows.Forms.TabPage cTab;
        private System.Windows.Forms.RichTextBox txtCommandbox;
        private System.Windows.Forms.TabPage ErrorsTab;
        private System.Windows.Forms.RichTextBox txtErrorbox;
        private System.Windows.Forms.TabPage debugTab;
        private System.Windows.Forms.RichTextBox txtDebugBox;
        private System.Windows.Forms.TabPage settingsTab;
        private System.Windows.Forms.TabPage mapsTab;
        private System.Windows.Forms.TabPage commandsTab;
        private System.Windows.Forms.TabPage blocksTab;
        private System.Windows.Forms.TabPage ranksTab;
        private System.Windows.Forms.TabPage dbTab;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gUIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendToTrayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miniModeToolStripMenuItem;
        private System.Windows.Forms.Button btnRuleEdit;
        private System.Windows.Forms.GroupBox grpSettingsChat;
        private System.Windows.Forms.TextBox txtChatDivider;
        private System.Windows.Forms.Label lblDivider;
        private System.Windows.Forms.TextBox txtPlayerlist;
        private System.Windows.Forms.Label lblPlayerList;
        private System.Windows.Forms.Label lblSystem;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtChatSystem;
        private System.Windows.Forms.TextBox txtChatError;
        private System.Windows.Forms.GroupBox grpNetwork;
        private System.Windows.Forms.NumericUpDown numMaxPlayers;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.CheckBox chkPub;
        private System.Windows.Forms.CheckBox chkVerifyNames;
        private System.Windows.Forms.Label lblMaxPlayers;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.GroupBox grpSystem;
        private System.Windows.Forms.Label lblHistory;
        private System.Windows.Forms.NumericUpDown numHistory;
        private System.Windows.Forms.Label lblBlockChn;
        private System.Windows.Forms.NumericUpDown numMaxBlocks;
        private System.Windows.Forms.CheckBox chkComArgs;
        private System.Windows.Forms.CheckBox chkMaphistory;
        private System.Windows.Forms.CheckBox chkLogs;
        private System.Windows.Forms.CheckBox chkRotLogs;
        private System.Windows.Forms.TextBox txtWelcomeMess;
        private System.Windows.Forms.Label lblWelcmess;
        private System.Windows.Forms.TextBox txtSrvMotd;
        private System.Windows.Forms.Label lblSrvMotd;
        private System.Windows.Forms.TextBox txtSrvName;
        private System.Windows.Forms.Label lblSrvName;
        private System.Windows.Forms.Label lblMaps;
        private System.Windows.Forms.ListBox lstMaps;
        private System.Windows.Forms.Label lblMapMotd;
        private System.Windows.Forms.TextBox txtMapName;
        private System.Windows.Forms.Label lblMapName;
        private System.Windows.Forms.ContextMenuStrip contextMapStrip;
        private System.Windows.Forms.ToolStripMenuItem addMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMapToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLoadStatus;
        private System.Windows.Forms.Label lblMapPhys;
        private System.Windows.Forms.Label lblMapClients;
        private System.Windows.Forms.Label lblMapPhysqueue;
        private System.Windows.Forms.Label lblMapBlockqueue;
        private System.Windows.Forms.Label lblMapBuilding;
        private System.Windows.Forms.Label lblMapHist;
        private System.Windows.Forms.TextBox txtMapMotd;
        private System.Windows.Forms.Label lblMapSize;
        private System.Windows.Forms.ListBox lstBuildPerms;
        private System.Windows.Forms.ListBox lstShowPerms;
        private System.Windows.Forms.ListBox lstJoinPerms;
        private System.Windows.Forms.GroupBox grpMapControl;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblMapSpawn;
        private System.Windows.Forms.Label lblMapGen;
        private System.Windows.Forms.Label lblMapBPerms;
        private System.Windows.Forms.Label lblMapSPerms;
        private System.Windows.Forms.Label lblMapJperms;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.ListBox lstBlocks;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
    }
}

