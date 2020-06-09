using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace soft_cold_mock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Text = "Start";
            //label1.Visible = false;
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                timer1.Enabled = true;
                button1.Text = "Stop";
                label1.Text = "";
                textBox1.Enabled = false;
            }
            else
            {
                timer1.Enabled = false;
                button1.Text = "Start";
                textBox1.Enabled = true;
            }
        }

        private string url_action = "http://ddesmintyserver.ml:5002/action"; //local test: http://127.0.0.1:5002/action
        private string url_sensor = "http://ddesmintyserver.ml:5002/sensor";

        private string temp = "0";
        private void timer1_Tick(object sender, EventArgs e)
        {
            using (var webClient = new WebClient())
            {
                string header = "D-ID: " + textBox1.Text;
                webClient.Headers.Add(header);

                try
                {
                    string json = "{\"temp\":" + textBox2.Text + "}";
                    //webClient.Encoding = System.Text.Encoding.UTF8;
                    string reply = webClient.UploadString(url_sensor, json);
                }
                catch { }
            }

            using (var webClient = new WebClient())
            {
                string header = "D-ID: " + textBox1.Text;
                webClient.Headers.Add(header); //D-ID: room1

                try
                {
                    Stream data = webClient.OpenRead(url_action);
                    StreamReader reader = new StreamReader(data);
                    string response = reader.ReadToEnd();
                    data.Close();
                    reader.Close();

                    try
                    {
                        var values = JsonConvert.DeserializeObject<Dictionary<string, float>>(response);
                        int x = Int32.Parse(values["temp"].ToString());
                        if (x > 0 && x < 100)
                        {
                            temp = x.ToString();
                        }
                        else
                        {
                            //label1.Text = x.ToString();
                        }
                    }
                    catch { }

                    //label1.Text = response;
                    label3.Text = temp;
                }
                catch (WebException exeption)
                {
                    label1.Text = "Connection error";//exeption.ToString();
                    timer1.Enabled = false;
                    textBox1.Enabled = true;
                    button1.Text = "Start";
                }
            }
        }
    }
}
