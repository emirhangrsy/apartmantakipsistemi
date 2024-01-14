using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Threading;


namespace ApartmanTakipSistemi
{
    class Program
    {
        public enum KullaniciRol
        {
            Admin,
            Standart
        }

        public class ApartmanSakini
        {
            public int Id { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string DaireNo { get; set; }
            public string eposta { get; set; }
            public string Telefon { get; set; }
            public bool AidatOdendiMi { get; set; } = false;
        }

        static List<ApartmanSakini> apartmanSakinleri = new List<ApartmanSakini>();
        static string dosyaYolu = "sakinler.txt";

        static void Main(string[] args)
        {
            VerileriDosyadanOku();

            bool girisBasarili = false;
            while (!girisBasarili)
            {
                Console.WriteLine("Lütfen giriş yapın");
                Console.Write("Kullanıcı Adı: ");
                string kullaniciAdi = Console.ReadLine();

                Console.Write("Şifre: ");
                string sifre = SifreGirisi();  // Şifre girişini bu fonksiyonla yapalım.

                KullaniciRol kullaniciRol = GirisKontrol(kullaniciAdi, sifre);

                if (kullaniciRol == KullaniciRol.Admin)
                {
                    Console.Clear();
                    BaslatYoneticiApartmanTakipSistemi();
                    girisBasarili = true;
                }
                else if (kullaniciRol == KullaniciRol.Standart)
                {
                    Console.Clear();
                    BaslatStandartApartmanTakipSistemi();
                    girisBasarili = true;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Kullanıcı adı veya şifre yanlış! Tekrar deneyin.\n------------------------------------------------");
                }

            }


            VerileriDosyayaKaydet();
        }

        // Kullanıcının şifresini gizli bir şekilde girmesini sağlayan fonksiyon
        static string SifreGirisi()
        {
            string sifre = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Eğer kullanıcı backspace tuşuna basarsa, karakteri sifreden çıkar.
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    sifre += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && sifre.Length > 0)
                {
                    sifre = sifre.Substring(0, (sifre.Length - 1));
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine(); // Yeni satıra geçelim.

            return sifre;
        }

        static KullaniciRol GirisKontrol(string kullaniciAdi, string sifre)
        {
            if (kullaniciAdi == "yonetici" && sifre == "6161")
            {
                return KullaniciRol.Admin;
            }
            else if (kullaniciAdi == "standart" && sifre == "6161")
            {
                return KullaniciRol.Standart;
            }
            else
            {
                return (KullaniciRol)(-1); // Geçersiz rol.
            }
        }

        static void BaslatYoneticiApartmanTakipSistemi()
        {
            while (true)
            {
                Console.WriteLine("1. Sakini Ekle");
                Console.WriteLine("2. Sakini Sil");
                Console.WriteLine("3. Sakini Güncelle");
                Console.WriteLine("4. Sakinleri Listele");
                Console.WriteLine("5. Çıkış");
                Console.Write("Seçiminiz: ");
                string secim = Console.ReadLine();
                Console.Clear();

                switch (secim)
                {
                    case "1":
                        SakinEkle();
                        break;
                    case "2":
                        SakinSil();
                        break;
                    case "3":
                        SakinGuncelle();
                        break;
                    case "4":
                        SakinleriListele();
                        break;
                    case "5":
                        return;
                    default:
                        Console.Clear();
                        Console.WriteLine("Geçersiz seçim! Lütfen geçerli bir seçim yapınız.\n-------------------------------------------------");
                        
                        break;
                }
            }
        }

