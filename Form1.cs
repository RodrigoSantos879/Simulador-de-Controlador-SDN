using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using QRCoder;

namespace LTILAB1
{
    public partial class Form1 : Form
    {
        // Campo agora é readonly conforme solicitado
        private string _baseUri = null;
        private string _username = null;
        private string _password = null;

        public List<Interface> interfaces = new List<Interface>();
        public List<Bridge> bridges = new List<Bridge>();
        public List<Route> routes = new List<Route>();
        public List<Address> addresses = new List<Address>();
        public List<DNSStatic> dnsServers = new List<DNSStatic>();
        public List<WGInterface> wgInterfaces = new List<WGInterface>();

       
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (LoginForm login = new LoginForm())
            {
                // ShowDialog blocks the SDN form until the login window is closed
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // Ensure the IP doesn't already contain https:// before adding it
                    string inputIp = login.BaseUri.Replace("https://", "").Replace("http://", "");
                    this._baseUri = $"https://{inputIp}";
                    this._username = login.Username;
                    this._password = login.Password;
                    label2.Text = $"Connected";
                    label2.ForeColor = Color.Green;
                    RefreshList<Bridge>("Bridge", "/rest/interface/bridge", comboBox1, i => $"{i.Name}");
                    RefreshList<SecurityProfile>("Security Profiles", "/rest/interface/wireless/security-profiles", comboBox2, i => $"{i.Name}");
                    RefreshList<Interface>("wireless interfaces", "/rest/interface/wireless", comboBox3, i => i.Name);
                    RefreshList<Route>("static routes","/rest/ip/route", comboBox4,i => $"{i.DstAddress} via {i.Gateway} (Dist: {i.Distance})");
                    RefreshList<Address>("static routes", "/rest/ip/address", comboBox5, i => $"{i.address} via {i.iNTERFACE} (Network: {i.network})");
                    RefreshList<DHCPServer>("DHCP Servers", "/rest/ip/dhcp-server", comboBox6, i => $"{i.Name}");
                    RefreshList<BridgePort>("Bridge Port", "/rest/interface/bridge/port", comboBox9, i => $"{i.InterfaceName}");
                    MessageBox.Show("Authenticated successfully to SDN Controller.");
                }
                else
                {
                    // User cancelled or login failed
                    Application.Exit();
                }
                
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)
            };

            var client = new RestClient(options);
            var req = new RestRequest("rest/interface", Method.Get);

            var response = client.Execute<List<Interface>>(req);
            if (response.IsSuccessful && response.Data != null)
            {
                ShowDataInNewWindow(response.Data, "Interface List");
            }
            else
            {
                MessageBox.Show($"Erro: {response.StatusCode} - {response.ErrorMessage}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)
            };

            var client = new RestClient(options);
            var req = new RestRequest("rest/interface/wireless", Method.Get);

            var response = client.Execute<List<Interface>>(req);
            if (response.IsSuccessful && response.Data != null)
            {
                ShowDataInNewWindow(response.Data, "Wireless Interface List");
            }
            else
            {
                MessageBox.Show($"Erro: {response.StatusCode} - {response.ErrorMessage}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)
            };

            var client = new RestClient(options);
            var req = new RestRequest("rest/interface/bridge", Method.Get);
            var req2 = new RestRequest("rest/interface/bridge/port", Method.Get);

            var bridgeRes = client.Execute<List<Bridge>>(req);
            var portRes = client.Execute<List<BridgePort>>(req2);
            //if (response.IsSuccessful && response.Data != null)
            //{
            //    ShowDataInNewWindow(response.Data, "Bridge List and Ports associated");
            //}
            //else
            //{
            //    MessageBox.Show($"Erro: {response.StatusCode} - {response.ErrorMessage}");
            //}
            if (bridgeRes.IsSuccessful && portRes.IsSuccessful)
            {
                // This is the list we will actually show in the DataGridView
                List<BridgeViewModel> displayList = new List<BridgeViewModel>();

                // Loop through every Bridge found
                foreach (var bridge in bridgeRes.Data)
                {
                    // Find ports that belong to this specific bridge
                    // Note: Adjust 'bridge.Name' or 'bridge.Id' based on your MikroTik JSON
                    var matchingPorts = portRes.Data.Where(p => p.BridgeName == bridge.Name).ToList();

                    if (matchingPorts.Any())
                    {
                        foreach (var port in matchingPorts)
                        {
                            displayList.Add(new BridgeViewModel
                            {
                                BridgeName = bridge.Name,
                                PortName = port.InterfaceName,
                                PortStatus = port.Disabled == "true" ? "Disabled" : "Active",
                                BridgeRunning = bridge.Running
                            });
                        }
                    }
                    else
                    {
                        // Add the bridge anyway so the user knows it exists even with no ports
                        displayList.Add(new BridgeViewModel
                        {
                            BridgeName = bridge.Name,
                            PortName = "(No Ports Connected)",
                            PortStatus = "N/A",
                            BridgeRunning = bridge.Running
                        });
                    }
                }

                // Now call your runtime window with the MERGED list
                ShowDataInNewWindow(displayList, "Bridge Mapping");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)
                
            };
            var client = new RestClient(options);
            Bridge bridge2 = null;
            var req = new RestRequest("rest/interface/bridge/add", Method.Post);
            Form2 CreatedBridge = new Form2(bridge2);
            if (CreatedBridge.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "name", CreatedBridge.bridge.Name },
                    { "mtu" , CreatedBridge.bridge.Mtu },
                    { "arp" , CreatedBridge.bridge.Arp },
                    { "ageing-time" , CreatedBridge.bridge.AgeingTime }
                };
                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Bridge Interface {CreatedBridge.bridge.Name} Created");
                    RefreshList<Bridge>("Bridge", "/rest/interface/bridge", comboBox1, i => $"{i.Name}");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Creating Bridge Interface canceled by the user.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            string Bridgename = comboBox1.Text;
            var client = new RestClient(options);
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a dns server first!");
                return;
            }
            var req2 = new RestRequest($"rest/interface/bridge/{Bridgename}", Method.Get);
            var response2 = client.Execute<Bridge>(req2);
            Bridge bridge2 = null;
            if (response2.IsSuccessful && response2.Data != null)
            {
                bridge2 = response2.Data;
            }
            var req = new RestRequest($"rest/interface/bridge/{bridge2.Id}", Method.Patch);
            Form2 EditedBridge = new Form2(bridge2);
            if (EditedBridge.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "name", EditedBridge.bridge.Name },
                    { "mtu" , EditedBridge.bridge.Mtu },
                    { "arp" , EditedBridge.bridge.Arp },
                    { "ageing-time" , EditedBridge.bridge.AgeingTime }
                };
                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Bridge Interface {EditedBridge.bridge.Name} Edited");
                    RefreshList<Bridge>("Bridge", "/rest/interface/bridge", comboBox1, i => $"{i.Name}");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Editing canceled by the user.");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            string Bridgename = comboBox1.Text;
            var client = new RestClient(options);
            var req = new RestRequest($"rest/interface/bridge/{Bridgename}", Method.Delete);
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"Bridge Interface {Bridgename} Deleted");
                RefreshList<Bridge>("Bridge", "/rest/interface/bridge", comboBox1, i => $"{i.Name}");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }
        private void button34_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            BridgePort bridge2 = new BridgePort();
            var req2 = new RestRequest($"rest/interface/bridge", Method.Get);
            var response2 = client.Execute<List<Bridge>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                bridges = response2.Data;
            }
            var req3 = new RestRequest($"rest/interface", Method.Get);
            var response3 = client.Execute<List<Interface>>(req3);
            if (response3.IsSuccessful && response3.Data != null)
            {
                interfaces = response3.Data;
            }
            var req = new RestRequest("rest/interface/bridge/port/add", Method.Post);
            BridgePortForm CreatedBridgePort = new BridgePortForm(bridges, interfaces, bridge2);
            if (CreatedBridgePort.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "interface", CreatedBridgePort.bridgePort.InterfaceName},
                    { "bridge", CreatedBridgePort.bridgePort.BridgeName}
                };
                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Bridge Port {CreatedBridgePort.bridgePort.InterfaceName} Created");
                    RefreshList<BridgePort>("Bridge Port", "/rest/interface/bridge/port", comboBox9, i => $"{i.InterfaceName}");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {

                MessageBox.Show("Editing canceled by the user.");
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            if (comboBox9.Text == null)
            {
                MessageBox.Show("Please select a bridge port first!");
                return;
            }
            string BPname = comboBox9.Text;
            BridgePort bridge2 = new BridgePort();
            var req3 = new RestRequest($"rest/interface/bridge/port?interface={BPname}", Method.Get);
            var response3 = client.Execute<List<BridgePort>>(req3);
            if (response3.IsSuccessful && response3.Data != null)
            {
                bridge2 = response3.Data.FirstOrDefault(bp => bp.InterfaceName == BPname);
            }else
            {
                string errorContent = response3.Content ?? "Sem conteúdo de erro";
                string errorMsg = response3.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response3.StatusCode} - {errorMsg}\n{errorContent}");
                return;
            }
            var req2 = new RestRequest($"rest/interface/bridge", Method.Get);
            var response2 = client.Execute<List<Bridge>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                bridges = response2.Data;
            }
            var req4 = new RestRequest($"rest/interface", Method.Get);
            var response4 = client.Execute<List<Interface>>(req4);
            if (response4.IsSuccessful && response4.Data != null)
            {
                interfaces = response4.Data;
            }
            var req = new RestRequest($"rest/interface/bridge/port/{bridge2.Id}", Method.Patch);
            BridgePortForm EditedBridgePort = new BridgePortForm(bridges,interfaces,bridge2);
            if (EditedBridgePort.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "interface", EditedBridgePort.bridgePort.InterfaceName},
                    { "bridge", EditedBridgePort.bridgePort.BridgeName}
                };
                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Bridge Port {EditedBridgePort.bridgePort.InterfaceName} Edited");
                    RefreshList<BridgePort>("Bridge Port", "/rest/interface/bridge/port", comboBox9, i => $"{i.InterfaceName}");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {

                MessageBox.Show("Editing canceled by the user.");
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            if (comboBox9.Text == null)
            {
                MessageBox.Show("Please select a bridge port first!");
                return;
            }
            string BPname = comboBox9.Text;
            BridgePort bridgePort = new BridgePort();
            var req2 = new RestRequest($"rest/interface/bridge/port?interface={BPname}", Method.Get);
            var response2 = client.Execute<List<BridgePort>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                bridgePort = response2.Data.FirstOrDefault(bp => bp.InterfaceName == BPname);
            }
            var req = new RestRequest($"rest/interface/bridge/port/{bridgePort.Id}", Method.Delete);
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"Bridge Port {BPname} Deleted");
                RefreshList<BridgePort>("Bridge Port", "/rest/interface/bridge/port", comboBox9, i => $"{i.InterfaceName}");
            }else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }            
        }



        private void button7_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            SecurityProfile sp = new SecurityProfile();
            var client = new RestClient(options);
            var req = new RestRequest("rest/interface/wireless/security-profiles/add", Method.Post);
            Form3 CreatedSP = new Form3(sp);
            if (CreatedSP.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "name", CreatedSP.secprof.Name },
                    { "mode", CreatedSP.secprof.Mode },
                    { "authentication-types", CreatedSP.secprof.AuthenticationTypes },
                    { "unicast-ciphers", CreatedSP.secprof.UnicastCiphers },
                    { "group-ciphers", CreatedSP.secprof.GroupCiphers },
                    { "wpa-pre-shared-key", CreatedSP.secprof.WpaPreSharedKey },
                    { "wpa2-pre-shared-key", CreatedSP.secprof.Wpa2PreSharedKey },
                    { "group-key-update", CreatedSP.secprof.GroupKeyUpdate },
                    { "management-protection", CreatedSP.secprof.ManagementProtection },
                    { "management-protection-key", CreatedSP.secprof.ManagementProtectionKey },
                    { "disable-pmkid", CreatedSP.secprof.DisablePmkid }
                };
                req.AddJsonBody(body);
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Security Profile {CreatedSP.secprof.Name} Created");
                    RefreshList<Bridge>("Security Profiles", "/rest/interface/wireless/security-profiles", comboBox2, i => $"{i.Name}");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            SecurityProfile sp = new SecurityProfile();
            string SPname = comboBox2.Text;
            var client = new RestClient(options);
            var req2 = new RestRequest($"rest/interface/wireless/security-profiles/{SPname}", Method.Get);
            var response2 = client.Execute<SecurityProfile>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                sp = response2.Data;
            }
            var req = new RestRequest($"rest/interface/wireless/security-profiles/{sp.Id}", Method.Patch);
            Form3 EditedSP = new Form3(sp);
            if (EditedSP.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "name", EditedSP.secprof.Name },
                    { "mode", EditedSP.secprof.Mode },
                    { "authentication-types", EditedSP.secprof.AuthenticationTypes },
                    { "unicast-ciphers", EditedSP.secprof.UnicastCiphers },
                    { "group-ciphers", EditedSP.secprof.GroupCiphers },
                    { "wpa-pre-shared-key", EditedSP.secprof.WpaPreSharedKey },
                    { "wpa2-pre-shared-key", EditedSP.secprof.Wpa2PreSharedKey },
                    { "group-key-update", EditedSP.secprof.GroupKeyUpdate },
                    { "management-protection-key", EditedSP.secprof.ManagementProtectionKey },
                    { "disable-pmkid", EditedSP.secprof.DisablePmkid }
                };
                req.AddBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Security Profile {EditedSP.secprof.Name} Edited");
                    RefreshList<Bridge>("Security Profiles", "/rest/interface/wireless/security-profiles", comboBox2, i => $"{i.Name}");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Editing canceled by the user.");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            string SPname = comboBox2.Text;
            var client = new RestClient(options);
            var req = new RestRequest($"rest/interface/wireless/security-profiles/{SPname}", Method.Delete);
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"Security Profile {SPname} Deleted");
                RefreshList<Bridge>("Security Profiles", "/rest/interface/wireless/security-profiles", comboBox2, i => $"{i.Name}");   
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }



        private void button11_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Please select a wireless interface first!");
                return;
            }
            string WNname = comboBox3.Text;
            var client = new RestClient(options);
            var req = new RestRequest($"rest/interface/wireless/{WNname}", Method.Patch);
            req.AddJsonBody(new
            {
                disabled= "true"
            });
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"Wireless Network {WNname} Disabled");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Please select a wireless interface first!");
                return;
            }
            string WNname = comboBox3.Text;
            var client = new RestClient(options);
            var req = new RestRequest($"rest/interface/wireless/{WNname}", Method.Patch);
            req.AddJsonBody(new
            {
                disabled = "false"
            });
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"Wireless Network {WNname} Enabled");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Please select a wireless interface first!");
                return;
            }
            WirelessNetwork wn = new WirelessNetwork();
            string WNname = comboBox3.Text;
            var client = new RestClient(options);
            var req2 = new RestRequest($"rest/interface/wireless/{WNname}", Method.Get);
            var response2 = client.Execute<WirelessNetwork>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                wn = response2.Data;
            }
            List<SecurityProfile> sps = new List<SecurityProfile>();
            var req5 = new RestRequest($"rest/interface/wireless/security-profiles", Method.Get);
            var response5 = client.Execute<List<SecurityProfile>>(req5);
            if (response5.IsSuccessful && response5.Data != null)
            {
                sps = response5.Data;
            }
            var req = new RestRequest($"rest/interface/wireless/{WNname}", Method.Patch);
            WirelessForm EditedWN = new WirelessForm(wn,sps);
            if (EditedWN.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "name", EditedWN.wirenet.Name },
                    { "ssid", EditedWN.wirenet.Ssid },
                    { "security-profile", EditedWN.wirenet.SecurityProfile },
                    { "wps-mode", EditedWN.wirenet.WpsMode },
                    { "hide-ssid", EditedWN.wirenet.HideSsid }
                };
                req.AddBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Wireless Network {EditedWN.wirenet.Name} Edited");
                    RefreshList<Interface>("wireless interfaces","/rest/interface/wireless",comboBox3, i => i.Name);
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Editing canceled by the user.");
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest($"rest/ip/route", Method.Get);
            var response = client.Execute<List<Route>>(req);

            if (response.IsSuccessful && response.Data != null)
            {
                ShowDataInNewWindow(response.Data, "Routing Table");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Error: {response.ErrorMessage}");
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req1 = new RestRequest("rest/interface", Method.Get);
            var response1 = client.Execute<List<Interface>>(req1);
            if (response1.IsSuccessful && response1.Data != null)
            {
                interfaces = response1.Data;
            }
            var req = new RestRequest($"rest/ip/route/add", Method.Post);
            StaticRoute EditedSR = new StaticRoute(interfaces, new Route());
            if (EditedSR.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "dst-address", EditedSR.DstAddress },
                    { "gateway" , EditedSR.Gateway },
                    { "distance" , EditedSR.Distance }
                };

                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"New Route Created");
                    RefreshList<Route>("static routes", "/rest/ip/route", comboBox4, i => $"{i.DstAddress} via {i.Gateway} (Dist: {i.Distance})");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Creating canceled by the user.");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            if (comboBox4.SelectedItem == null)
            {
                MessageBox.Show("Please select a route first!");
                return;
            }
            string SRname = comboBox4.Text;
            var client = new RestClient(options);
            var req1 = new RestRequest("rest/interface", Method.Get);
            var response1 = client.Execute<List<Interface>>(req1);
            if (response1.IsSuccessful && response1.Data != null)
            {
                interfaces = response1.Data;
            }
            var req2 = new RestRequest("rest/ip/route", Method.Get);
            var response2 = client.Execute<List<Route>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                routes = response2.Data;
            }
            Route selectedSR = FindSR(routes, SRname);
            if (selectedSR == null)
            {
                MessageBox.Show("Route not found.");
                return;
            }
            var req = new RestRequest($"rest/ip/route/{selectedSR.Id}", Method.Patch);
            StaticRoute EditedSR = new StaticRoute(interfaces,selectedSR);
            if (EditedSR.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "dst-address", EditedSR.DstAddress },
                    { "gateway" , EditedSR.Gateway },
                    { "distance" , EditedSR.Distance }
                };

                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Route Edited");
                    RefreshList<Route>("static routes", "/rest/ip/route", comboBox4, i => $"{i.DstAddress} via {i.Gateway} (Dist: {i.Distance})");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Editing canceled by the user.");
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            if (comboBox4.SelectedItem == null)
            {
                MessageBox.Show("Please select a route first!");
                return;
            }
            string SRname = comboBox4.Text;
            var client = new RestClient(options);
            var req2 = new RestRequest("rest/ip/route", Method.Get);
            var response2 = client.Execute<List<Route>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                routes = response2.Data;
            }
            Route selectedSR = FindSR(routes, SRname);
            if (selectedSR == null)
            {
                MessageBox.Show("Route not found.");
                return;
            }
            var req = new RestRequest($"rest/ip/route/{selectedSR.Id}", Method.Delete);
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"Route Deleted");
                RefreshList<Route>("static routes", "/rest/ip/route", comboBox4, i => $"{i.DstAddress} via {i.Gateway} (Dist: {i.Distance})");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest($"rest/ip/address", Method.Get);
            var response = client.Execute<List<Address>>(req);

            if (response.IsSuccessful && response.Data != null)
            {
                ShowDataInNewWindow(response.Data, "IP Addresses Table");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Error: {errorMsg}\n{errorContent}");
            }
        }
        private void button19_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req1 = new RestRequest("rest/interface", Method.Get);
            var response1 = client.Execute<List<Interface>>(req1);
            if (response1.IsSuccessful && response1.Data != null)
            {
                interfaces = response1.Data;
            }
            var req = new RestRequest($"rest/ip/address/add", Method.Post);
            AddressForm EditedIPAdd = new AddressForm(interfaces, new Address());
            if (EditedIPAdd.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "address", EditedIPAdd.Address },
                    { "interface" , EditedIPAdd.Gateway }
                };

                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"New IP Address Created");
                    RefreshList<Address>("IP Addresses", "/rest/ip/address", comboBox5, i => $"{i.address} via {i.iNTERFACE} (Network: {i.network})");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Creating canceled by the user.");
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            if (comboBox5.SelectedItem == null)
            {
                MessageBox.Show("Please select a IP Address first!");
                return;
            }
            string IPAddname = comboBox5.Text;
            var client = new RestClient(options);
            var req1 = new RestRequest("rest/interface", Method.Get);
            var response1 = client.Execute<List<Interface>>(req1);
            if (response1.IsSuccessful && response1.Data != null)
            {
                interfaces = response1.Data;
            }
            var req2 = new RestRequest("rest/ip/address", Method.Get);
            var response2 = client.Execute<List<Address>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                addresses = response2.Data;
            }
            Address selectedIPAdd = FindIPAdd(addresses, IPAddname);
            if (selectedIPAdd == null)
            {
                MessageBox.Show("IP Address not found.");
                return;
            }
            var req = new RestRequest($"rest/ip/address/{selectedIPAdd.Id}", Method.Patch);
            AddressForm EditedIPAdd = new AddressForm(interfaces,selectedIPAdd);
            if (EditedIPAdd.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "address", EditedIPAdd.Address },
                    { "interface" , EditedIPAdd.Gateway }
                };

                req.AddJsonBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"New IP Address Created");
                    RefreshList<Address>("IP Addresses", "/rest/ip/address", comboBox5, i => $"{i.address} via {i.iNTERFACE} (Dist: {i.network})");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                 
                MessageBox.Show("Creating canceled by the user.");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            if (comboBox5.SelectedItem == null)
            {
                MessageBox.Show("Please select a IP Address first!");
                return;
            }
            string IPAddname = comboBox5.Text;
            var client = new RestClient(options);
            var req2 = new RestRequest("rest/ip/address", Method.Get);
            var response2 = client.Execute<List<Address>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                addresses = response2.Data;
            }
            Address selectedIPAdd = FindIPAdd(addresses, IPAddname);
            if (selectedIPAdd == null)
            {
                MessageBox.Show("IP Address not found.");
                return;
            }
            var req = new RestRequest($"rest/ip/address/{selectedIPAdd.Id}", Method.Delete);
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"IP Address Deleted");
                RefreshList<Address>("IP Addresses", "/rest/ip/address", comboBox5, i => $"{i.address} via {i.iNTERFACE} (Network: {i.network})");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }

        private void button21_Click(object sender, EventArgs e) 
        {
            string gateway = null, dnsServer = null, id = null, address = null, pId = null, pRange = null;
            DHCPServer dhcpSelecionado = new DHCPServer();
            if (comboBox6.Text == null)
            {
                MessageBox.Show("Selecione um DHCP Server para editar.");
                return;
            }

            
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);

            string dhcpS = comboBox6.Text;
            var req5 = new RestRequest($"rest/ip/dhcp-server?name={dhcpS}", Method.Get);
            var response5 = client.Execute<List<DHCPServer>>(req5);
            if (response5.IsSuccessful && response5.Data != null)
            {
                dhcpSelecionado = response5.Data.FirstOrDefault();
            }


            var req1 = new RestRequest("rest/interface", Method.Get);
            var response1 = client.Execute<List<Interface>>(req1);
            if (response1.IsSuccessful && response1.Data != null)
            {
                interfaces = response1.Data;
            }
            var req2 = new RestRequest("rest/ip/dns/static", Method.Get);
            var response2 = client.Execute<List<DNSStatic>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                dnsServers = response2.Data;
            }
            string dns = string.Join(",", dnsServers.Select(d => d.Name));
            if (dns.EndsWith(","))
            {
                dns = dns.TrimEnd(',');
            }
            var req3 = new RestRequest($"rest/ip/dhcp-server/network?comment={dhcpSelecionado.Name}", Method.Get);
            var response3 = client.Execute<List<Dictionary<string, string>>>(req3);
            if (response3.IsSuccessful && response3.Data != null)
            {
                var Network = response3.Data.FirstOrDefault(n => n.ContainsKey("comment") && n["comment"] == dhcpSelecionado.Name);

                if (Network != null)
                {
                    gateway = Network["gateway"];
                    dnsServer = Network.ContainsKey("dns-server") ? Network["dns-server"] : "";
                    address = Network["address"];
                    id = Network[".id"];
                }
            }
            var req4 = new RestRequest($"rest/ip/pool?name={dhcpSelecionado.AddressPool}", Method.Get);
            var response4 = client.Execute<List<Dictionary<string, string>>>(req4);
            if (response4.IsSuccessful && response4.Data != null)
            {
                var Network = response4.Data.FirstOrDefault(n => n.ContainsKey("name") && n["name"] == dhcpSelecionado.AddressPool);

                if (Network != null)
                {
                    pRange = Network["ranges"];
                    pId = Network[".id"];
                }
            }
            bool dnsEnabled = bool.TryParse(dhcpSelecionado.Disabled, out bool result);
            DHCPForm DHCPCreated = new DHCPForm(interfaces, dhcpSelecionado, gateway, address, pRange, dhcpSelecionado.AddressPool, dnsEnabled);
            if (DHCPCreated.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    // PASSO 1: Editar a IP Pool
                    var poolReq = new RestRequest($"rest/ip/pool/{pId}", Method.Patch);
                    poolReq.AddJsonBody(new
                    {
                        name = DHCPCreated.poolName,
                        ranges = DHCPCreated.poolRange
                    });
                    var responsePool = client.Execute(poolReq);
                    if (!responsePool.IsSuccessful)
                    {
                        string errorContent = responsePool.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responsePool.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro: {responsePool.StatusCode} - {errorMsg}\n{errorContent}");
                        return; // Para aqui para não tentar os passos seguintes
                    }

                    // PASSO 2: Editar o DHCP Server
                    var serverReq = new RestRequest($"rest/ip/dhcp-server/{dhcpSelecionado.Id}", Method.Patch);
                    var body = new Dictionary<string, string>
                    {
                        { "name", DHCPCreated.poolName },
                        { "interface" , DHCPCreated.dhcp.Interface },
                        { "address-pool" , DHCPCreated.poolName }
                    };
                    serverReq.AddJsonBody(body);
                    var responseServerReq = client.Execute(serverReq);
                    if (!responseServerReq.IsSuccessful)
                    {
                        string errorContent = responseServerReq.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responseServerReq.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro: {responseServerReq.StatusCode} - {errorMsg}\n{errorContent}");
                        return;
                    }

                    var netReq = new RestRequest($"rest/ip/dhcp-server/network/{id}", Method.Patch);
                    var body1 = new Dictionary<string, string>
                    {
                        { "address", DHCPCreated.DHCPnetwork },
                        { "gateway" , DHCPCreated.gatewayIp },
                        { "dns-server" , DHCPCreated.DNS == true ? dns : "" },
                        { "comment" , DHCPCreated.dhcp.Name }
                    };
                    netReq.AddJsonBody(body1);
                    var responseNetReq = client.Execute(netReq);
                    if (!responseNetReq.IsSuccessful)
                    {
                        string errorContent = responseNetReq.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responseNetReq.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro ao criar DHCP Server Network: {errorMsg}\n{errorContent}");
                        return; // Para aqui para não tentar os passos seguintes
                    }

                    MessageBox.Show("DHCP Setup concluído com sucesso!");
                    RefreshList<DHCPServer>("DHCP Servers", "/rest/ip/dhcp-server", comboBox6, i => $"{i.Name}");
                }
                catch (Exception ex)
                {
                    string message = ex.Message ?? "Erro desconhecido";
                    MessageBox.Show("Erro no Setup: " + message);
                }
            }
            else
            {
                MessageBox.Show("Edição cancelada pelo utilizador.");
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            string id = null,pId = null;
            DHCPServer dhcpSelecionado = new DHCPServer();
            if (comboBox6.Text == null)
            {
                MessageBox.Show("Selecione um DHCP Server para editar.");
                return;
            }


            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);

            string dhcpS = comboBox6.Text;
            var req5 = new RestRequest($"rest/ip/dhcp-server?name={dhcpS}", Method.Get);
            var response5 = client.Execute<List<DHCPServer>>(req5);
            if (response5.IsSuccessful && response5.Data != null)
            {
                dhcpSelecionado = response5.Data.FirstOrDefault();
            }
            var req4 = new RestRequest($"rest/ip/pool?name={dhcpSelecionado.AddressPool}", Method.Get);
            var response4 = client.Execute<List<Dictionary<string, string>>>(req4);
            if (response4.IsSuccessful && response4.Data != null)
            {
                var Network = response4.Data.FirstOrDefault(n => n.ContainsKey("name") && n["name"] == dhcpSelecionado.AddressPool);

                if (Network != null)
                {
                    pId = Network[".id"];
                }
            }
            var req3 = new RestRequest($"rest/ip/dhcp-server/network?comment={dhcpSelecionado.Name}", Method.Get);
            var response3 = client.Execute<List<Dictionary<string, string>>>(req3);
            if (response3.IsSuccessful && response3.Data != null)
            {
                var Network = response3.Data.FirstOrDefault(n => n.ContainsKey("comment") && n["comment"] == dhcpSelecionado.Name);

                if (Network != null)
                {
                    id = Network[".id"];
                }
            }
                try
                {
                    // PASSO 1: Apagar a IP Pool
                    var poolReq = new RestRequest($"rest/ip/pool/{pId}", Method.Delete);
                    var responsePool = client.Execute(poolReq);
                    if (!responsePool.IsSuccessful)
                    {
                        string errorContent = responsePool.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responsePool.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro ao apagar Pool: {errorMsg}\n{errorContent}");
                        return; 
                    }

                    // PASSO 2: Apagar o DHCP Server
                    var serverReq = new RestRequest($"rest/ip/dhcp-server/{dhcpSelecionado.Id}", Method.Delete);
                    var responseServerReq = client.Execute(serverReq);
                    if (!responseServerReq.IsSuccessful)
                    {
                        string errorContent = responseServerReq.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responseServerReq.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro ao apagar DHCP Server: {responseServerReq.StatusCode} - {errorMsg}\n{errorContent}");
                        return;
                    }

                    var netReq = new RestRequest($"rest/ip/dhcp-server/network/{id}", Method.Delete);
                    var responseNetReq = client.Execute(netReq);
                    if (!responseNetReq.IsSuccessful)
                    {
                        string errorContent = responseNetReq.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responseNetReq.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro ao apagar DHCP Server Network: {responseNetReq.StatusCode} - {errorMsg}\n{errorContent}");
                        return; 
                    }

                    MessageBox.Show("DHCP Setup concluído com sucesso!");
                    RefreshList<DHCPServer>("DHCP Servers", "/rest/ip/dhcp-server", comboBox6, i => $"{i.Name}");
                }
                catch (Exception ex)
                {
                string message = ex.Message ?? "Erro desconhecido";
                MessageBox.Show("Erro a apagar o Servidor DHCP: " + ex.Message);
                }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest($"rest/ip/dhcp-server", Method.Get);
            var response = client.Execute<List<DHCPServer>>(req);

            if (response.IsSuccessful && response.Data != null)
            {
                ShowDataInNewWindow(response.Data, "DHCP Servers Table");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Error: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            DHCPServer dhcpS = new DHCPServer();
            var client = new RestClient(options);
            var req1 = new RestRequest("rest/interface", Method.Get);
            var response1 = client.Execute<List<Interface>>(req1);
            if (response1.IsSuccessful && response1.Data != null)
            {
                interfaces = response1.Data;
            }
            var req2 = new RestRequest("rest/ip/dns/static", Method.Get);
            var response2 = client.Execute<List<DNSStatic>>(req2);
            if (response2.IsSuccessful && response2.Data != null)
            {
                dnsServers = response2.Data;
            }
            string dns = string.Join(",", dnsServers.Select(d => d.Name));
            if (dns.EndsWith(","))
            {
                dns = dns.TrimEnd(',');
            }
            var req = new RestRequest($"rest/ip/dhcp-server/add", Method.Post);
            DHCPForm DHCPCreated = new DHCPForm(interfaces, dhcpS,null ,null, null,null,false);
            if (DHCPCreated.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    // PASSO 1: Criar o IP Pool
                    var poolReq = new RestRequest("rest/ip/pool", Method.Put);
                    poolReq.AddJsonBody(new
                    {
                        name = DHCPCreated.poolName,
                        ranges = DHCPCreated.poolRange
                    });
                    var responsePool = client.Execute(poolReq);
                    if (!responsePool.IsSuccessful)
                    {
                        string errorContent = responsePool.Content ?? "Sem conteúdo de erro";   
                        string errorMsg = responsePool.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro ao criar Pool: {responsePool.StatusCode} - {errorMsg}\n{errorContent}");
                        return; // Para aqui para não tentar os passos seguintes
                    }

                    // PASSO 2: Criar o DHCP Server
                    var serverReq = new RestRequest("rest/ip/dhcp-server", Method.Put);
                    var body = new Dictionary<string, string>
                    {
                        { "name", DHCPCreated.dhcp.Name },
                        { "interface" , DHCPCreated.dhcp.Interface },
                        { "address-pool" , DHCPCreated.poolName },
                        { "lease-time" , DHCPCreated.dhcp.LeaseTime }
                    };
                    serverReq.AddJsonBody(body);
                    var responseServerReq = client.Execute(serverReq);
                    if (!responseServerReq.IsSuccessful)
                    {
                        string errorContent = responseServerReq.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responseServerReq.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro ao criar DHCP Server: {responseServerReq.StatusCode} - {errorMsg}\n{errorContent}");
                        return; // Para aqui para não tentar os passos seguintes
                    }

                    var netReq = new RestRequest("rest/ip/dhcp-server/network", Method.Put);
                    var body1 = new Dictionary<string, string>
                    {
                        { "address", DHCPCreated.DHCPnetwork },
                        { "gateway" , DHCPCreated.gatewayIp },
                        { "dns-server" , DHCPCreated.DNS == true ? dns : "" },
                        { "comment" , DHCPCreated.dhcp.Name }
                    };
                    netReq.AddJsonBody(body1);
                    var responseNetReq = client.Execute(netReq);
                    if (!responseNetReq.IsSuccessful)
                    {
                        string errorContent = responseNetReq.Content ?? "Sem conteúdo de erro";
                        string errorMsg = responseNetReq.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Erro ao criar DHCP Server Network: {responseNetReq.StatusCode} - {errorMsg}\n{errorContent}");
                        return; // Para aqui para não tentar os passos seguintes
                    }

                    MessageBox.Show("DHCP Setup concluído com sucesso!");
                    RefreshList<DHCPServer>("DHCP Servers", "/rest/ip/dhcp-server", comboBox6, i => $"{i.Name}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro no Setup: " + ex.Message);
                }
            }
            else
            {  
                MessageBox.Show("Creating canceled by the user.");
            }
        }


        private void button25_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                // Restarts the app and starts fresh
                Application.Restart();
                label2.Text = "Disconnected";
                label2.ForeColor = Color.Red;
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest($"rest/ip/dns", Method.Patch);
            var body = new Dictionary<string, object>
                {
                    { "allow-remote-requests", "true" }
                };
            req.AddJsonBody(body);
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"DNS Servers Enabled");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }


        private void button28_Click(object sender, EventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest($"/rest/ip/dns", Method.Patch);
            var body = new Dictionary<string, object>
                {
                    { "allow-remote-requests", "false" }
                };
            req.AddJsonBody(body);
            // Executa a requisição
            var response = client.Execute(req);
            if (response.IsSuccessful)
            {
                MessageBox.Show($"DNS Server Disabled");
            }
            else
            {
                string errorContent = response.Content ?? "Sem conteúdo de erro";
                string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            DNServer server = new DNServer();
            var client = new RestClient(options);
            var req1 = new RestRequest($"/rest/ip/dns", Method.Get);
            var response1 = client.Execute<DNServer>(req1);
            if (response1.IsSuccessful && response1.Data != null)
            {
                server = response1.Data;
            }
            var req = new RestRequest($"/rest/ip/dns/set", Method.Post);
            DNSForm DNSEdited = new DNSForm(server);
            if (DNSEdited.ShowDialog() == DialogResult.OK)
            {
                var body = new Dictionary<string, string>
                {
                    { "servers", DNSEdited.dnss.Servers },

                    { "max-udp-packet-size", DNSEdited.dnss.MaxUdpPacketSize },
                    { "query-server-timeout", DNSEdited.dnss.QueryServerTimeout },
                    { "query-total-timeout", DNSEdited.dnss.QueryTotalTimeout },
                };
                req.AddBody(body);
                // Executa a requisição
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"DNS Server Configured");
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else {
                MessageBox.Show("Creating canceled by the user.");
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest($"/rest/interface/wireguard/add", Method.Post);
            WireguardInterface wgI = new WireguardInterface(null,null,null);
            if (wgI.ShowDialog() == DialogResult.OK)
            {
                
                var body1 = new Dictionary<string, object>
                    {
                        { "name", wgI.WGName },
                        { "listen-port" , wgI.LPort },
                        { "mtu" , wgI.MTU }
                    };
                
                req.AddJsonBody(body1);
                var response = client.Execute(req);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Wireguard Interface {wgI.WGName} Configured");
                    var req1 = new RestRequest($"/rest/ip/firewall/filter/add", Method.Post);
                    var body2 = new Dictionary<string, object>
                {
                    { "chain", "input" },
                    { "action", "accept" },
                    { "protocol", "udp" }, // Protocol 17 is UDP
                    { "dst-port", wgI.LPort },
                    { "comment", $"Allow WireGuard Port {wgI.LPort}" },
                    { "place-before", 2 }
                };
                    req1.AddJsonBody(body2);
                    var response1 = client.Execute(req1);

                    if (response1.IsSuccessful)
                    {
                        MessageBox.Show("Firewall rule added successfully!");
                    }
                    else
                    {
                        string errorContent = response1.Content ?? "Sem conteúdo de erro";
                        string errorMsg = response1.ErrorMessage ?? "Erro desconhecido";
                        MessageBox.Show($"Error: {errorMsg}\n{errorContent}");
                    }
                }
                else
                {
                    string errorContent = response.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                MessageBox.Show("Creating canceled by the user.");
            }
        }

     

        private void button29_Click(object sender, EventArgs e)
        {
            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);

            var req = new RestRequest($"/rest/interface/wireguard", Method.Get);
            var response = client.Execute<List<WGInterface>>(req);
            if (response.IsSuccessful && response.Data != null)
            {
                wgInterfaces = response.Data;
            }
            var req2 = new RestRequest($"/rest/interface/wireguard/peers/add", Method.Post);
            PeerForm peerForm = new PeerForm(null, wgInterfaces);
            if (peerForm.ShowDialog() == DialogResult.OK)
            {
                var body1 = new Dictionary<string, string>
                    {
                        { "name", peerForm.peer.Name },
                        { "interface" , peerForm.peer.Interface },
                        { "private-key" , "auto" },
                        { "allowed-address" , peerForm.peer.ClientAddress+"/32" },
                        { "client-address" , peerForm.peer.ClientAddress+"/32" },
                        { "client-allowed-address" , "0.0.0.0/0" },
                        { "endpoint-port" , peerForm.peer.CurrentEndpointPort }
                    };
                req2.AddJsonBody(body1);
                var response2 = client.Execute(req2);
                if (response2.IsSuccessful)
                {
                    MessageBox.Show($"Peer {peerForm.peer.Name} Added");
                    var req6 = new RestRequest($"rest/ip/address/add", Method.Post);
                    string result = GetGatewayAddress(peerForm.peer.ClientAddress + "/24");
                    var body6 = new Dictionary<string, string>
                    {
                        { "address", result },
                        { "interface" , peerForm.peer.Interface  }
                    };
                    req6.AddJsonBody(body6);
                    // Executa a requisição
                    var response6 = client.Execute(req6);
                    if (response6.IsSuccessful) {
                        MessageBox.Show($"Peer {peerForm.peer.ClientAddress} Added to Interface {peerForm.peer.Interface}");
                    }
                        var req3 = new RestRequest($"/rest/interface/wireguard/peers?name={peerForm.peer.Name}", Method.Get);
                    var response3 = client.Execute<List<Peer>>(req3);
                    if (response3.IsSuccessful && response3.Data != null)
                    {
                        Peer peer = response3.Data.FirstOrDefault();
                        if (peer != null)
                        {
                            var req1 = new RestRequest($"/rest/ip/firewall/nat/add", Method.Post);
                            var body2 = new Dictionary<string, string>
                            {
                                { "chain", "srcnat" },
                                { "action", "masquerade" },
                                { "src-address", peer.ClientAddress }
                            };
                            req1.AddJsonBody(body2);
                            var response1 = client.Execute(req1);

                            if (response1.IsSuccessful)
                            {
                                MessageBox.Show("Firewall NAT masquerade rule added successfully!");
                                var req5 = new RestRequest($"/rest/interface/wireguard?name={peer.Interface}", Method.Get);
                                var response5 = client.Execute<List<WGInterface>>(req5);
                                WGInterface wgi = new WGInterface();
                                if (response5.IsSuccessful && response5.Data != null)
                                {
                                    wgi = response5.Data.FirstOrDefault();
                                }
                                ClientConfigForm clientConfigForm = new ClientConfigForm(peer,wgi);
                                clientConfigForm.ShowDialog();
                            }
                            else
                            {
                                string errorContent = response1.Content ?? "Sem conteúdo de erro";
                                string errorMsg = response1.ErrorMessage ?? "Erro desconhecido";
                                MessageBox.Show($"Error: {errorMsg}\n{errorContent}");
                            }
                        }
                    }
                }
                else
                {
                    string errorContent = response2.Content ?? "Sem conteúdo de erro";
                    string errorMsg = response2.ErrorMessage ?? "Erro desconhecido";
                    MessageBox.Show($"Erro: {response2.StatusCode} - {errorMsg}\n{errorContent}");
                }
            }
            else
            {
                MessageBox.Show("Creating canceled by the user.");
            }
        }

  




        private Route FindSR(List<Route> routes, string displayText)
        {
            foreach (var route in routes)
            {
                string routeDisplay = $"{route.DstAddress} via {route.Gateway} (Dist: {route.Distance})";
                if (routeDisplay == displayText)
                {
                    return route;
                }
            }
            return null; // Not found
        }
        private Address FindIPAdd(List<Address> adds, string displayText)  
        {
            foreach (var add in adds)
            {
                string addressDisplay = $"{add.address} via {add.iNTERFACE} (Network: {add.network})";
                if (addressDisplay == displayText)
                {
                    return add;
                }
            }
            return null; // Not found
        }


        private void RefreshList<T>(string type,string endpoint,ComboBox combobox, Func<T,string> displaySelector)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest(endpoint, Method.Get);

            var response = client.Execute<List<T>>(req);

            if (response.IsSuccessful && response.Data != null)
            {
                combobox.Items.Clear();
                foreach (var item in response.Data)
                {
                    // Add the name of the wireless interface (e.g., wlan1) to the combo
                    combobox.Items.Add(displaySelector(item));
                }

                if (combobox.Items.Count > 0)
                    combobox.SelectedIndex = 0; // Select the first one by default
            }
            else
            {
                MessageBox.Show($"Failed to load {type}.");
            }
        }

        
        private void ShowDataInNewWindow(object data, string title)
        {
            // 1. Create a temporary form
            Form popup = new Form
            {
                Text = title,
                Width = 600,
                Height = 400,
                StartPosition = FormStartPosition.CenterParent
            };

            // 2. Create the DataGridView
            DataGridView gv = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = data,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // 3. Add grid to the popup and show it
            popup.Controls.Add(gv);
            popup.ShowDialog(); // Use ShowDialog so the user focuses on the data
        }

            public void UpdateWinboxFooter()
            {
                
            if (_baseUri == null || _username == null || _password == null)
            {
                label3.Text = "CPU: 0%";
                label5.Text = "Free: 0 MB";
                label7.Text = "Uptime: 0s";
                return;
            }
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslErrors) => true;

            var options = new RestClientOptions(_baseUri)
            {
                Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(_username, _password)

            };
            var client = new RestClient(options);
            var req = new RestRequest($"rest/system/resource", Method.Get);
            var response = client.Execute(req);

            if (response.IsSuccessful && response.Content != null)
            {
                dynamic resources = JsonConvert.DeserializeObject(response.Content);
                // Update UI
                label3.Text = $"CPU: {resources["cpu-load"]}%";
                label5.Text = $"Free: {long.Parse((string)resources["free-memory"]) / 1024 / 1024} MB";
                label7.Text = $"Uptime: {resources["uptime"]}";

                // Logic for colors
                label3.ForeColor = (int)resources["cpu-load"] > 80 ? Color.Red : Color.ForestGreen;
                
            }
            else
            {
                label2.Text = $"Disconnected";
                label2.ForeColor = Color.Red;
            }
            
        }

       
        public string GetGatewayAddress(string clientCidr)
        {
            // 1. Split the string into "IP" and "Prefix" (e.g., "192.168.1.5" and "24")
            string[] parts = clientCidr.Split('/');
            if (parts.Length != 2) return clientCidr; // Return original if format is unexpected

            if (IPAddress.TryParse(parts[0], out IPAddress ip))
            {
                byte[] bytes = ip.GetAddressBytes();

                // 2. Ensure it's IPv4 (4 bytes)
                if (bytes.Length == 4)
                {
                    // 3. Change the last octet to 254
                    bytes[3] = 254;

                    IPAddress newIp = new IPAddress(bytes);

                    // 4. Return the new IP with the original /prefix
                    return $"{newIp}/{parts[1]}";
                }
            }

            return clientCidr;
        }



    private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateWinboxFooter();

        }

        private void button25_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // 2. Clear sensitive credentials
                this._baseUri = null;
                this._username = null;
                this._password = null;

                // 3. Update UI Elements to reflect disconnected state
                label2.Text = "Disconnected";
                label2.ForeColor = Color.Red;

                // 4. Clear ComboBoxes and internal lists for security
                comboBox1.DataSource = null;
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();
                comboBox6.Items.Clear();

                interfaces.Clear();
                routes.Clear();
                addresses.Clear();
                dnsServers.Clear();
                wgInterfaces.Clear();

                
                Form1_Load(this, EventArgs.Empty);

                // Ensure the form shows back up if login is successful
                if (this._baseUri != null)
                {
                    this.Show();
                }
            }
        }
    }
}