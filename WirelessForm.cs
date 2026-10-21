using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace LTILAB1
{
    public partial class WirelessForm : Form
    {
        public WirelessNetwork wirenet { get; set; }

        public WirelessForm(WirelessNetwork wn, List<SecurityProfile> sps)
        {
            InitializeComponent();

            // 1. Setup Dropdown lists
            comboBox2.Items.AddRange(new string[] { "disabled", "push-button", "virtual-pbc", "display" });

            // Populate Security Profiles from the list passed from Main Form
            if (sps != null)
            {
                comboBox1.DataSource = sps;
                comboBox1.DisplayMember = "Name"; // Shows the profile name
                comboBox1.ValueMember = "Name";   // The value we want to save
            }

            // 2. Initialize Data
            if (wn == null)
            {
                this.wirenet = new WirelessNetwork();
                comboBox2.SelectedIndex = 0; // default to disabled
            }
            else
            {
                this.wirenet = wn;
                LoadWirelessData();
            }
        }

        private void LoadWirelessData()
        {
            textBox1.Text = wirenet.Name;
            textBox2.Text = wirenet.Ssid;

            // Match Security Profile
            if (!string.IsNullOrEmpty(wirenet.SecurityProfile))
                comboBox1.Text = wirenet.SecurityProfile;

            // Match WPS Mode
            int wpsIndex = comboBox2.FindStringExact(wirenet.WpsMode);
            comboBox2.SelectedIndex = wpsIndex != -1 ? wpsIndex : 0;

            // MikroTik sends "true"/"false" as strings for hide-ssid
            checkBox1.Checked = (wirenet.HideSsid == "true" || wirenet.HideSsid == "yes");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // --- VALIDATION ---
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Name and SSID are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- SAVE TO OBJECT ---
            wirenet.Name = textBox1.Text;
            wirenet.Ssid = textBox2.Text;

            // Get selected Security Profile Name
            wirenet.SecurityProfile = comboBox1.Text;

            // Get WPS Mode
            wirenet.WpsMode = comboBox2.Text;

            // MikroTik usually expects "yes" or "no" for boolean-like flags in REST
            wirenet.HideSsid = checkBox1.Checked ? "yes" : "no";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}