        static void BaslatStandartApartmanTakipSistemi()
        {
            while (true)
            {
                
                Console.WriteLine("1. Sakinleri Listele");
                Console.WriteLine("2. Aidat Ödeme");
                Console.WriteLine("3. Çıkış");
                Console.Write("Seçiminiz: ");
                string secim = Console.ReadLine();
                Console.Clear() ;

                switch (secim)
                {
                    case "1":
                        SakinleriListele();
                        break;
                    case "2":
                        AidatOde();
                        break;
                     case "3":
                        return;
                    default:
                        Console.Clear();
                        Console.WriteLine("Geçersiz seçim! Lütfen geçerli bir seçim yapınız.\n-------------------------------------------------");
                        break;
                }
            }
        }


        static void SakinEkle()
        {
            ApartmanSakini sakin = new ApartmanSakini();

            Console.Write("Ad: ");
            sakin.Ad = Console.ReadLine();

            Console.Write("Soyad: ");
            sakin.Soyad = Console.ReadLine();

            // Daire No için 1 ile 20 arasında sayı kontrolü
            bool validDaireNo = false;
            while (!validDaireNo)
            {
                Console.Write("Daire No (1-20 arası bir sayı girin): ");
                string daireNoInput = Console.ReadLine();

                if (int.TryParse(daireNoInput, out int daireNo) && daireNo >= 1 && daireNo <= 20)
                {
                    sakin.DaireNo = daireNoInput;
                    validDaireNo = true;
                }
                else
                {
                    Console.WriteLine("Yanlış daire girişi. Lütfen doğru daire numarası giriniz.");
                }
            }

            // Telefon için sadece sayı ve 10 haneden fazla olmayan giriş kontrolü
            bool validTelefon = false;
            while (!validTelefon)
            {
                Console.Write("Telefon (10 haneden az sayı girin): ");
                string telefonInput = Console.ReadLine();

                if (long.TryParse(telefonInput, out long telefonNumarasi) && telefonInput.Length == 10)
                {
                    sakin.Telefon = telefonInput;
                    validTelefon = true;
                }
                else
                {
                    Console.WriteLine("Yanlış telefon numarası girişi. Lütfen doğru bir telefon numarası giriniz.");
                }
            }

            // E-posta adresi için basit bir doğrulama
            bool validEmail = false;
            while (!validEmail)
            {
                Console.Write("E-posta Adresi: ");
                string emailInput = Console.ReadLine();

                if (IsValidEmail(emailInput))
                {
                    sakin.eposta = emailInput;
                    validEmail = true;
                }
                else
                {
                    Console.WriteLine("Geçersiz e-posta adresi formatı. Lütfen doğru bir e-posta adresi giriniz.");
                }
            }

            sakin.Id = apartmanSakinleri.Count + 1;
            apartmanSakinleri.Add(sakin);

            Console.WriteLine("Sakin başarıyla eklendi.");
            Console.WriteLine("\nAna menüye otomatik olarak dönülmek için 5 saniye bekleniyor. Hızlı dönmek için bir tuşa basabilirsiniz...");
            for (int i = 5; i > 0; i--)
            {
                Console.Write($"\r{i} saniye sonra ana menüye dönülüyor...");
                Thread.Sleep(1000); // 1 saniye bekleyin.
            }

            // Ekrandaki mesajı temizleyin.
            Console.Clear();
        }

        // Basit e-posta adresi doğrulama fonksiyonu
        static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        static string SonKullanımTarihiAl()
        {
            while (true) // Doğru bir tarih girilene kadar döngüyü sürdür.
            {
                Console.Write("Son Kullanım Tarihi (AA/YY formatında giriniz): ");
                string tarih = Console.ReadLine();

                // Basit bir doğrulama: İki rakamın yan yana gelmesi bekleniyor (Örn: 01/26).
                if (tarih.Length == 5 && tarih[2] == '/' && int.TryParse(tarih.Substring(0, 2), out int ay) && int.TryParse(tarih.Substring(3), out int yil))
                {
                    if (ay >= 1 && ay <= 12 && yil >= 0 && yil <= 99)
                    {
                        return tarih; // Doğru format ve değer girildiğinde tarihi döndür.
                    }
                }

                Console.WriteLine("Geçersiz tarih formatı! Lütfen AA/YY formatında bir tarih giriniz.");
            }
        }

