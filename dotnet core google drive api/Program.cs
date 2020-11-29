using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace dotnet_core_google_drive_api
{
    class Program
    {
        public static int GetMenu()// ilk açılışta ki bilgilendirme-yönlendirme kısmı
        {
            Console.Clear();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Dosya İşlemleri.");
            Console.WriteLine("0. Login. ");
            Console.WriteLine("1. Dosya Yükle. ");
            Console.WriteLine("2. Çıkış. ");
            Console.WriteLine("--------------------------------------");
            Char vKey = new char();
            int inputKey = 0;
            bool isValidKey = false;
            do
            {
                vKey = Console.ReadKey().KeyChar;
                isValidKey = int.TryParse(vKey.ToString(), out inputKey);
            } while (!isValidKey);
            return inputKey;
        }
        static bool Login()// kullanıcı hesabına bağlantı yapılan kısım
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Drive.Auth.Store")).Result;
                Console.WriteLine("Kaydedildi.: " + credPath);
            }

            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return true;
        }
        static bool UploadFiles()// dosya gönderme işleminin yapıldığı kısım
        {
            Console.Clear();
            Console.WriteLine("Yüklenecek Dosyanın path ini yazınız.");
            string FullFileName = Console.ReadLine();
            Console.WriteLine("Yüklenecek Dosyanın adını yazınız.");
            string FileName = Console.ReadLine();
            if (System.IO.File.Exists(FullFileName))
            {
                var fileMetaData = new Google.Apis.Drive.v3.Data.File();
                fileMetaData.Name = FileName;
                Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(FullFileName, FileMode.Open))
                {
                    request = service.Files.Create(fileMetaData, stream, "image/jpeg");
                    request.Fields = "id";
                    request.Upload();
                    var fileId = request.ResponseBody;
                }
            }
            return true;
        }

        static string[] Scopes = new[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
        static string ApplicationName = "OAuthWebFilePermission"; // Olusturulan Uygulama Adı
        static DriveService service;

        static void Main(string[] args)
        {
                int vKey = 0;
                Login();
                do
                {
                    vKey = GetMenu();
                    switch (vKey)
                    {
                        case 0:
                            Login();
                            break;
                        case 1:
                            UploadFiles();
                            break;               
                        default:
                            break;
                    }

                } while (vKey != 2);
            }
        }
 }
