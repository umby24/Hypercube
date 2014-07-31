namespace ServerGUI {
    partial class mainForm {
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
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
            this.label7 = new System.Windows.Forms.Label();
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
            this.groupBox3.SuspendLayout();
            this.grpNetwork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.grpSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBlocks)).BeginInit();
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
            this.infotab.Size = new System.Drawing.Size(967, 227);
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
            this.txtInfobox.Size = new System.Drawing.Size(961, 221);
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
            this.chatTab.Size = new System.Drawing.Size(967, 227);
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
            this.txtChatbox.Size = new System.Drawing.Size(964, 198);
            this.txtChatbox.TabIndex = 2;
            this.txtChatbox.Text = "";
            // 
            // btnSendChat
            // 
            this.btnSendChat.Location = new System.Drawing.Point(883, 201);
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
            this.txtChat.Location = new System.Drawing.Point(3, 204);
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(874, 20);
            this.txtChat.TabIndex = 0;
            // 
            // cTab
            // 
            this.cTab.Controls.Add(this.txtCommandbox);
            this.cTab.Location = new System.Drawing.Point(4, 22);
            this.cTab.Name = "cTab";
            this.cTab.Size = new System.Drawing.Size(967, 227);
            this.cTab.TabIndex = 3;
            this.cTab.Text = "Commands";
            this.cTab.UseVisualStyleBackColor = true;
            // 
            // txtCommandbox
            // 
            this.txtCommandbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommandbox.Location = new System.Drawing.Point(0, 0);
            this.txtCommandbox.Name = "txtCommandbox";
            this.txtCommandbox.Size = new System.Drawing.Size(967, 227);
            this.txtCommandbox.TabIndex = 0;
            this.txtCommandbox.Text = "";
            // 
            // ErrorsTab
            // 
            this.ErrorsTab.Controls.Add(this.txtErrorbox);
            this.ErrorsTab.Location = new System.Drawing.Point(4, 22);
            this.ErrorsTab.Name = "ErrorsTab";
            this.ErrorsTab.Size = new System.Drawing.Size(967, 227);
            this.ErrorsTab.TabIndex = 4;
            this.ErrorsTab.Text = "Errors";
            this.ErrorsTab.UseVisualStyleBackColor = true;
            // 
            // txtErrorbox
            // 
            this.txtErrorbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrorbox.Location = new System.Drawing.Point(0, 0);
            this.txtErrorbox.Name = "txtErrorbox";
            this.txtErrorbox.Size = new System.Drawing.Size(967, 227);
            this.txtErrorbox.TabIndex = 0;
            this.txtErrorbox.Text = "";
            // 
            // debugTab
            // 
            this.debugTab.Controls.Add(this.txtDebugBox);
            this.debugTab.Location = new System.Drawing.Point(4, 22);
            this.debugTab.Name = "debugTab";
            this.debugTab.Size = new System.Drawing.Size(967, 227);
            this.debugTab.TabIndex = 5;
            this.debugTab.Text = "Debug";
            this.debugTab.UseVisualStyleBackColor = true;
            // 
            // txtDebugBox
            // 
            this.txtDebugBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDebugBox.Location = new System.Drawing.Point(0, 0);
            this.txtDebugBox.Name = "txtDebugBox";
            this.txtDebugBox.Size = new System.Drawing.Size(967, 227);
            this.txtDebugBox.TabIndex = 0;
            this.txtDebugBox.Text = "";
            // 
            // settingsTab
            // 
            this.settingsTab.Controls.Add(this.btnRuleEdit);
            this.settingsTab.Controls.Add(this.groupBox3);
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtChatDivider);
            this.groupBox3.Controls.Add(this.lblDivider);
            this.groupBox3.Controls.Add(this.txtPlayerlist);
            this.groupBox3.Controls.Add(this.lblPlayerList);
            this.groupBox3.Controls.Add(this.lblSystem);
            this.groupBox3.Controls.Add(this.lblError);
            this.groupBox3.Controls.Add(this.txtChatSystem);
            this.groupBox3.Controls.Add(this.txtChatError);
            this.groupBox3.Location = new System.Drawing.Point(241, 123);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 136);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Chat";
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
            this.grpNetwork.Controls.Add(this.label7);
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
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Max Players";
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
            this.mapsTab.Location = new System.Drawing.Point(4, 22);
            this.mapsTab.Name = "mapsTab";
            this.mapsTab.Size = new System.Drawing.Size(981, 278);
            this.mapsTab.TabIndex = 2;
            this.mapsTab.Text = "Maps";
            this.mapsTab.UseVisualStyleBackColor = true;
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
            // mainForm
            // 
            this.AcceptButton = this.btnSendChat;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 328);
            this.Controls.Add(this.mainTabs);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "mainForm";
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpNetwork.ResumeLayout(false);
            this.grpNetwork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.grpSystem.ResumeLayout(false);
            this.grpSystem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBlocks)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox3;
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
        private System.Windows.Forms.Label label7;
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
    }
}

