using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LTILAB1
{
    public partial class Form2 : Form
    {
        public Bridge bridge { get; set; }
        public Form2(Bridge brdg)
        {
            InitializeComponent();
            comboBox1.Items.Add("disabled");
            comboBox1.Items.Add("enabled");
            comboBox1.Items.Add("local-proxy-arp");
            comboBox1.Items.Add("proxy-arp");
            comboBox1.Items.Add("reply-only");
            comboBox1.SelectedText = "enabled";
            bridge = new Bridge();
            if (!(brdg == null)) {
                textBox1.Text = brdg.Name;
                textBox2.Text = brdg.Mtu;
                textBox3.Text = TranformAgeingTime(brdg.AgeingTime);
                int index = comboBox1.FindStringExact(brdg.Arp);
                if (index != -1)
                    comboBox1.SelectedIndex = index;
                else if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int mtuValue = textBox2.Text != "" ? int.Parse(textBox2.Text) : 0;
            bool AGEval = CheckAndConvertAgeingTime(textBox3.Text);
            if (mtuValue < 64 || mtuValue > 65535) {
                MessageBox.Show("MTU must be more than 64 and less than 65535\n *AUTO MTU HAS BEEN USED*");
                textBox2.Text = "1500";
            }
            if (!AGEval)
            {
                MessageBox.Show("AgeingTime must have this format hh:mm:ss and be more than 10 seconds");
                return;
            }
            bridge.Name = textBox1.Text;
            bridge.Mtu = textBox2.Text;
            bridge.AgeingTime = textBox3.Text;
            bridge.Arp = comboBox1.SelectedItem.ToString();

            this.DialogResult = DialogResult.OK; // Indica que o usuário confirmou
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Define o resultado como Cancelar
            this.DialogResult = DialogResult.Cancel;

            // Fecha o formulário sem salvar nada na propriedade NomeEditado
            this.Close();
        }

        private bool CheckAndConvertAgeingTime(string time) {
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

        private string TranformAgeingTime(string time) {
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
