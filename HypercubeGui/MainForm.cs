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
using Hypercube_Classic;

namespace HypercubeGui {
    public partial class MainForm : Form {
        Hypercube ServerCore;

        public MainForm() {
            InitializeComponent();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {

        }

        private void MainForm_Load(object sender, EventArgs e) {
            Console.SetOut(new ControlWriter(txtInfo, txtError, txtChatPanel));
            ServerCore = new Hypercube();

        }

        private void btnStart_Click(object sender, EventArgs e) {
            ServerCore.Start();
        }

        private void btnStop_Click(object sender, EventArgs e) {
            ServerCore.Stop();
        }

        private void chkVerifyNames_CheckedChanged(object sender, EventArgs e) {
            ServerCore.nh.VerifyNames = false;
        }
    }

    public class ControlWriter : TextWriter {
        private Control infoTB;
        private Control errTB;
        private Control ChatTB;

        public ControlWriter(Control ITB, Control ETB, Control CTB) {
            this.infoTB = ITB;
            this.errTB = ETB;
            this.ChatTB = CTB;
        }

        public override void Write(char value) {
            infoTB.Text += value;
        }

        public override void Write(string value) {
            string[] splits = value.Split(' ');

            if (splits.Length > 2) {
                switch (splits[1]) {
                    case "[Info]":
                        infoTB.Text += value;
                        break;
                    case "[Error]":
                        errTB.Text += value;
                        break;
                    case "[Warning]":
                        errTB.Text += value;
                        break;
                    case "[Critical]":
                        errTB.Text += value;
                        break;
                    case "[Chat]":
                        ChatTB.Text += value;
                        break;
                    case "[Command]":
                        ChatTB.Text += value;
                        break;
                    default:
                        infoTB.Text += value;
                        break;
                }
            }
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
