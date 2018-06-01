using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YurtOtomasyonu.Forms;

namespace YurtOtomasyonu
{
    //Yurt Yöneticileri ve Öğrencilerin Tüm CRUD İşlemleri Bu Metod Altında Yapılacak
    public class databaseUser
    {
        #region Variables
        private SqlConnection con = new SqlConnection("Data Source=FARUK;Initial Catalog=YurtOtomasyonu;User Id=root;Password=03102593");
        private SqlCommand cmd;
        private SqlDataAdapter adapter;
        private DataTable dt;
        public string query;
        private FileStream fs;
        private BinaryWriter bw;
        private MemoryStream ms;
        #endregion
        public void garbageCollector() // Azda Olsa Bellek Kullanımı Düşürecektir.
        {
            //Sayfa Geçişleri Arası Kullanılacak
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public string md5crypto(string pass)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            StringBuilder sb = new StringBuilder();
            byte[] byt = Encoding.UTF8.GetBytes(pass);
            byt = md5.ComputeHash(byt);
            foreach(byte b in byt)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            return sb.ToString();
        } //MD5 Şifreleme Yapar
        public async Task<Boolean> internetKontrol()
        {
            try
            {
                TcpClient client = new TcpClient("www.google.com.tr", 80);
                client.Close();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        } //İnternet Kontrol
        //Ana Form İşlemleri Başlangıç
        public async Task<DataTable>yurtlar()
        {
            adapter = new SqlDataAdapter("SELECT YRT.ad,YRT.il,YRT.ilce,YRT.aidat,YRT.odaSayisi,YRT.yatakSayisi,YRT.ogrenciSayisi,YRT.kapasite,YRT.tel FROM dbo.Yurtlar AS YRT WHERE aktif='EVET' and sil='HAYIR' and ogrenciSayisi<>kapasite",con);
            dt = new DataTable();
            adapter.Fill(dt);
            adapter.Dispose();
            return dt;
        } //Yurtları Getirir
        public async Task<ArrayList>iller()
        {
            ArrayList list = new ArrayList();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                adapter = new SqlDataAdapter("SELECT DISTINCT BLG.il FROM dbo.Bölgeler AS BLG WHERE aktif='EVET'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                adapter.Dispose();
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    list.Add(dt.Rows[i][0]);
                }
                return list;
            }
            else
            {
                return list;
            }
        } //İller
        public async Task<ArrayList>ilceler()
        {
            ArrayList ilce = new ArrayList();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                adapter = new SqlDataAdapter(query,con);
                dt = new DataTable();
                adapter.Fill(dt);
                adapter.Dispose();
                query = "";
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    ilce.Add(dt.Rows[i][0].ToString());
                }
                return ilce;
            }
            else
            {
                return ilce;
            }
        } //İlçeler
        public async Task<DataTable>yurtFiltre()
        {
            dt = new DataTable();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                adapter = new SqlDataAdapter(query,con);
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
            else
            {
                return dt;
            }
        } //Yurt Filtrelemeler
        public async Task<Boolean> ogrGiris()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("EXEC login_Ogr '"+AnaForm.ögrenci.tc+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
                    {
                        AnaForm.ögrenci.id = Convert.ToInt32(dt.Rows[0][0]);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }catch(Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Öğrenci Girişi
        public async Task<Boolean> yurtKayit()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("EXEC sp_YurtKontrol '" + AnaForm.yurt.ad + "'", con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if (dt.Rows.Count > 0)
                    {
                        return false;
                    }
                    else
                    {
                        await con.OpenAsync();
                        cmd = new SqlCommand("EXEC sp_AddYurt @ad,@il,@ilce,@aidat,@odaSay,@yatakSay,@ogrSay,@kapSay,@tel,@aktif,@sil",con);
                        cmd.Parameters.AddWithValue("@ad",AnaForm.yurt.ad);
                        cmd.Parameters.AddWithValue("@il", AnaForm.yurt.il);
                        cmd.Parameters.AddWithValue("@ilce", AnaForm.yurt.ilce);
                        cmd.Parameters.AddWithValue("@aidat", AnaForm.yurt.aidat);
                        cmd.Parameters.AddWithValue("@odaSay", AnaForm.yurt.odaSayisi);
                        cmd.Parameters.AddWithValue("@yatakSay", AnaForm.yurt.yatakSayisi);
                        cmd.Parameters.AddWithValue("@ogrSay", 0);
                        cmd.Parameters.AddWithValue("@kapSay", AnaForm.yurt.kapasite);
                        cmd.Parameters.AddWithValue("@tel", AnaForm.yurt.tel);
                        cmd.Parameters.AddWithValue("@aktif","HAYIR");
                        cmd.Parameters.AddWithValue("@sil", "HAYIR");
                        await cmd.ExecuteNonQueryAsync();
                        adapter = new SqlDataAdapter("SELECT id FROM dbo.Yurtlar WHERE ad='"+AnaForm.yurt.ad+"' and tel='"+AnaForm.yurt.tel+"'",con);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        cmd = new SqlCommand("EXEC sp_setYönetici @ID,@USERNAME,@PASSWORD,@SEC",con);
                        cmd.Parameters.AddWithValue("@ID",Convert.ToInt32(dt.Rows[0][0].ToString()));
                        cmd.Parameters.AddWithValue("@USERNAME",AnaForm.yurt.username);
                        cmd.Parameters.AddWithValue("@PASSWORD",AnaForm.yurt.password);
                        Random rnd = new Random();
                        string sec = Convert.ToString(rnd.Next(0,10000));
                        cmd.Parameters.AddWithValue("@SEC",sec);
                        await cmd.ExecuteNonQueryAsync();
                        con.Close();
                        MessageBox.Show(sec+" Güvenlik Kodunuzu Unutmayınız!\nŞifre Yenileme İşlemlerinde KullanabilirsiniZ!");
                        return true;
                        
                    }
                }
                catch(Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Yurt Kayıt
        //Ana Form İşlemleri Bitiş
        //Yönetici Paaneli Başlangıç
        public async Task<Boolean> yöneticiSifreYenile()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM dbo.YurtYönetici WHERE username='"+AnaForm.yurt.username+"' and güvenlik_kodu='"+AnaForm.yurt.secCode+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
                    {
                        await con.OpenAsync();
                        cmd = new SqlCommand("EXEC sp_updatePassword_ @user,@pass,@sec",con);
                        cmd.Parameters.AddWithValue("@user",AnaForm.yurt.username);
                        cmd.Parameters.AddWithValue("@pass", AnaForm.yurt.password);
                        cmd.Parameters.AddWithValue("@sec", AnaForm.yurt.secCode);
                        int count = await cmd.ExecuteNonQueryAsync();
                        if(count>0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }catch(Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Yönetici Şifremi Unuttum
        public async Task<Boolean> yurtYöneticiGiris()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT id FROM dbo.YurtYönetici WHERE username='"+AnaForm.yurt.username+"' and password_='"+AnaForm.yurt.password+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
                    {
                        AnaForm.yurtId = Convert.ToInt32(dt.Rows[0][0]);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }catch(Exception)
                { return false;
                }
            }
            else
            {
                return false;
            }
        } //Yönetici Giriş
        public async Task<Boolean> odaOlustur() // Oda Oluştur
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT odaSayisi FROM dbo.Yurtlar WHERE id='"+AnaForm.yurtId+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    int toplamOdaSay = Convert.ToInt32(dt.Rows[0][0].ToString());
                    adapter = new SqlDataAdapter("SELECT COUNT(*) FROM dbo.Odalar WHERE yurtId='"+AnaForm.yurtId+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    int odaSay = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if(odaSay<toplamOdaSay)
                    {
                        adapter = new SqlDataAdapter("SELECT * FROM dbo.Odalar WHERE yurtId='"+AnaForm.yurtId+"' and odaNo='"+IdarePanel.oda.odaNo+"' and aktif='EVET'",con);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        adapter.Dispose();
                        if(dt.Rows.Count>0)
                        {
                            return false;
                        }
                        else
                        {
                            await con.OpenAsync();
                            cmd = new SqlCommand("EXEC sp_InsertRoom @ID,@OdaNo", con);
                            cmd.Parameters.AddWithValue("@ID", AnaForm.yurtId);
                            cmd.Parameters.AddWithValue("OdaNo", IdarePanel.oda.odaNo);
                            int count = await cmd.ExecuteNonQueryAsync();
                            con.Close();
                            if (count > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }

                }catch(Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<DataTable> odalari_Cek()
        {
            dt = new DataTable();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT odaNo FROM dbo.Odalar WHERE yurtId='"+AnaForm.yurtId+"' and aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }catch(Exception)
                {
                    return dt;
                }
            }
            else
            {
                return dt; // Boş Dönecek
            }
        }
        public async Task<Boolean> ogr_odaKontrol() //Öğrenci oda Kontrol
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT yurtId,odaNo FROM dbo.Öğrenciler WHERE tc='"+IdarePanel.öğrenci.tc+"' and aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][1].ToString()=="0"&&Convert.ToInt32(dt.Rows[0][0]) ==0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }catch(Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<Boolean> ogr_kontrol()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if (internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM dbo.Öğrenciler WHERE tc='" + IdarePanel.öğrenci.tc + "' and aktif='EVET'", con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Öğrenci Varmı Kontrol
        public async Task<Boolean> ogr_Ekle()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM dbo.Öğrenciler WHERE tc='"+OgrEkle.öğrenci.tc+"' and aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
                    {
                        return false;
                    }
                    else
                    {
                        await con.OpenAsync();
                        cmd = new SqlCommand("EXEC sp_InsertStudent @resim,@tc,@adsoyad,@tel,@dt,@kan,@anne,@baba,@cins,@ıd",con);
                        SqlParameter resim = cmd.Parameters.AddWithValue("@resim",OgrEkle.öğrenci.resim);
                        resim.DbType = DbType.Binary;
                        cmd.Parameters.AddWithValue("@tc",OgrEkle.öğrenci.tc);
                        cmd.Parameters.AddWithValue("@adsoyad", OgrEkle.öğrenci.adSoyad);
                        cmd.Parameters.AddWithValue("@tel", OgrEkle.öğrenci.tel);
                        cmd.Parameters.AddWithValue("@dt", OgrEkle.öğrenci.dogumTarihi);
                        cmd.Parameters.AddWithValue("@kan", OgrEkle.öğrenci.kanGrubu);
                        cmd.Parameters.AddWithValue("@anne", OgrEkle.öğrenci.anneAdi);
                        cmd.Parameters.AddWithValue("@baba", OgrEkle.öğrenci.babaAdi);
                        cmd.Parameters.AddWithValue("@cins", OgrEkle.öğrenci.cinsiyet);
                        cmd.Parameters.AddWithValue("@ıd", 0);
                        await cmd.ExecuteNonQueryAsync();
                        cmd = new SqlCommand("",con);
                        con.Close();
                        return true;

                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        } // Öğrenci Ekleme
        public async Task<DataTable> ogr_cek()
        {
            dt = new DataTable();
            bool internet = await Task.Run(internetKontrol);
            if(internet==true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT OGR.tc,OGR.adSoyad,OGR.telefon,OGR.dogumTarihi,OGR.kanGrubu,Ogr.anneAdı,Ogr.babaAdı,OGR.cinsiyet,OGR.odaNo FROM dbo.Ögrenciler as OGR WHERE yurtId='"+AnaForm.yurtId+"' and aktif='EVET'", con);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch(Exception)
                {
                    return dt;
                }
            }
            else
            {
                return dt;
            }
        } // Öğrencileri Çekme Metodu
        public async Task<Boolean> oda_sil() //Oda Silme İşlemi
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE dbo.Odalar SET aktif='HAYIR' WHERE yurtId='"+AnaForm.yurtId+"' and odaNo='"+IdarePanel.oda.odaNo+"'",con);
                    int rowCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if(rowCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }catch(Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<Boolean> oda_guncelle() //Oda Güncelleme İşlemi
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE dbo.Odalar SET odaNo='"+IdarePanel.oda.yeniOdaNo+"' WHERE odaNo='"+IdarePanel.oda.odaNo+"' and yurtId='"+AnaForm.yurtId+"' and aktif='EVET'",con);
                    int rowsCount = await cmd.ExecuteNonQueryAsync();
                    if(rowsCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<Boolean> ogr_yerlestir() //Öğrenciyi Odaya Yerleştirme İşlemi
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT yatakSayisi FROM dbo.Yurtlar WHERE id='"+AnaForm.yurtId+"'",con); // Yurttaki odalarda bulunan yatak sayılarının alınması
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    int yatakSayisi = Convert.ToInt32(dt.Rows[0][0].ToString());
                    adapter = new SqlDataAdapter("SELECT Count(*) FROM dbo.Öğrenciler WHERE odaNo='"+IdarePanel.oda.odaNo+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    int ogrSay = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if(ogrSay<yatakSayisi)
                    {
                        await con.OpenAsync();
                        cmd = new SqlCommand("UPDATE dbo.Öğrenciler SET odaNo='"+IdarePanel.oda.odaNo+"' WHERE tc='"+IdarePanel.öğrenci.tc+"'",con);
                        int rowsCount = await cmd.ExecuteNonQueryAsync();
                        cmd = new SqlCommand("UPDATE dbo.Öğrenciler SET yurtId='" + AnaForm.yurtId + "' WHERE tc='" + IdarePanel.öğrenci.tc + "'", con);
                        await cmd.ExecuteNonQueryAsync();
                        con.Close();
                        if (rowsCount>0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception)
                {

                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<Boolean> ogr_odaSil() // Öğrenciyi Odadan Siler
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE dbo.Öğrenciler SET odaNo=0,yurtId=0 WHERE tc='"+IdarePanel.öğrenci.tc+"'",con);
                    int rowsCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if(rowsCount>0)
                    {
                        await con.OpenAsync();
                        cmd = new SqlCommand("UPDATE dbo.Devamsizlik SET yurtId=@p WHERE tc='"+IdarePanel.öğrenci.tc+"'",con);
                        cmd.Parameters.AddWithValue("@p",0);
                        await cmd.ExecuteNonQueryAsync();
                        con.Close();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {

                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<DataTable> devamsizlik_cek()
        {
            dt = new DataTable();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT ogrenciTc,devamsizlikTarihi FROM dbo.Devamsizlik WHERE yurtId='"+AnaForm.yurtId+"' and sil='HAYIR'",con);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (Exception)
                {
                    return dt;
                }
            }
            else
            {
                return dt;
            }
        } //Devamsizliklari Sistemden Çeker
        public async Task<Boolean> devamsizlik_ekle()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM dbo.Öğrenciler WHERE tc='"+IdarePanel.öğrenci.tc+"' and yurtId='"+AnaForm.yurtId+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
                    {
                        adapter = new SqlDataAdapter("SELECT * FROM dbo.Devamsizlik WHERE ogrenciTc='"+IdarePanel.öğrenci.tc+"' and devamsizlikTarihi='"+IdarePanel.devamsizlik.devamsizlikTarihi+"' and sil='HAYIR'",con);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        adapter.Dispose();
                        if(dt.Rows.Count>0)
                        {
                            return false;
                        }
                        else
                        {
                            await con.OpenAsync();
                            cmd = new SqlCommand("EXEC sp_Devamsizlik @id,@tc,@tarih,@yil", con);
                            cmd.Parameters.AddWithValue("@id", AnaForm.yurtId);
                            cmd.Parameters.AddWithValue("@tc", IdarePanel.öğrenci.tc);
                            cmd.Parameters.AddWithValue("@tarih", IdarePanel.devamsizlik.devamsizlikTarihi);
                            cmd.Parameters.AddWithValue("@yil", IdarePanel.devamsizlik.devamsizlikYili);
                            int rowsCount = await cmd.ExecuteNonQueryAsync();
                            con.Close();
                            if (rowsCount > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        } // Devamsizliklari Ekler
        public async Task<Boolean> devamsizlik_sil()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if (internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE dbo.Devamsizlik SET sil='EVET' WHERE ogrenciTc='"+IdarePanel.öğrenci.tc+"' and devamsizlikTarihi='"+IdarePanel.devamsizlik.devamsizlikTarihi+"'",con);
                    int rowCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if(rowCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception )
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Devamsizlik Sil
        public async Task<Boolean> devamsizlik_güncelle()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE dbo.Devamsizlik SET devamsizlikTarihi='"+IdarePanel.devamsizlik.devamsizlikGüncellenecek+"' WHERE ogrenciTc='"+IdarePanel.öğrenci.tc+"' and devamsizlikTarihi='"+IdarePanel.devamsizlik.devamsizlikTarihi+"'",con);
                    int rowCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if(rowCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Devamsizlik Güncelle
        public async Task<DataTable> ziyaretci_cek()
        {
            dt = new DataTable();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT ziyaretciAd,adSoyad,öğrenciTc,tarih FROM Ziyaretciler WHERE yurtId='"+AnaForm.yurtId+"' and sil='HAYIR'",con);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (Exception)
                {
                    return dt;
                }
            }
            else
            {
                return dt;
            }
        } //Ziyaretçi Çekme İşlemi
        public async Task<Boolean> ziyaretci_ekle()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if (internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM dbo.Öğrenciler WHERE tc='"+IdarePanel.ziyaretci.ogrTc+"' and yurtId='"+AnaForm.yurtId+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if (dt.Rows.Count>0)
                    {
                        adapter = new SqlDataAdapter("SELECT * FROM Ziyaretciler WHERE yurtId='"+AnaForm.yurtId+"' and ziyaretciAd='"+IdarePanel.ziyaretci.ziyaretciAd+"' and adSoyad='"+IdarePanel.ziyaretci.ogrAd+"' and öğrenciTc='"+IdarePanel.ziyaretci.ogrTc+"' and tarih='"+DateTime.Now.ToShortDateString()+"' and sil='HAYIR'",con);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        adapter.Dispose();
                        if (dt.Rows.Count>0)
                        {
                            return false;
                        }
                        else
                        {
                            await con.OpenAsync();
                            cmd = new SqlCommand("EXEC sp_Ziyaretci @id,@ad,@ogr,@tc,@tarih",con);
                            cmd.Parameters.AddWithValue("@id",AnaForm.yurtId);
                            cmd.Parameters.AddWithValue("@ad",IdarePanel.ziyaretci.ziyaretciAd);
                            cmd.Parameters.AddWithValue("ogr",IdarePanel.ziyaretci.ogrAd);
                            cmd.Parameters.AddWithValue("@tc",IdarePanel.ziyaretci.ogrTc);
                            cmd.Parameters.AddWithValue("@tarih",DateTime.Now.ToShortDateString());
                            int rowCount = await cmd.ExecuteNonQueryAsync();
                            con.Close();
                            if(rowCount>0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Ziyaretçi Ekleme İşlemi
        public async Task<Boolean> ziyaretci_sil() //Ziyaretçi Silme İşlemi
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if (internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE Ziyaretciler SET sil='EVET' WHERE yurtId='"+AnaForm.yurtId+"' and ziyaretciAd='"+IdarePanel.ziyaretci.ziyaretciAd+"' and adSoyad='"+IdarePanel.ziyaretci.ogrAd+"' and öğrenciTc='"+IdarePanel.ziyaretci.ogrTc+"' and tarih='"+IdarePanel.ziyaretci.tarih+"'",con);
                    int rowsCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if (rowsCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<Boolean> ziyaretci_güncelle() //Ziyaretçi Güncelleme
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if (internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE Ziyaretciler SET ziyaretciAd='"+IdarePanel.ziyaretci.ziyaretciAdG+"' WHERE yurtId='" + AnaForm.yurtId + "' and ziyaretciAd='" + IdarePanel.ziyaretci.ziyaretciAd + "' and adSoyad='" + IdarePanel.ziyaretci.ogrAd + "' and öğrenciTc='" + IdarePanel.ziyaretci.ogrTc + "' and tarih='" + IdarePanel.ziyaretci.tarih + "'", con);
                    int rowsCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if (rowsCount > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public async Task<DataTable>pdf_cek()
        {
            dt = new DataTable();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT raporAdı FROM Raporlar WHERE yurtId='"+AnaForm.yurtId+"' and aktif='EVET'",con);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (Exception)
                {
                    return dt;
                }
            }
            else
            {
                return dt;
            }
        } //PDF Çekme İşlemi
        public async Task<Boolean> pdf_Ekle()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM Raporlar WHERE id='"+AnaForm.yurtId+"' and raporAdı='"+IdarePanel.rapor.raporAdi+"' and aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
                    {
                        return false;
                    }
                    else
                    {
                        await con.OpenAsync();
                        cmd = new SqlCommand("INSERT INTO Raporlar(yurtId,raporAdı,rapor,aktif) VALUES('"+AnaForm.yurtId+"','"+IdarePanel.rapor.raporAdi+"',@rap,'EVET')",con);
                        SqlParameter param = cmd.Parameters.AddWithValue("@rap",IdarePanel.rapor.dosya);
                        param.DbType = DbType.Binary;
                        int rowsCount = await cmd.ExecuteNonQueryAsync();
                        con.Close();
                        if (rowsCount > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }//Pdf Ekleme
        public async Task<Boolean> pdf_Sil() 
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE Raporlar SET aktif='HAYIR' WHERE yurtId='"+AnaForm.yurtId+"' and raporAdı='"+IdarePanel.rapor.raporAdi+"'",con);
                    int rowsCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if(rowsCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //PDF Silme
        public async Task<Boolean> pdf_Yedekle()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                adapter = new SqlDataAdapter("SELECT raporAdı,rapor FROM Raporlar WHERE yurtId='"+AnaForm.yurtId+"' and aktif='EVET'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                adapter.Dispose();
                if(dt.Rows.Count>0)
                {
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        fs = new FileStream(IdarePanel.rapor.kayıtDizini+@"\"+dt.Rows[i][0].ToString(),FileMode.Create,FileAccess.ReadWrite);
                        bw = new BinaryWriter(fs);
                        byte[] doc = null;
                        doc = (byte[])dt.Rows[i][1];
                        bw.Write(doc);
                        bw.Close();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //PDF Yedekleme İşlemleri
        public async Task<Boolean> pdf_indir()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT rapor FROM Raporlar WHERE yurtId='"+AnaForm.yurtId+"' and raporAdı='"+IdarePanel.rapor.raporAdi+"' and aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
                    {
                        fs = new FileStream(IdarePanel.rapor.kayıtDizini,FileMode.Create,FileAccess.ReadWrite);
                        bw = new BinaryWriter(fs);
                        byte[] doc = null;
                        doc = (byte[])dt.Rows[0][0];
                        bw.Write(doc);
                        bw.Close();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //PDF İndir
        public async Task<DataTable> aidat_Cek()
        {
            dt = new DataTable();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                adapter = new SqlDataAdapter("SELECT ogrTc,ay,yıl,aidatTutar,odendi FROM Aidatlar WHERE yurtId='"+AnaForm.yurtId+"'",con);
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
            else
            {
                return dt;
            }
        } //Aidat Cek
        public async Task<Boolean> aidat_Ekle()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT aidat FROM Yurtlar WHERE id='"+AnaForm.yurtId+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    int aidat = Convert.ToInt32(dt.Rows[0][0]);
                    adapter = new SqlDataAdapter("SELECT tc FROM Ögrenciler WHERE yurtId='"+AnaForm.yurtId+"' and aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    await con.OpenAsync();
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        cmd = new SqlCommand("INSERT INTO Aidatlar(yurtId,ogrTc,ay,yıl,aidatTutar,odendi) VALUES('"+AnaForm.yurtId+"','"+dt.Rows[i][0].ToString()+"','"+DateTime.Now.Month.ToString()+"','"+DateTime.Now.Year.ToString()+"','"+aidat+"','HAYIR')",con);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    con.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } // Aidat Ekle
        public async Task<Boolean> aidat_Ode()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand("UPDATE Aidatlar SET odendi='EVET' WHERE yurtId='"+AnaForm.yurtId+"' and ogrTc='"+IdarePanel.aidat.tc+"' and ay='"+IdarePanel.aidat.ay+"' and yıl='"+IdarePanel.aidat.yıl+"'",con);
                    int rowsCount = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if(rowsCount>0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } //Aidat Ödeme
        public async Task<DataTable> ogr_Filtre()
        {
            dt = new DataTable();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                adapter = new SqlDataAdapter(query,con);
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
            else
            {
                return dt;
            }
        }
        //Öğrenci Detay Sayfası
        public async Task<MemoryStream> ogr_Foto()
        {
            adapter = new SqlDataAdapter("SELECT resim FROM Ögrenciler WHERE yurtId='" + AnaForm.yurtId + "' and tc='" + IdarePanel.öğrenci.tc + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);
            adapter.Dispose();
            byte[] doc = null;
            doc = (byte[])dt.Rows[0][0];
            ms = new MemoryStream(doc);
            return ms;
        }
        public async Task<ArrayList> ogr_Bilgi()
        {
            ArrayList data = new ArrayList();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT adSoyad,telefon,dogumTarihi,kanGrubu,anneAdı,babaAdı,odaNo FROM Ögrenciler WHERE tc='"+IdarePanel.öğrenci.tc+"' and aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        for(int k=0;k<dt.Columns.Count;k++)
                        {
                            data.Add(dt.Rows[i][k].ToString());
                        }
                    }
                    return data;
                }
                catch (Exception)
                {
                    return data;
                }
            }
            else
            {
                return data;
            }
        }
        //Öğrenci Detay Sayfası
        //Yönetici Paneli İşlemleri Bitiş
        //Öğrenci Paneli İşlemleri Başlangıç
        public async Task<MemoryStream> ogrFoto()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT resim FROM Ögrenciler WHERE tc='"+AnaForm.ogrTc+"' and aktif='EVET'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                adapter.Dispose();
                byte[] doc = (byte[])dt.Rows[0][0];
                ms = new MemoryStream(doc);
                return ms;
            }
            catch (Exception)
            {
                return new MemoryStream();
            }
        }
        public async Task<ArrayList> ogrBilgi()
        {
            ArrayList data = new ArrayList();
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if (internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT adSoyad,telefon,dogumTarihi,kanGrubu,anneAdı,babaAdı,odaNo FROM Ögrenciler WHERE tc='" + AnaForm.ogrTc + "' and aktif='EVET'", con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            data.Add(dt.Rows[i][k].ToString());
                        }
                    }
                    return data;
                }
                catch (Exception)
                {
                    return data;
                }
            }
            else
            {
                return data;
            }
        }
        public async Task<DataTable> devamsizlik_cekOGR()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT devamsizlikTarihi FROM Devamsizlik WHERE sil='HAYIR' and ogrenciTc='"+AnaForm.ogrTc+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (Exception ex)
                {
                    return new DataTable();
                }
            }
            else
            {
                return new DataTable();
            }
        }
        public async Task<DataTable> ziyaretci_cekOGR()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT ziyaretciAd,tarih FROM Ziyaretciler WHERE öğrenciTc='"+AnaForm.ogrTc+"' and sil='HAYIR'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (Exception)
                {
                    return new DataTable();
                }
            }
            else
            {
                return new DataTable();
            }
        }

        public async Task<DataTable> aidat_cekOGR()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT ay,yıl,aidatTutar,odendi FROM Aidatlar WHERE ogrTc='"+AnaForm.ogrTc+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (Exception)
                {
                    return new DataTable();
                }
            }
            else
            {
                return new DataTable();
            }
        }
        public async Task<DataTable> oda_Arkadas()
        {
            bool internet = false;
            internet = await Task.Run(internetKontrol);
            if(internet == true)
            {
                try
                {
                    //Eğer İleride 1'den fazla SubQuery'den veri dönerse güvenlik açığı çıkacaktır Bilginize...
                    adapter = new SqlDataAdapter("SELECT adSoyad,telefon,dogumTarihi,kanGrubu FROM Öğrenciler WHERE odaNo=(SELECT odaNo FROM Öğrenciler WHERE tc='"+AnaForm.ogrTc+"' and aktif='EVET') and tc<>'"+AnaForm.ogrTc+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (Exception)
                {
                    return new DataTable();
                }
            }
            else
            {
                return new DataTable();
            }
        }
        //Öğrenci Paneli İşlemleri Bitiş
    }
}
