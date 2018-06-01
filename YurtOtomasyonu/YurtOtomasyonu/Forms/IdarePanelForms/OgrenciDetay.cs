using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YurtOtomasyonu.Forms.IdarePanelForms
{
    public partial class OgrenciDetay : Form
    {
        #region FormInitialize
        public OgrenciDetay()
        {
            InitializeComponent();
        }
        #endregion
        #region Variables
        private databaseUser user = new databaseUser();
        #endregion
        private async void OgrenciDetay_Load(object sender, EventArgs e)
        {
            this.Text = IdarePanel.öğrenci.tc+" No Öğrencinin Bilgileri";
            pictureBox1.BackgroundImage = Image.FromStream(await Task.Run(user.ogr_Foto));
            tb1.Text = IdarePanel.öğrenci.tc;
            ArrayList data = await Task.Run(user.ogr_Bilgi);
            tb2.Text = data[0].ToString();
            tb3.Text = data[1].ToString();
            tb4.Text = data[2].ToString();
            tb5.Text = data[3].ToString();
            tb6.Text = data[4].ToString();
            tb7.Text = data[5].ToString();
            tb8.Text = data[6].ToString();
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Form x = new IdarePanel();
            user.garbageCollector();
            this.Close();
            x.Show();
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if(printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font font = new Font("Verdana",11,FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            e.Graphics.DrawString("T.C='"+tb1.Text+"'",font,brush,0,15);
            e.Graphics.DrawString("Ad Soyad='"+tb2.Text+"'", font, brush, 0, 30);
            e.Graphics.DrawString("Telefon='"+tb3.Text+"'", font, brush, 0, 45);
            e.Graphics.DrawString("Doğum Tarihi='"+tb4.Text+"'", font, brush, 0,60);
            e.Graphics.DrawString("Kan Grubu='"+tb5.Text+"'", font, brush, 0, 75);
            e.Graphics.DrawString("Anne Adı='"+tb6.Text+"'", font, brush, 0, 90);
            e.Graphics.DrawString("Baba Adı='"+tb7.Text+"'", font, brush, 0, 105);
            e.Graphics.DrawString("Oda No='"+tb8.Text+"'", font, brush, 0, 120);
            //e.Graphics.DrawImage((Image)pictureBox1.BackgroundImage, 0, 0);
            //Fotoğraf Koyulamıyor Sıkıntı Çıkıyor
        }
    }
}
