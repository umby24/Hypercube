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
            this.btnSaveSettings = new System.Windows.Forms.Button();
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
            this.btnReloadCmd = new System.Windows.Forms.Button();
            this.btnSaveCmd = new System.Windows.Forms.Button();
            this.btnDeleteCmd = new System.Windows.Forms.Button();
            this.btnAddCommand = new System.Windows.Forms.Button();
            this.lblCmdHelp = new System.Windows.Forms.Label();
            this.lblCmdPlugin = new System.Windows.Forms.Label();
            this.lblCmdGrp = new System.Windows.Forms.Label();
            this.lblCmdName = new System.Windows.Forms.Label();
            this.chkCmdConsole = new System.Windows.Forms.CheckBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkCmdAllPerms = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstCmdShowPerms = new System.Windows.Forms.ListBox();
            this.lstCmdUsePerms = new System.Windows.Forms.ListBox();
            this.lstCommands = new System.Windows.Forms.ListBox();
            this.blocksTab = new System.Windows.Forms.TabPage();
            this.selectCPELevel = new System.Windows.Forms.ComboBox();
            this.selectCPEReplace = new System.Windows.Forms.ComboBox();
            this.lblBlockCPEReplace = new System.Windows.Forms.Label();
            this.lblBlockCPELevel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblblkpermDelete = new System.Windows.Forms.Label();
            this.lblblkpermPlace = new System.Windows.Forms.Label();
            this.lstDeletePerms = new System.Windows.Forms.ListBox();
            this.lstPlacePerms = new System.Windows.Forms.ListBox();
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
            this.chkBlockDisable = new System.Windows.Forms.CheckBox();
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
            this.commandsTab.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.blocksTab.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.mainTabs.Location = new System.Drawing.Point(0, 28);
            this.mainTabs.Margin = new System.Windows.Forms.Padding(4);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.Size = new System.Drawing.Size(1319, 376);
            this.mainTabs.TabIndex = 0;
            this.mainTabs.SelectedIndexChanged += new System.EventHandler(this.mainTabs_SelectedIndexChanged);
            // 
            // consoleTab
            // 
            this.consoleTab.Controls.Add(this.consoletabs);
            this.consoleTab.Location = new System.Drawing.Point(4, 25);
            this.consoleTab.Margin = new System.Windows.Forms.Padding(4);
            this.consoleTab.Name = "consoleTab";
            this.consoleTab.Padding = new System.Windows.Forms.Padding(4);
            this.consoleTab.Size = new System.Drawing.Size(1311, 347);
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
            this.consoletabs.Location = new System.Drawing.Point(4, 4);
            this.consoletabs.Margin = new System.Windows.Forms.Padding(4);
            this.consoletabs.Name = "consoletabs";
            this.consoletabs.SelectedIndex = 0;
            this.consoletabs.Size = new System.Drawing.Size(1303, 339);
            this.consoletabs.TabIndex = 2;
            // 
            // Alltab
            // 
            this.Alltab.Controls.Add(this.txtAllConsole);
            this.Alltab.Location = new System.Drawing.Point(4, 25);
            this.Alltab.Margin = new System.Windows.Forms.Padding(4);
            this.Alltab.Name = "Alltab";
            this.Alltab.Padding = new System.Windows.Forms.Padding(4);
            this.Alltab.Size = new System.Drawing.Size(1295, 310);
            this.Alltab.TabIndex = 0;
            this.Alltab.Text = "All";
            this.Alltab.UseVisualStyleBackColor = true;
            // 
            // txtAllConsole
            // 
            this.txtAllConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAllConsole.Location = new System.Drawing.Point(4, 4);
            this.txtAllConsole.Margin = new System.Windows.Forms.Padding(4);
            this.txtAllConsole.Multiline = true;
            this.txtAllConsole.Name = "txtAllConsole";
            this.txtAllConsole.ReadOnly = true;
            this.txtAllConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAllConsole.Size = new System.Drawing.Size(1287, 302);
            this.txtAllConsole.TabIndex = 0;
            // 
            // infotab
            // 
            this.infotab.Controls.Add(this.txtInfobox);
            this.infotab.Location = new System.Drawing.Point(4, 25);
            this.infotab.Margin = new System.Windows.Forms.Padding(4);
            this.infotab.Name = "infotab";
            this.infotab.Padding = new System.Windows.Forms.Padding(4);
            this.infotab.Size = new System.Drawing.Size(1295, 310);
            this.infotab.TabIndex = 1;
            this.infotab.Text = "Info";
            this.infotab.UseVisualStyleBackColor = true;
            // 
            // txtInfobox
            // 
            this.txtInfobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfobox.Location = new System.Drawing.Point(4, 4);
            this.txtInfobox.Margin = new System.Windows.Forms.Padding(4);
            this.txtInfobox.Name = "txtInfobox";
            this.txtInfobox.ReadOnly = true;
            this.txtInfobox.Size = new System.Drawing.Size(1287, 302);
            this.txtInfobox.TabIndex = 0;
            this.txtInfobox.Text = "";
            // 
            // chatTab
            // 
            this.chatTab.Controls.Add(this.txtChatbox);
            this.chatTab.Controls.Add(this.btnSendChat);
            this.chatTab.Controls.Add(this.txtChat);
            this.chatTab.Location = new System.Drawing.Point(4, 25);
            this.chatTab.Margin = new System.Windows.Forms.Padding(4);
            this.chatTab.Name = "chatTab";
            this.chatTab.Size = new System.Drawing.Size(1295, 310);
            this.chatTab.TabIndex = 2;
            this.chatTab.Text = "Chat";
            this.chatTab.UseVisualStyleBackColor = true;
            // 
            // txtChatbox
            // 
            this.txtChatbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChatbox.Location = new System.Drawing.Point(0, 0);
            this.txtChatbox.Margin = new System.Windows.Forms.Padding(4);
            this.txtChatbox.Name = "txtChatbox";
            this.txtChatbox.ReadOnly = true;
            this.txtChatbox.Size = new System.Drawing.Size(1287, 262);
            this.txtChatbox.TabIndex = 2;
            this.txtChatbox.Text = "";
            // 
            // btnSendChat
            // 
            this.btnSendChat.Location = new System.Drawing.Point(1177, 271);
            this.btnSendChat.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendChat.Name = "btnSendChat";
            this.btnSendChat.Size = new System.Drawing.Size(100, 28);
            this.btnSendChat.TabIndex = 1;
            this.btnSendChat.Text = "Send";
            this.btnSendChat.UseVisualStyleBackColor = true;
            this.btnSendChat.Click += new System.EventHandler(this.btnSendChat_Click);
            // 
            // txtChat
            // 
            this.txtChat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChat.Location = new System.Drawing.Point(4, 275);
            this.txtChat.Margin = new System.Windows.Forms.Padding(4);
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(1167, 22);
            this.txtChat.TabIndex = 0;
            // 
            // cTab
            // 
            this.cTab.Controls.Add(this.txtCommandbox);
            this.cTab.Location = new System.Drawing.Point(4, 25);
            this.cTab.Margin = new System.Windows.Forms.Padding(4);
            this.cTab.Name = "cTab";
            this.cTab.Size = new System.Drawing.Size(1295, 310);
            this.cTab.TabIndex = 3;
            this.cTab.Text = "Commands";
            this.cTab.UseVisualStyleBackColor = true;
            // 
            // txtCommandbox
            // 
            this.txtCommandbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommandbox.Location = new System.Drawing.Point(0, 0);
            this.txtCommandbox.Margin = new System.Windows.Forms.Padding(4);
            this.txtCommandbox.Name = "txtCommandbox";
            this.txtCommandbox.ReadOnly = true;
            this.txtCommandbox.Size = new System.Drawing.Size(1295, 310);
            this.txtCommandbox.TabIndex = 0;
            this.txtCommandbox.Text = "";
            // 
            // ErrorsTab
            // 
            this.ErrorsTab.Controls.Add(this.txtErrorbox);
            this.ErrorsTab.Location = new System.Drawing.Point(4, 25);
            this.ErrorsTab.Margin = new System.Windows.Forms.Padding(4);
            this.ErrorsTab.Name = "ErrorsTab";
            this.ErrorsTab.Size = new System.Drawing.Size(1295, 310);
            this.ErrorsTab.TabIndex = 4;
            this.ErrorsTab.Text = "Errors";
            this.ErrorsTab.UseVisualStyleBackColor = true;
            // 
            // txtErrorbox
            // 
            this.txtErrorbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorbox.Location = new System.Drawing.Point(0, 0);
            this.txtErrorbox.Margin = new System.Windows.Forms.Padding(4);
            this.txtErrorbox.Name = "txtErrorbox";
            this.txtErrorbox.ReadOnly = true;
            this.txtErrorbox.Size = new System.Drawing.Size(1295, 310);
            this.txtErrorbox.TabIndex = 0;
            this.txtErrorbox.Text = "";
            // 
            // debugTab
            // 
            this.debugTab.Controls.Add(this.txtDebugBox);
            this.debugTab.Location = new System.Drawing.Point(4, 25);
            this.debugTab.Margin = new System.Windows.Forms.Padding(4);
            this.debugTab.Name = "debugTab";
            this.debugTab.Size = new System.Drawing.Size(1295, 310);
            this.debugTab.TabIndex = 5;
            this.debugTab.Text = "Debug";
            this.debugTab.UseVisualStyleBackColor = true;
            // 
            // txtDebugBox
            // 
            this.txtDebugBox.BackColor = System.Drawing.SystemColors.Control;
            this.txtDebugBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDebugBox.Location = new System.Drawing.Point(0, 0);
            this.txtDebugBox.Margin = new System.Windows.Forms.Padding(4);
            this.txtDebugBox.Name = "txtDebugBox";
            this.txtDebugBox.ReadOnly = true;
            this.txtDebugBox.Size = new System.Drawing.Size(1295, 310);
            this.txtDebugBox.TabIndex = 0;
            this.txtDebugBox.Text = "";
            // 
            // settingsTab
            // 
            this.settingsTab.Controls.Add(this.btnSaveSettings);
            this.settingsTab.Controls.Add(this.btnRuleEdit);
            this.settingsTab.Controls.Add(this.grpSettingsChat);
            this.settingsTab.Controls.Add(this.grpNetwork);
            this.settingsTab.Controls.Add(this.grpSystem);
            this.settingsTab.Location = new System.Drawing.Point(4, 25);
            this.settingsTab.Margin = new System.Windows.Forms.Padding(4);
            this.settingsTab.Name = "settingsTab";
            this.settingsTab.Padding = new System.Windows.Forms.Padding(4);
            this.settingsTab.Size = new System.Drawing.Size(1311, 347);
            this.settingsTab.TabIndex = 1;
            this.settingsTab.Text = "Settings";
            this.settingsTab.UseVisualStyleBackColor = true;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(596, 51);
            this.btnSaveSettings.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(267, 28);
            this.btnSaveSettings.TabIndex = 4;
            this.btnSaveSettings.Text = "Save Changes";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnRuleEdit
            // 
            this.btnRuleEdit.Location = new System.Drawing.Point(596, 15);
            this.btnRuleEdit.Margin = new System.Windows.Forms.Padding(4);
            this.btnRuleEdit.Name = "btnRuleEdit";
            this.btnRuleEdit.Size = new System.Drawing.Size(267, 28);
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
            this.grpSettingsChat.Location = new System.Drawing.Point(321, 151);
            this.grpSettingsChat.Margin = new System.Windows.Forms.Padding(4);
            this.grpSettingsChat.Name = "grpSettingsChat";
            this.grpSettingsChat.Padding = new System.Windows.Forms.Padding(4);
            this.grpSettingsChat.Size = new System.Drawing.Size(267, 167);
            this.grpSettingsChat.TabIndex = 2;
            this.grpSettingsChat.TabStop = false;
            this.grpSettingsChat.Text = "Chat";
            // 
            // txtChatDivider
            // 
            this.txtChatDivider.Location = new System.Drawing.Point(77, 48);
            this.txtChatDivider.Margin = new System.Windows.Forms.Padding(4);
            this.txtChatDivider.Name = "txtChatDivider";
            this.txtChatDivider.Size = new System.Drawing.Size(179, 22);
            this.txtChatDivider.TabIndex = 7;
            // 
            // lblDivider
            // 
            this.lblDivider.AutoSize = true;
            this.lblDivider.Location = new System.Drawing.Point(12, 52);
            this.lblDivider.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDivider.Name = "lblDivider";
            this.lblDivider.Size = new System.Drawing.Size(56, 17);
            this.lblDivider.TabIndex = 6;
            this.lblDivider.Text = "Divider:";
            // 
            // txtPlayerlist
            // 
            this.txtPlayerlist.Location = new System.Drawing.Point(77, 112);
            this.txtPlayerlist.Margin = new System.Windows.Forms.Padding(4);
            this.txtPlayerlist.Name = "txtPlayerlist";
            this.txtPlayerlist.Size = new System.Drawing.Size(179, 22);
            this.txtPlayerlist.TabIndex = 5;
            // 
            // lblPlayerList
            // 
            this.lblPlayerList.AutoSize = true;
            this.lblPlayerList.Location = new System.Drawing.Point(0, 116);
            this.lblPlayerList.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPlayerList.Name = "lblPlayerList";
            this.lblPlayerList.Size = new System.Drawing.Size(70, 17);
            this.lblPlayerList.TabIndex = 4;
            this.lblPlayerList.Text = "PlayerList";
            // 
            // lblSystem
            // 
            this.lblSystem.AutoSize = true;
            this.lblSystem.Location = new System.Drawing.Point(11, 84);
            this.lblSystem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSystem.Name = "lblSystem";
            this.lblSystem.Size = new System.Drawing.Size(58, 17);
            this.lblSystem.TabIndex = 3;
            this.lblSystem.Text = "System:";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(27, 20);
            this.lblError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(44, 17);
            this.lblError.TabIndex = 2;
            this.lblError.Text = "Error:";
            // 
            // txtChatSystem
            // 
            this.txtChatSystem.Location = new System.Drawing.Point(77, 80);
            this.txtChatSystem.Margin = new System.Windows.Forms.Padding(4);
            this.txtChatSystem.Name = "txtChatSystem";
            this.txtChatSystem.Size = new System.Drawing.Size(179, 22);
            this.txtChatSystem.TabIndex = 1;
            // 
            // txtChatError
            // 
            this.txtChatError.Location = new System.Drawing.Point(77, 16);
            this.txtChatError.Margin = new System.Windows.Forms.Padding(4);
            this.txtChatError.Name = "txtChatError";
            this.txtChatError.Size = new System.Drawing.Size(179, 22);
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
            this.grpNetwork.Location = new System.Drawing.Point(321, 7);
            this.grpNetwork.Margin = new System.Windows.Forms.Padding(4);
            this.grpNetwork.Name = "grpNetwork";
            this.grpNetwork.Padding = new System.Windows.Forms.Padding(4);
            this.grpNetwork.Size = new System.Drawing.Size(267, 137);
            this.grpNetwork.TabIndex = 1;
            this.grpNetwork.TabStop = false;
            this.grpNetwork.Text = "Network";
            // 
            // numMaxPlayers
            // 
            this.numMaxPlayers.Location = new System.Drawing.Point(101, 49);
            this.numMaxPlayers.Margin = new System.Windows.Forms.Padding(4);
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
            this.numMaxPlayers.Size = new System.Drawing.Size(160, 22);
            this.numMaxPlayers.TabIndex = 5;
            this.numMaxPlayers.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(101, 17);
            this.numPort.Margin = new System.Windows.Forms.Padding(4);
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
            this.numPort.Size = new System.Drawing.Size(160, 22);
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
            this.chkPub.Location = new System.Drawing.Point(185, 110);
            this.chkPub.Margin = new System.Windows.Forms.Padding(4);
            this.chkPub.Name = "chkPub";
            this.chkPub.Size = new System.Drawing.Size(68, 21);
            this.chkPub.TabIndex = 3;
            this.chkPub.Text = "Public";
            this.chkPub.UseVisualStyleBackColor = true;
            // 
            // chkVerifyNames
            // 
            this.chkVerifyNames.AutoSize = true;
            this.chkVerifyNames.Location = new System.Drawing.Point(141, 81);
            this.chkVerifyNames.Margin = new System.Windows.Forms.Padding(4);
            this.chkVerifyNames.Name = "chkVerifyNames";
            this.chkVerifyNames.Size = new System.Drawing.Size(114, 21);
            this.chkVerifyNames.TabIndex = 2;
            this.chkVerifyNames.Text = "Verify Names";
            this.chkVerifyNames.UseVisualStyleBackColor = true;
            // 
            // lblMaxPlayers
            // 
            this.lblMaxPlayers.AutoSize = true;
            this.lblMaxPlayers.Location = new System.Drawing.Point(8, 52);
            this.lblMaxPlayers.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMaxPlayers.Name = "lblMaxPlayers";
            this.lblMaxPlayers.Size = new System.Drawing.Size(84, 17);
            this.lblMaxPlayers.TabIndex = 1;
            this.lblMaxPlayers.Text = "Max Players";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(59, 20);
            this.lblPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(34, 17);
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
            this.grpSystem.Location = new System.Drawing.Point(11, 7);
            this.grpSystem.Margin = new System.Windows.Forms.Padding(4);
            this.grpSystem.Name = "grpSystem";
            this.grpSystem.Padding = new System.Windows.Forms.Padding(4);
            this.grpSystem.Size = new System.Drawing.Size(303, 325);
            this.grpSystem.TabIndex = 0;
            this.grpSystem.TabStop = false;
            this.grpSystem.Text = "System";
            // 
            // lblHistory
            // 
            this.lblHistory.AutoSize = true;
            this.lblHistory.Location = new System.Drawing.Point(29, 289);
            this.lblHistory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHistory.Name = "lblHistory";
            this.lblHistory.Size = new System.Drawing.Size(126, 17);
            this.lblHistory.TabIndex = 13;
            this.lblHistory.Text = "Max history entries";
            // 
            // numHistory
            // 
            this.numHistory.Location = new System.Drawing.Point(163, 287);
            this.numHistory.Margin = new System.Windows.Forms.Padding(4);
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
            this.numHistory.Size = new System.Drawing.Size(132, 22);
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
            this.lblBlockChn.Location = new System.Drawing.Point(4, 257);
            this.lblBlockChn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockChn.Name = "lblBlockChn";
            this.lblBlockChn.Size = new System.Drawing.Size(143, 17);
            this.lblBlockChn.TabIndex = 11;
            this.lblBlockChn.Text = "Max block changes /s";
            // 
            // numMaxBlocks
            // 
            this.numMaxBlocks.Location = new System.Drawing.Point(163, 255);
            this.numMaxBlocks.Margin = new System.Windows.Forms.Padding(4);
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
            this.numMaxBlocks.Size = new System.Drawing.Size(132, 22);
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
            this.chkComArgs.Location = new System.Drawing.Point(8, 226);
            this.chkComArgs.Margin = new System.Windows.Forms.Padding(4);
            this.chkComArgs.Name = "chkComArgs";
            this.chkComArgs.Size = new System.Drawing.Size(200, 21);
            this.chkComArgs.TabIndex = 9;
            this.chkComArgs.Text = "Show command arguments";
            this.chkComArgs.UseVisualStyleBackColor = true;
            // 
            // chkMaphistory
            // 
            this.chkMaphistory.AutoSize = true;
            this.chkMaphistory.Location = new System.Drawing.Point(8, 198);
            this.chkMaphistory.Margin = new System.Windows.Forms.Padding(4);
            this.chkMaphistory.Name = "chkMaphistory";
            this.chkMaphistory.Size = new System.Drawing.Size(166, 21);
            this.chkMaphistory.TabIndex = 8;
            this.chkMaphistory.Text = "Compress Maphistory";
            this.chkMaphistory.UseVisualStyleBackColor = true;
            // 
            // chkLogs
            // 
            this.chkLogs.AutoSize = true;
            this.chkLogs.Location = new System.Drawing.Point(123, 170);
            this.chkLogs.Margin = new System.Windows.Forms.Padding(4);
            this.chkLogs.Name = "chkLogs";
            this.chkLogs.Size = new System.Drawing.Size(92, 21);
            this.chkLogs.TabIndex = 7;
            this.chkLogs.Text = "Log to file";
            this.chkLogs.UseVisualStyleBackColor = true;
            // 
            // chkRotLogs
            // 
            this.chkRotLogs.AutoSize = true;
            this.chkRotLogs.Location = new System.Drawing.Point(8, 170);
            this.chkRotLogs.Margin = new System.Windows.Forms.Padding(4);
            this.chkRotLogs.Name = "chkRotLogs";
            this.chkRotLogs.Size = new System.Drawing.Size(102, 21);
            this.chkRotLogs.TabIndex = 6;
            this.chkRotLogs.Text = "Rotate logs";
            this.chkRotLogs.UseVisualStyleBackColor = true;
            // 
            // txtWelcomeMess
            // 
            this.txtWelcomeMess.Location = new System.Drawing.Point(8, 138);
            this.txtWelcomeMess.Margin = new System.Windows.Forms.Padding(4);
            this.txtWelcomeMess.Name = "txtWelcomeMess";
            this.txtWelcomeMess.Size = new System.Drawing.Size(285, 22);
            this.txtWelcomeMess.TabIndex = 5;
            // 
            // lblWelcmess
            // 
            this.lblWelcmess.AutoSize = true;
            this.lblWelcmess.Location = new System.Drawing.Point(4, 116);
            this.lblWelcmess.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWelcmess.Name = "lblWelcmess";
            this.lblWelcmess.Size = new System.Drawing.Size(131, 17);
            this.lblWelcmess.TabIndex = 4;
            this.lblWelcmess.Text = "Welcome message:";
            // 
            // txtSrvMotd
            // 
            this.txtSrvMotd.Location = new System.Drawing.Point(8, 87);
            this.txtSrvMotd.Margin = new System.Windows.Forms.Padding(4);
            this.txtSrvMotd.Name = "txtSrvMotd";
            this.txtSrvMotd.Size = new System.Drawing.Size(285, 22);
            this.txtSrvMotd.TabIndex = 3;
            // 
            // lblSrvMotd
            // 
            this.lblSrvMotd.AutoSize = true;
            this.lblSrvMotd.Location = new System.Drawing.Point(4, 68);
            this.lblSrvMotd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSrvMotd.Name = "lblSrvMotd";
            this.lblSrvMotd.Size = new System.Drawing.Size(99, 17);
            this.lblSrvMotd.TabIndex = 2;
            this.lblSrvMotd.Text = "Server MOTD:";
            // 
            // txtSrvName
            // 
            this.txtSrvName.Location = new System.Drawing.Point(8, 39);
            this.txtSrvName.Margin = new System.Windows.Forms.Padding(4);
            this.txtSrvName.Name = "txtSrvName";
            this.txtSrvName.Size = new System.Drawing.Size(285, 22);
            this.txtSrvName.TabIndex = 1;
            // 
            // lblSrvName
            // 
            this.lblSrvName.AutoSize = true;
            this.lblSrvName.Location = new System.Drawing.Point(4, 20);
            this.lblSrvName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSrvName.Name = "lblSrvName";
            this.lblSrvName.Size = new System.Drawing.Size(93, 17);
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
            this.mapsTab.Location = new System.Drawing.Point(4, 25);
            this.mapsTab.Margin = new System.Windows.Forms.Padding(4);
            this.mapsTab.Name = "mapsTab";
            this.mapsTab.Size = new System.Drawing.Size(1311, 347);
            this.mapsTab.TabIndex = 2;
            this.mapsTab.Text = "Maps";
            this.mapsTab.UseVisualStyleBackColor = true;
            // 
            // lblMapBPerms
            // 
            this.lblMapBPerms.AutoSize = true;
            this.lblMapBPerms.Location = new System.Drawing.Point(531, 63);
            this.lblMapBPerms.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapBPerms.Name = "lblMapBPerms";
            this.lblMapBPerms.Size = new System.Drawing.Size(119, 17);
            this.lblMapBPerms.TabIndex = 17;
            this.lblMapBPerms.Text = "Build Permissions";
            // 
            // lblMapSPerms
            // 
            this.lblMapSPerms.AutoSize = true;
            this.lblMapSPerms.Location = new System.Drawing.Point(363, 63);
            this.lblMapSPerms.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapSPerms.Name = "lblMapSPerms";
            this.lblMapSPerms.Size = new System.Drawing.Size(122, 17);
            this.lblMapSPerms.TabIndex = 16;
            this.lblMapSPerms.Text = "Show Permissions";
            // 
            // lblMapJperms
            // 
            this.lblMapJperms.AutoSize = true;
            this.lblMapJperms.Location = new System.Drawing.Point(191, 63);
            this.lblMapJperms.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapJperms.Name = "lblMapJperms";
            this.lblMapJperms.Size = new System.Drawing.Size(114, 17);
            this.lblMapJperms.TabIndex = 15;
            this.lblMapJperms.Text = "Join Permissions";
            // 
            // btnMapUpdate
            // 
            this.btnMapUpdate.Location = new System.Drawing.Point(596, 17);
            this.btnMapUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapUpdate.Name = "btnMapUpdate";
            this.btnMapUpdate.Size = new System.Drawing.Size(100, 28);
            this.btnMapUpdate.TabIndex = 14;
            this.btnMapUpdate.Text = "Update";
            this.btnMapUpdate.UseVisualStyleBackColor = true;
            this.btnMapUpdate.Click += new System.EventHandler(this.btnMapUpdate_Click);
            // 
            // lstBuildPerms
            // 
            this.lstBuildPerms.FormattingEnabled = true;
            this.lstBuildPerms.ItemHeight = 16;
            this.lstBuildPerms.Location = new System.Drawing.Point(531, 82);
            this.lstBuildPerms.Margin = new System.Windows.Forms.Padding(4);
            this.lstBuildPerms.Name = "lstBuildPerms";
            this.lstBuildPerms.Size = new System.Drawing.Size(159, 244);
            this.lstBuildPerms.TabIndex = 13;
            // 
            // lstShowPerms
            // 
            this.lstShowPerms.FormattingEnabled = true;
            this.lstShowPerms.ItemHeight = 16;
            this.lstShowPerms.Location = new System.Drawing.Point(363, 82);
            this.lstShowPerms.Margin = new System.Windows.Forms.Padding(4);
            this.lstShowPerms.Name = "lstShowPerms";
            this.lstShowPerms.Size = new System.Drawing.Size(159, 244);
            this.lstShowPerms.TabIndex = 12;
            // 
            // lstJoinPerms
            // 
            this.lstJoinPerms.FormattingEnabled = true;
            this.lstJoinPerms.ItemHeight = 16;
            this.lstJoinPerms.Location = new System.Drawing.Point(195, 82);
            this.lstJoinPerms.Margin = new System.Windows.Forms.Padding(4);
            this.lstJoinPerms.Name = "lstJoinPerms";
            this.lstJoinPerms.Size = new System.Drawing.Size(159, 244);
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
            this.grpMapControl.Location = new System.Drawing.Point(1011, 160);
            this.grpMapControl.Margin = new System.Windows.Forms.Padding(4);
            this.grpMapControl.Name = "grpMapControl";
            this.grpMapControl.Padding = new System.Windows.Forms.Padding(4);
            this.grpMapControl.Size = new System.Drawing.Size(287, 178);
            this.grpMapControl.TabIndex = 10;
            this.grpMapControl.TabStop = false;
            this.grpMapControl.Text = "Control";
            // 
            // btnMapDefault
            // 
            this.btnMapDefault.Location = new System.Drawing.Point(140, 130);
            this.btnMapDefault.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapDefault.Name = "btnMapDefault";
            this.btnMapDefault.Size = new System.Drawing.Size(112, 28);
            this.btnMapDefault.TabIndex = 18;
            this.btnMapDefault.Text = "Make Default";
            this.btnMapDefault.UseVisualStyleBackColor = true;
            this.btnMapDefault.Click += new System.EventHandler(this.btnMapDefault_Click);
            // 
            // btnMapClearQueue
            // 
            this.btnMapClearQueue.Location = new System.Drawing.Point(140, 95);
            this.btnMapClearQueue.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapClearQueue.Name = "btnMapClearQueue";
            this.btnMapClearQueue.Size = new System.Drawing.Size(112, 28);
            this.btnMapClearQueue.TabIndex = 17;
            this.btnMapClearQueue.Text = "Clear Queue";
            this.btnMapClearQueue.UseVisualStyleBackColor = true;
            this.btnMapClearQueue.Click += new System.EventHandler(this.btnMapClearQueue_Click);
            // 
            // btnMapResend
            // 
            this.btnMapResend.Location = new System.Drawing.Point(140, 23);
            this.btnMapResend.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapResend.Name = "btnMapResend";
            this.btnMapResend.Size = new System.Drawing.Size(112, 28);
            this.btnMapResend.TabIndex = 16;
            this.btnMapResend.Text = "Resend";
            this.btnMapResend.UseVisualStyleBackColor = true;
            this.btnMapResend.Click += new System.EventHandler(this.btnMapResend_Click);
            // 
            // btnMapBuilding
            // 
            this.btnMapBuilding.Location = new System.Drawing.Point(12, 95);
            this.btnMapBuilding.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapBuilding.Name = "btnMapBuilding";
            this.btnMapBuilding.Size = new System.Drawing.Size(116, 28);
            this.btnMapBuilding.TabIndex = 15;
            this.btnMapBuilding.Text = "Building";
            this.btnMapBuilding.UseVisualStyleBackColor = true;
            this.btnMapBuilding.Click += new System.EventHandler(this.btnMapBuilding_Click);
            // 
            // btnMapHistoryOn
            // 
            this.btnMapHistoryOn.Location = new System.Drawing.Point(140, 59);
            this.btnMapHistoryOn.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapHistoryOn.Name = "btnMapHistoryOn";
            this.btnMapHistoryOn.Size = new System.Drawing.Size(112, 28);
            this.btnMapHistoryOn.TabIndex = 14;
            this.btnMapHistoryOn.Text = "History";
            this.btnMapHistoryOn.UseVisualStyleBackColor = true;
            this.btnMapHistoryOn.Click += new System.EventHandler(this.btnMapHistoryOn_Click);
            // 
            // btnMapPhysics
            // 
            this.btnMapPhysics.Location = new System.Drawing.Point(12, 130);
            this.btnMapPhysics.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapPhysics.Name = "btnMapPhysics";
            this.btnMapPhysics.Size = new System.Drawing.Size(120, 28);
            this.btnMapPhysics.TabIndex = 13;
            this.btnMapPhysics.Text = "Physics";
            this.btnMapPhysics.UseVisualStyleBackColor = true;
            this.btnMapPhysics.Click += new System.EventHandler(this.btnMapPhysics_Click);
            // 
            // btnMapFill
            // 
            this.btnMapFill.Location = new System.Drawing.Point(12, 59);
            this.btnMapFill.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapFill.Name = "btnMapFill";
            this.btnMapFill.Size = new System.Drawing.Size(116, 28);
            this.btnMapFill.TabIndex = 12;
            this.btnMapFill.Text = "Fill";
            this.btnMapFill.UseVisualStyleBackColor = true;
            this.btnMapFill.Click += new System.EventHandler(this.btnMapFill_Click);
            // 
            // btnMapResize
            // 
            this.btnMapResize.Location = new System.Drawing.Point(12, 23);
            this.btnMapResize.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapResize.Name = "btnMapResize";
            this.btnMapResize.Size = new System.Drawing.Size(116, 28);
            this.btnMapResize.TabIndex = 11;
            this.btnMapResize.Text = "Resize";
            this.btnMapResize.UseVisualStyleBackColor = true;
            this.btnMapResize.Click += new System.EventHandler(this.btnMapResize_Click);
            // 
            // txtMapMotd
            // 
            this.txtMapMotd.Location = new System.Drawing.Point(455, 20);
            this.txtMapMotd.Margin = new System.Windows.Forms.Padding(4);
            this.txtMapMotd.Name = "txtMapMotd";
            this.txtMapMotd.Size = new System.Drawing.Size(132, 22);
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
            this.groupBox1.Location = new System.Drawing.Point(1011, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(287, 149);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // lblMapPhysqueue
            // 
            this.lblMapPhysqueue.AutoSize = true;
            this.lblMapPhysqueue.Location = new System.Drawing.Point(8, 118);
            this.lblMapPhysqueue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapPhysqueue.Name = "lblMapPhysqueue";
            this.lblMapPhysqueue.Size = new System.Drawing.Size(119, 17);
            this.lblMapPhysqueue.TabIndex = 6;
            this.lblMapPhysqueue.Text = "Physics Queue: 0";
            // 
            // lblMapSpawn
            // 
            this.lblMapSpawn.AutoSize = true;
            this.lblMapSpawn.Location = new System.Drawing.Point(151, 79);
            this.lblMapSpawn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapSpawn.Name = "lblMapSpawn";
            this.lblMapSpawn.Size = new System.Drawing.Size(98, 17);
            this.lblMapSpawn.TabIndex = 9;
            this.lblMapSpawn.Text = "Spawn: 0, 0, 0";
            // 
            // lblMapBlockqueue
            // 
            this.lblMapBlockqueue.AutoSize = true;
            this.lblMapBlockqueue.Location = new System.Drawing.Point(8, 98);
            this.lblMapBlockqueue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapBlockqueue.Name = "lblMapBlockqueue";
            this.lblMapBlockqueue.Size = new System.Drawing.Size(136, 17);
            this.lblMapBlockqueue.TabIndex = 5;
            this.lblMapBlockqueue.Text = "Blocksend Queue: 0";
            // 
            // lblMapSize
            // 
            this.lblMapSize.AutoSize = true;
            this.lblMapSize.Location = new System.Drawing.Point(131, 59);
            this.lblMapSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapSize.Name = "lblMapSize";
            this.lblMapSize.Size = new System.Drawing.Size(119, 17);
            this.lblMapSize.TabIndex = 7;
            this.lblMapSize.Text = "Size: 64 x 64 x 64";
            // 
            // lblMapGen
            // 
            this.lblMapGen.AutoSize = true;
            this.lblMapGen.Location = new System.Drawing.Point(116, 39);
            this.lblMapGen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapGen.Name = "lblMapGen";
            this.lblMapGen.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblMapGen.Size = new System.Drawing.Size(139, 17);
            this.lblMapGen.TabIndex = 8;
            this.lblMapGen.Text = "Generator: Flatgrass";
            // 
            // lblMapBuilding
            // 
            this.lblMapBuilding.AutoSize = true;
            this.lblMapBuilding.Location = new System.Drawing.Point(8, 79);
            this.lblMapBuilding.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapBuilding.Name = "lblMapBuilding";
            this.lblMapBuilding.Size = new System.Drawing.Size(85, 17);
            this.lblMapBuilding.TabIndex = 4;
            this.lblMapBuilding.Text = "Building: On";
            // 
            // lblMapHist
            // 
            this.lblMapHist.AutoSize = true;
            this.lblMapHist.Location = new System.Drawing.Point(8, 59);
            this.lblMapHist.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapHist.Name = "lblMapHist";
            this.lblMapHist.Size = new System.Drawing.Size(79, 17);
            this.lblMapHist.TabIndex = 3;
            this.lblMapHist.Text = "History: On";
            // 
            // lblLoadStatus
            // 
            this.lblLoadStatus.AutoSize = true;
            this.lblLoadStatus.Location = new System.Drawing.Point(147, 20);
            this.lblLoadStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLoadStatus.Name = "lblLoadStatus";
            this.lblLoadStatus.Size = new System.Drawing.Size(104, 17);
            this.lblLoadStatus.TabIndex = 2;
            this.lblLoadStatus.Text = "Status: Loaded";
            // 
            // lblMapPhys
            // 
            this.lblMapPhys.AutoSize = true;
            this.lblMapPhys.Location = new System.Drawing.Point(8, 39);
            this.lblMapPhys.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapPhys.Name = "lblMapPhys";
            this.lblMapPhys.Size = new System.Drawing.Size(83, 17);
            this.lblMapPhys.TabIndex = 1;
            this.lblMapPhys.Text = "Physics: On";
            // 
            // lblMapClients
            // 
            this.lblMapClients.AutoSize = true;
            this.lblMapClients.Location = new System.Drawing.Point(8, 20);
            this.lblMapClients.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapClients.Name = "lblMapClients";
            this.lblMapClients.Size = new System.Drawing.Size(66, 17);
            this.lblMapClients.TabIndex = 0;
            this.lblMapClients.Text = "Clients: 0";
            // 
            // lblMapMotd
            // 
            this.lblMapMotd.AutoSize = true;
            this.lblMapMotd.Location = new System.Drawing.Point(391, 23);
            this.lblMapMotd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapMotd.Name = "lblMapMotd";
            this.lblMapMotd.Size = new System.Drawing.Size(53, 17);
            this.lblMapMotd.TabIndex = 4;
            this.lblMapMotd.Text = "MOTD:";
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(249, 20);
            this.txtMapName.Margin = new System.Windows.Forms.Padding(4);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.Size = new System.Drawing.Size(132, 22);
            this.txtMapName.TabIndex = 3;
            // 
            // lblMapName
            // 
            this.lblMapName.AutoSize = true;
            this.lblMapName.Location = new System.Drawing.Point(191, 23);
            this.lblMapName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapName.Name = "lblMapName";
            this.lblMapName.Size = new System.Drawing.Size(49, 17);
            this.lblMapName.TabIndex = 2;
            this.lblMapName.Text = "Name:";
            // 
            // lblMaps
            // 
            this.lblMaps.AutoSize = true;
            this.lblMaps.Location = new System.Drawing.Point(0, 0);
            this.lblMaps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMaps.Name = "lblMaps";
            this.lblMaps.Size = new System.Drawing.Size(46, 17);
            this.lblMaps.TabIndex = 1;
            this.lblMaps.Text = "Maps:";
            // 
            // lstMaps
            // 
            this.lstMaps.ContextMenuStrip = this.contextMapStrip;
            this.lstMaps.FormattingEnabled = true;
            this.lstMaps.ItemHeight = 16;
            this.lstMaps.Location = new System.Drawing.Point(11, 23);
            this.lstMaps.Margin = new System.Windows.Forms.Padding(4);
            this.lstMaps.Name = "lstMaps";
            this.lstMaps.Size = new System.Drawing.Size(159, 308);
            this.lstMaps.TabIndex = 0;
            this.lstMaps.SelectedIndexChanged += new System.EventHandler(this.lstMaps_SelectedIndexChanged);
            // 
            // contextMapStrip
            // 
            this.contextMapStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMapToolStripMenuItem,
            this.deleteMapToolStripMenuItem});
            this.contextMapStrip.Name = "contextMapStrip";
            this.contextMapStrip.Size = new System.Drawing.Size(157, 52);
            // 
            // addMapToolStripMenuItem
            // 
            this.addMapToolStripMenuItem.Name = "addMapToolStripMenuItem";
            this.addMapToolStripMenuItem.Size = new System.Drawing.Size(156, 24);
            this.addMapToolStripMenuItem.Text = "Add Map";
            // 
            // deleteMapToolStripMenuItem
            // 
            this.deleteMapToolStripMenuItem.Name = "deleteMapToolStripMenuItem";
            this.deleteMapToolStripMenuItem.Size = new System.Drawing.Size(156, 24);
            this.deleteMapToolStripMenuItem.Text = "Delete Map";
            // 
            // commandsTab
            // 
            this.commandsTab.Controls.Add(this.btnReloadCmd);
            this.commandsTab.Controls.Add(this.btnSaveCmd);
            this.commandsTab.Controls.Add(this.btnDeleteCmd);
            this.commandsTab.Controls.Add(this.btnAddCommand);
            this.commandsTab.Controls.Add(this.lblCmdHelp);
            this.commandsTab.Controls.Add(this.lblCmdPlugin);
            this.commandsTab.Controls.Add(this.lblCmdGrp);
            this.commandsTab.Controls.Add(this.lblCmdName);
            this.commandsTab.Controls.Add(this.chkCmdConsole);
            this.commandsTab.Controls.Add(this.textBox4);
            this.commandsTab.Controls.Add(this.textBox3);
            this.commandsTab.Controls.Add(this.textBox2);
            this.commandsTab.Controls.Add(this.textBox1);
            this.commandsTab.Controls.Add(this.groupBox3);
            this.commandsTab.Controls.Add(this.lstCommands);
            this.commandsTab.Location = new System.Drawing.Point(4, 25);
            this.commandsTab.Margin = new System.Windows.Forms.Padding(4);
            this.commandsTab.Name = "commandsTab";
            this.commandsTab.Size = new System.Drawing.Size(1311, 347);
            this.commandsTab.TabIndex = 3;
            this.commandsTab.Text = "Commands";
            this.commandsTab.UseVisualStyleBackColor = true;
            // 
            // btnReloadCmd
            // 
            this.btnReloadCmd.Location = new System.Drawing.Point(572, 311);
            this.btnReloadCmd.Name = "btnReloadCmd";
            this.btnReloadCmd.Size = new System.Drawing.Size(136, 28);
            this.btnReloadCmd.TabIndex = 33;
            this.btnReloadCmd.Text = "Reload Commands";
            this.btnReloadCmd.UseVisualStyleBackColor = true;
            // 
            // btnSaveCmd
            // 
            this.btnSaveCmd.Location = new System.Drawing.Point(430, 311);
            this.btnSaveCmd.Name = "btnSaveCmd";
            this.btnSaveCmd.Size = new System.Drawing.Size(136, 28);
            this.btnSaveCmd.TabIndex = 32;
            this.btnSaveCmd.Text = "Save Commands";
            this.btnSaveCmd.UseVisualStyleBackColor = true;
            // 
            // btnDeleteCmd
            // 
            this.btnDeleteCmd.Location = new System.Drawing.Point(288, 311);
            this.btnDeleteCmd.Name = "btnDeleteCmd";
            this.btnDeleteCmd.Size = new System.Drawing.Size(136, 28);
            this.btnDeleteCmd.TabIndex = 31;
            this.btnDeleteCmd.Text = "Delete Command";
            this.btnDeleteCmd.UseVisualStyleBackColor = true;
            // 
            // btnAddCommand
            // 
            this.btnAddCommand.Location = new System.Drawing.Point(146, 311);
            this.btnAddCommand.Name = "btnAddCommand";
            this.btnAddCommand.Size = new System.Drawing.Size(136, 28);
            this.btnAddCommand.TabIndex = 30;
            this.btnAddCommand.Text = "Add Command";
            this.btnAddCommand.UseVisualStyleBackColor = true;
            // 
            // lblCmdHelp
            // 
            this.lblCmdHelp.AutoSize = true;
            this.lblCmdHelp.Location = new System.Drawing.Point(153, 101);
            this.lblCmdHelp.Name = "lblCmdHelp";
            this.lblCmdHelp.Size = new System.Drawing.Size(41, 17);
            this.lblCmdHelp.TabIndex = 29;
            this.lblCmdHelp.Text = "Help:";
            // 
            // lblCmdPlugin
            // 
            this.lblCmdPlugin.AutoSize = true;
            this.lblCmdPlugin.Location = new System.Drawing.Point(143, 73);
            this.lblCmdPlugin.Name = "lblCmdPlugin";
            this.lblCmdPlugin.Size = new System.Drawing.Size(51, 17);
            this.lblCmdPlugin.TabIndex = 28;
            this.lblCmdPlugin.Text = "Plugin:";
            // 
            // lblCmdGrp
            // 
            this.lblCmdGrp.AutoSize = true;
            this.lblCmdGrp.Location = new System.Drawing.Point(142, 45);
            this.lblCmdGrp.Name = "lblCmdGrp";
            this.lblCmdGrp.Size = new System.Drawing.Size(52, 17);
            this.lblCmdGrp.TabIndex = 27;
            this.lblCmdGrp.Text = "Group:";
            // 
            // lblCmdName
            // 
            this.lblCmdName.AutoSize = true;
            this.lblCmdName.Location = new System.Drawing.Point(145, 15);
            this.lblCmdName.Name = "lblCmdName";
            this.lblCmdName.Size = new System.Drawing.Size(49, 17);
            this.lblCmdName.TabIndex = 26;
            this.lblCmdName.Text = "Name:";
            // 
            // chkCmdConsole
            // 
            this.chkCmdConsole.AutoSize = true;
            this.chkCmdConsole.Location = new System.Drawing.Point(200, 126);
            this.chkCmdConsole.Name = "chkCmdConsole";
            this.chkCmdConsole.Size = new System.Drawing.Size(155, 21);
            this.chkCmdConsole.TabIndex = 25;
            this.chkCmdConsole.Text = "Console Compatible";
            this.chkCmdConsole.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(200, 98);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(247, 22);
            this.textBox4.TabIndex = 24;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(200, 70);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(247, 22);
            this.textBox3.TabIndex = 23;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(200, 42);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(247, 22);
            this.textBox2.TabIndex = 22;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(200, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(247, 22);
            this.textBox1.TabIndex = 21;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkCmdAllPerms);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.lstCmdShowPerms);
            this.groupBox3.Controls.Add(this.lstCmdUsePerms);
            this.groupBox3.Location = new System.Drawing.Point(1029, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(274, 242);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Permissions";
            // 
            // chkCmdAllPerms
            // 
            this.chkCmdAllPerms.AutoSize = true;
            this.chkCmdAllPerms.Location = new System.Drawing.Point(6, 215);
            this.chkCmdAllPerms.Name = "chkCmdAllPerms";
            this.chkCmdAllPerms.Size = new System.Drawing.Size(179, 21);
            this.chkCmdAllPerms.TabIndex = 26;
            this.chkCmdAllPerms.Text = "Require All Permissions";
            this.chkCmdAllPerms.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Show";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Use";
            // 
            // lstCmdShowPerms
            // 
            this.lstCmdShowPerms.FormattingEnabled = true;
            this.lstCmdShowPerms.ItemHeight = 16;
            this.lstCmdShowPerms.Location = new System.Drawing.Point(141, 30);
            this.lstCmdShowPerms.Name = "lstCmdShowPerms";
            this.lstCmdShowPerms.Size = new System.Drawing.Size(120, 180);
            this.lstCmdShowPerms.TabIndex = 1;
            // 
            // lstCmdUsePerms
            // 
            this.lstCmdUsePerms.FormattingEnabled = true;
            this.lstCmdUsePerms.ItemHeight = 16;
            this.lstCmdUsePerms.Location = new System.Drawing.Point(6, 30);
            this.lstCmdUsePerms.Name = "lstCmdUsePerms";
            this.lstCmdUsePerms.Size = new System.Drawing.Size(120, 180);
            this.lstCmdUsePerms.TabIndex = 0;
            // 
            // lstCommands
            // 
            this.lstCommands.FormattingEnabled = true;
            this.lstCommands.ItemHeight = 16;
            this.lstCommands.Location = new System.Drawing.Point(8, 3);
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(120, 340);
            this.lstCommands.TabIndex = 0;
            // 
            // blocksTab
            // 
            this.blocksTab.Controls.Add(this.chkBlockDisable);
            this.blocksTab.Controls.Add(this.selectCPELevel);
            this.blocksTab.Controls.Add(this.selectCPEReplace);
            this.blocksTab.Controls.Add(this.lblBlockCPEReplace);
            this.blocksTab.Controls.Add(this.lblBlockCPELevel);
            this.blocksTab.Controls.Add(this.groupBox2);
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
            this.blocksTab.Location = new System.Drawing.Point(4, 25);
            this.blocksTab.Margin = new System.Windows.Forms.Padding(4);
            this.blocksTab.Name = "blocksTab";
            this.blocksTab.Size = new System.Drawing.Size(1311, 347);
            this.blocksTab.TabIndex = 4;
            this.blocksTab.Text = "Blocks";
            this.blocksTab.UseVisualStyleBackColor = true;
            // 
            // selectCPELevel
            // 
            this.selectCPELevel.FormattingEnabled = true;
            this.selectCPELevel.Items.AddRange(new object[] {
            "0",
            "1"});
            this.selectCPELevel.Location = new System.Drawing.Point(265, 195);
            this.selectCPELevel.Margin = new System.Windows.Forms.Padding(4);
            this.selectCPELevel.Name = "selectCPELevel";
            this.selectCPELevel.Size = new System.Drawing.Size(160, 24);
            this.selectCPELevel.TabIndex = 23;
            this.selectCPELevel.Text = "0";
            // 
            // selectCPEReplace
            // 
            this.selectCPEReplace.FormattingEnabled = true;
            this.selectCPEReplace.Location = new System.Drawing.Point(265, 225);
            this.selectCPEReplace.Margin = new System.Windows.Forms.Padding(4);
            this.selectCPEReplace.Name = "selectCPEReplace";
            this.selectCPEReplace.Size = new System.Drawing.Size(160, 24);
            this.selectCPEReplace.TabIndex = 22;
            // 
            // lblBlockCPEReplace
            // 
            this.lblBlockCPEReplace.AutoSize = true;
            this.lblBlockCPEReplace.Location = new System.Drawing.Point(165, 228);
            this.lblBlockCPEReplace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockCPEReplace.Name = "lblBlockCPEReplace";
            this.lblBlockCPEReplace.Size = new System.Drawing.Size(95, 17);
            this.lblBlockCPEReplace.TabIndex = 21;
            this.lblBlockCPEReplace.Text = "CPE Replace:";
            // 
            // lblBlockCPELevel
            // 
            this.lblBlockCPELevel.AutoSize = true;
            this.lblBlockCPELevel.Location = new System.Drawing.Point(180, 202);
            this.lblBlockCPELevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockCPELevel.Name = "lblBlockCPELevel";
            this.lblBlockCPELevel.Size = new System.Drawing.Size(77, 17);
            this.lblBlockCPELevel.TabIndex = 20;
            this.lblBlockCPELevel.Text = "CPE Level:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblblkpermDelete);
            this.groupBox2.Controls.Add(this.lblblkpermPlace);
            this.groupBox2.Controls.Add(this.lstDeletePerms);
            this.groupBox2.Controls.Add(this.lstPlacePerms);
            this.groupBox2.Location = new System.Drawing.Point(583, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 215);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Permissions";
            // 
            // lblblkpermDelete
            // 
            this.lblblkpermDelete.AutoSize = true;
            this.lblblkpermDelete.Location = new System.Drawing.Point(138, 10);
            this.lblblkpermDelete.Name = "lblblkpermDelete";
            this.lblblkpermDelete.Size = new System.Drawing.Size(49, 17);
            this.lblblkpermDelete.TabIndex = 3;
            this.lblblkpermDelete.Text = "Delete";
            // 
            // lblblkpermPlace
            // 
            this.lblblkpermPlace.AutoSize = true;
            this.lblblkpermPlace.Location = new System.Drawing.Point(3, 13);
            this.lblblkpermPlace.Name = "lblblkpermPlace";
            this.lblblkpermPlace.Size = new System.Drawing.Size(43, 17);
            this.lblblkpermPlace.TabIndex = 2;
            this.lblblkpermPlace.Text = "Place";
            // 
            // lstDeletePerms
            // 
            this.lstDeletePerms.FormattingEnabled = true;
            this.lstDeletePerms.ItemHeight = 16;
            this.lstDeletePerms.Location = new System.Drawing.Point(141, 30);
            this.lstDeletePerms.Name = "lstDeletePerms";
            this.lstDeletePerms.Size = new System.Drawing.Size(120, 180);
            this.lstDeletePerms.TabIndex = 1;
            // 
            // lstPlacePerms
            // 
            this.lstPlacePerms.FormattingEnabled = true;
            this.lstPlacePerms.ItemHeight = 16;
            this.lstPlacePerms.Location = new System.Drawing.Point(6, 30);
            this.lstPlacePerms.Name = "lstPlacePerms";
            this.lstPlacePerms.Size = new System.Drawing.Size(120, 180);
            this.lstPlacePerms.TabIndex = 0;
            // 
            // btnReloadBlocks
            // 
            this.btnReloadBlocks.Location = new System.Drawing.Point(538, 300);
            this.btnReloadBlocks.Margin = new System.Windows.Forms.Padding(4);
            this.btnReloadBlocks.Name = "btnReloadBlocks";
            this.btnReloadBlocks.Size = new System.Drawing.Size(113, 28);
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
            this.selectBlockSpecial.Location = new System.Drawing.Point(264, 165);
            this.selectBlockSpecial.Margin = new System.Windows.Forms.Padding(4);
            this.selectBlockSpecial.Name = "selectBlockSpecial";
            this.selectBlockSpecial.Size = new System.Drawing.Size(160, 24);
            this.selectBlockSpecial.TabIndex = 17;
            this.selectBlockSpecial.Text = "No";
            // 
            // selectBlockKills
            // 
            this.selectBlockKills.FormattingEnabled = true;
            this.selectBlockKills.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectBlockKills.Location = new System.Drawing.Point(265, 132);
            this.selectBlockKills.Margin = new System.Windows.Forms.Padding(4);
            this.selectBlockKills.Name = "selectBlockKills";
            this.selectBlockKills.Size = new System.Drawing.Size(160, 24);
            this.selectBlockKills.TabIndex = 16;
            this.selectBlockKills.Text = "No";
            // 
            // picBlockColor
            // 
            this.picBlockColor.Location = new System.Drawing.Point(265, 100);
            this.picBlockColor.Margin = new System.Windows.Forms.Padding(4);
            this.picBlockColor.Name = "picBlockColor";
            this.picBlockColor.Size = new System.Drawing.Size(27, 25);
            this.picBlockColor.TabIndex = 15;
            this.picBlockColor.TabStop = false;
            // 
            // numBlockCId
            // 
            this.numBlockCId.Location = new System.Drawing.Point(265, 68);
            this.numBlockCId.Margin = new System.Windows.Forms.Padding(4);
            this.numBlockCId.Maximum = new decimal(new int[] {
            65,
            0,
            0,
            0});
            this.numBlockCId.Name = "numBlockCId";
            this.numBlockCId.Size = new System.Drawing.Size(160, 22);
            this.numBlockCId.TabIndex = 14;
            // 
            // txtBlockName
            // 
            this.txtBlockName.Location = new System.Drawing.Point(265, 36);
            this.txtBlockName.Margin = new System.Windows.Forms.Padding(4);
            this.txtBlockName.Name = "txtBlockName";
            this.txtBlockName.Size = new System.Drawing.Size(159, 22);
            this.txtBlockName.TabIndex = 13;
            // 
            // numBlockId
            // 
            this.numBlockId.Location = new System.Drawing.Point(265, 4);
            this.numBlockId.Margin = new System.Windows.Forms.Padding(4);
            this.numBlockId.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBlockId.Name = "numBlockId";
            this.numBlockId.Size = new System.Drawing.Size(160, 22);
            this.numBlockId.TabIndex = 12;
            // 
            // btnSaveBlocks
            // 
            this.btnSaveBlocks.Location = new System.Drawing.Point(417, 300);
            this.btnSaveBlocks.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveBlocks.Name = "btnSaveBlocks";
            this.btnSaveBlocks.Size = new System.Drawing.Size(113, 28);
            this.btnSaveBlocks.TabIndex = 11;
            this.btnSaveBlocks.Text = "Save Blocks";
            this.btnSaveBlocks.UseVisualStyleBackColor = true;
            this.btnSaveBlocks.Click += new System.EventHandler(this.btnSaveBlocks_Click);
            // 
            // btnDeleteBlock
            // 
            this.btnDeleteBlock.Location = new System.Drawing.Point(296, 300);
            this.btnDeleteBlock.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteBlock.Name = "btnDeleteBlock";
            this.btnDeleteBlock.Size = new System.Drawing.Size(113, 28);
            this.btnDeleteBlock.TabIndex = 10;
            this.btnDeleteBlock.Text = "Delete Block";
            this.btnDeleteBlock.UseVisualStyleBackColor = true;
            this.btnDeleteBlock.Click += new System.EventHandler(this.btnDeleteBlock_Click);
            // 
            // btnCreateBlock
            // 
            this.btnCreateBlock.Location = new System.Drawing.Point(175, 300);
            this.btnCreateBlock.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateBlock.Name = "btnCreateBlock";
            this.btnCreateBlock.Size = new System.Drawing.Size(113, 28);
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
            this.groupBox4.Location = new System.Drawing.Point(864, 6);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(433, 218);
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
            this.selectPhysMapLoad.Location = new System.Drawing.Point(163, 178);
            this.selectPhysMapLoad.Margin = new System.Windows.Forms.Padding(4);
            this.selectPhysMapLoad.Name = "selectPhysMapLoad";
            this.selectPhysMapLoad.Size = new System.Drawing.Size(160, 24);
            this.selectPhysMapLoad.TabIndex = 23;
            this.selectPhysMapLoad.Text = "No";
            this.selectPhysMapLoad.Visible = false;
            // 
            // txtPhysScript
            // 
            this.txtPhysScript.Location = new System.Drawing.Point(163, 146);
            this.txtPhysScript.Margin = new System.Windows.Forms.Padding(4);
            this.txtPhysScript.Name = "txtPhysScript";
            this.txtPhysScript.Size = new System.Drawing.Size(160, 22);
            this.txtPhysScript.TabIndex = 22;
            // 
            // selectRepPhysics
            // 
            this.selectRepPhysics.FormattingEnabled = true;
            this.selectRepPhysics.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectRepPhysics.Location = new System.Drawing.Point(163, 113);
            this.selectRepPhysics.Margin = new System.Windows.Forms.Padding(4);
            this.selectRepPhysics.Name = "selectRepPhysics";
            this.selectRepPhysics.Size = new System.Drawing.Size(160, 24);
            this.selectRepPhysics.TabIndex = 21;
            this.selectRepPhysics.Text = "No";
            // 
            // txtPhysRandom
            // 
            this.txtPhysRandom.Location = new System.Drawing.Point(163, 81);
            this.txtPhysRandom.Margin = new System.Windows.Forms.Padding(4);
            this.txtPhysRandom.Name = "txtPhysRandom";
            this.txtPhysRandom.Size = new System.Drawing.Size(160, 22);
            this.txtPhysRandom.TabIndex = 20;
            // 
            // txtPhysDelay
            // 
            this.txtPhysDelay.Location = new System.Drawing.Point(163, 49);
            this.txtPhysDelay.Margin = new System.Windows.Forms.Padding(4);
            this.txtPhysDelay.Name = "txtPhysDelay";
            this.txtPhysDelay.Size = new System.Drawing.Size(160, 22);
            this.txtPhysDelay.TabIndex = 19;
            // 
            // selectBlockPhysics
            // 
            this.selectBlockPhysics.FormattingEnabled = true;
            this.selectBlockPhysics.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.selectBlockPhysics.Location = new System.Drawing.Point(163, 16);
            this.selectBlockPhysics.Margin = new System.Windows.Forms.Padding(4);
            this.selectBlockPhysics.Name = "selectBlockPhysics";
            this.selectBlockPhysics.Size = new System.Drawing.Size(160, 24);
            this.selectBlockPhysics.TabIndex = 18;
            this.selectBlockPhysics.Text = "No";
            // 
            // lblBlockPhysMapload
            // 
            this.lblBlockPhysMapload.AutoSize = true;
            this.lblBlockPhysMapload.Location = new System.Drawing.Point(12, 182);
            this.lblBlockPhysMapload.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockPhysMapload.Name = "lblBlockPhysMapload";
            this.lblBlockPhysMapload.Size = new System.Drawing.Size(142, 17);
            this.lblBlockPhysMapload.TabIndex = 13;
            this.lblBlockPhysMapload.Text = "Physics on map load:";
            this.lblBlockPhysMapload.Visible = false;
            // 
            // lblBlockPhysScript
            // 
            this.lblBlockPhysScript.AutoSize = true;
            this.lblBlockPhysScript.Location = new System.Drawing.Point(53, 150);
            this.lblBlockPhysScript.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockPhysScript.Name = "lblBlockPhysScript";
            this.lblBlockPhysScript.Size = new System.Drawing.Size(100, 17);
            this.lblBlockPhysScript.TabIndex = 12;
            this.lblBlockPhysScript.Text = "Physics Script:";
            // 
            // lblBlockRepPhys
            // 
            this.lblBlockRepPhys.AutoSize = true;
            this.lblBlockRepPhys.Location = new System.Drawing.Point(43, 117);
            this.lblBlockRepPhys.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockRepPhys.Name = "lblBlockRepPhys";
            this.lblBlockRepPhys.Size = new System.Drawing.Size(110, 17);
            this.lblBlockRepPhys.TabIndex = 11;
            this.lblBlockRepPhys.Text = "Repeat Physics:";
            // 
            // lblBlockPhysRandom
            // 
            this.lblBlockPhysRandom.AutoSize = true;
            this.lblBlockPhysRandom.Location = new System.Drawing.Point(36, 85);
            this.lblBlockPhysRandom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockPhysRandom.Name = "lblBlockPhysRandom";
            this.lblBlockPhysRandom.Size = new System.Drawing.Size(117, 17);
            this.lblBlockPhysRandom.TabIndex = 10;
            this.lblBlockPhysRandom.Text = "Physics Random:";
            // 
            // lblBlockPhysDelay
            // 
            this.lblBlockPhysDelay.AutoSize = true;
            this.lblBlockPhysDelay.Location = new System.Drawing.Point(53, 53);
            this.lblBlockPhysDelay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockPhysDelay.Name = "lblBlockPhysDelay";
            this.lblBlockPhysDelay.Size = new System.Drawing.Size(100, 17);
            this.lblBlockPhysDelay.TabIndex = 9;
            this.lblBlockPhysDelay.Text = "Physics Delay:";
            // 
            // lblBlockPhysics
            // 
            this.lblBlockPhysics.AutoSize = true;
            this.lblBlockPhysics.Location = new System.Drawing.Point(93, 20);
            this.lblBlockPhysics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockPhysics.Name = "lblBlockPhysics";
            this.lblBlockPhysics.Size = new System.Drawing.Size(60, 17);
            this.lblBlockPhysics.TabIndex = 8;
            this.lblBlockPhysics.Text = "Physics:";
            // 
            // lblBlockSpecial
            // 
            this.lblBlockSpecial.AutoSize = true;
            this.lblBlockSpecial.Location = new System.Drawing.Point(196, 169);
            this.lblBlockSpecial.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockSpecial.Name = "lblBlockSpecial";
            this.lblBlockSpecial.Size = new System.Drawing.Size(58, 17);
            this.lblBlockSpecial.TabIndex = 6;
            this.lblBlockSpecial.Text = "Special:";
            // 
            // lblBlockKills
            // 
            this.lblBlockKills.AutoSize = true;
            this.lblBlockKills.Location = new System.Drawing.Point(220, 135);
            this.lblBlockKills.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockKills.Name = "lblBlockKills";
            this.lblBlockKills.Size = new System.Drawing.Size(37, 17);
            this.lblBlockKills.TabIndex = 5;
            this.lblBlockKills.Text = "Kills:";
            // 
            // lblBlockColor
            // 
            this.lblBlockColor.AutoSize = true;
            this.lblBlockColor.Location = new System.Drawing.Point(212, 106);
            this.lblBlockColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockColor.Name = "lblBlockColor";
            this.lblBlockColor.Size = new System.Drawing.Size(45, 17);
            this.lblBlockColor.TabIndex = 4;
            this.lblBlockColor.Text = "Color:";
            // 
            // lblBlockCId
            // 
            this.lblBlockCId.AutoSize = true;
            this.lblBlockCId.Location = new System.Drawing.Point(171, 70);
            this.lblBlockCId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockCId.Name = "lblBlockCId";
            this.lblBlockCId.Size = new System.Drawing.Size(84, 17);
            this.lblBlockCId.TabIndex = 3;
            this.lblBlockCId.Text = "ID on Client:";
            // 
            // lblBlockName
            // 
            this.lblBlockName.AutoSize = true;
            this.lblBlockName.Location = new System.Drawing.Point(207, 39);
            this.lblBlockName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockName.Name = "lblBlockName";
            this.lblBlockName.Size = new System.Drawing.Size(49, 17);
            this.lblBlockName.TabIndex = 2;
            this.lblBlockName.Text = "Name:";
            // 
            // lblBlockId
            // 
            this.lblBlockId.AutoSize = true;
            this.lblBlockId.Location = new System.Drawing.Point(232, 6);
            this.lblBlockId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlockId.Name = "lblBlockId";
            this.lblBlockId.Size = new System.Drawing.Size(23, 17);
            this.lblBlockId.TabIndex = 1;
            this.lblBlockId.Text = "Id:";
            // 
            // lstBlocks
            // 
            this.lstBlocks.FormattingEnabled = true;
            this.lstBlocks.ItemHeight = 16;
            this.lstBlocks.Location = new System.Drawing.Point(4, 4);
            this.lstBlocks.Margin = new System.Windows.Forms.Padding(4);
            this.lstBlocks.Name = "lstBlocks";
            this.lstBlocks.Size = new System.Drawing.Size(159, 324);
            this.lstBlocks.TabIndex = 0;
            this.lstBlocks.SelectedIndexChanged += new System.EventHandler(this.lstBlocks_SelectedIndexChanged);
            // 
            // ranksTab
            // 
            this.ranksTab.Location = new System.Drawing.Point(4, 25);
            this.ranksTab.Margin = new System.Windows.Forms.Padding(4);
            this.ranksTab.Name = "ranksTab";
            this.ranksTab.Size = new System.Drawing.Size(1311, 347);
            this.ranksTab.TabIndex = 5;
            this.ranksTab.Text = "Ranks";
            this.ranksTab.UseVisualStyleBackColor = true;
            // 
            // dbTab
            // 
            this.dbTab.Location = new System.Drawing.Point(4, 25);
            this.dbTab.Margin = new System.Windows.Forms.Padding(4);
            this.dbTab.Name = "dbTab";
            this.dbTab.Size = new System.Drawing.Size(1311, 347);
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
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1319, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startServerToolStripMenuItem,
            this.stopServerToolStripMenuItem});
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.controlToolStripMenuItem.Text = "&Control";
            // 
            // startServerToolStripMenuItem
            // 
            this.startServerToolStripMenuItem.Name = "startServerToolStripMenuItem";
            this.startServerToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.startServerToolStripMenuItem.Text = "&Start Server";
            this.startServerToolStripMenuItem.Click += new System.EventHandler(this.startServerToolStripMenuItem_Click);
            // 
            // stopServerToolStripMenuItem
            // 
            this.stopServerToolStripMenuItem.Name = "stopServerToolStripMenuItem";
            this.stopServerToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.stopServerToolStripMenuItem.Text = "S&top Server";
            this.stopServerToolStripMenuItem.Click += new System.EventHandler(this.stopServerToolStripMenuItem_Click);
            // 
            // gUIToolStripMenuItem
            // 
            this.gUIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendToTrayToolStripMenuItem,
            this.miniModeToolStripMenuItem});
            this.gUIToolStripMenuItem.Name = "gUIToolStripMenuItem";
            this.gUIToolStripMenuItem.Size = new System.Drawing.Size(45, 24);
            this.gUIToolStripMenuItem.Text = "&GUI";
            // 
            // sendToTrayToolStripMenuItem
            // 
            this.sendToTrayToolStripMenuItem.Name = "sendToTrayToolStripMenuItem";
            this.sendToTrayToolStripMenuItem.Size = new System.Drawing.Size(161, 24);
            this.sendToTrayToolStripMenuItem.Text = "&Send to Tray";
            // 
            // miniModeToolStripMenuItem
            // 
            this.miniModeToolStripMenuItem.Name = "miniModeToolStripMenuItem";
            this.miniModeToolStripMenuItem.Size = new System.Drawing.Size(161, 24);
            this.miniModeToolStripMenuItem.Text = "&Mini mode";
            // 
            // chkBlockDisable
            // 
            this.chkBlockDisable.AutoSize = true;
            this.chkBlockDisable.Location = new System.Drawing.Point(264, 256);
            this.chkBlockDisable.Name = "chkBlockDisable";
            this.chkBlockDisable.Size = new System.Drawing.Size(115, 21);
            this.chkBlockDisable.TabIndex = 24;
            this.chkBlockDisable.Text = "Disable Block";
            this.chkBlockDisable.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnSendChat;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1319, 404);
            this.Controls.Add(this.mainTabs);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Hypercube";
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
            this.commandsTab.ResumeLayout(false);
            this.commandsTab.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.blocksTab.ResumeLayout(false);
            this.blocksTab.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblblkpermDelete;
        private System.Windows.Forms.Label lblblkpermPlace;
        private System.Windows.Forms.ListBox lstDeletePerms;
        private System.Windows.Forms.ListBox lstPlacePerms;
        private System.Windows.Forms.Label lblBlockCPEReplace;
        private System.Windows.Forms.Label lblBlockCPELevel;
        private System.Windows.Forms.ComboBox selectCPELevel;
        private System.Windows.Forms.ComboBox selectCPEReplace;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstCmdShowPerms;
        private System.Windows.Forms.ListBox lstCmdUsePerms;
        private System.Windows.Forms.ListBox lstCommands;
        private System.Windows.Forms.CheckBox chkCmdConsole;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox chkCmdAllPerms;
        private System.Windows.Forms.Label lblCmdHelp;
        private System.Windows.Forms.Label lblCmdPlugin;
        private System.Windows.Forms.Label lblCmdGrp;
        private System.Windows.Forms.Label lblCmdName;
        private System.Windows.Forms.Button btnReloadCmd;
        private System.Windows.Forms.Button btnSaveCmd;
        private System.Windows.Forms.Button btnDeleteCmd;
        private System.Windows.Forms.Button btnAddCommand;
        private System.Windows.Forms.CheckBox chkBlockDisable;
    }
}

