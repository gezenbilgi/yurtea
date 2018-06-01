using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YurtOtomasyonu.Properties;

namespace YurtOtomasyonu.Forms
{
    public partial class AnaForm : Form
    {
        #region FormInitialize
        public AnaForm()
        {
            InitializeComponent();
        }
        #endregion
        #region Variables
        private databaseUser user = new databaseUser();
        private databaseAdmin admin = new databaseAdmin();
        public static int yurtId;
        public static string ogrTc;
        public static ögrenci ögrenci = new ögrenci();
        public static Yurtlar yurt = new Yurtlar();
        public static Yöneticler yöneticler = new Yöneticler();
        #endregion
        private async void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)//Tab Page Değiştiğinde
        {
            user.garbageCollector(); // Bellek Tazeleme
            switch (xtraTabControl1.SelectedTabPageIndex)
            {
                case 3: // Veri çekme İşlemleri
                    progressYurt.Visible = true;
                    if (dataGridView1.Rows.Count==0)
                    {
                        bool internet = false;
                        internet = await Task.Run(user.internetKontrol);
                        if (internet == true)
                        {
                            dataGridView1.DataSource = await Task.Run(user.yurtlar);
                        }
                        else
                        {
                            MessageBox.Show("İnternet Bağlantınız Bulunamadı!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        dataGridİsimlendir();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("Listeyi Yenilemek İster Misiniz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                        if(result == DialogResult.Yes)
                        {
                            bool internet = false;
                            internet = await Task.Run(user.internetKontrol);
                            if (internet == true)
                            {
                                dataGridView1.DataSource = await Task.Run(user.yurtlar);
                            }
                            else
                            {
                                MessageBox.Show("İnternet Bağlantınız Bulunamadı!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    progressYurt.Visible = false;
                    break;
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e) //Öğrenci Olarak Giriş Yap Butonu
        {
            ogrTb1.Text = Settings.Default.ogrTc;
            groupControl1.Visible = true;
            groupControl2.Visible = groupControl3.Visible = false;
        }
        private void simpleButton2_Click(object sender, EventArgs e) //Yurt Yöneticisi Olarak Giriş Yap Butonu
        {
            groupControl2.Visible = true;
            yurtTb1.Text = Settings.Default.username;
            yurtTb2.Text = Settings.Default.password;
            groupControl1.Visible = groupControl3.Visible = false;
        }
        private void simpleButton3_Click(object sender, EventArgs e) //Sistem Yöneticisi Olarak Giriş Yap Butonu
        {
            groupControl3.Visible = true;
            groupControl1.Visible = groupControl2.Visible = false;
            adminTb1.Text = Settings.Default.username;
            adminTb2.Text = Settings.Default.password;
        }
        private async void AnaForm_Load(object sender, EventArgs e)
        {
            progressYurt.Visible = false;
            progressGiris.Visible = false;
            ArrayList listİller = await Task.Run(user.iller);
            if(listİller.Count>0)
            {
                cbxİller.Items.AddRange(listİller.ToArray());
                cbxİller.SelectedIndex = cbxİlce.SelectedIndex =cbxFiltre.SelectedIndex=-1;
                kayitTb2.Items.AddRange(listİller.ToArray());
            }
            else
            {
                MessageBox.Show("Aktif Bir İnternet Bağlantınız Bulunamadı!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            
        }//Form Load
        private async void cbxİller_SelectedIndexChanged(object sender, EventArgs e) //İllere Göre Filtreleme
        {
            progressYurt.Visible = true;
            cbxİller.Enabled = false;
            switch(cbxİller.Text)
            {
                case "Tümü":
                    try
                    {
                        dataGridView1.DataSource = await Task.Run(user.yurtlar);
                        cbxİlce.Items.Clear();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                default:
                    try
                    {
                        cbxİlce.SelectedIndex = -1;
                        cbxİlce.Items.Clear();
                        cbxİlce.Items.Add("Tümü");
                        user.query = "SELECT DISTINCT BLG.ilce FROM dbo.Bölgeler AS BLG WHERE il='" + cbxİller.Text + "'";
                        ArrayList ilceler = await Task.Run(user.ilceler);
                        if (ilceler.Count > 0)
                        {
                            cbxİlce.Items.AddRange(ilceler.ToArray());
                            user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "'";
                            dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                        }
                        else
                        {
                            MessageBox.Show("Aktif Bir İnternet Bağlantınız Bulunamadı!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                    break;
            }
            cbxİller.Enabled = true;
            progressYurt.Visible = false;
            dataGridİsimlendir();
        }
        private async void cbxİlce_SelectedIndexChanged(object sender, EventArgs e) //İlçeye Göre
        {
            progressYurt.Visible = true;
            cbxİlce.Enabled = false;
            switch(cbxİlce.Text)
            {
                case "Tümü":
                    if (cbxİller.Text != "Tümü")
                    {
                        user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "'";
                        try
                        {
                            dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    break;
                default:
                    user.query = "SELECT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='"+cbxİller.Text+"' and ilce='"+cbxİlce.Text+"'";
                    try
                    {
                        dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
            }
            progressYurt.Visible = false;
            cbxİlce.Enabled = true;
            dataGridİsimlendir();
        }
        private async void cbxFiltre_SelectedIndexChanged(object sender, EventArgs e) //Normal Kriter Filtreleme
        {
            progressYurt.Visible = true;
            cbxFiltre.Enabled = false;
            switch(cbxFiltre.Text)
            {
                case "Tümü":
                    try
                    {
                        if (cbxİller.Text != "" && cbxİller.Text != "Tümü")
                        {
                            if (cbxİlce.Text != "" && cbxİlce.Text != "Tümü")
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' and ilce='"+cbxİlce.Text+"'";
                            }
                            else
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "'";
                            }
                        }
                        else
                        {
                            user.query = "SELECT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and kapasite<>ogrenciSayisi";
                        }
                        dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "Fiyata Göre(Artan)":
                    try
                    {
                        if (cbxİller.Text != "" && cbxİller.Text != "Tümü")
                        {
                            if (cbxİlce.Text != "" && cbxİlce.Text != "Tümü")
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' and ilce='"+cbxİlce.Text+"' ORDER BY aidat ASC";
                            }
                            else
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' ORDER BY aidat ASC";
                            }
                        }
                        else
                        {
                            user.query = "SELECT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and kapasite<>ogrenciSayisi ORDER BY aidat ASC";
                        }
                        dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "Fiyata Göre(Azalan)":
                    try
                    {
                        if (cbxİller.Text != "" && cbxİller.Text != "Tümü")
                        {
                            if (cbxİlce.Text != "" && cbxİlce.Text != "Tümü")
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' and ilce='"+cbxİlce.Text+"' ORDER BY aidat DESC";
                            }
                            else
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' ORDER BY aidat DESC";
                            }
                        }
                        else
                        {
                            user.query = "SELECT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and kapasite<>ogrenciSayisi ORDER BY aidat DESC";
                        }
                        dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "Öğrenci Sayısına Göre(Artan)":
                    try
                    {
                        if (cbxİller.Text != "" && cbxİller.Text != "Tümü")
                        {
                            if (cbxİlce.Text != "" && cbxİlce.Text != "Tümü")
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' and ilce='" + cbxİlce.Text + "' ORDER BY ogrenciSayisi ASC";
                            }
                            else
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' ORDER BY ogrenciSayisi ASC";
                            }
                        }
                        else
                        {
                            user.query = "SELECT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and kapasite<>ogrenciSayisi ORDER BY ogrenciSayisi ASC";
                        }
                        dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "Öğrenci Sayısına Göre(Azalan)":
                    try
                    {
                        if (cbxİller.Text != "" && cbxİller.Text != "Tümü")
                        {
                            if (cbxİlce.Text != "" && cbxİlce.Text != "Tümü")
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' and ilce='" + cbxİlce.Text + "' ORDER BY ogrenciSayisi DESC";
                            }
                            else
                            {
                                user.query = "SELECT DISTINCT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite and il='" + cbxİller.Text + "' ORDER BY ogrenciSayisi DESC";
                            }
                        }
                        else
                        {
                            user.query = "SELECT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and kapasite<>ogrenciSayisi ORDER BY ogrenciSayisi DESC";
                        }
                        dataGridView1.DataSource = await Task.Run(user.yurtFiltre);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
            }
            cbxFiltre.Enabled = true;
            progressYurt.Visible = false;
            dataGridİsimlendir();
        }
        private void ogrHatirla_CheckedChanged(object sender, EventArgs e) //Öğrenci T.C Kimlik Hatırlama
        {
            if(ogrHatirla.Checked == true && ogrTb1.Text!="")
            {
                Settings.Default.ogrTc = ogrTb1.Text;
                Settings.Default.Save();
            }
        }
        private async void simpleButton4_Click(object sender, EventArgs e)// Öğrenci Girişi
        {
            simpleButton4.Enabled = false;
            progressGiris.Visible = true;
            if(ogrTb1.Text!="")
            {
                ögrenci.tc = ogrTb1.Text;
                bool giris = false;
                giris = await Task.Run(user.ogrGiris);
                user.garbageCollector();
                if(giris == true)
                {
                    ogrTc = ogrTb1.Text;
                    Form form = new OgrenciPanel();
                    this.Hide();
                    form.Show();
                }
                else
                {
                    MessageBox.Show("Giriş Yapılamadı!\nİnternet Bağlantınız da Hata Olabilir!\nBöyle Bir Öğrenci Olmayabilir!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Öğrenci Bilgilerini Giriniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progressGiris.Visible = false;
            simpleButton4.Enabled = true;
        }
        private void yurtSifreGöster_CheckedChanged(object sender, EventArgs e) //Yurt Yöneticisi Şifremi Göster
        {
            if(yurtTb2.UseSystemPasswordChar == true)
            {
                yurtTb2.UseSystemPasswordChar = false;
            }
            else
            {
                yurtTb2.UseSystemPasswordChar = true;
            }
        }
        private void yurtHatirla_CheckedChanged(object sender, EventArgs e)//Yurt Yönetici Bilgilerini Hatırlama
        {
            if(yurtHatirla.Checked==true&&yurtTb1.Text!=""&&yurtTb2.Text!="")
            {
                Settings.Default.username = yurtTb1.Text;
                Settings.Default.password = yurtTb2.Text;
                Settings.Default.Save();
            }
        }
        private async void kayitTb2_SelectedIndexChanged(object sender, EventArgs e)//Kayıt Paneli İlçe Getirme İşlemi
        {
            kayitTb3.Items.Clear();
            user.query = "SELECT DISTINCT BLG.ilce FROM dbo.Bölgeler AS BLG WHERE il='"+kayitTb2.Text+"' and aktif='EVET'";
            ArrayList ilceler = await Task.Run(user.ilceler);
            kayitTb3.Items.AddRange(ilceler.ToArray());
        }
        private async void simpleButton6_Click(object sender, EventArgs e) //Yurt Kayit İşlemi
        {
            simpleButton6.Enabled = false;
            if(kayitTb1.Text!=""&&kayitTb2.Text!=""&&kayitTb3.Text!=""&&kayitTb4.Text!=""&&kayitTb5.Value!=0&&kayitTb6.Value!=0&&kayitTb7.MaskFull==true)
            {
                try
                {
                    yurt.ad = kayitTb1.Text;
                    yurt.il = kayitTb2.Text;
                    yurt.ilce = kayitTb3.Text;
                    yurt.aidat = Convert.ToInt32(kayitTb4.Text);
                    yurt.odaSayisi = Convert.ToInt32(kayitTb5.Value);
                    yurt.yatakSayisi = Convert.ToInt32(kayitTb6.Value);
                    yurt.kapasite = yurt.odaSayisi * yurt.yatakSayisi;
                    yurt.tel = kayitTb7.Text;
                    yurt.username = kayitTb8.Text;
                    yurt.password = user.md5crypto(kayitTb9.Text);
                    bool isOk = false;
                    isOk = await Task.Run(user.yurtKayit);
                    if(isOk == true)
                    {
                        MessageBox.Show("Yurt Başarıyla Oluşturuldu!\nYöneticiler Aktifleştirdiğinde Sisteme Girebileceksiniz!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Kayıt Yapılamadı!\nİnternet Bağlantınız Olmayabilir!\nBöyle Bir Yurt Adı Olabilir!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen İlgili Yerleri Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            simpleButton6.Enabled = true;
        }
        private async void simpleButton5_Click(object sender, EventArgs e) //Yurt Yönetici Giriş Paneli
        {
            simpleButton5.Enabled = false;
            progressGiris.Visible = true;
            if(yurtTb1.Text!=""&&yurtTb2.Text!="")
            {
                yurt.username = yurtTb1.Text;
                yurt.password = user.md5crypto(yurtTb2.Text);
                bool isOk = false;
                isOk = await Task.Run(user.yurtYöneticiGiris);
                user.garbageCollector();
                if(isOk == true)
                {
                    Form x = new IdarePanel();
                    this.Hide();
                    x.Show();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya Şifre Yanlış!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen İlgili Yerleri Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            simpleButton5.Enabled = true;
            progressGiris.Visible = false;
        }
        private async void simpleButton7_Click(object sender, EventArgs e)
        {
            if(tb1.Text!=""&&tb2.Text!=""&&tb3.Text!=""&&tb4.Text!="")
            {
                if(tb3.Text==tb4.Text)
                {
                    yurt.username = tb1.Text;
                    yurt.password = user.md5crypto(tb3.Text);
                    yurt.secCode = tb2.Text;
                    bool isOk = false;
                    isOk = await Task.Run(user.yöneticiSifreYenile);
                    if(isOk == true)
                    {
                        MessageBox.Show("Şifreniz Başarıyla Yenilendi!","BAŞARILI",MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Şifre Yenileme İşlemi Başarısız!","BAŞARISIZ",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Şifreler Birbirleriyle Uyumsuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen İlgili Yerleri Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private async void simpleButton8_Click(object sender, EventArgs e)//Sistem Yöneticilerinin Giriş Paneli
        {
            simpleButton8.Enabled = false;
            progressGiris.Visible = true;
            if(adminTb1.Text!=""&&adminTb2.Text!="")
            {
                yöneticler.username = adminTb1.Text;
                yöneticler.password = adminTb2.Text;
                bool isOk = false;
                isOk = await Task.Run(admin.yöneticiGiris);
                admin.garbageCollector();
                if(isOk == true)
                {
                    Form x = new SistemAdminPanel();
                    this.Hide();
                    x.Show();
                }
                else
                {
                    MessageBox.Show("Kullanıcı Adı veya Şifre Yanlış!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen İlgili Yerleri Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progressGiris.Visible = false;
            simpleButton8.Enabled = true;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) //Yönetici Şifre Hatırla Paneli
        {
            if(adminTb1.Text!=""&&adminTb2.Text!="")
            {
                Settings.Default.username = adminTb1.Text;
                Settings.Default.password = adminTb2.Text;
                Settings.Default.Save();
            }
        }
        private void dataGridİsimlendir()
        {
            dataGridView1.Columns[0].HeaderText = "Ad";
            dataGridView1.Columns[1].HeaderText = "İl";
            dataGridView1.Columns[2].HeaderText = "İlçe";
            dataGridView1.Columns[3].HeaderText = "Aidat";
            dataGridView1.Columns[4].HeaderText = "Oda Sayisi";
            dataGridView1.Columns[5].HeaderText = "Yatak Sayisi";
            dataGridView1.Columns[6].HeaderText = "Öğrenci Sayısı";
            dataGridView1.Columns[7].HeaderText = "Kapasite";
            dataGridView1.Columns[8].HeaderText = "Tel";
        }
    }
}
