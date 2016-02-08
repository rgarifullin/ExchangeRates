using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;

namespace ExchangeRate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    GetDataCBR();
                    break;
                case 1:
                    GetDataECB();
                    break;
            }              
        }

        private void GetDataCBR()
        {
            var url = "http://www.cbr.ru/scripts/XML_daily.asp";
            var data = GetXMLData(url);

            dataGridView1.Rows.Clear();
            var parentNode = data.SelectNodes("ValCurs/Valute");
            label2.Text = DateTime.Parse(data["ValCurs"].Attributes["Date"].Value).ToString("yyyy-MM-d");

            foreach (XmlNode node in parentNode)
            {
                dataGridView1.Rows.Add(node["CharCode"].InnerText, node["Value"].InnerText);              
            }

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        private void GetDataECB()
        {
            var url = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
            var data = GetXMLData(url);

            dataGridView1.Rows.Clear();
            var parentNode = data.GetElementsByTagName("Cube");
            label2.Text = DateTime.Parse(parentNode[1].Attributes["time"].Value).ToString("yyyy-MM-d");

            foreach (XmlNode node in parentNode)
            {
                if (node.Attributes.Count == 2)
                    dataGridView1.Rows.Add(node.Attributes["currency"].Value, node.Attributes["rate"].Value);
            }
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        private XmlDocument GetXMLData(string url)
        {
            var data = new XmlDocument();

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            var stream = request.GetRequestStream();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                data.LoadXml(reader.ReadToEnd().ToString());
            }
            stream.Close();
            response.Close();

            return data;
        }
    }
}
