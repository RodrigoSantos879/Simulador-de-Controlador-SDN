using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Windows.Forms;

namespace LTILAB1
{
    public partial class DHCPForm : Form
    {
        public DHCPServer dhcp = new DHCPServer();
        public string DHCPnetwork;
        public string gatewayIp;
        public string poolRange;
        public string poolName;
        public bool DNS;

        public DHCPForm(List<Interface> interfaces, DHCPServer dhcpS,string gIP,string nIP,string pRange,string pName, bool dns)
        {
            InitializeComponent();
            dhcp = dhcpS;
            string rou = nIP;
            if (!string.IsNullOrEmpty(rou) && rou.Contains("/"))
            {
                textBox1.Text = rou.Split('/')[0]; // Address
                textBox2.Text = rou.Split('/')[1]; // Mask
            }
            if (gIP!=null){ 
                textBox3.Text = gIP;
            }
            if (!string.IsNullOrEmpty(pRange) && pRange.Contains("-"))
            {
                textBox4.Text = pRange.Split('-')[0]; // Address
                textBox5.Text = pRange.Split('-')[1]; // Mask
            }
            if (dhcp.LeaseTime != null ) { 
                textBox6.Text = TranformAgeingTime(dhcp.LeaseTime);
            }
            if (dhcp.Name != null) {
                textBox8.Text = dhcp.Name;
            }
            if (pName != null) {
                textBox7.Text = pName;
            }
            checkBox1.Checked = dns;
            
            foreach (var iface in interfaces)
            {
                comboBox1.Items.Add(iface.Name);
            }
            int index = comboBox1.FindStringExact(dhcp.Interface);
            if (index != -1) comboBox1.SelectedIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool Leaseval = CheckAndConvertAgeingTime(textBox6.Text);
            // Salva os valores dos TextBoxes nas propriedades públicas
            dhcp.Name = textBox1.Text;
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a interface first!");
                return;
            }
            dhcp.Interface = comboBox1.Text;
            string network = textBox1.Text;
            string mask = textBox2.Text;

            if (IPAddress.TryParse(network, out IPAddress address))
            {
                int maskValue = int.TryParse(mask, out int maskInt) ? maskInt : -1;
                if (maskValue >= 0 && maskValue <= 32)
                {
                    // Valid IP and mask, you can proceed with saving or using the values

                }
                else
                {
                    MessageBox.Show("Mask must be between 0 and 32.");
                    return;
                }
            }
            else
            {
                // It is not a valid IP (e.g., "999.999.999.999" or "hello")
                MessageBox.Show("Invalid IP address.");
                return;
            }
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text)) {
                MessageBox.Show("Please enter both network and mask.");
                return;
            }
            DHCPnetwork = textBox1.Text + "/" + textBox2.Text;
            if (!IsIpInRange(textBox3.Text, textBox1.Text, textBox2.Text)) { 
                MessageBox.Show("Gateway IP is not in the network range.");
                return;
            }
            if (string.IsNullOrEmpty(textBox3.Text)) {
                MessageBox.Show("Please enter a gateway IP.");
                return;
            }
            gatewayIp = textBox3.Text;
            if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Pool IP addresses cant be empty.");
                return;
            }
            if (!IPAddress.TryParse(textBox4.Text, out IPAddress address2) || 
                !IPAddress.TryParse(textBox5.Text, out IPAddress address3))
            {
                MessageBox.Show("Invalid Pool IP address.");
                return;
            }
            if (!IsIpInRange(textBox4.Text, textBox1.Text, textBox2.Text) || !IsIpInRange(textBox5.Text, textBox1.Text, textBox2.Text))
            {
                MessageBox.Show("Pool IP Addresses are not in the network range.");
                return;
            }
            poolRange = textBox4.Text + "-" + textBox5.Text;
            if (!Leaseval)
            {
                MessageBox.Show("AgeingTime must have this format hh:mm:ss and be more than 10 seconds");
                return;
            }
            dhcp.LeaseTime = textBox6.Text;
            if(string.IsNullOrEmpty(textBox7.Text))
            {
                MessageBox.Show("Please enter a pool name.");
                return;
            }
            poolName = textBox7.Text;
            if (string.IsNullOrEmpty(textBox8.Text))
            {
                MessageBox.Show("Please enter a DHCP name.");
                return;
            }   
            dhcp.Name = textBox8.Text;
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

        public bool IsIpInRange(string targetIp, string startIp, string subnetMask)
        {
            string endIp = GetLastUsableIp(startIp, subnetMask);
            try
            {
                // Converte as strings para uint (número inteiro sem sinal)
                uint target = IpToUint(IPAddress.Parse(targetIp));
                uint start = IpToUint(IPAddress.Parse(startIp));
                uint end = IpToUint(IPAddress.Parse(endIp));

                // Verifica se o IP está entre o menor e o maior
                return target > start && target < end;
            }
            catch
            {
                return false;
            }
        }

        private uint IpToUint(IPAddress ipAddress)
        {
            byte[] bytes = ipAddress.GetAddressBytes();

            // Inverte a ordem se o sistema for Little Endian (comum em Windows)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToUInt32(bytes, 0);
        }

        public string GetLastUsableIp(string networkIp, string subnetMask)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(networkIp);
                IPAddress mask = IPAddress.Parse(subnetMask);

                byte[] ipBytes = ip.GetAddressBytes();
                byte[] maskBytes = mask.GetAddressBytes();

                byte[] lastIpBytes = new byte[4];

                for (int i = 0; i < 4; i++)
                {
                    // O último IP (Broadcast) é: IP_Rede OR (NOT Mascara)
                    lastIpBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
                }

                

                return new IPAddress(lastIpBytes).ToString();
            }
            catch
            {
                return "0.0.0.0";
            }
        }

        private bool CheckAndConvertAgeingTime(string time)
        {
            string format = @"hh\:mm\:ss";

            if (TimeSpan.TryParseExact(time, format, null, out TimeSpan parsedTime))
            {
                // SUCCESS: The string matches hh:mm:ss
                return parsedTime.TotalSeconds >= 10 ? true : false;
            }
            else
            {
                return false;
            }
        }

        private string TranformAgeingTime(string time)
        {
            // Matches the numbers associated with h, m, and s
            // It handles cases where hours or minutes might be missing (e.g., "5m10s")
            int hours = 0, minutes = 0, seconds = 0;

            var hMatch = Regex.Match(time, @"(\d+)h");
            var mMatch = Regex.Match(time, @"(\d+)m");
            var sMatch = Regex.Match(time, @"(\d+)s");

            if (hMatch.Success) hours = int.Parse(hMatch.Groups[1].Value);
            if (mMatch.Success) minutes = int.Parse(mMatch.Groups[1].Value);
            if (sMatch.Success) seconds = int.Parse(sMatch.Groups[1].Value);

            // Format as 00:00:00
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }



    }
}
