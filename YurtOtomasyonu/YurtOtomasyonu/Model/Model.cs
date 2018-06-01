using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YurtOtomasyonu
{
    public class Bölgeler
    {
        //Bölgeler
        public string il { get; set; }
        public string ilce { get; set; }
    }
    public class Devamsızlık
    {
        //Devamsızlık
        public int ögrenciId { get; set; }
        public string devamsizlikTarihi { get; set; }
        public string devamsizlikYili { get; set; }
        public string devamsizlikGüncellenecek { get; set; }
    }
    public class ögrenci
    {
        //Öğrenci
        public int id { get; set; }
        public byte[] resim { get; set; }
        public string tc { get; set; }
        public string adSoyad { get; set; }
        public string tel { get; set; }
        public string dogumTarihi { get; set; }
        public string kanGrubu { get; set; }
        public string anneAdi { get; set; }
        public string babaAdi { get; set; }
        public string cinsiyet { get; set; }
        public int yurtİd { get; set; }
        public string odaNo { get; set; }
        public string yatakNo { get; set; }
    }
    public class Raporlar
    {
        //Raporlar
        public string raporAdi { get; set; }
        public byte[] dosya { get; set; }
        public string kayıtDizini { get; set; } //Bilgisayara İndirme İçin
    }
    public class Yöneticler
    {
        //Yöneticiler Doğrudan MSSQL Üzerinden Eklenecektir!
        public string username { get; set; }
        public string password { get; set; }
    }
    public class Yurtlar
    {
        //Yurtlar
        public string ad { get; set; }
        public string il { get; set; }
        public string ilce { get; set; }
        public int aidat { get; set; }
        public int odaSayisi { get; set; }
        public int yatakSayisi { get; set; }
        public int kapasite { get; set; }
        public string tel { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string secCode { get; set; }
    }  
    public class Ziyaretciler
    {
        //Ziyaretçiler
        public string ziyaretciAd { get; set; }
        public string ziyaretciAdG { get; set; } // Güncelleme
        public string ogrAd { get; set; }
        public string ogrTc { get; set; }
        public string tarih { get; set; }
    }
    public class oda
    {
        public string odaNo { get; set; }
        public string yeniOdaNo { get; set; }
    }
    public class Aidat
    {
        public string tc { get; set; }
        public string ay { get; set; }
        public string yıl { get; set; }
    }
}
