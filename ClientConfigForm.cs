using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LTILAB1
{
    public partial class ClientConfigForm : Form
    {
        string clientPrivateKey = null;
        string clientAddress = null;
        string serverPublicKey = null;
        string port = null;
        string MTU = null;
        string serverEndpoint = null;
        public ClientConfigForm(Peer peer,WGInterface wg)
        {

            InitializeComponent();
            clientPrivateKey = peer.PrivateKey;
            clientAddress= peer.ClientAddress;
            serverPublicKey = wg.PublicKey;
            port = peer.EndpointPort;
            MTU = wg.Mtu;
            serverEndpoint = wg.ListenPort;
            DisplayWireGuardConfig(clientPrivateKey, clientAddress, serverPublicKey,port, MTU, serverEndpoint);
        }

        private void ClientConfigForm_Load(object sender, EventArgs e)
        {

        }


        private void DisplayWireGuardConfig(string clientPrivateKey, string clientAddress, string serverPublicKey, string port,string MTU, string serverEndpoint)
        {
            // 1. Create the WireGuard Config String (The format required by apps)
            // Use StringBuilder or simple interpolation
            string wgConfig = "[Interface]\r\n" +
                              $"ListenPort = {port}\r\n" +
                              $"PrivateKey = {clientPrivateKey}\r\n" +
                              $"Address = {clientAddress}\r\n" +
                              $"MTU = {MTU}\r\n" +
                              "DNS = 8.8.8.8\r\n\r\n" + // Optional DNS
                              "[Peer]\r\n" +
                              $"PublicKey = {serverPublicKey}\r\n" +
                              $"Endpoint = ipdamaquina:{serverEndpoint}\r\n" +
                              "AllowedIPs = 0.0.0.0/0\r\n" +
                              "PersistentKeepalive = 25";

            richTextBox1.Text = wgConfig; // Display the config string in the RichTextBox

            // 3. Generate the QR Code from that exact string
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                // ECCLevel.M or Q is usually best for scanning from screens
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(wgConfig, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeImage = qrCode.GetGraphic(10); // 10 is the pixel size per module

                    using (MemoryStream ms = new MemoryStream(qrCodeImage))
                    {
                        // 4. Display in the PictureBox
                        pictureBox1.Image = Image.FromStream(ms);
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
            }

        }
    }
}
