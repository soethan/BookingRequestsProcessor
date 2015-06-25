using BookingRequestsProcessor.Models;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BookingRequestsProcessor
{
    class Program
    {
        const string FTP_REQUESTS = @"D:\Personal\TestPrjs\BookingRequestsProcessor\BookingRequestsProcessor\Data\FTP_Requests";
        const string FTP_RESPONSE = @"D:\Personal\TestPrjs\BookingRequestsProcessor\BookingRequestsProcessor\Data\FTP_Response";
        const string FTP_READ = @"D:\Personal\TestPrjs\BookingRequestsProcessor\BookingRequestsProcessor\Data\FTP_Read";
        const string FTP_CORRUPTED = @"D:\Personal\TestPrjs\BookingRequestsProcessor\BookingRequestsProcessor\Data\FTP_Corrupted";

        static void Main(string[] args)
        {
            WatchBookingRequestsFolder();
            Console.WriteLine("Press any key to terminate the application...");
            Console.Read();
        }

        private static void WatchBookingRequestsFolder()
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = FTP_REQUESTS;
            watcher.NotifyFilter = NotifyFilters.Size;//.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnBookingRequest);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnBookingRequest(object sender, FileSystemEventArgs e)
        {
            var fileName = e.Name.Replace(".csv", string.Empty).Replace(".md5", string.Empty);

            if (File.Exists(string.Format("{0}\\{1}.csv", FTP_REQUESTS, fileName)) 
                && File.Exists(string.Format("{0}\\{1}.md5", FTP_REQUESTS, fileName)))
            {
                Console.WriteLine(string.Format("Process {0}", fileName));
                ProcessBookingRequests();
            }
            
        }

        static void ProcessBookingRequests()
        {
            
            foreach (var fileName in Directory.GetFiles(FTP_REQUESTS, "*.csv"))
            {
                if (!IsFileCorrupted(fileName))
                {
                    ProcessFile(fileName);
                }
                else
                {
                    var requestFileName = Path.GetFileName(fileName);
                    File.Move(fileName, string.Format("{0}\\{1}", FTP_CORRUPTED, requestFileName));
                    File.Move(fileName.Replace(".csv", ".md5"), string.Format("{0}\\{1}", FTP_CORRUPTED, requestFileName.Replace(".csv", ".md5")));
                }
            }
        }

        private static bool IsFileCorrupted(string fileName)
        {
            var md5HashOfMD5 = File.ReadAllBytes(fileName.Replace(".csv", ".md5"));
            var md5HashOfCsv = GetMD5Hash(fileName);
            if (md5HashOfCsv.SequenceEqual(md5HashOfMD5))
            {
                return false;
            }
            return true;
        }

        static void ProcessFile(string fileName)
        {
            var engine = new FileHelperEngine(typeof(BookingRequest));

            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;

            var bookingRequests = (BookingRequest[])engine.ReadFile(fileName);

            if (engine.ErrorManager.Errors.Length > 0) {
                ProcessErrorRecords(engine, bookingRequests.Length, fileName);
            }

            ProcessCorrectRecords(bookingRequests);
            var requestFileName = Path.GetFileName(fileName);

            File.Move(fileName, string.Format("{0}\\{1}", FTP_READ, requestFileName));
            File.Move(fileName.Replace(".csv", ".md5"), string.Format("{0}\\{1}", FTP_READ, requestFileName.Replace(".csv", ".md5")));
        }

        private static void ProcessCorrectRecords(BookingRequest[] bookingRequests)
        {
            foreach (BookingRequest req in bookingRequests)
            {
                //TODO: Save record to BookingRequestsDB
                Console.WriteLine(string.Format("SNo={0},RecipientFirstName={1},RecipientLastName={2}", req.SerialNo, req.RecipientFirstName, req.RecipientLastName));
            }
        }

        private static void ProcessErrorRecords(FileHelperEngine engine, int numberOfGoodEntries, string fileName)
        {
            var sbErrors = new StringBuilder();
            sbErrors.AppendLine(string.Format("Number of good entries: {0}", numberOfGoodEntries));
            sbErrors.AppendLine(string.Format("Number of bad entries: {0}", engine.ErrorManager.Errors.Length));
            sbErrors.AppendLine("List of bad entries:");
            
            foreach (var item in engine.ErrorManager.Errors)
            {
                sbErrors.AppendLine(item.RecordString);
            }
            sbErrors.Append("\"EOF\"");

            var requestFileName = Path.GetFileName(fileName);
            File.WriteAllText(string.Format("{0}\\{1}", FTP_RESPONSE, requestFileName.Replace("booking-request", "booking-response")), sbErrors.ToString());
        }

        private static byte[] GetMD5Hash(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        //private static void CreateMD5File(string fileName)
        //{
        //    var md5Hash = GetMD5Hash(fileName);
        //    File.WriteAllBytes(fileName.Replace(".csv", ".md5"), md5Hash);
        //}
    }
}
