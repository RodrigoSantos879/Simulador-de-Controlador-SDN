using RestSharp;
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
    public partial class StaticRoute : Form
    {
        public string DstAddress { get; set; }
        public string Gateway { get; set; }
        public string Distance { get; set; }

        public StaticRoute( List<Interface> interfaces,Route route)
        {
            InitializeComponent();
            string rou = route.DstAddress;
            if (!string.IsNullOrEmpty(rou) && rou.Contains("/"))
            {
                textBox1.Text = rou.Split('/')[0]; // Address
                textBox2.Text = rou.Split('/')[1]; // Mask
            }
            foreach (var iface in interfaces)
            {
                comboBox1.Items.Add(iface.Name);
            }
            int index = comboBox1.FindStringExact(route.Gateway);
            if (index != -1) comboBox1.SelectedIndex = index;
            if(route.Distance != null)
            {
                textBox3.Text = route.Distance;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

           
            DstAddress =textBox1.Text+"/"+textBox2.Text;
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a interface first!");
                return;
            }
            Gateway = comboBox1.Text;

            
                if(int.TryParse(textBox3.Text, out int distance))
                {
                    if (distance > 0 && distance < 256)
                    {
                        Distance = textBox3.Text;
                    }
                    else
                    {
                        MessageBox.Show("Distance must be a integer between 0 and 255.");
                        return;
                    }
                }
                else
                {
                Distance = "1";
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
