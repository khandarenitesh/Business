using CNF.Business.Model.ChequeAccounting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Web.Configuration;

namespace CNF.Business.BusinessConstant
{
    public class EmailNotification
    {
        #region Send Notification To Picker For Picklist Add
        /// <summary>
        /// For Notification related to picklist allotment
        /// </summary>
        /// <param name="ToEmail"></param>
        /// <param name="CCEmail"></param>
        /// <param name="Subject"></param>
        /// <param name="PicklistNo"></param>
        /// <param name="MailFilePath"></param>
        /// <returns></returns>
        public static bool SendEmails(string ToEmail, string CCEmail, string Subject, string PicklistNo, string MailFilePath)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            // Create the mail message
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(ToEmail))//Recipient Email
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(CCEmail))//Recipient Email cc
                {
                    string CCEmailId = CCEmail.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(ToEmail) && !string.IsNullOrEmpty(CCEmail))
                {
                    string messageformat = File.OpenText(MailFilePath).ReadToEnd().ToString();
                    // Replace Notification Table
                    messageformat = messageformat.Replace("<!--SchedulerTableString-->", PicklistNo);
                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = messageformat;
                    mailClient.Send(mailMessage);

                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmails", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion

        #region Send Emails To Stockist For New Cheque
        public bool SendEmailsToStockist(string ToEmail, string StockistName, string EmailCC, string Subject, string BodyText, string[] Attachment)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            MailMessage mailMessage = null;
            Attachment attachment1 = null;
            string MailFilePath = string.Empty;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();

                if (!string.IsNullOrWhiteSpace(ToEmail))
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                          mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                       mailMessage.To.Add(ToEmailId.Trim());
                    }
                }
                if (!string.IsNullOrWhiteSpace(EmailCC))
                {
                    string CCEmailId = EmailCC.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                           mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                       mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (Attachment != null)
                {
                    foreach (string filePath in Attachment)
                    {
                        if (filePath != null)
                        {
                            attachment1 = new Attachment(filePath);
                            mailMessage.Attachments.Add(attachment1);
                            attachment1 = null;
                        }
                    }
                }
                string MappedFilePath = AppDomain.CurrentDomain.BaseDirectory;
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailForCheque.html");
                string messageformat =File.OpenText(MailFilePath).ReadToEnd().ToString();
                messageformat = messageformat.Replace("<!--StockiestName-->", StockistName);
                messageformat = messageformat.Replace("<!--SchedulerTableString-->", BodyText);
                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = messageformat;
                mailClient.Send(mailMessage);
                bResult = true;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailsToStockist", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            finally
            {
                if (mailClient != null)
                    mailClient.Dispose();

                if (mailMessage != null)
                    mailMessage.Dispose();

                if (attachment1 != null)
                    attachment1.Dispose();
            }

            return bResult;
        }
        #endregion

        #region Send Email To Stockiest For Dispatch Done
        //Send Email To Stockiest For Dispatch Done
        public static bool SendEmailForDispatchDone(string ToEmail, string CCEmail, string Subject, string StockistName, string PONo, string PODate, string TransporterName,string CompanyName, string MailFilePath)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            // Create the mail message
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(ToEmail))//Recipient Email
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(CCEmail))//Recipient Email cc
                {
                    string CCEmailId = CCEmail.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(ToEmail) && !string.IsNullOrEmpty(CCEmail))
                {
                    string messageformat = File.OpenText(MailFilePath).ReadToEnd().ToString();
                    // Replace Notification Table
                    messageformat = messageformat.Replace("<!--StockiestName-->", StockistName);
                    messageformat = messageformat.Replace("<!--OrderNo-->", PONo);
                    messageformat = messageformat.Replace("<!--Date-->", PODate);
                    messageformat = messageformat.Replace("<!--DatedOn-->", DateTime.Today.Date.ToString("dd-MM-yyyy"));
                    messageformat = messageformat.Replace("<!--TransportName-->", TransporterName);
                    messageformat = messageformat.Replace("<!--companyName-->", CompanyName);
                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = messageformat;
                    mailClient.Send(mailMessage);

                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForDispatchDone", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion

        #region Send Email To Stockiest For Outstanding Alert
        //Send Email To Stockiest For Outstanding
        public static bool SendEmailForOutstanding(string ToEmail, string CCEmail, string Subject, decimal TotOverdueAmt, string StockistName, string OSDate ,string MailFilePath)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            // Create the mail message
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(ToEmail))//Recipient Email
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(CCEmail))//Recipient Email cc
                {
                    string CCEmailId = CCEmail.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(ToEmail) && !string.IsNullOrEmpty(CCEmail))
                {
                    string messageformat = File.OpenText(MailFilePath).ReadToEnd().ToString();
                    // Replace Notification Table
                    messageformat = messageformat.Replace("<!--StockiestName-->", StockistName);
                    messageformat = messageformat.Replace("<!--TotOverdueAmt-->", Convert.ToString(TotOverdueAmt));
                    messageformat = messageformat.Replace("<!--Date-->", OSDate);
                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = messageformat;
                    mailClient.Send(mailMessage);

                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForOutstanding", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion

        #region Send Emails To Stockist For Cheque Deposited
        public bool SendEmailsForChqDeposit(string ToEmail, string StockistName, string EmailCC, string Subject, string ChqNo, string InvNo)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            MailMessage mailMessage = null;
            string MailFilePath = string.Empty;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();

                if (!string.IsNullOrWhiteSpace(ToEmail))
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId.Trim());
                    }
                }
                if (!string.IsNullOrWhiteSpace(EmailCC))
                {
                    string CCEmailId = EmailCC.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                
                string MappedFilePath = AppDomain.CurrentDomain.BaseDirectory;
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailForChqDeposit.html");
                string messageformat = File.OpenText(MailFilePath).ReadToEnd().ToString();
                messageformat = messageformat.Replace("<!--StockiestName-->", StockistName);
                messageformat = messageformat.Replace("<!--ChqNo-->", ChqNo);
                messageformat = messageformat.Replace("<!--InvNo-->", InvNo);
                messageformat = messageformat.Replace("<!--Date-->", DateTime.Today.Date.ToString("dd-MM-yyyy"));
                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = messageformat;
                mailClient.Send(mailMessage);
                bResult = true;
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailsForChqDeposit", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            finally
            {
                if (mailClient != null)
                    mailClient.Dispose();

                if (mailMessage != null)
                    mailMessage.Dispose();
            }

            return bResult;
        }
        #endregion

        #region Send Email For Alert for Internal Audit
        //Send Email To Stockiest For Outstanding
        public bool SendEmailForInternalAudit(string ToEmail,string CCEmail, string result, string Subject)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            // Create the mail message
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(ToEmail))//Recipient Email
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(CCEmail))//Recipient Email cc
                {
                    string CCEmailId = CCEmail.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(ToEmail) && !string.IsNullOrEmpty(CCEmail))
                {
                    if (result != null && result != "")
                    {
                        mailMessage.Subject = Subject;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = result;
                        mailClient.Send(mailMessage);

                        bResult = true;
                    }
                    else
                    {
                        bResult = false;
                    }
                    //string messageformat = File.OpenText(MailFilePath).ReadToEnd().ToString();
                    // Replace Notification Table
                    //messageformat = messageformat.Replace("<!--TableAudit-->", result);
                    //messageformat = messageformat.Replace("<!--OrderNo-->", SONo);
                    //messageformat = messageformat.Replace("<!--Date-->", SODate);
                    //messageformat = messageformat.Replace("<!--DatedOn-->", DateTime.Today.Date.ToString("dd-MM-yyyy"));
                    //messageformat = messageformat.Replace("<!--TransportName-->", TransporterName);

                    //mailMessage.Subject = Subject;
                    //mailMessage.IsBodyHtml = true;
                    //mailMessage.Body = result;
                    //mailClient.Send(mailMessage);

                    //bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForInternalAudit", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion

        #region Send Email To Stockist For LR Import
        public bool SendLRImportEmailToStockist(string ToEmail, string EmailCC, string Subject, string StockistName, string PONo, DateTime PODate, string TransporterName,string LRNo, string MailFilePath)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            var date = PODate.Date.ToString("dd-MM-yyyy");
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(ToEmail))//Recipient Email
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(EmailCC))//Recipient Email cc
                {
                    string CCEmailId = EmailCC.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(ToEmail) && !string.IsNullOrEmpty(EmailCC))
                {
                    string messageformat = File.OpenText(MailFilePath).ReadToEnd().ToString();
                    messageformat = messageformat.Replace("<!--StockiestName-->", StockistName);
                    messageformat = messageformat.Replace("<!--OrderNo-->", PONo);
                    messageformat = messageformat.Replace("<!--Date-->", date);
                    messageformat = messageformat.Replace("<!--DatedOn-->", DateTime.Today.Date.ToString("dd-MM-yyyy"));
                    messageformat = messageformat.Replace("<!--TransportName-->", TransporterName);
                    messageformat = messageformat.Replace("<!--LRNo-->", LRNo);
                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = messageformat;
                    mailClient.Send(mailMessage);

                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendLRImportEmailToStockist", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion

        #region Send Email To Cheque Summary of Previous Month/Week
        public bool SendEmailToChqSummaryForMonthlyOrWeekly(string ToEmail, string EmailCC, string Subject, string BodyText)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(ToEmail))//Recipient Email
                {
                    string ToEmailId = ToEmail.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(EmailCC))//Recipient Email cc
                {
                    string CCEmailId = EmailCC.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(ToEmail) && !string.IsNullOrEmpty(EmailCC))
                {
                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = BodyText;
                    mailClient.Send(mailMessage);

                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailToChqSummaryForMonthlyOrWeekly", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion

        #region Send Alert To Internal Team For Chq Smmry
        //Send Alert To Internal Team For Chq Smmry
        public bool SendAlertToSalesTeamForChqSmmry(string Email, string EmailCC, string Subject, string BodyText)
        {
            bool bResult = false;
            SmtpClient mailClient = null;
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(Email))
                {
                    string ToEmailId = Email.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }
                if (!string.IsNullOrWhiteSpace(EmailCC))
                {
                    string CCEmailId = EmailCC.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(EmailCC))
                {
                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = BodyText;
                    mailClient.Send(mailMessage);

                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendAlertToInternalTeamForChqSmmry", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion

        #region  Send Email For approval Update Alert
        public bool sendEmailForApproval(string Emailid,string EmailCC,string Subject, string MailFilePath)
        {
            bool bResult = false;
             SmtpClient mailClient = null;
            // Create the mail message
            MailMessage mailMessage = null;
            try
            {
                mailClient = new SmtpClient();
                mailMessage = new MailMessage();
                if (!string.IsNullOrWhiteSpace(Emailid))//Recipient Email
                {
                    string ToEmailId = Emailid.Trim();
                    if (ToEmailId.Contains(";"))
                    {
                        string[] emails = ToEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.To.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.To.Add(ToEmailId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(EmailCC))//Recipient Email cc
                {
                    string CCEmailId = EmailCC.Trim();
                    if (CCEmailId.Contains(";"))
                    {
                        string[] emails = CCEmailId.Trim().Split(';');
                        foreach (string email in emails)
                        {
                            mailMessage.CC.Add(email.Trim());
                        }
                    }
                    else
                    {
                        mailMessage.CC.Add(CCEmailId);
                    }
                }
                if (!string.IsNullOrEmpty(Emailid) && !string.IsNullOrEmpty(EmailCC))
                {
                    //string messageformat = File.OpenText(MailFilePath).ReadToEnd().ToString();
                    //// Replace Notification Table
                    //messageformat = messageformat.Replace("<!--ClaimNo-->", ClaimNo);
                    //messageformat = messageformat.Replace("<!--ClaimDate-->", Convert.ToString(ClaimDate));
                    //messageformat = messageformat.Replace("<!--ClaimAmount-->",ClaimAmount);
                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = MailFilePath;
                    mailClient.Send(mailMessage);

                    bResult = true;
                }
                else
                {
                    bResult = false;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "sendEmailForApproval", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return bResult;
        }
        #endregion
    }
}
