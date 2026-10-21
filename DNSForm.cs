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

namespace LTILAB1
{
    public partial class DNSForm : Form
    {
        public DNServer dnss = new DNServer();
        public DNSForm(DNServer dns)
        {
            InitializeComponent();
            if (dns.Servers != null)
            {
                textBox1.Text = dns.Servers;
            }
            if (dns.MaxUdpPacketSize != null)
            {
                textBox2.Text = dns.MaxUdpPacketSize;
            }
            if (dns.QueryServerTimeout != null)
            {
                textBox3.Text = dns.QueryServerTimeout;
            }
            if (dns.QueryTotalTimeout != null)
            {
                textBox4.Text = dns.QueryTotalTimeout;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {


            if (IPAddress.TryParse(textBox1.Text, out IPAddress address))
            {
                dnss.Servers = address.ToString();
            } else
            {
                MessageBox.Show("O campo 'Address' não é um endereço IP válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Impede o fechamento do formulário
            }

            if (int.TryParse(textBox2.Text, out int udp))
            {
                if (udp > 49 && udp < 65508)
                {
                    dnss.MaxUdpPacketSize = textBox2.Text;
                }
                else
                {
                    MessageBox.Show("O campo 'MaxUdpPacketSize' deve ser um número entre 64 e 65507.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Impede o fechamento do formulário
                }
            }
            else
            {
                MessageBox.Show("O campo 'MaxUdpPacketSize' não pode estar vazio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Impede o fechamento do formulário
            }

            if (int.TryParse(textBox3.Text, out int serverT))
            {
                if (serverT > 0 && serverT < 100001)
                {
                    dnss.QueryServerTimeout = textBox2.Text;
                }
                else
                {
                    MessageBox.Show("O campo 'QueryServerTimeout' deve ser um número entre 1 e 100000.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Impede o fechamento do formulário
                }
            }
            else
            {
                MessageBox.Show("O campo 'QueryServerTimeout' não pode estar vazio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Impede o fechamento do formulário
            }

            if (int.TryParse(textBox4.Text, out int totalT))
            {
                if (totalT > 0 && totalT < 100001)
                {
                    dnss.QueryTotalTimeout = textBox4.Text;
                }
                else
                {
                    MessageBox.Show("O campo 'QueryTotalTimeout' deve ser um número entre 1 e 100000.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Impede o fechamento do formulário
                }
            }
            else
            {
                MessageBox.Show("O campo 'QueryTotalTimeout' não pode estar vazio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Impede o fechamento do formulário
            }

            this.DialogResult = DialogResult.OK; // Indica que o usuário confirmou
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Define o resultado como Cancelar
            this.DialogResult = DialogResult.Cancel;

            // Fecha o formulário sem salvar nada na propriedade NomeEditado
            this.Close();

        }
    }
}
