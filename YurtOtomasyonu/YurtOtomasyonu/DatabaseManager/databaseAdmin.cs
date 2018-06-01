using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YurtOtomasyonu.Forms;

namespace YurtOtomasyonu
{
    //Sistem Yöneticilerin Yurt Onaylama ve Yayınlama İşlemleri Burada Kodlanacak
    //Burada Çok Bir İşlem Olmayacak!
    public class databaseAdmin
    {
        #region Variables
        private SqlConnection con = new SqlConnection("Data Source=FARUK;Initial Catalog=YurtOtomasyonu;User Id=root;Password=03102593");
        private SqlCommand cmd;
        private SqlDataAdapter adapter;
        private DataTable dt;
        public string query;
        private databaseUser user = new databaseUser();
        #endregion
        public void garbageCollector()
        {
            // :D
            user.garbageCollector();
        }
        public async Task<Boolean> yöneticiGiris()
        {
            //MD5 OLMADAN GİRİŞ YAPACAK SQL'DEN DOĞRUDAN EKLENECEĞİ İÇİN!
            bool internet = false;
            internet = await Task.Run(user.internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT * FROM Yöneticiler WHERE username='"+AnaForm.yöneticler.username+"' and password='"+AnaForm.yöneticler.password+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    if(dt.Rows.Count>0)
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
        public async Task<DataTable> yurtlar()
        {
            bool internet = false;
            internet = await Task.Run(user.internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT ad,il,ilce,aidat,odaSayisi,yatakSayisi,ogrenciSayisi,kapasite,tel,aktif FROM dbo.Yurtlar WHERE sil='HAYIR'",con);
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
        public async Task<ArrayList> iller()
        {
            bool internet = false;
            internet = await Task.Run(user.internetKontrol);
            if(internet == true)
            {
                try
                {
                    ArrayList array = new ArrayList();
                    adapter = new SqlDataAdapter("SELECT DISTINCT il FROM Bölgeler WHERE aktif='EVET'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        array.Add(dt.Rows[i][0].ToString());
                    }
                    return array;
                }
                catch (Exception)
                {
                    return new ArrayList();
                }
            }
            else
            {
                return new ArrayList();
            }
        }
        public async Task<ArrayList> ilceler()
        {
            bool internet = false;
            internet = await Task.Run(user.internetKontrol);
            if(internet == true)
            {
                try
                {
                    ArrayList array = new ArrayList();
                    adapter = new SqlDataAdapter(query,con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    adapter.Dispose();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        array.Add(dt.Rows[i][0].ToString());
                    }
                    return array;
                }
                catch (Exception)
                {
                    return new ArrayList();
                }
            }
            else
            {
                return new ArrayList();
            }
        }
        public async Task<DataTable> yurtFiltre()
        {
            bool internet = false;
            internet = await Task.Run(user.internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter(query,con);
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
        public async Task<Boolean> yurtİslem()
        {
            bool internet = false;
            internet = await Task.Run(user.internetKontrol);
            if(internet == true)
            {
                try
                {
                    await con.OpenAsync();
                    cmd = new SqlCommand(query,con);
                    int rows = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    if(rows>0)
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
        public async Task<DataTable> yurtlarvebölgeler()
        {
            bool internet = false;
            internet = await Task.Run(user.internetKontrol);
            if(internet == true)
            {
                try
                {
                    adapter = new SqlDataAdapter("SELECT il,ilce FROM Bölgeler WHERE aktif='EVET'",con);
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
    }
}
