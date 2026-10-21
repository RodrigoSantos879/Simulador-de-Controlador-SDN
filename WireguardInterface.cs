using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LTILAB1
{
    public partial class WireguardInterface : Form
    {
        public string WGName { get; set; }
        public int LPort { get; set; }
        public int MTU { get; set; }

        public WireguardInterface(string wgName, string lPort, string mtu)
        {
            InitializeComponent();
            WGName = wgName;
            LPort = int.TryParse(lPort, out int lPortValue) ? lPortValue : 0;
            MTU = int.TryParse(mtu, out int mtuValue) ? mtuValue : 0;
            if (!string.IsNullOrEmpty(wgName))
            {
                textBox1.Text = wgName;
            }
            if (!string.IsNullOrEmpty(lPort))
            {
                 textBox2.Text = lPort;
            }   
            if (!string.IsNullOrEmpty(mtu))
            {
                textBox3.Text = mtu;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WGName = textBox1.Text;
            string LPort1 = textBox2.Text;
            int port1 = int.TryParse(LPort1, out int port) ? port : -1;
            if (port > 0 && port <= 65535)
            {
                // Valid IP and mask, you can proceed with saving or using the values
                LPort = port;
                }
                else
                {
                    MessageBox.Show("Port must be between 1 and 65535.");
                    return;
                }
            string MTU1 = textBox3.Text;
            int mtu = int.TryParse(MTU1, out int mtuValue) ? mtuValue : -1;
            if (mtu > 63 && mtu < 65536)
            {
                // Valid IP and mask, you can proceed with saving or using the values
                MTU = mtuValue;
            }
            else
            {
                MessageBox.Show("MTU must be between 64 and 65535.");
                return;
            }


            this.DialogResult = DialogResult.OK; // Indica que o usuário confirmou
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            // Fecha o formulário sem salvar nada na propriedade NomeEditado
            this.Close();
        }
    }
}
