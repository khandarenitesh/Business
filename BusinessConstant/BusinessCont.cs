using CNF.Business.Model.Context;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace CNF.Business.BusinessConstant
{
    public class BusinessCont
    {
        public static string ActiveRecord = "active";
        public static string AllRecord = "all";
        public static string EditRecord = "EDIT";
        public static string DeleteRecord = "DELETE";
        public static string SourceStaff = "MANUAL";
        public static string SourceAuto = "AUTO";
        public static string ADDRecord = "ADD";
        public static string GETRecord = "GET";

        public static string DateTimeSSFormat = "dd/MM/yyyy hh:mm:ss tt";
        public static string DateTimeFormat = "dd-MM-yyyy hh:mm tt";
        public static string DateFormat = "dd-MM-yyyy";
        public static string MonthFormat = "MM";
        public static string YearFormat = "yyyy";
        public static string DateTime24HrFormat = "dd-MM-yyyy HH:mm";
        public static string TimeFormat = "HH:mm";
        public static string StatusYes = "Y"; 
        public static string StatusNo = "N";

        public static string MediaDate = DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Day.ToString();
        public static DateTime StrConvertIntoDatetime(string Datetime)
        {
            return DateTime.ParseExact(Datetime, new string[] { "dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy-MM-dd HH:mm:ss.fff", "yyyy/MM/dd HH:mm:ss.fff", "dd-MM-yyyy HH:mm", "dd-MM-yyyy HH:mm:ss.fff", "dd/MM/yyyy HH:mm:ss.fff", "yyyy-MM-dd HH:mm tt", "yyyy/MM/dd hh:mm tt", "dd-MM-yyyy hh:mm tt", "dd/MM/yyyy hh:mm tt", "dd/MM/yyyy hh:mm:ss tt", "dd-MM-yyyy HH:mm tt", "dd/MM/yyyy HH:mm tt", "dd/MM/yyyy hh:mm:ss tt", "dd/MM/yyyy HH:mm:ss tt", "dd/MM/yyyy h:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "yyyy-dd-MM" }, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
        public static string ExceptionMsg(Exception exception)
        {
            return "Exception= " + exception.ToString() + ",  Inner Exception= " + exception.InnerException?.ToString() ?? string.Empty;
        }

        public static string CheckNullandConvertDate(DateTime? DT)
        {
            return DT ? .ToString(DateFormat) ?? string.Empty;
        }
        public static string CheckNullandConvertDateWithTime(DateTime? DT)
        {
            return DT?.ToString(DateTime24HrFormat) ?? string.Empty;
        }
        public static string CheckEmptyandConvertDateWithTime(DateTime? DT)
        {
            return DT?.ToString(DateTime24HrFormat) ?? "";
        }
        public static string CheckNullandConvertTime(DateTime? DT)
        {
            return DT?.ToString(TimeFormat) ?? string.Empty;
        }

        public static string CylinderPriceIncorrect = "Cylinder Price is not correct";

        public static decimal SaveLog(decimal logID,int ServiceId, int UserId, string logFor, string logData, string logStatus,string logException)
        {
            decimal LogId = 0;
            string SaveLog = ConfigurationManager.AppSettings["SaveLog"];
            if (SaveLog == "Yes")
            {
                using (CFADBEntities contextManager = new CFADBEntities())
                {
                    var ID = contextManager.usp_AuditLogAddEdit(logID, ServiceId, UserId, logFor, logData, logStatus, DateTime.Now, logException).FirstOrDefault();
                    if (ID != null)
                        LogId = ID.Value;
                }
            }
            return LogId;
        }

        public static string DecryptM(string cipherText)
        {
            try
            {
                if (String.IsNullOrEmpty(cipherText))
                    return null;
                var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
                var iv = Encoding.UTF8.GetBytes("8080808080808080");

                var encrypted = Convert.FromBase64String(cipherText);
                var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
                return string.Format(decriptedFromJavascript);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string EncryptM(string cipherText)
        {
            try
            {
                if (String.IsNullOrEmpty(cipherText))
                    return null;
                var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
                var iv = Encoding.UTF8.GetBytes("8080808080808080");

                // var encrypted = Convert.FromBase64String(cipherText);
                var decriptedFromJavascript = EncryptStringToBytes(cipherText, keybytes, iv);
                return string.Format(decriptedFromJavascript);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        private static string EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            string encrypted;
            // Create a RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.  
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.  
                            swEncrypt.Write(plainText);
                        }
                        // encrypted = msEncrypt.ToArray();
                        encrypted = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.  
            return encrypted;
        }

        #region Firebase push Notification

        /// <summary>
        /// Method for send notification to android user
        /// </summary>
        /// <param name="DistributorId">Distributor Id for send notification</param>
        /// <param name="StaffRefNo">StaffRefNo for send notification</param>
        /// <param name="Title">Notification title</param>
        /// <param name="Message">Notification message</param>
        /// <returns></returns>
        public static void SendNotification(int DistributorId, string StaffRefNo, string Title, string Message)
        {
            string Result = BusinessCont.FailStatus, ResponseStr = string.Empty;
            //NotificationDtls Response = null;           
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    //var DeviceDetails = _contextManager.usp_GetUserDeviceDetails(DistributorId, Convert.ToDecimal(StaffRefNo)).FirstOrDefault();
                    //if (DeviceDetails != null)
                    //{
                    //    AndroidNotification(DeviceDetails.DeviceId, Title, Message);
                    //    //ResponseStr = AndroidNotification(DeviceDetails.Token, Title, Message);
                    //    //if (ResponseStr != null)
                    //    //{
                    //    //    Response = JsonConvert.DeserializeObject<NotificationDtls>(ResponseStr);
                    //    //    if (Response.success > 0)
                    //    //        Result = BusinessCont.SuccessStatus; 
                    //    //}
                    //}
                }
                
            }
            catch (Exception ex)
            {
            //    BusinessCont.SaveLog(0, 0, DistributorId, 0, 0, "Send Notification", "StaffRefNo= " + StaffRefNo + ", Title=" + Title + ", Message=" + Message, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
                BusinessCont.SaveLog(0, 0, 0, "Send Notification", " ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));

            }
        }

        /// <summary>
        /// Private method for send notification to android user
        /// </summary>
        /// <param name="Token">send notification to Mobile token using FCM</param>
        /// <param name="NotiTitle">Notification title</param>
        /// <param name="NotificationMsg">Notification message</param>
        /// <returns></returns>
        private static string AndroidNotification(string Token, string NotiTitle, string NotificationMsg)
        {
            string ResponseStr = string.Empty;
            try
            {
                //string ApplicationKey = "AAAA_qgHXfo:APA91bETnfI9NSewh2aj3Z4JhQ_JtNyzNRlXe7xevelX6oOAWNjNYVp47L0CjLlTGbyegHJLk9r8b5W-F3vsuh136_Ypp4usAyVYnwChQiKBbHpLRArJigiI49f27ma6TVI-9ovdQvhJ";
                //string senderId = "1093740748282"; // senderId is sender key of web project in fcm

                string ApplicationKey = "AAAAljq9VvM:APA91bEJGpZqPoIILrIiWSt4akihHq7GDiAJMx_mExhfh0yh3IthTNdH9qPjM5A4Xmfw8lT4ZdCZSFq3tlq93tWyz8dMqX21D6Y7iME4fIBE2hsKilYHm7UXUnQPhBen8K-xNPIVB9b8";
                string senderId = "645230581491"; // senderId is sender key of web project in fcm

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send") as HttpWebRequest;

                tRequest.Method = "Post";
                tRequest.ContentType = "application/json";
                tRequest.UseDefaultCredentials = true;
                tRequest.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        Nick = NotiTitle,
                        body = NotificationMsg
                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ApplicationKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (HttpWebResponse tResponse = tRequest.GetResponse() as HttpWebResponse)
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                ResponseStr = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ResponseStr;
        }

        public static void InsertLog(string data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(data);

            //string FilePath = "D:/Workspace/Smart Delivery System/Aadyam.SDS/CNF.Business/Log";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mailfile");
            File.AppendAllText(filePath + "/log.txt", sb.ToString());
            sb.Clear();
        }
        #endregion

        public static string InvalidClientRqst = "Invalid client request.";
        public static string LoginFromCDCMS = "CDCMS";
        public static string LoginFromSDS = "SDS";
        public static string UserByUserNamePwd = "UNamePWD";
        public static string UserByUserNameUID = "UNameUID";
        public static string SomethngWntWrng = "Something went wrong, Please try again.";
        public static string SuccessStatus = "Success";
        public static string FailStatus = "Fail";
        public static string msg_exist = "Exists";  
        public static string msg_stsChange = "Status Changed";
        public static string UnauthorizedUser = "Unauthorized User.";
        public static string PickListNotificationHeader = "Picklist Allotment";
        public static string PickListNotificationMsg = "You have new picklist!";
        public static string ResolveConcernHeader = "Concern Resolved";
        public static string PickListResolveConcernMsg = "Picklist Concern Resolved!";
        public static string InvoiceResolveConcernMsg = "Invoice Concern Resolved!";

        // Print Configuration Message Setting
        public static string PendingPrinterMsg = "Pending";
        public static string QueuedPrinterMsg = "Queued";
        public static string InProcessPrinterMsg = "In-Process";
        public static string CompletedPrinterMsg = "Completed";

        public static FileStream _FileStream;
        public static BinaryReader _BinaryReader;
        public static long _TotalBytes;

        /// <summary>
        /// File To Byte Array
        /// </summary>
        /// <param name="_FileName"></param>
        /// <returns></returns>
        public static byte[] FileToByteArray(string _FileName)
        {
            byte[] _Buffer = null;
            try
            {
                _FileStream = new FileStream(_FileName, FileMode.Open, FileAccess.Read);  // Open file for reading  
                _BinaryReader = new BinaryReader(_FileStream); // attach filestream to binary reader  
                _TotalBytes = new FileInfo(_FileName).Length; // get total byte length of the file 
                _Buffer = _BinaryReader.ReadBytes((Int32)_TotalBytes);  // read entire file into buffer
            }
            catch (Exception ex)
            {
                SaveLog(0, 0, 0, "FileToByteArray", "File To Byte Array", FailStatus, ExceptionMsg(ex.InnerException));
            }
            finally
            {
                // close file reader 
                _FileStream.Close();
                _FileStream.Dispose();
                _BinaryReader.Close();
            }
            return _Buffer;
        }

        // Import Messages
        public static string msg_NoRecordFound = "No Record Found";
        public static string msg_NoRecordFoundExcelFile = "No Record Found in Excel File";
        public static string msg_InvalidExcelFile = "Invalid Excel File";
        public static string msg_InvalidInvDtsImported = "Invoice Details are invalid imported";

    }
}
