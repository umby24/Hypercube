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
            this.btnMapUpdate = new System.Windows.Forms.Button();
            this.lstBuildPerms = new System.Windows.Forms.ListBox();
            this.lstShowPerms = new System.Windows.Forms.ListBox();
            this.lstJoinPerms = new System.Windows.Forms.ListBox();
            this.grpMapControl = new System.Windows.Forms.GroupBox();
            this.btnMapDefault = new System.Windows.Forms.Button();
            this.btnMapClearQueue = new System.Windows.Forms.Button();
            this.btnMapResend = new System.Windows.Forms.Button();
            this.btnMapBuilding = new System.Windows.Forms.Button();
            this.btnMapHistoryOn = new System.Windows.Forms.Button();
            this.btnMapPhysics = new System.Windows.Forms.Button();
            this.btnMapFill = new System.Windows.Forms.Button();
            this.btnMapResize = new System.Windows.Forms.Button();
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
            this.btnReloadBlocks = new System.Windows.Forms.Button();
            this.selectBlockSpecial = new System.Windows.Forms.ComboBox();
            this.selectBlockKills = new System.Windows.Forms.ComboBox();
            this.picBlockColor = new System.Windows.Forms.PictureBox();
            this.numBlockCId = new System.Windows.Forms.NumericUpDown();
            this.txtBlockName = new System.Windows.Forms.TextBox();
            this.numBlockId = new System.Windows.Forms.NumericUpDown();
            this.btnSaveBlocks = new System.Windows.Forms.Button();
            this.btnDeleteBlock = new System.Windows.Forms.Button();
            this.btnCreateBlock = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.selectPhysMapLoad = new System.Windows.Forms.ComboBox();
            this.txtPhysScript = new System.Windows.Forms.TextBox();
            this.selectRepPhysics = new System.Windows.Forms.ComboBox();
            this.txtPhysRandom = new System.Windows.Forms.TextBox();
            this.txtPhysDelay = new System.Windows.Forms.TextBox();
            this.selectBlockPhysics = new System.Windows.Forms.ComboBox();
            this.lblBlockPhysMapload = new System.Windows.Forms.Label();
            this.lblBlockPhysScript = new System.Windows.Forms.Label();
            this.lblBlockRepPhys = new System.Windows.Forms.Label();
            this.lblBlockPhysRandom = new System.Windows.Forms.Label();
            this.lblBlockPhysDelay = new System.Windows.Forms.Label();
            this.lblBlockPhysics = new System.Windows.Forms.Label();
            this.lblBlockSpecial = new System.Windows.Forms.Label();
            this.lblBlockKills = new System.Windows.Forms.Label();
            this.lblBlockColor = new System.Windows.Forms.Label();
            this.lblBlockCId = new System.Windows.Forms.Label();
            this.lblBlockName = new System.Windows.Forms.Label();
            this.lblBlockId = new System.Windows.Forms.Label();
            this.lstBlocks = new System.Windows.Forms.ListBox();
            this.ranksTab = new System.Windows.Forms.TabPage();
            this.dbTab = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miniModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            ((System.ComponentModel.ISupportInitialize)(this.picBlockColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlockCId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlockId)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.mapsTab.Controls.Add(this.btnMapUpdate);
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
            // btnMapUpdate
            // 
            this.btnMapUpdate.Location = new System.Drawing.Point(447, 14);
            this.btnMapUpdate.Name = "btnMapUpdate";
            this.btnMapUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnMapUpdate.TabIndex = 14;
            this.btnMapUpdate.Text = "Update";
            this.btnMapUpdate.UseVisualStyleBackColor = true;
            this.btnMapUpdate.Click += new System.EventHandler(this.btnMapUpdate_Click);
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
            this.grpMapControl.Controls.Add(this.btnMapDefault);
            this.grpMapControl.Controls.Add(this.btnMapClearQueue);
            this.grpMapControl.Controls.Add(this.btnMapResend);
            this.grpMapControl.Controls.Add(this.btnMapBuilding);
            this.grpMapControl.Controls.Add(this.btnMapHistoryOn);
            this.grpMapControl.Controls.Add(this.btnMapPhysics);
            this.grpMapControl.Controls.Add(this.btnMapFill);
            this.grpMapControl.Controls.Add(this.btnMapResize);
            this.grpMapControl.Location = new System.Drawing.Point(758, 130);
            this.grpMapControl.Name = "grpMapControl";
            this.grpMapControl.Size = new System.Drawing.Size(215, 145);
            this.grpMapControl.TabIndex = 10;
            this.grpMapControl.TabStop = false;
            this.grpMapControl.Text = "Control";
            // 
            // btnMapDefault
            // 
            this.btnMapDefault.Location = new System.Drawing.Point(105, 106);
            this.btnMapDefault.Name = "btnMapDefault";
            this.btnMapDefault.Size = new System.Drawing.Size(84, 23);
            this.btnMapDefault.TabIndex = 18;
            this.btnMapDefault.Text = "Make Default";
            this.btnMapDefault.UseVisualStyleBackColor = true;
            this.btnMapDefault.Click += new System.EventHandler(this.btnMapDefault_Click);
            // 
            // btnMapClearQueue
            // 
            this.btnMapClearQueue.Location = new System.Drawing.Point(105, 77);
            this.btnMapClearQueue.Name = "btnMapClearQueue";
            this.btnMapClearQueue.Size = new System.Drawing.Size(84, 23);
            this.btnMapClearQueue.TabIndex = 17;
            this.btnMapClearQueue.Text = "Clear Queue";
            this.btnMapClearQueue.UseVisualStyleBackColor = true;
            this.btnMapClearQueue.Click += new System.EventHandler(this.btnMapClearQueue_Click);
            // 
            // btnMapResend
            // 
            this.btnMapResend.Location = new System.Drawing.Point(105, 19);
            this.btnMapResend.Name = "btnMapResend";
            this.btnMapResend.Size = new System.Drawing.Size(84, 23);
            this.btnMapResend.TabIndex = 16;
            this.btnMapResend.Text = "Resend";
            this.btnMapResend.UseVisualStyleBackColor = true;
            this.btnMapResend.Click += new System.EventHandler(this.btnMapResend_Click);
            // 
            // btnMapBuilding
            // 
            this.btnMapBuilding.Location = new System.Drawing.Point(9, 77);
            this.btnMapBuilding.Name = "btnMapBuilding";
            this.btnMapBuilding.Size = new System.Drawing.Size(87, 23);
            this.btnMapBuilding.TabIndex = 15;
            this.btnMapBuilding.Text = "Building";
            this.btnMapBuilding.UseVisualStyleBackColor = true;
            this.btnMapBuilding.Click += new System.EventHandler(this.btnMapBuilding_Click);
            // 
            // btnMapHistoryOn
            // 
            this.btnMapHistoryOn.Location = new System.Drawing.Point(105, 48);
            this.btnMapHistoryOn.Name = "btnMapHistoryOn";
            this.btnMapHistoryOn.Size = new System.Drawing.Size(84, 23);
            this.btnMapHistoryOn.TabIndex = 14;
            this.btnMapHistoryOn.Text = "History";
            this.btnMapHistoryOn.UseVisualStyleBackColor = true;
            this.btnMapHistoryOn.Click += new System.EventHandler(this.btnMapHistoryOn_Click);
            // 
            // btnMapPhysics
            // 
            this.btnMapPhysics.Location = new System.Drawing.Point(9, 106);
            this.btnMapPhysics.Name = "btnMapPhysics";
            this.btnMapPhysics.Size = new System.Drawing.Size(90, 23);
            this.btnMapPhysics.TabIndex = 13;
            this.btnMapPhysics.Text = "Physics";
            this.btnMapPhysics.UseVisualStyleBackColor = true;
            this.btnMapPhysics.Click += new System.EventHandler(this.btnMapPhysics_Click);
            // 
            // btnMapFill
            // 
            this.btnMapFill.Location = new System.Drawing.Point(9, 48);
            this.btnMapFill.Name = "btnMapFill";
            this.btnMapFill.Size = new System.Drawing.Size(87, 23);
            this.btnMapFill.TabIndex = 12;
            this.btnMapFill.Text = "Fill";
            this.btnMapFill.UseVisualStyleBackColor = true;
            this.btnMapFill.Click += new System.EventHandler(this.btnMapFill_Click);
            // 
            // btnMapResize
            // 
            this.btnMapResize.Location = new System.Drawing.Point(9, 19);
            this.btnMapResize.Name = "btnMapResize";
            this.btnMapResize.Size = new System.Drawing.Size(87, 23);
            this.btnMapResize.TabIndex = 11;
            this.btnMapResize.Text = "Resize";
            this.btnMapResize.UseVisualStyleBackColor = true;
            this.btnMapResize.Click += new System.EventHandler(this.btnMapResize_Click);
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
            this.groupBox1.Location = new System.Drawing.Point(758, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 121);
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
            this.lstMaps.SelectedIndexChanged += new System.EventHandler(this.lstMaps_SelectedIndexChanged);
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
            this.blocksTab.Controls.Add(this.btnReloadBlocks);
            this.blocksTab.Controls.Add(this.selectBlockSpecial);
            this.blocksTab.Controls.Add(this.selectBlockKills);
            this.blocksTab.Controls.Add(this.picBlockColor);
            this.blocksTab.Controls.Add(this.numBlockCId);
            this.blocksTab.Controls.Add(this.txtBlockName);
            this.blocksTab.Controls.Add(this.numBlockId);
            this.blocksTab.Controls.Add(this.btnSaveBlocks);
            this.blocksTab.Controls.Add(this.btnDeleteBlock);
            this.blocksTab.Controls.Add(this.btnCreateBlock);
            this.blocksTab.Controls.Add(this.groupBox4);
            this.blocksTab.Controls.Add(this.lblBlockSpecial);
            this.blocksTab.Controls.Add(this.lblBlockKills);
            this.blocksTab.Controls.Add(this.lblBlockColor);
            this.blocksTab.Controls.Add(this.lblBlockCId);
            this.blocksTab.Controls.Add(this.lblBlockName);
            this.blocksTab.Controls.Add(this.lblBlockId);
            this.blocksTab.Controls.Add(this.lstBlocks);
            this.blocksTab.Location = new System.Drawing.Point(4, 22);
            this.blocksTab.Name = "blocksTab";
            this.blocksTab.Size = new System.Drawing.Size(981, 278);
            this.blocksTab.TabIndex = 4;
            this.blocksTab.Text = "Blocks";
            this.blocksTab.UseVisualStyleBackColor = true;
            // 
            // btnReloadBlocks
            // 
            this.btnReloadBlocks.Location = new System.Drawing.Point(416, 244);
            this.btnReloadBlocks.Name = "btnReloadBlocks";
            this.btnReloadBlocks.Size = new System.Drawing.Size(85, 23);
            this.btnReloadBlocks.TabIndex = 18;
            this.btnReloadBlocks.Text = "Reload Blocks";
            this.btnReloadBlocks.UseVisualStyleBackColor = true;
            this.btnReloadBlocks.Click += new System.EventHandler(this.btnReloadBlocks_Click);
            // 
            // selectBlockSpecial
            // 
            this.selectBlockSpecial.FormattingEnabled = true;
            this.selectBlockSpecial.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectBlockSpecial.Location = new System.Drawing.Point(198, 134);
            this.selectBlockSpecial.Name = "selectBlockSpecial";
            this.selectBlockSpecial.Size = new System.Drawing.Size(121, 21);
            this.selectBlockSpecial.TabIndex = 17;
            this.selectBlockSpecial.Text = "No";
            // 
            // selectBlockKills
            // 
            this.selectBlockKills.FormattingEnabled = true;
            this.selectBlockKills.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectBlockKills.Location = new System.Drawing.Point(199, 107);
            this.selectBlockKills.Name = "selectBlockKills";
            this.selectBlockKills.Size = new System.Drawing.Size(121, 21);
            this.selectBlockKills.TabIndex = 16;
            this.selectBlockKills.Text = "No";
            // 
            // picBlockColor
            // 
            this.picBlockColor.Location = new System.Drawing.Point(199, 81);
            this.picBlockColor.Name = "picBlockColor";
            this.picBlockColor.Size = new System.Drawing.Size(20, 20);
            this.picBlockColor.TabIndex = 15;
            this.picBlockColor.TabStop = false;
            // 
            // numBlockCId
            // 
            this.numBlockCId.Location = new System.Drawing.Point(199, 55);
            this.numBlockCId.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBlockCId.Name = "numBlockCId";
            this.numBlockCId.Size = new System.Drawing.Size(120, 20);
            this.numBlockCId.TabIndex = 14;
            // 
            // txtBlockName
            // 
            this.txtBlockName.Location = new System.Drawing.Point(199, 29);
            this.txtBlockName.Name = "txtBlockName";
            this.txtBlockName.Size = new System.Drawing.Size(120, 20);
            this.txtBlockName.TabIndex = 13;
            // 
            // numBlockId
            // 
            this.numBlockId.Location = new System.Drawing.Point(199, 3);
            this.numBlockId.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBlockId.Name = "numBlockId";
            this.numBlockId.Size = new System.Drawing.Size(120, 20);
            this.numBlockId.TabIndex = 12;
            // 
            // btnSaveBlocks
            // 
            this.btnSaveBlocks.Location = new System.Drawing.Point(325, 244);
            this.btnSaveBlocks.Name = "btnSaveBlocks";
            this.btnSaveBlocks.Size = new System.Drawing.Size(85, 23);
            this.btnSaveBlocks.TabIndex = 11;
            this.btnSaveBlocks.Text = "Save Blocks";
            this.btnSaveBlocks.UseVisualStyleBackColor = true;
            this.btnSaveBlocks.Click += new System.EventHandler(this.btnSaveBlocks_Click);
            // 
            // btnDeleteBlock
            // 
            this.btnDeleteBlock.Location = new System.Drawing.Point(234, 244);
            this.btnDeleteBlock.Name = "btnDeleteBlock";
            this.btnDeleteBlock.Size = new System.Drawing.Size(85, 23);
            this.btnDeleteBlock.TabIndex = 10;
            this.btnDeleteBlock.Text = "Delete Block";
            this.btnDeleteBlock.UseVisualStyleBackColor = true;
            this.btnDeleteBlock.Click += new System.EventHandler(this.btnDeleteBlock_Click);
            // 
            // btnCreateBlock
            // 
            this.btnCreateBlock.Location = new System.Drawing.Point(131, 244);
            this.btnCreateBlock.Name = "btnCreateBlock";
            this.btnCreateBlock.Size = new System.Drawing.Size(96, 23);
            this.btnCreateBlock.TabIndex = 9;
            this.btnCreateBlock.Text = "Create Block";
            this.btnCreateBlock.UseVisualStyleBackColor = true;
            this.btnCreateBlock.Click += new System.EventHandler(this.btnCreateBlock_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.selectPhysMapLoad);
            this.groupBox4.Controls.Add(this.txtPhysScript);
            this.groupBox4.Controls.Add(this.selectRepPhysics);
            this.groupBox4.Controls.Add(this.txtPhysRandom);
            this.groupBox4.Controls.Add(this.txtPhysDelay);
            this.groupBox4.Controls.Add(this.selectBlockPhysics);
            this.groupBox4.Controls.Add(this.lblBlockPhysMapload);
            this.groupBox4.Controls.Add(this.lblBlockPhysScript);
            this.groupBox4.Controls.Add(this.lblBlockRepPhys);
            this.groupBox4.Controls.Add(this.lblBlockPhysRandom);
            this.groupBox4.Controls.Add(this.lblBlockPhysDelay);
            this.groupBox4.Controls.Add(this.lblBlockPhysics);
            this.groupBox4.Location = new System.Drawing.Point(648, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(325, 177);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Physics";
            // 
            // selectPhysMapLoad
            // 
            this.selectPhysMapLoad.FormattingEnabled = true;
            this.selectPhysMapLoad.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectPhysMapLoad.Location = new System.Drawing.Point(122, 145);
            this.selectPhysMapLoad.Name = "selectPhysMapLoad";
            this.selectPhysMapLoad.Size = new System.Drawing.Size(121, 21);
            this.selectPhysMapLoad.TabIndex = 23;
            this.selectPhysMapLoad.Text = "No";
            this.selectPhysMapLoad.Visible = false;
            // 
            // txtPhysScript
            // 
            this.txtPhysScript.Location = new System.Drawing.Point(122, 119);
            this.txtPhysScript.Name = "txtPhysScript";
            this.txtPhysScript.Size = new System.Drawing.Size(121, 20);
            this.txtPhysScript.TabIndex = 22;
            // 
            // selectRepPhysics
            // 
            this.selectRepPhysics.FormattingEnabled = true;
            this.selectRepPhysics.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectRepPhysics.Location = new System.Drawing.Point(122, 92);
            this.selectRepPhysics.Name = "selectRepPhysics";
            this.selectRepPhysics.Size = new System.Drawing.Size(121, 21);
            this.selectRepPhysics.TabIndex = 21;
            this.selectRepPhysics.Text = "No";
            // 
            // txtPhysRandom
            // 
            this.txtPhysRandom.Location = new System.Drawing.Point(122, 66);
            this.txtPhysRandom.Name = "txtPhysRandom";
            this.txtPhysRandom.Size = new System.Drawing.Size(121, 20);
            this.txtPhysRandom.TabIndex = 20;
            // 
            // txtPhysDelay
            // 
            this.txtPhysDelay.Location = new System.Drawing.Point(122, 40);
            this.txtPhysDelay.Name = "txtPhysDelay";
            this.txtPhysDelay.Size = new System.Drawing.Size(121, 20);
            this.txtPhysDelay.TabIndex = 19;
            // 
            // selectBlockPhysics
            // 
            this.selectBlockPhysics.FormattingEnabled = true;
            this.selectBlockPhysics.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectBlockPhysics.Location = new System.Drawing.Point(122, 13);
            this.selectBlockPhysics.Name = "selectBlockPhysics";
            this.selectBlockPhysics.Size = new System.Drawing.Size(121, 21);
            this.selectBlockPhysics.TabIndex = 18;
            this.selectBlockPhysics.Text = "No";
            // 
            // lblBlockPhysMapload
            // 
            this.lblBlockPhysMapload.AutoSize = true;
            this.lblBlockPhysMapload.Location = new System.Drawing.Point(9, 148);
            this.lblBlockPhysMapload.Name = "lblBlockPhysMapload";
            this.lblBlockPhysMapload.Size = new System.Drawing.Size(107, 13);
            this.lblBlockPhysMapload.TabIndex = 13;
            this.lblBlockPhysMapload.Text = "Physics on map load:";
            this.lblBlockPhysMapload.Visible = false;
            // 
            // lblBlockPhysScript
            // 
            this.lblBlockPhysScript.AutoSize = true;
            this.lblBlockPhysScript.Location = new System.Drawing.Point(40, 122);
            this.lblBlockPhysScript.Name = "lblBlockPhysScript";
            this.lblBlockPhysScript.Size = new System.Drawing.Size(76, 13);
            this.lblBlockPhysScript.TabIndex = 12;
            this.lblBlockPhysScript.Text = "Physics Script:";
            // 
            // lblBlockRepPhys
            // 
            this.lblBlockRepPhys.AutoSize = true;
            this.lblBlockRepPhys.Location = new System.Drawing.Point(32, 95);
            this.lblBlockRepPhys.Name = "lblBlockRepPhys";
            this.lblBlockRepPhys.Size = new System.Drawing.Size(84, 13);
            this.lblBlockRepPhys.TabIndex = 11;
            this.lblBlockRepPhys.Text = "Repeat Physics:";
            // 
            // lblBlockPhysRandom
            // 
            this.lblBlockPhysRandom.AutoSize = true;
            this.lblBlockPhysRandom.Location = new System.Drawing.Point(27, 69);
            this.lblBlockPhysRandom.Name = "lblBlockPhysRandom";
            this.lblBlockPhysRandom.Size = new System.Drawing.Size(89, 13);
            this.lblBlockPhysRandom.TabIndex = 10;
            this.lblBlockPhysRandom.Text = "Physics Random:";
            // 
            // lblBlockPhysDelay
            // 
            this.lblBlockPhysDelay.AutoSize = true;
            this.lblBlockPhysDelay.Location = new System.Drawing.Point(40, 43);
            this.lblBlockPhysDelay.Name = "lblBlockPhysDelay";
            this.lblBlockPhysDelay.Size = new System.Drawing.Size(76, 13);
            this.lblBlockPhysDelay.TabIndex = 9;
            this.lblBlockPhysDelay.Text = "Physics Delay:";
            // 
            // lblBlockPhysics
            // 
            this.lblBlockPhysics.AutoSize = true;
            this.lblBlockPhysics.Location = new System.Drawing.Point(70, 16);
            this.lblBlockPhysics.Name = "lblBlockPhysics";
            this.lblBlockPhysics.Size = new System.Drawing.Size(46, 13);
            this.lblBlockPhysics.TabIndex = 8;
            this.lblBlockPhysics.Text = "Physics:";
            // 
            // lblBlockSpecial
            // 
            this.lblBlockSpecial.AutoSize = true;
            this.lblBlockSpecial.Location = new System.Drawing.Point(147, 137);
            this.lblBlockSpecial.Name = "lblBlockSpecial";
            this.lblBlockSpecial.Size = new System.Drawing.Size(45, 13);
            this.lblBlockSpecial.TabIndex = 6;
            this.lblBlockSpecial.Text = "Special:";
            // 
            // lblBlockKills
            // 
            this.lblBlockKills.AutoSize = true;
            this.lblBlockKills.Location = new System.Drawing.Point(165, 110);
            this.lblBlockKills.Name = "lblBlockKills";
            this.lblBlockKills.Size = new System.Drawing.Size(28, 13);
            this.lblBlockKills.TabIndex = 5;
            this.lblBlockKills.Text = "Kills:";
            // 
            // lblBlockColor
            // 
            this.lblBlockColor.AutoSize = true;
            this.lblBlockColor.Location = new System.Drawing.Point(159, 86);
            this.lblBlockColor.Name = "lblBlockColor";
            this.lblBlockColor.Size = new System.Drawing.Size(34, 13);
            this.lblBlockColor.TabIndex = 4;
            this.lblBlockColor.Text = "Color:";
            // 
            // lblBlockCId
            // 
            this.lblBlockCId.AutoSize = true;
            this.lblBlockCId.Location = new System.Drawing.Point(128, 57);
            this.lblBlockCId.Name = "lblBlockCId";
            this.lblBlockCId.Size = new System.Drawing.Size(65, 13);
            this.lblBlockCId.TabIndex = 3;
            this.lblBlockCId.Text = "ID on Client:";
            // 
            // lblBlockName
            // 
            this.lblBlockName.AutoSize = true;
            this.lblBlockName.Location = new System.Drawing.Point(155, 32);
            this.lblBlockName.Name = "lblBlockName";
            this.lblBlockName.Size = new System.Drawing.Size(38, 13);
            this.lblBlockName.TabIndex = 2;
            this.lblBlockName.Text = "Name:";
            // 
            // lblBlockId
            // 
            this.lblBlockId.AutoSize = true;
            this.lblBlockId.Location = new System.Drawing.Point(174, 5);
            this.lblBlockId.Name = "lblBlockId";
            this.lblBlockId.Size = new System.Drawing.Size(19, 13);
            this.lblBlockId.TabIndex = 1;
            this.lblBlockId.Text = "Id:";
            // 
            // lstBlocks
            // 
            this.lstBlocks.FormattingEnabled = true;
            this.lstBlocks.Location = new System.Drawing.Point(3, 3);
            this.lstBlocks.Name = "lstBlocks";
            this.lstBlocks.Size = new System.Drawing.Size(120, 264);
            this.lstBlocks.TabIndex = 0;
            this.lstBlocks.SelectedIndexChanged += new System.EventHandler(this.lstBlocks_SelectedIndexChanged);
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
            // MainForm
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
            ((System.ComponentModel.ISupportInitialize)(this.picBlockColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlockCId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlockId)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.Button btnMapDefault;
        private System.Windows.Forms.Button btnMapClearQueue;
        private System.Windows.Forms.Button btnMapResend;
        private System.Windows.Forms.Button btnMapBuilding;
        private System.Windows.Forms.Button btnMapHistoryOn;
        private System.Windows.Forms.Button btnMapPhysics;
        private System.Windows.Forms.Button btnMapFill;
        private System.Windows.Forms.Button btnMapResize;
        private System.Windows.Forms.Label lblMapSpawn;
        private System.Windows.Forms.Label lblMapGen;
        private System.Windows.Forms.Label lblMapBPerms;
        private System.Windows.Forms.Label lblMapSPerms;
        private System.Windows.Forms.Label lblMapJperms;
        private System.Windows.Forms.Button btnMapUpdate;
        private System.Windows.Forms.ListBox lstBlocks;
        private System.Windows.Forms.Label lblBlockName;
        private System.Windows.Forms.Label lblBlockId;
        private System.Windows.Forms.Label lblBlockSpecial;
        private System.Windows.Forms.Label lblBlockKills;
        private System.Windows.Forms.Label lblBlockColor;
        private System.Windows.Forms.Label lblBlockCId;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSaveBlocks;
        private System.Windows.Forms.Button btnDeleteBlock;
        private System.Windows.Forms.Button btnCreateBlock;
        private System.Windows.Forms.Label lblBlockPhysMapload;
        private System.Windows.Forms.Label lblBlockPhysScript;
        private System.Windows.Forms.Label lblBlockRepPhys;
        private System.Windows.Forms.Label lblBlockPhysRandom;
        private System.Windows.Forms.Label lblBlockPhysDelay;
        private System.Windows.Forms.Label lblBlockPhysics;
        private System.Windows.Forms.NumericUpDown numBlockCId;
        private System.Windows.Forms.TextBox txtBlockName;
        private System.Windows.Forms.NumericUpDown numBlockId;
        private System.Windows.Forms.ComboBox selectBlockSpecial;
        private System.Windows.Forms.ComboBox selectBlockKills;
        private System.Windows.Forms.PictureBox picBlockColor;
        private System.Windows.Forms.ComboBox selectPhysMapLoad;
        private System.Windows.Forms.TextBox txtPhysScript;
        private System.Windows.Forms.ComboBox selectRepPhysics;
        private System.Windows.Forms.TextBox txtPhysRandom;
        private System.Windows.Forms.TextBox txtPhysDelay;
        private System.Windows.Forms.ComboBox selectBlockPhysics;
        private System.Windows.Forms.Button btnReloadBlocks;
    }
}

