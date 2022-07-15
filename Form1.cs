using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace AzureRecognition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();

            using (HttpResponseMessage response = await client.GetAsync(textBox1.Text))
            {
                response.EnsureSuccessStatusCode();
                using (Stream ImageStream = await response.Content.ReadAsStreamAsync())
                {
                    pictureBox1.Image = Image.FromStream(ImageStream);
                }
            }
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            //Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "f483ed35bc5e42569a906b8eca9ca2c4");

            // Request parameters
            queryString["visualFeatures"] = "Faces,description";
            //queryString["details"] = "25454e5a9b514d29bf42ee4757fff19e";
            queryString["language"] = "en";
            queryString["model-version"] = "latest";
            var uri = "https://msit141resou2.cognitiveservices.azure.com/vision/v3.2/analyze?" + queryString;

            JObject data = new JObject { ["url"] = textBox1.Text };
            string json = JsonConvert.SerializeObject(data);
            StringContent stringContent = new StringContent(json,Encoding.UTF8,"application/json");

            HttpResponseMessage faceresponse = await client.PostAsync(uri, stringContent);
            //faceresponse.EnsureSuccessStatusCode();
            string result = await faceresponse.Content.ReadAsStringAsync();
            //MessageBox.Show(result);
            dynamic faces = JsonConvert.DeserializeObject(result);
            JObject jFaces = faces as JObject;
            string Text = Convert.ToString(jFaces["description"]["captions"][0]["text"]);
            double Confidence = Convert.ToDouble(jFaces["description"]["captions"][0]["confidence"]);
            MessageBox.Show($"內容:{Text},信心指數:{Confidence}");
        }   

    }
}
