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

namespace YurtOtomasyonu.Forms
{
    public partial class OgrEkle : Form
    {
        #region FormInitialize
        public OgrEkle()
        {
            InitializeComponent();
        }
        #endregion
        #region Variables
        private databaseUser user = new databaseUser();
        public static ögrenci öğrenci = new ögrenci();
        private string cinsiyet;
        #endregion
       private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            user.garbageCollector();
            Form x = new IdarePanel();
            this.Close();
            x.Show();
        }
        private void OgrEkle_Load(object sender, EventArgs e)
        {

        }
        private void pictureBox1_Click(object sender, EventArgs e) //Resim Seçme
        {
            xtraOpenFileDialog1.Multiselect = false;
            xtraOpenFileDialog1.Filter = "PNG Files(*.png)|*.png|JPG Files(*.jpg)|*.jpg";
            if(xtraOpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackgroundImage = Image.FromFile(xtraOpenFileDialog1.FileName);
                byte[] byt = File.ReadAllBytes(xtraOpenFileDialog1.FileName);
                öğrenci.resim = byt;
            }
        }
        private void cins_kontrol(object sender, EventArgs e)
        {
            cinsiyet = (sender as RadioButton).Text;
        } //Cinsiyet Kontrolü
        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            Ekle();
        } //Öğrenci Ekleme
        private async void Ekle()
        {
            if (radioButton1.Checked == true || radioButton2.Checked == true)
            {
                if (tb1.Text != "" && tb2.MaskFull == true && tb4.Text != "" && tb5.Text != "" && tb6.Text != "" && tb7.Text != "")
                {
                    öğrenci.adSoyad = tb1.Text;
                    öğrenci.tc = tb7.Text;
                    öğrenci.tel = tb2.Text;
                    öğrenci.dogumTarihi = Convert.ToString(tb3.Value.ToShortDateString());
                    öğrenci.kanGrubu = tb4.Text;
                    öğrenci.anneAdi = tb5.Text;
                    öğrenci.babaAdi = tb6.Text;
                    öğrenci.cinsiyet = cinsiyet;
                    öğrenci.yurtİd = AnaForm.yurtId;
                    bool isOk = false;
                    isOk = await Task.Run(user.ogr_Ekle);
                    user.garbageCollector();
                    if (isOk == true)
                    {
                        MessageBox.Show("Öğrenci Başarıyla Eklendi!", "BAŞARILI", MessageBoxButtons.OK);
                        Form x = new IdarePanel();
                        this.Close();
                        x.Show();
                    }
                    else
                    {
                        MessageBox.Show("Öğrenci Eklenemedi!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen İlgili Yerleri Doldurunuz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Cinsiyet Seçiniz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Ekle();
        }
    }
}
