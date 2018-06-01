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
using YurtOtomasyonu.Forms.IdarePanelForms;

namespace YurtOtomasyonu.Forms
{
    public partial class IdarePanel : Form
    {
        #region Form Initialize
        public IdarePanel()
        {
            InitializeComponent();
        }
        #endregion
        #region Variables
        private databaseUser user = new databaseUser();
        public static oda oda = new oda();
        public static ögrenci öğrenci = new ögrenci();
        public static Devamsızlık devamsizlik = new Devamsızlık();
        public static Ziyaretciler ziyaretci = new Ziyaretciler();
        public static Aidat aidat = new Aidat();
        public static Raporlar rapor = new Raporlar();
        private bool oda_Ekle = false;
        private byte[] pdfBytes = null;
        #endregion
        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)//Sayfa Her Değiştiğinde Bellek Tazelensin
        {
            if(this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            user.garbageCollector();
            switch(xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    dataGrid1();
                    break;
                case 1:
                    dataGrid2();
                    break;
                case 2:
                    dataGrid3();
                    break;
                case 3:
                    dataGrid5();
                    break;
                case 4:
                    dataGrid4();
                    break;
                case 5: // PDF Açma
                    customOpenFile.Multiselect = false;
                    customOpenFile.Filter = "PDF Files (*.pdf)|*.pdf";
                    if(customOpenFile.ShowDialog() == DialogResult.OK)
                    {
                        this.WindowState = FormWindowState.Maximized;
                        axAcroPDF1.LoadFile(customOpenFile.FileName);
                    }
                    break;
                case 6:
                    dataGrid6();
                    break;
            }
        }
        private void ribbonControl1_SelectedPageChanged(object sender, EventArgs e)//Ribbon Sayfası Her Değiştiğinde Bellek Tazelensin
        {
            user.garbageCollector();
        }
        private async void IdarePanel_Load(object sender, EventArgs e)
        {
            user.garbageCollector();
            veri_cek("Hepsi");
            if(dataGridView1.Rows.Count>0)
            {
                dataGrid1();
            }
            if(DateTime.Now.Day.ToString()=="1")
            {
                progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                bool isOk = false;
                isOk = await Task.Run(user.aidat_Ekle);
                if(isOk == true)
                {
                    MessageBox.Show("Yeni Aidatlar Sisteme İşlendi!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Yeni Aidatlar Sisteme İşlenemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        private async void veri_cek(string secim) //Veri Çekme Metodu
        {
            try
            {
               if(secim == "Hepsi")
                {
                    dataGridView1.DataSource = await Task.Run(user.odalari_Cek);
                    dataGridView2.DataSource = await Task.Run(user.ogr_cek);
                    dataGridView3.DataSource = await Task.Run(user.devamsizlik_cek);
                    dataGridView4.DataSource = await Task.Run(user.pdf_cek);
                    dataGridView5.DataSource = await Task.Run(user.ziyaretci_cek);
                    dataGridView6.DataSource = await Task.Run(user.aidat_Cek);
                }
                else if(secim == "Oda")
                {
                    dataGridView1.DataSource = await Task.Run(user.odalari_Cek);
                }else if(secim == "Öğrenci")
                {
                    dataGridView2.DataSource = await Task.Run(user.ogr_cek);
                }
                else if(secim  == "Devamsizlik")
                {
                    dataGridView3.DataSource = await Task.Run(user.devamsizlik_cek);
                }else if(secim == "Ziyaretci")
                {
                    dataGridView5.DataSource = await Task.Run(user.ziyaretci_cek);
                }else if(secim == "Rapor")
                {
                    dataGridView4.DataSource = await Task.Run(user.pdf_cek);
                }else if(secim == "Aidat")
                {
                    dataGridView6.DataSource = await Task.Run(user.aidat_Cek);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void dataGrid1()
        {
            if (dataGridView1.Rows.Count>0)
            {
                dataGridView1.Columns[0].HeaderText = "Oda No";
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Height = 35;
                }
            }
        } //DataGrid1 İsimlendirme
        private void dataGrid2()
        {
            if (dataGridView2.Rows.Count>0)
            {
                dataGridView2.Columns[0].HeaderText = "T.C No";
                dataGridView2.Columns[1].HeaderText = "Ad Soyad";
                dataGridView2.Columns[2].HeaderText = "Telefon";
                dataGridView2.Columns[3].HeaderText = "Doğum Tarihi";
                dataGridView2.Columns[4].HeaderText = "Kan Grubu";
                dataGridView2.Columns[5].HeaderText = "Anne Adı";
                dataGridView2.Columns[6].HeaderText = "Baba Adı";
                dataGridView2.Columns[7].HeaderText = "Cinsiyet";
                dataGridView2.Columns[8].HeaderText = "Oda No";
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    dataGridView2.Rows[i].Height = 35;
                }

            }
        } //DataGrid2 İsimlendirme
        private void dataGrid3() //DataGrid3 İsimlendirme
        {
            if (dataGridView3.Rows.Count>0)
            {
                dataGridView3.Columns[0].HeaderText = "Öğrenci T.C";
                dataGridView3.Columns[1].HeaderText = "Devamsızlık Tarihi";
                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    dataGridView3.Rows[i].Height = 35;
                }
            }
        }
        private void dataGrid4()//DataGrid4 İsimlendirme
        {
            if (dataGridView4.Rows.Count>0)
            {
                dataGridView4.Columns[0].HeaderText = "Rapor Adı";
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    dataGridView4.Rows[i].Height = 35;
                }
            }
        }
        private void dataGrid5()
        {
            if(dataGridView5.Rows.Count>0)
            {
                dataGridView5.Columns[0].HeaderText = "Ziyaretçi Adı";
                dataGridView5.Columns[1].HeaderText = "Öğrenci Adı";
                dataGridView5.Columns[2].HeaderText = "Öğrenci T.C";
                dataGridView5.Columns[3].HeaderText = "Tarih";
                for(int i=0;i<dataGridView5.Rows.Count;i++)
                {
                    dataGridView5.Rows[i].Height = 35;
                }
            }
        } //DataGrid 5 İsimlendirme
        private void dataGrid6() //DataGrid 6 İsimlendirme
        {
           if(dataGridView6.Rows.Count>0)
            {
                dataGridView6.Columns[0].HeaderText = "Öğrenci TC";
                dataGridView6.Columns[1].HeaderText = "Ay";
                dataGridView6.Columns[2].HeaderText = "Yıl";
                dataGridView6.Columns[3].HeaderText = "Tutar";
                dataGridView6.Columns[4].HeaderText = "Ödeme Durumu";
                for(int i=0;i<dataGridView6.Rows.Count;i++)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    if(dataGridView6.Rows[i].Cells[4].Value.ToString()=="HAYIR")
                    {
                        style.BackColor = Color.Red;
                    }
                    else
                    {
                        style.BackColor = Color.Green;
                    }
                    dataGridView6.Rows[i].Height = 35;
                    dataGridView6.Rows[i].DefaultCellStyle = style;
                }
            }

        }
        private async void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Oda Oluşturma 
        {
            barButtonItem1.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(odaNoTxt.Text!="")
            {
                bool isOk = false;
                oda.odaNo = odaNoTxt.Text;
                isOk = await Task.Run(user.odaOlustur);
                if(isOk == true)
                {
                    MessageBox.Show(oda.odaNo+" Numaralı Oda Başarıyla Oluşturuldu!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Böyle Bir Oda Oluşturulamadı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Oda No Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem1.Enabled = true;
        }
        private async void textBox1_KeyPress(object sender, KeyPressEventArgs e) //Öğrenci T.c Kimlik Kontrol
        {
            
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(e.KeyChar == 13&&ogrTc1.TextLength==11)
            {
                ogrTc1.Enabled = false;
                bool isOk = false;
                öğrenci.tc = ogrTc1.Text;
                isOk = await Task.Run(user.ogr_odaKontrol);
                if(isOk == true)
                {
                    MessageBox.Show("Öğrenci Odaya Başarıyla Yerleştirilebilir!","BAŞARILI",MessageBoxButtons.OK);
                    oda_Ekle = true;
                }
                else
                {
                    MessageBox.Show("Öğrenci Odaya Yerleştirilemez!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    isOk = false;
                    isOk = await Task.Run(user.ogr_kontrol);
                    user.garbageCollector();
                    if(isOk == true)
                    {
                        //Kayıt Yapılacak
                    }
                    else
                    {
                        //Öğrenci Oluşturulacak
                        Form x = new OgrEkle();
                        this.Hide();
                        x.Show();
                    }

                }
                ogrTc1.Enabled = true;
            }     
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//Odaların Listesini Güncelle
        {
            barButtonItem4.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            veri_cek("Oda");
            barButtonItem4.Enabled = true;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private void barButtonItem26_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//Öğrenci Ekleme Paneli Açılır
        {
            user.garbageCollector();
            Form x = new OgrEkle();
            this.Hide();
            x.Show();
        }
        private void barButtonItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Öğrenciler Listesini Güncelleme
        {
            barButtonItem27.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            veri_cek("Öğrenci");
            barButtonItem27.Enabled = true;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

        }
        private async void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Oda Silme
        {
            barButtonItem2.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView1.SelectedRows.Count>0)
            {
                DialogResult result = MessageBox.Show(dataGridView1.CurrentRow.Cells[0].Value.ToString()+" Numaralı Odayı Silmek İstediğinize Emin Misiniz!","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    oda.odaNo = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    bool isOk = false;
                    isOk = await Task.Run(user.oda_sil);
                    if (isOk == true)
                    {
                        MessageBox.Show("Silme İşlemi Başarıyla Yapıldı!", "BAŞARILI", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Silme İşlemi Başarısız!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen Bir Oda Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem2.Enabled = true;
        }
        private async void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Oda Güncelleme
        {
            barButtonItem3.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(odaNoTxt.Text!=""&&dataGridView1.SelectedRows.Count>0)
            {
                DialogResult result = MessageBox.Show(dataGridView1.CurrentRow.Cells[0].Value.ToString()+" Numaralı Odayı "+odaNoTxt.Text+" Numarasıyla Güncellemek İstiyor Musunuz!","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    oda.odaNo = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    oda.yeniOdaNo = odaNoTxt.Text;
                    bool isOk = false;
                    isOk = await Task.Run(user.oda_guncelle);
                    if(isOk ==true)
                    {
                        MessageBox.Show("Oda Numaranız Başarıyla Güncelledi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Oda Numaranız Güncellenemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                if(odaNoTxt.Text=="")
                {
                    MessageBox.Show("Güncellenecek Odanın Yeni Numarasını Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Güncellenecek Odayı Seçiniz!", "HATA", MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem3.Enabled = true;
        }
        private async void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Öğrenci Odaya Yerleştirme
        {
            barButtonItem6.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(ogrTc1.Text!=""&&dataGridView1.SelectedRows.Count>0&&oda_Ekle==true&&ogrTc1.TextLength==11)
            {
                oda.odaNo = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                öğrenci.tc = ogrTc1.Text;
                bool isOk = false;
                isOk = await Task.Run(user.ogr_yerlestir);
                if(isOk == true)
                {
                    MessageBox.Show("Öğrenci Odaya Başarıyla Yerleştirildi!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Öğrenci Odaya Yerleştirilemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                oda_Ekle = false;
            }
            else
            {
                if(ogrTc1.Text=="")
                {
                    MessageBox.Show("Lütfen Öğrenci T.C numarasını Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else if(dataGridView1.SelectedRows.Count==0)
                {
                    MessageBox.Show("Lütfen Oda Numarasını Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lütfen Öğrenciyi Kontrol Ediniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem6.Enabled = true;
        }
        private async void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Öğrenciyi Bulunduğu Odadan Siler!
        {
            if(dataGridView2.SelectedRows.Count>0)
            {
                DialogResult result = MessageBox.Show("Öğrenciyi Odadan Çıkarmak İstediğinize Emin Misiniz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    öğrenci.tc = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                    bool isOk = false;
                    isOk = await Task.Run(user.ogr_odaSil);
                    if (isOk == true)
                    {
                        MessageBox.Show("Öğreci Odadan Başarıyla Silindi!", "BAŞARILI", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Öğrenci Odadan Silinemedi!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen Öğrenci Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Öğrencileri Yazdırma
        {
            barButtonItem9.Enabled = false;
            if(dataGridView2.Rows.Count>0)
            {
                if(ogrPriviewDialog.ShowDialog()== DialogResult.OK)
                {
                    ogrDocument.Print();
                }
            }
            else
            {
                MessageBox.Show("Yazdırılabilicek Öğrenci Bulunamadı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem9.Enabled = true;
        }
        private void ogrDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font font = new Font("Verdana",11,FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            e.Graphics.DrawString("Öğrenci Sayısı:"+dataGridView2.RowCount,font,brush,650,0);
            int y = 25;
            for(int i=0;i<dataGridView2.RowCount;i++)
            {
                e.Graphics.DrawString("Öğrenci T.C="+dataGridView2.Rows[i].Cells[0].Value.ToString(),font,brush,0,y);
                y += 15;
                e.Graphics.DrawString("Öğrenci Oda No=" + dataGridView2.Rows[i].Cells[8].Value.ToString(), font, brush, 0, y);
                y += 30;
            }
        }//Yazdırma Modülü
        private async void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Öğrenci Devamsızlık Ekleme
        {
            barButtonItem10.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(devamsizlikTc.Text!=""&&devamsizlikTc.TextLength==11)
            {
                öğrenci.tc = devamsizlikTc.Text;
                devamsizlik.devamsizlikTarihi = dateTimePicker1.Value.ToShortDateString();
                devamsizlik.devamsizlikYili = dateTimePicker1.Value.Year.ToString();
                bool isOk = false;
                isOk = await Task.Run(user.devamsizlik_ekle);
                if(isOk == true)
                {
                    MessageBox.Show("Devamsızlık Başarıyla Sisteme Kayıt Edildi!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Devamsızlık Sisteme Kaydedilemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Verileri Doğru Bir Şekilde Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem10.Enabled = true;
        }
        private void barButtonItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem19.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            veri_cek("Devamsizlik");
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem19.Enabled = true;
        }
        private async void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Devamsızlık Silme
        {
            barButtonItem11.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView3.SelectedRows.Count>0)
            {
                DialogResult result = MessageBox.Show(dataGridView3.CurrentRow.Cells[0].Value.ToString()+" T.C Numaralı Öğrencinin "+dataGridView3.CurrentRow.Cells[1].Value.ToString()+" Tarihindeki Devamsızlığını Silmek İstiyor Musunuz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    öğrenci.tc = dataGridView3.CurrentRow.Cells[0].Value.ToString();
                    devamsizlik.devamsizlikTarihi = dataGridView3.CurrentRow.Cells[1].Value.ToString();
                    bool isOk = false;
                    isOk = await Task.Run(user.devamsizlik_sil);
                    if(isOk == true)
                    {
                        MessageBox.Show("Devamsizlik Başarıyla Silindi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Devamsizlik Silme İşlemi Başarısız!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen Bir Devamsızlık Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem11.Enabled = true;
        }
        private void IdarePanel_FormClosed(object sender, FormClosedEventArgs e) // Form Kapatma
        {
            //Environment.Exit(Environment.ExitCode); Bazen ASYNC Metodların Yapısını Bozuyor
            Application.Exit();
        }
        private async void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Devamsızlık Güncelleme
        {
            barButtonItem13.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView3.SelectedRows.Count>0)
            {
                if(dateTimePicker1.Value.DayOfYear<=DateTime.Now.DayOfYear)
                {
                    DialogResult result = MessageBox.Show(dataGridView3.CurrentRow.Cells[0].Value.ToString() + " T.C Numaralı Öğrencinin '" + dataGridView3.CurrentRow.Cells[1].Value.ToString() + "' Günü Olan Devamsızlığını '" + dateTimePicker1.Value.ToShortDateString() + "' Olarak Güncellemek İstiyor Musunuz?", "EMİN MİSİNİZ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        öğrenci.tc = dataGridView3.CurrentRow.Cells[0].Value.ToString();
                        devamsizlik.devamsizlikTarihi = dataGridView3.CurrentRow.Cells[1].Value.ToString();
                        devamsizlik.devamsizlikGüncellenecek = dateTimePicker1.Value.ToShortDateString();
                        bool isOk = false;
                        isOk = await Task.Run(user.devamsizlik_güncelle);
                        if(isOk == true)
                        {
                            MessageBox.Show("Devamsızlık Başarıyla Güncellendi!","BAŞARILI",MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show("Devamsizlik Güncellenirken Hataya Rastlandı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("İleri Tarihe Devamsizlik Giremezsiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Güncellenecek Devamsızlığı Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            dataGrid3();
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem13.Enabled = true;
        }
        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Devamsizlik Yazdırma Ön İzleme
        {
            if(dataGridView3.Rows.Count>0)
            {
                if (devamsizlikPreviewDialog.ShowDialog() == DialogResult.OK)
                {
                    devamsizlikDoc.Print();
                }
            }
            else
            {
                MessageBox.Show("Yazdırılacak Devamsızlık Yok!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void devamsizlikDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) //Yazdırma
        {
            Font font = new Font("Verdana",11,FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            e.Graphics.DrawString("Tarih:"+DateTime.Now.ToShortDateString(),font,brush,650,0);
            int y = 20;
            for(int i=0;i<dataGridView3.Rows.Count;i++)
            {
                e.Graphics.DrawString("Öğrenci T.C="+dataGridView3.Rows[i].Cells[0].Value.ToString(),font,brush,0,y);
                y += 15;
                e.Graphics.DrawString("Devamsızlık Zamanı="+dataGridView3.Rows[i].Cells[1].Value.ToString(),font,brush,0,y);
                y += 15;
                e.Graphics.DrawString("---------------------------------------------", font, brush, 0, y);
                y += 15;
            }
        }
        private async void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Ziyaretçi Ekleme İşlemi
        {
            barButtonItem14.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if (ziyaretciTb1.Text!=""&&ziyaretciTb2.Text!=""&&ziyaretciTb3.Text!=""&&ziyaretciTb3.TextLength==11)
            {
                ziyaretci.ziyaretciAd = ziyaretciTb1.Text;
                ziyaretci.ogrAd = ziyaretciTb2.Text;
                ziyaretci.ogrTc = ziyaretciTb3.Text;
                bool isOk = false;
                isOk = await Task.Run(user.ziyaretci_ekle);
                if(isOk == true)
                {
                    MessageBox.Show("Ziyaretçi Sisteme Başarıyla Eklendi!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Ziyaretçi Sisteme Eklenemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Verileri İstenilen Şekilde Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem14.Enabled = true;
        }
        private void barButtonItem17_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//Ziyaretçi Listesinin Güncellenmesi
        {
            barButtonItem17.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            veri_cek("Ziyaretci");
            if(dataGridView5.Rows.Count>0)
            {
                dataGrid5();
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem17.Enabled = true;
        }
        private async void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//Ziyaretçi Silme
        {
            barButtonItem15.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if (dataGridView5.SelectedRows.Count>0)
            {
                DialogResult result = MessageBox.Show(dataGridView5.CurrentRow.Cells[0].Value.ToString()+" Adlı Ziyaretçi Kaydını Silmek İstiyor Musunuz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    ziyaretci.ziyaretciAd = dataGridView5.CurrentRow.Cells[0].Value.ToString();
                    ziyaretci.ogrAd = dataGridView5.CurrentRow.Cells[1].Value.ToString();
                    ziyaretci.ogrTc = dataGridView5.CurrentRow.Cells[2].Value.ToString();
                    ziyaretci.tarih = dataGridView5.CurrentRow.Cells[3].Value.ToString();
                    bool isOk = false;
                    isOk = await Task.Run(user.ziyaretci_sil);
                    if(isOk==true)
                    {
                        MessageBox.Show("Ziyaretçi Başarıyla Silindi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Ziyaretçi Silinirken Bir Hataya Rastlandı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Verileri Lütfen Doğru Şekilde Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem15.Enabled = true;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private async void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem16.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(ziyaretciTb1.Text!=""&&dataGridView5.SelectedRows.Count>0)
            {
                DialogResult result = MessageBox.Show(dataGridView5.CurrentRow.Cells[0].Value.ToString() + " Adlı Ziyaretçi Kaydını Silmek İstiyor Musunuz?", "EMİN MİSİNİZ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    ziyaretci.ziyaretciAd = dataGridView5.CurrentRow.Cells[0].Value.ToString();
                    ziyaretci.ogrAd = dataGridView5.CurrentRow.Cells[1].Value.ToString();
                    ziyaretci.ogrTc = dataGridView5.CurrentRow.Cells[2].Value.ToString();
                    ziyaretci.tarih = dataGridView5.CurrentRow.Cells[3].Value.ToString();
                    ziyaretci.ziyaretciAdG = ziyaretciTb1.Text;
                    bool isOk = false;
                    isOk = await Task.Run(user.ziyaretci_güncelle);
                    if (isOk == true)
                    {
                        MessageBox.Show("Ziyaretçi Başarıyla Güncellendi!", "BAŞARILI", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Ziyaretçi Güncellenirken Bir Hataya Rastlandı!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Verileri Lütfen Doğru Şekilde Giriniz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            barButtonItem16.Enabled = true;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        } //Ziyaretçi Güncelleme
        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem20.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView5.Rows.Count>0)
            {
                if (ziyaretciPreviewDialog.ShowDialog() == DialogResult.OK)
                {
                    ziyaretciDoc.Print();
                }
            }
            else
            {
                MessageBox.Show("Yazdırılacak Veri Bulunamadı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem20.Enabled = true;
        } //Ziyaretçileri Yazdırma Preview
        private void ziyaretciDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) //Ziyaretçileri Yazdırma Print
        {
            Font font = new Font("Verdana",11,FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            e.Graphics.DrawString("Tarih:"+DateTime.Now.ToShortDateString(),font,brush,650,0);
            int y = 20;
            for(int i=0;i<dataGridView5.Rows.Count;i++)
            {
                //Ziyaretçi Ad, Öğr Ad,Tc,Tarih
                e.Graphics.DrawString("Ziyaretçi Adı:"+dataGridView5.Rows[i].Cells[0].Value.ToString(),font,brush,0,y);
                y += 20;
                e.Graphics.DrawString("Öğrenci Adı:" + dataGridView5.Rows[i].Cells[1].Value.ToString(), font, brush, 0, y);
                y += 20;
                e.Graphics.DrawString("Öğrenci T.C:" + dataGridView5.Rows[i].Cells[2].Value.ToString(), font, brush, 0, y);
                y += 20;
                e.Graphics.DrawString("Ziyaret Tarihi:" + dataGridView5.Rows[i].Cells[3].Value.ToString(), font, brush, 0, y);
                y += 20;
                e.Graphics.DrawString("------------------------------------------------------", font, brush, 0, y);
            }
        }
        private async void barButtonItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Aidat Ödendi Olarak İşaretlendi
        {
            barButtonItem28.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView6.SelectedRows.Count>0)
            {
                aidat.tc = dataGridView6.CurrentRow.Cells[0].Value.ToString();
                aidat.ay = dataGridView6.CurrentRow.Cells[1].Value.ToString();
                aidat.yıl = dataGridView6.CurrentRow.Cells[2].Value.ToString();
                bool isOk = false;
                isOk = await Task.Run(user.aidat_Ode);
                if(isOk == true)
                {
                    MessageBox.Show("Aidat Başarıyla Ödendi!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Aidat Ödeme İşlemi Başarısız!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Aidat İşlemi Yapılacak Öğrenciyi Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem28.Enabled = true;
        }
        private async void barButtonItem21_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Pdf Ekleme
        {
            barButtonItem21.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(pdfPath.Text!="")
            {
                bool isOk = false;
                isOk = await user.pdf_Ekle();
                if(isOk == true)
                {
                    MessageBox.Show("PDF Başarıyla Eklendi!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Rapor Sisteme Eklenemdi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Sisteme Eklenecek Dosyayı Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem21.Enabled = true;
        }
        private void textBox1_Click(object sender, EventArgs e) //Dosya Yolu TextBox
        {
            customOpenFile.Filter = "PDF Files(*.pdf)|*.pdf";
            customOpenFile.Multiselect = false;
            if(customOpenFile.ShowDialog() == DialogResult.OK)
            {
                pdfPath.Text = customOpenFile.FileName;
                pdfBytes = File.ReadAllBytes(customOpenFile.FileName);
                rapor.raporAdi = customOpenFile.SafeFileName;
                rapor.dosya = pdfBytes;
            }
        }
        private async void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//PDF Silme
        {
            barButtonItem22.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView4.SelectedRows.Count>0)
            {
                DialogResult result = MessageBox.Show(dataGridView4.CurrentRow.Cells[0].Value.ToString().Replace(".pdf","")+" Adlı Kitabı Silmek İstiyor Musunuz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    rapor.raporAdi = dataGridView4.CurrentRow.Cells[0].Value.ToString();
                    bool isOk = false;
                    isOk = await Task.Run(user.pdf_Sil);
                    if(isOk == true)
                    {
                        MessageBox.Show("PDF Dosyası Başarıyla Silindi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("PDF Dosyası Silinemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
                else
                {
                    dataGridView4.CurrentRow.Selected = false;
                }
            }
            else
            {
                MessageBox.Show("Lütfen Tablodan Bir PDF Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem22.Enabled = true;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //PDF Listesini Güncelleme
        {
            barButtonItem23.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            veri_cek("Rapor");
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem23.Enabled = true;
        }
        private async void barButtonItem25_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //PDF Yedekleme İşlemi
        {
            DialogResult result = MessageBox.Show("Raporları Bilgisayarınıza Yedeklemek İstiyor Musunuz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                if(File.Exists(Application.StartupPath+@"\"+DateTime.Now.ToShortDateString().Replace(".","_")+"_Yedek"))
                {

                }
                else
                {
                    Directory.CreateDirectory(Application.StartupPath+@"\"+DateTime.Now.ToShortDateString().Replace(".","_")+"_Yedek");
                }
                rapor.kayıtDizini = Application.StartupPath + @"\" + DateTime.Now.ToShortDateString().Replace(".", "_") + "_Yedek";
                bool isOk = false;
                isOk = await Task.Run(user.pdf_Yedekle); 
                if(isOk == true)
                {
                    MessageBox.Show("Yedekleme İşlemi Başarıyla Yapıldı!\nDizin:'"+rapor.kayıtDizini+"'","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Yedekleme İşlemi Başarısız!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        private void barButtonItem24_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Başka Rapor Göster
        {
            customOpenFile.Multiselect = false;
            customOpenFile.Filter = "PDF Files(*.pdf)|*.pdf";
            if(customOpenFile.ShowDialog() == DialogResult.OK)
            {
                axAcroPDF1.LoadFile(customOpenFile.FileName);
            }
        }
        private async void dataGridView4_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) //Tekli Rapor İndirme
        {
            dataGridView4.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            DialogResult result = MessageBox.Show(dataGridView4.CurrentRow.Cells[0].Value.ToString().Replace(".pdf","")+" Adlı Kitabı İndirmek İstiyor Musunuz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                customSaveFile.Filter = "PDF Files(*.pdf)|*.pdf";
                customOpenFile.Title = "Kaydedilecek Dizini Seçiniz!";
                if(customSaveFile.ShowDialog() == DialogResult.OK)
                {
                    rapor.kayıtDizini = customSaveFile.FileName;
                    rapor.raporAdi = dataGridView4.CurrentRow.Cells[0].Value.ToString();
                    bool isOk = false;
                    isOk = await Task.Run(user.pdf_indir);
                    if(isOk == true)
                    {
                        MessageBox.Show(rapor.raporAdi+" Başarıyla İndirildi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Dosya İndirilemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            dataGridView4.Enabled = true;
        }
        private void barButtonItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Aidat Listesi Yenileme
        {
            barButtonItem29.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            veri_cek("Aidat");
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem29.Enabled = true;
        }
        private void dataGridView2_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)// Öğrenci Detay Sayfasına Yönlendirme
        {
            öğrenci.tc = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            user.garbageCollector();
            Form x = new OgrenciDetay();
            this.Hide();
            x.Show();
        }
        private void dataGridView6_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) //Öğrenci Detay
        {
            öğrenci.tc = dataGridView6.CurrentRow.Cells[0].Value.ToString();
            user.garbageCollector();
            Form x = new OgrenciDetay();
            this.Hide();
            x.Show();
        }
        private async void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if(ogrAd.Text!=""&e.KeyChar ==13)
            {
                ogrAd.Enabled = false;
                progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                öğrenci.adSoyad = ogrAd.Text;
                user.query = "SELECT OGR.tc,OGR.adSoyad,OGR.telefon,OGR.dogumTarihi,OGR.kanGrubu,Ogr.anneAdı,Ogr.babaAdı,OGR.cinsiyet,OGR.odaNo FROM dbo.Ögrenciler as OGR WHERE yurtId='" + AnaForm.yurtId + "' and aktif='EVET' and adSoyad LIKE '"+ogrAd.Text+"%'";
                try
                {
                    dataGridView2.DataSource = await Task.Run(user.ogr_Filtre);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }else if (ogrAd.Text == "" && e.KeyChar == 13)
            {
                ogrAd.Enabled = false;
                user.query = "SELECT OGR.tc,OGR.adSoyad,OGR.telefon,OGR.dogumTarihi,OGR.kanGrubu,Ogr.anneAdı,Ogr.babaAdı,OGR.cinsiyet,OGR.odaNo FROM dbo.Ögrenciler as OGR WHERE yurtId='" + AnaForm.yurtId + "' and aktif='EVET'";
                try
                {
                    dataGridView2.DataSource = await Task.Run(user.ogr_Filtre);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            ogrAd.Enabled = true;
        }
    }
}