        static void AidatOde()
        {
            Console.Write("Aidatını ödeyeceğiniz daire numarasını girin: ");
            string daireNo = Console.ReadLine();

            var sakinler = apartmanSakinleri.Where(s => s.DaireNo == daireNo).ToList();

            if (sakinler.Count == 0)
            {
                Console.WriteLine("Belirtilen daire numarasıyla eşleşen sakini bulunamadı.");
                return;
            }

            Console.WriteLine("Aidatını ödeyeceğiniz sakinleri listeleniyor:");
            foreach (var sakin in sakinler)
            {
                string aidatDurumu = sakin.AidatOdendiMi ? "Ödendi" : "Ödenmedi";
                Console.WriteLine($"Daire No: {sakin.DaireNo}, ID: {sakin.Id}, Ad: {sakin.Ad}, Soyad: {sakin.Soyad}, Aidat: {aidatDurumu}");
            }

            Console.Write("Aidatını ödeyeceğiniz sakinin ID'sini girin: ");
            if (int.TryParse(Console.ReadLine(), out int selectedId))
            {
                var selectedSakin = sakinler.FirstOrDefault(s => s.Id == selectedId);
                if (selectedSakin != null)
                {
                    bool isValid = false;
                    string kartNumarasi = "";
                    while (!isValid)
                    {
                        Console.Write("Kart Numarasını Girin (16 haneli): ");
                        kartNumarasi = Console.ReadLine();
                        if (kartNumarasi.Length == 16 && kartNumarasi.All(char.IsDigit))
                        {
                            isValid = true;
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz kart numarası girişi. Lütfen tekrar deneyin.");
                        }
                    }

                    string sonKullanımTarihi = SonKullanımTarihiAl();

                    isValid = false;
                    string cvv = "";
                    while (!isValid)
                    {
                        Console.Write("CVV (3 haneli): ");
                        cvv = Console.ReadLine();
                        if (cvv.Length == 3 && cvv.All(char.IsDigit))
                        {
                            isValid = true;
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz CVV girişi. Lütfen tekrar deneyin.");
                        }
                    }

                    if (!string.IsNullOrEmpty(selectedSakin.eposta))
                    {
                        string konu = "Aidat Ödemesi Tamamlandı";
                        string icerik = "Aidat ödemeniz başarıyla gerçekleşti. Teşekkür ederiz.";
                        MailGonder(selectedSakin.eposta, konu, icerik);

                        // Aidatın ödendiğini tüm dairedeki sakınlara uygula
                        foreach (var sakin in sakinler)
                        {
                            sakin.AidatOdendiMi = true;
                        }

                        VerileriDosyayaKaydet();

                        Console.WriteLine($"Aidat ödemesi {selectedSakin.Ad} {selectedSakin.Soyad} için başarıyla tamamlandı ve aynı dairedeki diğer sakinlerin aidat ödeme durumu güncellendi.");
                    }
                    else
                    {
                        Console.WriteLine("Seçilen sakinin e-posta adresi bulunamadı veya geçersiz.");
                    }
                }
                else
                {
                    Console.WriteLine("Belirtilen ID ile bir sakini bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz ID formatı.");
            }
        }


        static void MailGonder(string eposta, string konu, string icerik)
        {
            // SMTP sunucu bilgilerini ayarlayın (örnekte Gmail kullanıldı)
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Gmail için genellikle 587 portu kullanılır.
            string email = "apartmantakipsistemi@gmail.com";
            string password = "shbh uxmb jkug kyxw"; // Gmail şifrenizi buraya yazın.

            // E-posta bilgilerini oluşturun
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email);
            mail.To.Add(eposta); // Alıcı e-posta adresi
            mail.Subject = konu; // E-posta konusu
            mail.Body = icerik; // E-posta içeriği

            // SMTP istemcisini oluşturun ve ayarları yapın
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.EnableSsl = true; // SSL şifrelemesi aktif edilir (Gmail için gerekli)
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(email, password);

            try
            {
                // E-postayı gönderin
                smtpClient.Send(mail);
                Console.WriteLine("E-posta başarıyla gönderildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("E-posta gönderilirken bir hata oluştu: " + ex.Message);
            }
        }

        static void SakinSil()
        {
            Console.Write("Silmek istediğiniz sakinin ID'sini girin: ");
            int id;
            if (int.TryParse(Console.ReadLine(), out id))
            {
                ApartmanSakini sakin = apartmanSakinleri.Find(s => s.Id == id);
                if (sakin != null)
                {
                    apartmanSakinleri.Remove(sakin);
                    Console.WriteLine("Sakin başarıyla silindi.");
                }
                else
                {
                    Console.WriteLine("Belirtilen ID ile sakin bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz ID formatı.");
            }
            Console.WriteLine("\nAna menüye otomatik olarak dönülmek için 5 saniye bekleniyor. Hızlı dönmek için bir tuşa basabilirsiniz...");
            for (int i = 5; i > 0; i--)
            {
                Console.Write($"\r{i} saniye sonra ana menüye dönülüyor...");
                Thread.Sleep(1000); // 1 saniye bekleyin.
            }

            // Ekrandaki mesajı temizleyin.
            Console.Clear();
        }

        static void SakinGuncelle()
        {
            Console.Write("Hangi dairede yaşayanları güncellemek istiyorsunuz (Daire No): ");
            string daireNo = Console.ReadLine();

            var sakiniGuncellenecekListe = apartmanSakinleri.Where(s => s.DaireNo == daireNo).ToList();

            if (sakiniGuncellenecekListe.Count == 0)
            {
                Console.WriteLine("Belirttiğiniz dairede hiç sakini bulunmamaktadır.");
                return;
            }

            Console.Clear();
            Console.WriteLine("Güncellenecek Sakinler:");
            foreach (var sakin in sakiniGuncellenecekListe)
            {
                Console.WriteLine($"ID: {sakin.Id}, Ad: {sakin.Ad}, Soyad: {sakin.Soyad}, Telefon: {sakin.Telefon}, E-posta: {sakin.eposta}");
            }

            Console.Write("Hangi ID'li sakinin bilgisini güncellemek istiyorsunuz: ");
            int guncellenecekId;
            if (int.TryParse(Console.ReadLine(), out guncellenecekId))
            {
                var secilenSakin = sakiniGuncellenecekListe.FirstOrDefault(s => s.Id == guncellenecekId);
                if (secilenSakin != null)
                {
                    Console.Clear();
                    Console.WriteLine("Ne güncellemek istiyorsunuz?");
                    Console.WriteLine("1. Ad");
                    Console.WriteLine("2. Soyad");
                    Console.WriteLine("3. Telefon");
                    Console.WriteLine("4. E-posta");
                    Console.WriteLine("5. Ana Menüye Dön");

                    string secim = Console.ReadLine();
                    Console.Clear();

                    switch (secim)
                    {
                        case "1":
                            Console.Write("Yeni Ad: ");
                            secilenSakin.Ad = Console.ReadLine();
                            break;
                        case "2":
                            Console.Write("Yeni Soyad: ");
                            secilenSakin.Soyad = Console.ReadLine();
                            break;
                        case "3":
                            Console.Write("Yeni Telefon: ");
                            secilenSakin.Telefon = Console.ReadLine();
                            break;
                        case "4":
                            Console.Write("Yeni E-posta: ");
                            secilenSakin.eposta = Console.ReadLine();
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Geçersiz seçim!");
                            break;
                    }

                    Console.Clear();
                    Console.WriteLine("Sakin başarıyla güncellendi.");
                }
                else
                {
                    Console.WriteLine("Belirttiğiniz ID ile bir sakini bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz ID formatı.");
            }

            Console.Write("Başka bir güncelleme yapmak ister misiniz? (Evet/Hayır): ");
            string devamEt = Console.ReadLine();
            if (devamEt.ToLower() == "evet")
            {
                Console.Clear();
                SakinGuncelle(); // Rekürsif olarak kendisini çağırarak bir sonraki güncellemeyi yapabilir.
            }
            else
            {
                Console.WriteLine("Ana menüye dönülüyor...");
                Console.Clear();
            }
        }

        static void SakinleriListele()
        {
            Console.WriteLine("Apartman Sakinleri:");

            // Eğer apartmanSakinleri listesi boşsa, döngüye girmemek için kontrol ekleyelim.
            if (apartmanSakinleri.Count == 0)
            {
                Console.WriteLine("Sistemde kayıtlı sakin bulunmamaktadır.");
                return;
            }

            try
            {
                // Daire numarasına göre sıralama ve sakinleri listeleme
                var siraliSakinler = apartmanSakinleri.OrderBy(s => int.Parse(s.DaireNo)).ToList();

                foreach (var sakin in apartmanSakinleri)
                {
                    string aidatDurumu = sakin.AidatOdendiMi ? "Ödendi" : "Ödenmedi";
                    Console.WriteLine($"Daire No: {sakin.DaireNo}, ID: {sakin.Id}, Ad: {sakin.Ad}, Soyad: {sakin.Soyad}, Telefon: {sakin.Telefon}, E-posta: {sakin.eposta}, Aidat: {aidatDurumu}");
                }
            }
            catch (Exception ex)
            {
                // Eğer bir hata oluşursa, bu hatayı yakalayalım ve ekrana mesaj olarak gösterelim.
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
            Console.WriteLine("Lütfen Ana Menüye dönmek için bir tuşa basınız.");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("\nAna menüye otomatik olarak dönülmek için 5 saniye bekleniyor. Hızlı dönmek için bir tuşa basabilirsiniz...");
            for (int i = 5; i > 0; i--)
            {
                Console.Write($"\r{i} saniye sonra ana menüye dönülüyor...");
                Thread.Sleep(1000); // 1 saniye bekleyin.
            }

            // Ekrandaki mesajı temizleyin.
            Console.Clear();
        }

        static void VerileriDosyadanOku()
        {
            try
            {
                if (File.Exists(dosyaYolu))
                {
                    string[] satirlar = File.ReadAllLines(dosyaYolu);

                    foreach (var satir in satirlar)
                    {
                        string[] veri = satir.Split(',');

                        // Veri dizisinin boyutunu kontrol edelim
                        if (veri.Length >= 5) // Örneğin, en az 5 elemanı olması gerekiyorsa
                        {
                            ApartmanSakini sakin = new ApartmanSakini
                            {
                                Id = int.Parse(veri[0]),
                                Ad = veri[1],
                                Soyad = veri[2],
                                DaireNo = veri[3],
                                Telefon = veri[4],
                                eposta = veri[5],
                                AidatOdendiMi = bool.Parse(veri[6])
                            };

                            // Diğer sakin özelliklerini de ekleyebilirsiniz.
                            apartmanSakinleri.Add(sakin);
                        }
                        else
                        {
                            Console.WriteLine("Dosyadan okunan satır beklenen formatı karşılamıyor.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Veri dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }

        static void VerileriDosyayaKaydet()
        {
            List<string> satirlar = new List<string>();
    foreach (var sakin in apartmanSakinleri)
    {
        string satir = $"{sakin.Id},{sakin.Ad},{sakin.Soyad},{sakin.DaireNo},{sakin.Telefon},{sakin.eposta},{sakin.AidatOdendiMi}";
        satirlar.Add(satir);
    }
    File.WriteAllLines(dosyaYolu, satirlar);
     }
    }
}