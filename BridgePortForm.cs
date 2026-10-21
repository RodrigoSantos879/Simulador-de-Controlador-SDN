using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LTILAB1
{
    public partial class BridgePortForm : Form
    {
        public BridgePort bridgePort;
        public BridgePortForm(List<Bridge> bridges,List<Interface> interfaces,BridgePort bp)
        {
            
            InitializeComponent();
            bridgePort = new BridgePort();
            List<Interface> InterfacesREAIS = new List<Interface>(interfaces);
            var assignedInterfaceNames = bridges.Select(p => p.Name).ToList();

            // Remove interfaces that are already in the assigned list
            InterfacesREAIS.RemoveAll(iface => assignedInterfaceNames.Contains(iface.Name));
            foreach (Interface iface in InterfacesREAIS)
            {
                comboBox2.Items.Add(iface.Name);
            }
            comboBox2.SelectedIndex = 0;
            int index2 = comboBox2.FindStringExact(bp.InterfaceName);
            if (index2 != -1) comboBox2.SelectedIndex = index2;
            foreach (Bridge bridge in bridges)
            {
                comboBox1.Items.Add(bridge.Name);
            }
            comboBox1.SelectedIndex = 0;
            int index = comboBox1.FindStringExact(bp.BridgeName);
            if (index != -1) comboBox1.SelectedIndex = index;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox2.Text))
            {
                bridgePort.InterfaceName = comboBox2.Text;
            }else
            {
                MessageBox.Show("Por favor, selecione uma interface válida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Impede que o formulário seja fechado
            }
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                bridgePort.BridgeName = comboBox1.Text;
            }else
            {
                MessageBox.Show("Por favor, selecione uma bridge válida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Impede que o formulário seja fechado
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
