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

namespace YurtOtomasyonu.Forms
{
    public partial class SistemAdminPanel : Form
    {
        private databaseAdmin admin = new databaseAdmin();
        #region FormInitialize
        public SistemAdminPanel()
        {
            InitializeComponent();
        }
        #endregion
        private async void getData()
        {
            try
            {
                progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                dataGridView1.DataSource = await Task.Run(admin.yurtlar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                if(dataGridView1.Rows.Count>0)
                {
                    dataGridStyle();
                }
                progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }
        private void dataGridStyle()
        {
            dataGridView1.Columns[0].HeaderText = "Ad";
            dataGridView1.Columns[1].HeaderText = "İl";
            dataGridView1.Columns[2].HeaderText = "İlçe";
            dataGridView1.Columns[3].HeaderText = "Aidat";
            dataGridView1.Columns[4].HeaderText = "Oda Sayısı";
            dataGridView1.Columns[5].HeaderText = "Yatak Sayısı";
            dataGridView1.Columns[6].HeaderText = "Öğrenci Sayısı";
            dataGridView1.Columns[7].HeaderText = "Kapasite";
            dataGridView1.Columns[8].HeaderText = "Telefon";
            dataGridView1.Columns[8].HeaderText = "Aktif";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                if (dataGridView1.Rows[i].Cells[9].Value.ToString() == "EVET")
                {
                    style.BackColor = Color.Green;
                }
                else
                {
                    style.BackColor = Color.Red;
                }
                dataGridView1.Rows[i].DefaultCellStyle = style;
                dataGridView1.Rows[i].Height = 35;
            }
        }
        private async void getCitys()
        {
            ArrayList array = await Task.Run(admin.iller);
            if(array.Count>0)
            {
                cb1.Items.Clear();
                cb1.Items.AddRange(array.ToArray());
            }
            else
            {
                MessageBox.Show("İller Getirilemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void SistemAdminPanel_Load(object sender, EventArgs e)
        {
            getData();
            getCitys();
            getRegions();
        } 
        private async void  comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb2.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            admin.query = "SELECT ad,il,ilce,aidat,odaSayisi,yatakSayisi,ogrenciSayisi,kapasite,tel,aktif FROM dbo.Yurtlar WHERE sil='HAYIR' and il LIKE '"+cb1.Text+"%' and ilce LIKE '"+cb2.Text+"%'";
            dataGridView1.DataSource = await Task.Run(admin.yurtFiltre);
            if(dataGridView1.Rows.Count>0)
            {
                dataGridStyle();
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            cb2.Enabled = true;
        }
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Filtre Temizleme İşlemi
        {
            barButtonItem3.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            getData();
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem3.Enabled = true;
        }
        private async void cb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb1.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            admin.query = "SELECT DISTINCT ilce FROM dbo.Bölgeler WHERE il LIKE '" + cb1.Text + "%' and aktif='EVET'";
            cb2.Items.Clear();
            ArrayList ilceler = await Task.Run(admin.ilceler);
            cb2.Items.AddRange(ilceler.ToArray());
            admin.query = "SELECT ad,il,ilce,aidat,odaSayisi,yatakSayisi,ogrenciSayisi,kapasite,tel,aktif FROM dbo.Yurtlar WHERE sil='HAYIR' and il LIKE '" + cb1.Text + "%'";
            dataGridView1.DataSource = await Task.Run(admin.yurtFiltre);
            if(dataGridView1.Rows.Count>0)
            {
                dataGridStyle();
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            cb1.Enabled = true;
        }
        private async void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem1.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView1.SelectedRows.Count>0)
            {
                if(dataGridView1.CurrentRow.Cells[9].Value.ToString() == "HAYIR")
                {
                    DialogResult result = MessageBox.Show(dataGridView1.CurrentRow.Cells[0].Value.ToString()+" Adlı Yurdu Yayına Almak İstiyor Musunuz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if(result == DialogResult.Yes)
                    {
                        admin.query = "UPDATE Yurtlar SET aktif='EVET' WHERE ad='"+dataGridView1.CurrentRow.Cells[0].Value.ToString()+"' and sil='HAYIR'";
                        bool isOk = false;
                        isOk = await Task.Run(admin.yurtİslem);
                        if(isOk == true)
                        {
                            MessageBox.Show("Yurt Yayında!","BAŞARILI",MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show("İşlem Başarısız!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bu Kurs Zaten Yayında!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Bir Yurt Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem1.Enabled = true;
        }
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            admin.garbageCollector();
            dataGridStyle();
        }
        private async void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem2.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(dataGridView1.SelectedRows.Count>0)
            {
                if(dataGridView1.CurrentRow.Cells[9].Value.ToString()=="EVET")
                {
                    DialogResult result = MessageBox.Show(dataGridView1.CurrentRow.Cells[0].Value.ToString()+" Adlı Yurdu Yayından Kaldırma İstediğine Emin Misin?","EMİN MİSİN?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if(result == DialogResult.Yes)
                    {
                        admin.query = "UPDATE Yurtlar SET aktfi='HAYIR' WHERE ad='" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "' and sil='HAYIR'";
                        bool isOk = false;
                        isOk = await Task.Run(admin.yurtİslem);
                        if(isOk == true)
                        {
                            MessageBox.Show("Yurt Yayından Başarıyla Kaldırıldı!","BAŞARILI",MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show("İşlem Başarısız Oldu!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bu Yurt Zaten Yayından Kaldırılmış!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Bir Yurt Seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem2.Enabled = true;
        }
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem4.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            getData();
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem4.Enabled = true;
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            getRegions();
        }
        private async void getRegions()
        {
            try
            {
                dataGridView2.DataSource = await Task.Run(admin.yurtlarvebölgeler);
                if(dataGridView2.Rows.Count>0)
                {
                    dataGridView2.Columns[0].HeaderText = "İl";
                    dataGridView2.Columns[1].HeaderText = "İlçe";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem5.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if(tb1.Text!=""&&tb2.Text!="")
            {
                admin.query = "INSERT INTO dbo.Bölgeler(il,ilce,aktif) VALUES('"+tb1.Text+"','"+tb2.Text+"','EVET')";
                bool isOk = false;
                isOk = await Task.Run(admin.yurtİslem); // Kod Tasarrufu İçin Böyle Yaptım // Böyle Bir Veri Varmı Kontrol Etmedim Çünkü Genelde Eşsiz Verileri Çekiyorum(Distinct)
                if(isOk == true)
                {
                    MessageBox.Show("Başarıyla Eklendi!","BAŞARILI",MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Eklenemedi!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Verileri Lütfen Eksiksiz Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem5.Enabled = true;
        }

        private async void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem6.Enabled = false;
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if (dataGridView2.SelectedRows.Count>0)
            {
                admin.query = "DELETE FROM Bölgeler WHERE il='"+dataGridView2.CurrentRow.Cells[0].Value.ToString()+"' and ilce='"+dataGridView2.CurrentRow.Cells[1].Value.ToString()+"'";
                bool isOk = false;
                isOk = await Task.Run(admin.yurtİslem); // Kod Tasarrufu İçin Böyle Yaptım // Böyle Bir Veri Varmı Kontrol Etmedim Çünkü Genelde Eşsiz Verileri Çekiyorum(Distinct)
                if (isOk == true)
                {
                    MessageBox.Show("Başarıyla Silindi!", "BAŞARILI", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Silinemedi!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen Listeden Bir Veri Seçiniz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barButtonItem6.Enabled = true;
        }
    }
}
