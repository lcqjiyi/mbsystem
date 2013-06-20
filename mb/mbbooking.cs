using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using mb.Model;
using mb.Serve;

namespace mb
{
    public partial class mbbooking : Form
    {

        Dictionary<string, BoxSize> boxSizes = new Dictionary<string,BoxSize>();
        public mbbooking()
        {
            
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            boxSizes.Add("60x40x40", new BoxSize (){ name = "60x40x40", value ="1000", boxweight = 1.6f});
            boxSizes.Add("60x40x30",new BoxSize (){ name = "60x40x30", value ="1001" , boxweight = 1.38f});
            foreach (var item in boxSizes)
            {
                comboBox1.Items.Add(item.Key);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            webBrowser1.Navigate("http://192.168.103.98/irj/portal");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.GetElementById("logonuidfield").SetAttribute("value", "1100335");
            webBrowser1.Document.GetElementById("logonpassfield").SetAttribute("value", "225494");
            webBrowser1.Document.Forms[0].InvokeMember("submit");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://192.168.103.98/irj/servlet/prt/portal/prtroot/com.sap.portal.pagebuilder.IviewModeProxy?iview_id=pcd%3Aportal_content%2FMB%2FVendorPortal%2Frole_v_mb_vendor%2FMB%2FShipping%2FConsignmentChangeApp2&iview_mode=default&NavigationTarget=navurl://9ea23e5cac4294f3acb5a6c9a3857e32");
        }

        PackList packlist;
        private void button5_Click_1(object sender, EventArgs e)
        {
            packlist = WebDbServe.GetPackList(ref webBrowser1);
            packlist.SetMaxQuantity(textBox1.Text);
            packlist.Boxing();
            packlist.BoxZiseNumber = boxSizes[comboBox1.SelectedItem.ToString()].value;
            packlist.Unit = "件";
            packlist.Weight = Convert.ToSingle(textBox2.Text);
            dataGridView1.Rows.Clear();
            foreach (BoxItem box in packlist.BoxItems)
            {
                foreach (GridValueItem GridValue in box.GridValueItems)
                {
                    dataGridView1.Rows.Add(box.BoxMunber, GridValue.GoodColor,
                        GridValue.GridValueSize == "44" ? GridValue.Quantity.ToString() : "",
                        GridValue.GridValueSize == "46" ? GridValue.Quantity.ToString() : "",
                        GridValue.GridValueSize == "48" ? GridValue.Quantity.ToString() : "",
                        GridValue.GridValueSize == "50" ? GridValue.Quantity.ToString() : "",
                        GridValue.GridValueSize == "52" ? GridValue.Quantity.ToString() : "",
                        GridValue.GridValueSize == "54" ? GridValue.Quantity.ToString() : "");
                }
            }
            string path = Application.StartupPath + "\\生成的装箱单\\";
            Directory.CreateDirectory(path);
            path += Guid.NewGuid().ToString() + ".xls";
            ExcelDbServe.PackListToExcel(packlist, boxSizes[comboBox1.SelectedItem.ToString()].boxweight, path);
        }
        
    }
}
