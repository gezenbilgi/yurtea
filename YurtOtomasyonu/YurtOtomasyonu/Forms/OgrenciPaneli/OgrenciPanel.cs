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
    public partial class OgrenciPanel : Form
    {
        #region FormInitialize
        public OgrenciPanel()
        {
            InitializeComponent();
        }
        private databaseUser user = new databaseUser();
        #endregion 
        private async void OgrenciPanel_Load(object sender, EventArgs e) //Form Load
        {
            try
            {
                lblTarih.ResetText();
                Timer tarih = new Timer();
                tarih.Interval = 1000;
                tarih.Start();
                tarih.Tick += Tarih_Tick;
                progress.Visible = true;
                pictureBox1.BackgroundImage = Image.FromStream(await Task.Run(user.ogrFoto));
                ArrayList list = await Task.Run(user.ogrBilgi);
                tb1.Text =  AnaForm.ogrTc;
                tb2.Text = list[0].ToString();
                tb3.Text = list[1].ToString();
                tb4.Text = list[2].ToString();
                tb5.Text = list[3].ToString();
                tb6.Text = list[4].ToString();
                tb7.Text = list[5].ToString();
                tb8.Text = list[6].ToString();
                progress.Visible = false;
                veri_cek("Tümü");
                xtraTabControl1.SelectedPageChanged += XtraTabControl1_SelectedPageChanged;
                this.FormClosing += OgrenciPanel_FormClosing;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void OgrenciPanel_FormClosing(object sender, FormClosingEventArgs e) //Form Kapanam
        {
            user.garbageCollector();
            Form x = new AnaForm();
            x.Show();
        }
        private void XtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e) //Her Sayfa Değiştiğinde Bellek Temizlensin
        {
            user.garbageCollector();
            switch(xtraTabControl1.SelectedTabPageIndex)
            {
                case 1:
                    dataGrid1();
                    break;
                case 2:
                    dataGrid2();
                    break;
                case 3:
                    dataGrid3();
                    break;
                case 4:
                    dataGrid4();
                    break;
                   
            }
        }
        private void Tarih_Tick(object sender, EventArgs e) //Tarih Gösterme İşlemi
        {
            lblTarih.Text = DateTime.Now.ToString();
        }
        private async void veri_cek(string secim)
        {
            try
            {
                progress.Visible = true;
                if (secim == "Tümü")
                {
                    dataGridView1.DataSource = await Task.Run(user.devamsizlik_cekOGR);
                    dataGridView2.DataSource = await Task.Run(user.ziyaretci_cekOGR);
                    dataGridView3.DataSource = await Task.Run(user.aidat_cekOGR);
                    dataGridView4.DataSource = await Task.Run(user.oda_Arkadas);
                }
                else if (secim == "Devamsizlik")
                {
                    dataGridView1.DataSource = await Task.Run(user.devamsizlik_cekOGR);
                }else if(secim=="Ziyaretci")
                {
                    dataGridView2.DataSource = await Task.Run(user.ziyaretci_cekOGR);
                }else if(secim == "Aidat")
                {
                    dataGridView3.DataSource = await Task.Run(user.aidat_cekOGR);
                }else if(secim == "Arkadas")
                {
                    dataGridView4.DataSource = await Task.Run(user.oda_Arkadas);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dataGrid1();
                dataGrid2();
                dataGrid3();
                dataGrid4();
                progress.Visible = false;
            }
        }
        private void dataGrid1() //DataGrid1 Still
        {
            if(dataGridView1.Rows.Count>0)
            {
                dataGridView1.Columns[0].HeaderText = "Devamsızlık Tarihi";
                for(int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Height = 35;
                }
            }
        }
        private void dataGrid2() //DataGrid2 Still
        {
            if (dataGridView2.Rows.Count > 0)
            {
                dataGridView2.Columns[0].HeaderText = "Ziyaretçi Adı";
                dataGridView2.Columns[1].HeaderText = "Ziyaret Tarihi";
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    dataGridView2.Rows[i].Height = 35;
                }
            }
        }
        private void dataGrid3() //DataGrid3 Still
        {
            if(dataGridView3.Rows.Count>0)
            {
                dataGridView3.Columns[0].HeaderText = "Aidat Ay";
                dataGridView3.Columns[1].HeaderText = "Aidat Yıl";
                dataGridView3.Columns[2].HeaderText = "Aidat Tutar";
                dataGridView3.Columns[3].HeaderText = "Ödendi";
                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    if(dataGridView3.Rows[i].Cells[3].Value.ToString()=="EVET")
                    {
                        style.BackColor = Color.Green;
                    }
                    else
                    {
                        style.BackColor = Color.Red;
                    }
                    dataGridView3.Rows[i].DefaultCellStyle = style;
                    dataGridView3.Rows[i].Height = 35;
                }

            }
        }
        private void dataGrid4() //DataGrid4 Still
        {
            if(dataGridView4.Rows.Count>0)
            {
                dataGridView4.Columns[0].HeaderText = "Ad Soyad";
                dataGridView4.Columns[1].HeaderText = "Telefon";
                dataGridView4.Columns[2].HeaderText = "Doğum Tarihi";
                dataGridView4.Columns[3].HeaderText = "Kan Grubu";
                for(int i=0;i<dataGridView4.Rows.Count;i++)
                {
                    dataGridView4.Rows[i].Height = 35;
                }
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            user.garbageCollector();
            simpleButton1.Enabled = false;
            progress.Visible = true;
            switch (xtraTabControl1.SelectedTabPageIndex)
            {
                case 1:
                    veri_cek("Devamsizlik");
                    break;
                case 2:
                    veri_cek("Ziyaretci");
                    break;
                case 3:
                    veri_cek("Aidat");
                    break;
                case 4:
                    veri_cek("Arkadas");
                    break;

            }
            progress.Visible = false;
            simpleButton1.Enabled = true;
        } //Yenileme Butonu
    }
}
