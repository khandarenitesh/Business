using System;
using CNF.Business.BusinessConstant;
using CNF.Business.Model.Context;
using CNF.Business.Repositories.Repository;
using System.Data.Entity.Core.Objects;
using CNF.Business.Model.ChequeAccounting;
using System.Collections.Generic;
using CNF.Business.Model.Login;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.IO;
using System.Web.Configuration;

namespace CNF.Business.Repositories
{
    public class ChequeAccountingRepository : IChequeAccountingRepository
    {
        private CFADBEntities _contextManager;

        //Inject DB Context instance from Unit of Work to avoid unwanted memory allocations.
        public ChequeAccountingRepository(CFADBEntities contextManager)
        {
            _contextManager = contextManager;
        }

        #region Add Cheque Register
        public string ChequeRegisterAdd(ChequeRegisterModel model)
        {
            return ChequeRegisterAddPvt(model);
        }
        private string ChequeRegisterAddPvt(ChequeRegisterModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(string));
            try
            {
                RetValue = _contextManager.usp_CheuqeRegisterAdd(model.BranchId, model.CompId, model.ChqReceivedDate,
                model.StokistId, model.BankId, model.FromChqNo, model.ToChqNo, model.Addedby, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ChequeRegisterAddPvt", "Add Cheque Register", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else if(RetValue == 0)
            {
                return BusinessCont.SomethngWntWrng;
            }
            else 
            {
                return BusinessCont.msg_exist;
            }
        }
        #endregion

        #region Cheque Register Edit Delete
        public string ChequeRegisterEditDelete(ChequeRegisterModel model)
        {
            return ChequeRegisterEditDeletePvt(model);
        }
        private string ChequeRegisterEditDeletePvt(ChequeRegisterModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(string));
            try
            {
                RetValue = _contextManager.usp_CheuqeRegisterEdit(model.ChqRegId, model.BranchId, model.CompId, model.ChqReceivedDate,
                model.StokistId, model.BankId,model.IFSCCode,model.AccountNo, model.ChqNo, model.Updatedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ChequeRegisterEditDeletePvt", "Cheque Register Edit Delete Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.SomethngWntWrng;
            }
        }
        #endregion

        #region Cheque Register List
        public List<ChequeRegisterModel>ChequeRegisterList(int BranchId, int CompId, int StockistId)
        {
            return ChequeRegisterListPvt(BranchId, CompId, StockistId);
        }
        private List<ChequeRegisterModel> ChequeRegisterListPvt(int BranchId, int CompId, int StockistId)
        {
            List<ChequeRegisterModel> ChequeRegisterList = new List<ChequeRegisterModel>();
            try
            {
                ChequeRegisterList = _contextManager.usp_CheuqeRegisterList(BranchId, CompId, StockistId).Select(c => new ChequeRegisterModel
                { 
                    ChqRegId = c.ChqRegId,
                    StokistId = c.StokistId,
                    StockistNo = c.StockistNo,
                    StockistName = c.StockistName,
                    ChqReceivedDate = Convert.ToDateTime(c.ChqReceivedDate),
                    StockistCity = c.StockistCity,
                    CityName = c.CityName,
                    BankId = Convert.ToInt32(c.BankId),
                    BankName = c.BankName,
                    AccountNo = c.AccountNo,
                    IFSCCode = c.IFSCCode,
                    ChqNo = c.ChqNo,
                    ChqStatus = Convert.ToInt32(c.ChqStatus),
                    ChqStatusText = c.ChqStatusText,
                    ChqAmount = Convert.ToDouble(c.ChqAmount)
                }).OrderByDescending(x => x.ChqReceivedDate).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ChequeRegisterAddPvt", "Add Cheque Register", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeRegisterList;
        }
        #endregion

        #region Get Cheque Summary Count List
        public ChequeSummyCountModel ChequeSummyCountLst(int BranchId, int CompId, int StockistId)
        {
            return ChequeSummyCountLstPvt(BranchId, CompId, StockistId);
        }
        private ChequeSummyCountModel ChequeSummyCountLstPvt(int BranchId, int CompId, int StockistId)
        {
            ChequeSummyCountModel ChequeSummyCount = new ChequeSummyCountModel();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    ChequeSummyCount = _contextManager.usp_CheuqeRegisterCounts(BranchId, CompId, StockistId)
                        .Select(x => new ChequeSummyCountModel
                        {
                            BranchId = x.BranchId,
                            CompId = x.CompId,
                            Total = Convert.ToInt32(x.Total),
                            Blank = Convert.ToInt32(x.Blank),
                            Utilised = Convert.ToInt32(x.Utilised),
                            Prepare = Convert.ToInt32(x.Prepare),
                            Deposited = Convert.ToInt32(x.Deposited),
                            Discarded = Convert.ToInt32(x.Discarded),
                            Returned = Convert.ToInt32(x.Returned),
                            Settled = Convert.ToInt32(x.Settled)
                        }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ChequeSummyCountLstPvt", "Cheque Summary Count List" + "BranchId:  " + BranchId + "CompId:  " + CompId + "StockistId:" + StockistId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeSummyCount;
        }
        #endregion

        #region Get Stockist OutStanding List
        public List<ImportStockistOutStandingModel> GeStockistOSLst(int BranchId, int CompId)
        {
            return GetStockistOSLstPvt (BranchId, CompId);
        }
        private List<ImportStockistOutStandingModel> GetStockistOSLstPvt(int BranchId, int CompId)
        {
            List<ImportStockistOutStandingModel> LRStockistOSLst = new List<ImportStockistOutStandingModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    LRStockistOSLst = _contextManager.usp_StockistOSList(BranchId, CompId).Select(c => new ImportStockistOutStandingModel
                    {
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        Div_Cd = Convert.ToString(c.Div_Cd),
                        CustomerCode = c.CustomerCode,
                        StockistName = c.StockistName,
                        City = c.City,
                        DocName = c.DocName,
                        DocDate = c.DocDate,
                        DueDate = c.DueDate,
                        OpenAmt = c.OpenAmt,
                        ChqNo = c.ChqNo,
                        DistrChannel = c.DistrChannel,                       
                        DocType = c.DocType,
                        OverdueAmt = c.OverdueAmt,
                        Addedby = c.Addedby,
                        AddedOn = Convert.ToDateTime(c.AddedOn)
                    }).OrderByDescending(x => x.Div_Cd).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistOSLstPvt", "Get Stockist OS List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return LRStockistOSLst;
        }
        #endregion

        #region Get Admin Details
        public List<DetailsForEmail> GetAdminDetails(int EmailFor)
        {
            return GetAdminDetailsPvt(EmailFor);
        }
        private List<DetailsForEmail> GetAdminDetailsPvt(int EmailFor)
        {
            List<DetailsForEmail> model = new List<DetailsForEmail>();
            try
            {
                model = _contextManager.usp_GetAdminDetails(EmailFor).Select(x => new DetailsForEmail
                {
                    Id = x.PersonId,
                    Name = x.PersonName,
                    Email = x.EMAIL,
                    IsSelect = x.IsSelect
                }).ToList();

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Get Admin Details Pvt", "Get Admin Details Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get CCEmail and Purpose Details
        public List<DetailsForEmail> GetCCEmailandPurposeDetails(string Flag)
        {
            return GetCCEmailandPurposeDetailsPvt(Flag);
        }
        private List<DetailsForEmail> GetCCEmailandPurposeDetailsPvt(string Flag)
        {
            List<DetailsForEmail> model = new List<DetailsForEmail>();
            try
            {
                model = _contextManager.usp_GetCCEmailandPurposeDetails(Flag).Select(x => new DetailsForEmail
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.EMAIL
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCCEmailandPurposeDetailsPvt", "Get CC Email and Purpose Details Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Email Configuration Add 
        public string EmailConfigurationAdd(EmailModel model)
        {
            return EmailConfigurationAddPvt(model);
        }
        public string EmailConfigurationAddPvt(EmailModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_EmailConfigurationAdd(model.BranchId, model.CompanyId, model.EmailForId, model.EmailCCPersonId, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "EmailConfigurationAddPvt", "Email Configuration Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.FailStatus;
            }
        }
        #endregion 

        #region Get Email Configuration List
        public List<EmailConfigModel> GetEmailConfigList(int BranchId, int CompanyId)
        {
            return GetEmailConfigListPvt(BranchId, CompanyId);
        }
        private List<EmailConfigModel> GetEmailConfigListPvt(int BranchId, int CompanyId)
        {
            List<EmailConfigModel> RoleLst = new List<EmailConfigModel>();
            try
            {
                RoleLst = _contextManager.usp_GetEmailConfigurationList(BranchId, CompanyId).Select(c => new EmailConfigModel
                {
                    PersonName = c.PersonName,
                    Email = c.Email
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetEmailConfigListPvt", "Get Email Configuration List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RoleLst;
        }
        #endregion
        
        #region Update Cheque Status
        public string UpdateChequeStatus(UpdateChequeSts model)
        {
            return UpdateChequeStatusPvt(model);
        }
        private string UpdateChequeStatusPvt(UpdateChequeSts model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetVal", typeof(int));

            try
            {
                RetValue = _contextManager.usp_ChequeStatusUpdate(model.ChqRegId, model.BranchId, model.CompId,model.ChqNo, model.StockistId, model.ChqStatus, model.InvData, model.Remark, model.ReturnReasonId, model.Addedby, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "UpdateChequeStatusPvt", "Update Cheque Status", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.FailStatus;
            }
        }
        #endregion

        #region Get Invoice For Chq Block List 
        public List<InvoiceForChqBlockModel> InvoiceForChqBlockList(int stockistId)
        {
            return InvoiceForChqBlockListPvt(stockistId);
        }
        private List<InvoiceForChqBlockModel> InvoiceForChqBlockListPvt(int stockistId)
        {
            List<InvoiceForChqBlockModel> InvoiceForChqBlockList = new List<InvoiceForChqBlockModel>();
            try
            {
                InvoiceForChqBlockList = _contextManager.usp_GetInvoiceForChqBlock(stockistId, DateTime.Now).Select(c => new InvoiceForChqBlockModel
                {
                    InvId=Convert.ToInt32(c.InvId),
                    CbInvId=Convert.ToInt32(c.CbInvId),
                    BranchId =c.BranchId,
                    CompId = c.CompId,
                    InvNo = c.InvNo,
                    InvCreatedDate = Convert.ToDateTime(c.InvCreatedDate),
                    IsColdStorage=c.IsColdStorage,
                    SoldTo_StokistId=c.SoldTo_StokistId,
                    StockistName=c.StockistName,
                    StockistNo=Convert.ToInt32(c.StockistNo),
                    CityCode=Convert.ToString(c.CityCode),
                    CityName=c.CityName,
                    InvAmount=Convert.ToInt32(c.InvAmount),
                    InvStatus=Convert.ToInt32(c.InvStatus)
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InvoiceForChqBlockListPvt", "Invoice For Cheque Block List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvoiceForChqBlockList;
        }
        #endregion
             
        #region Get Deposited Cheque Receipt List
        public List<ImportDepositedChequeModel> GetChequeReceiptLst(int BranchId, int CompId)
        {
            return GetChequeReceiptLstPvt(BranchId, CompId);
        }
        private List<ImportDepositedChequeModel> GetChequeReceiptLstPvt(int BranchId, int CompId)
        {
            List<ImportDepositedChequeModel> ChequeReceiptLst = new List<ImportDepositedChequeModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    ChequeReceiptLst = _contextManager.usp_ChqDepoReceiptList(BranchId, CompId).Select(c => new ImportDepositedChequeModel
                    {
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        StockistCode=c.StockistCode,
                        StockistName=c.StockistName,
                        DepositeDate=c.DepositeDate,
                        BankName=c.BankName,
                        AccountNo=c.AccountNo,
                        StockistId=Convert.ToInt32(c.StockistId),
                        ChequeNo=c.ChequeNo,
                        Amount=c.Amount,
                        ChqRegId =Convert.ToString(c.ChqRegId),                       
                    }).OrderByDescending(x => x.DepositeDate).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChequeReceiptLstPvt", "Deposited Cheque Receipt List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeReceiptLst;
        }
        #endregion

        #region Get Email Count Details
        public List<StockistDetails> GetEmailCountDetails(int BranchId, int CompId)
        {
            return GetEmailCountDetailsPvt(BranchId, CompId);
        }
        private List<StockistDetails> GetEmailCountDetailsPvt(int BranchId, int CompId)
        {
            List<StockistDetails> EmailCntDtls = new List<StockistDetails>();
            try
            {
                EmailCntDtls = _contextManager.usp_GetStockistForNewChqEmail(BranchId, CompId).Select(x => new StockistDetails
                {
                    BranchId =x.BranchId,
                    CompId =x.CompId,
                    StockistId =x.StokistId,
                    StockistName =x.StockistName,
                    StockistNo =x.StockistNo,
                    StockistCity = x.StockistCity,
                    Emailid = x.Emailid,
                    MobNo = x.MobNo,
                    CityName = x.CityName,
                    ChqRegId= Convert.ToInt64(x.UsableChqs)
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetEmailCountDetailsPvt", "Get Email Count Details Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return EmailCntDtls;
        }
        #endregion

        #region Get Cheque Register Summary Reports List
        public List<ChequeRegstrSmmryRptModel> ChequeRegisterLst(int BranchId, int CompId)
        {
            return GetChequeRegisterLstLstPvt(BranchId, CompId);
        }
        private List<ChequeRegstrSmmryRptModel> GetChequeRegisterLstLstPvt(int BranchId, int CompId)
        {
            List<ChequeRegstrSmmryRptModel> ChequeRegisterLst = new List<ChequeRegstrSmmryRptModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    ChequeRegisterLst = _contextManager.usp_RptChqRegisterSummary(BranchId, CompId).Select(c => new ChequeRegstrSmmryRptModel
                    {
                        StokistId = c.StokistId,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        BankName = c.BankName,
                        BankId = Convert.ToInt32(c.BankId),
                        IFSCCode = c.IFSCCode,
                        AccountNo = c.AccountNo,
                        BlankChqs = Convert.ToInt32(c.BlankChqs),
                        UtilisedChqs = Convert.ToInt32(c.UtilisedChqs),
                        PrepareChqs = Convert.ToInt32(c.PrepareChqs),
                        DiscardedChqs = Convert.ToInt32(c.DiscardedChqs),
                        DepositedChqs = Convert.ToInt32(c.DepositedChqs),
                        ReturnedChqs = Convert.ToInt32(c.ReturnedChqs),
                        SettledChqs = Convert.ToInt32(c.SettledChqs)
                    }).OrderByDescending(x => x.StockistName).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChequeRegisterLstLstPvt", "Get Cheque Register Summary Reports List" + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeRegisterLst;
        }
        #endregion

        #region Get StkOutstanding Dtls For Email
        public List<OutStandingDtls> GetStkOutstandingDtlsForEmail(int BranchId, int CompId)
        {
            return GetStkOutstandingDtlsForEmailPvt(BranchId, CompId);
        }
        private List<OutStandingDtls> GetStkOutstandingDtlsForEmailPvt(int BranchId, int CompId)
        {
            List<OutStandingDtls> EmailDtls = new List<OutStandingDtls>();
            try
            {
                EmailDtls = _contextManager.usp_GetStkOutstandingDtlsForEmail(BranchId, CompId).Select(s => new OutStandingDtls
                {
                    OSDate = Convert.ToDateTime(s.OSDate),
                    BranchId = s.BranchId,
                    CompId = s.CompId,
                    StockistId = Convert.ToInt32(s.StockistId),
                    StockistCode = s.StockistCode,
                    StockistName = s.StockistName,
                    MobNo = s.MobNo,
                    PaymentStatus = Convert.ToInt32(s.PaymentStatus),
                    TotOverdueAmt = Convert.ToDecimal(s.TotOverdueAmt),
                    Emailid = s.Emailid
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStkOutstandingDtlsForEmailPvt", "Get StkOutstanding Dtls For Email Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return EmailDtls;
        }
        #endregion

        #region Send Email To Stockiest For Outstanding Alert
        public string sendEmailForOutstanding(string Emailid, int BranchId, int CompId,decimal TotOverdueAmt,string StockistName,DateTime OSDate)
        {
            bool bResult = false;
            string msg = string.Empty, messageformat = string.Empty, Date = string.Empty, Subject = string.Empty, MailFilePath = string.Empty, ToEmail = string.Empty, CCEmail = string.Empty;
            EmailSend Email = new EmailSend();
            List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
            CCEmailDtls CCEmailModel = new CCEmailDtls();

            try
            {
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                var date = OSDate.Date.ToString("dd-MM-yyyy");
                ToEmail = Emailid;
                Subject = ConfigurationManager.AppSettings["OutstandingSubj"] + Date + " ";
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\Outstanding_Update.html");
                CCEmailList = GetCCEmailDtlsPvt(BranchId, CompId, 3);
                if (CCEmailList.Count > 0)
                {

                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }

                    string EmailCC = CCEmail.TrimStart(';');

                    bResult = EmailNotification.SendEmailForOutstanding(ToEmail, EmailCC, Subject, TotOverdueAmt, StockistName, date, MailFilePath);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "sendEmailForOutstanding", "Send Email For Outstanding Alert", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (bResult == true)
            {
                return Email.Status = BusinessCont.SuccessStatus;
            }
            else
            {
                return Email.Status = BusinessCont.FailStatus;
            }
        }
        #endregion

        #region Get CCEmail Details
        public List<CCEmailDtls> GetCCEmailDtlsPvt(int BranchId, int CompId, int EmailFor)
        {
            List<CCEmailDtls> EmailCntDtls = new List<CCEmailDtls>();
            try
            {
                EmailCntDtls = _contextManager.usp_GetCCPersonDetailsForEmail(BranchId, CompId, EmailFor).Select(s => new CCEmailDtls
                {
                    BranchId = Convert.ToInt32(s.BranchId),
                    CompId = s.CompanyId,
                    EmailForId = Convert.ToInt32(s.EmailForId),
                    Name = s.Name,
                    Email = s.Email
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCCEmailDtlsPvt", "Get CC Email Details Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return EmailCntDtls;
        }
        #endregion

        #region Get Cheque Deposited List
        public List<ChqDepositDetails> GetChequeDepositedList(int BranchId, int CompId)
        {
            return GetChequeDepositedListPvt(BranchId, CompId);
        }
        private List<ChqDepositDetails> GetChequeDepositedListPvt(int BranchId, int CompId)
        {
            List<ChqDepositDetails> ChequeDepositLst = new List<ChqDepositDetails>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    ChequeDepositLst = _contextManager.usp_GetStkChqDepositDtlsForEmail(BranchId, CompId).Select(c => new ChqDepositDetails
                    {
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        StockistId = c.StokistId,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        AccountNo = c.AccountNo,
                        ChqNo = c.ChqNo,
                        ChqStatus = Convert.ToInt32(c.ChqStatus),
                        DepositedDate = Convert.ToDateTime(c.DepositedDate),
                        Emailid=c.Emailid,
                        MobNo =c.MobNo,
                        InvNo = c.InvNo
                    }).OrderByDescending(x => x.DepositedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChequeDepositedList", "Get Cheque Deposited List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeDepositLst;
        }
        #endregion

        #region Get Internal Team Email List
        public List<AuditDtls> GetInternalTeamEmailList(int BranchId, int CompId)
        {
            return GetInternalTeamEmailListPvt(BranchId, CompId);
        }
        private List<AuditDtls> GetInternalTeamEmailListPvt(int BranchId, int CompId)
        {
            List<AuditDtls> InternalTeamList = new List<AuditDtls>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InternalTeamList = _contextManager.usp_GetInternalAuditDtls(BranchId, CompId).Select(c => new AuditDtls
                    {
                        StokistId = c.StokistId,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        Emailid = c.Emailid,
                        CityName = c.CityName,
                        ChqNo = c.ChqNo,
                        ChqStatus = Convert.ToInt32(c.ChqStatus),
                        ChqStatusText = c.ChqStatusText,
                        ReleasedRemark = c.ReleasedRemark
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInternalTeamEmailList", "Get Internal Team Email List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InternalTeamList;
        }
        #endregion

        #region Get LR Import Details List
        public List<GetLRDetailsModel> GetLRImportDetailsList(int BranchId, int CompId)
        {
            return GetLRImportDetailsListPvt(BranchId, CompId);
        }
        private List<GetLRDetailsModel> GetLRImportDetailsListPvt(int BranchId, int CompId)
        {
            List<GetLRDetailsModel> LRImportDtlsLst = new List<GetLRDetailsModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    LRImportDtlsLst = _contextManager.usp_GetLRDtlsInvDtlsForEmail(BranchId, CompId).Select(c => new GetLRDetailsModel
                    {
                        PONo = c.PONo,
                        PODate = Convert.ToDateTime(c.PODate),
                        TransporterName =c.TransporterName,
                        LRNo = c.LRNo,
                        LRDate= Convert.ToString(c.LRDate),
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        Emailid = c.Emailid,
                    }).OrderByDescending(x => x.LRDate).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetLRImportDetailsList", "Get LR Import Details List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return LRImportDtlsLst;
        }
        #endregion

        #region Get Stockist OS Doc Types Reports List
        public List<StockistOsReportModel> OsdocTypesReportLst(int BranchId, int CompId)
        {
            return GetOsdocTypesReportPvt(BranchId, CompId);
        }
        private List<StockistOsReportModel> GetOsdocTypesReportPvt(int BranchId, int CompId)
        {
            List<StockistOsReportModel> OsdocTypesReportLst = new List<StockistOsReportModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    OsdocTypesReportLst = _contextManager.usp_StockistOSDocTypeWiseList(BranchId, CompId, DateTime.Now).Select(c => new StockistOsReportModel
                    {
                        City=c.City,
                        Div_Cd = Convert.ToInt32(c.Div_Cd),
                        StockistCode = Convert.ToString(c.StockistCode),
                        StockistName = c.StockistName,
                        DocName = c.DocName,
                        DocDate = Convert.ToString(c.DocDate),
                        DueDate = Convert.ToString(c.DueDate),
                        OpenAmt = c.OpenAmt,
                        ChqNo = c.ChqNo,
                        DistrChannel = c.DistrChannel,
                        RV = Convert.ToDecimal(c.RV),
                        AB = Convert.ToDecimal(c.AB),
                        CD = Convert.ToDecimal(c.CD),
                        CC = Convert.ToDecimal(c.CC),
                        DG = Convert.ToDecimal(c.DG),
                        DR = Convert.ToDecimal(c.DR),
                        DZ = Convert.ToDecimal(c.DZ),
                        Other = Convert.ToDecimal(c.Other),
                    }).OrderByDescending(x => x.StockistName).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetOsdocTypesReportPvt", "Get Stockist OS Doc Types Reports List" + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return OsdocTypesReportLst;
        }
        #endregion

        #region Get Cheque Summary of previous month/Week
        public List<ChqSummaryForMonthlyModel> GetChqSummaryForMonthlyList(int CompId, int BranchId, DateTime? FromDate, DateTime? ToDate)
        {
            return GetChqSummaryForMonthlyListPvt(CompId, BranchId, FromDate, ToDate);
        }
        private List<ChqSummaryForMonthlyModel> GetChqSummaryForMonthlyListPvt(int CompId, int BranchId, DateTime? FromDate, DateTime? ToDate)
        {
            List<ChqSummaryForMonthlyModel> ChqSummaryForMonthlyList = new List<ChqSummaryForMonthlyModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    ChqSummaryForMonthlyList = _contextManager.usp_ChqSummaryForMonthly(CompId, BranchId, FromDate, ToDate).Select(c => new ChqSummaryForMonthlyModel
                    {
                        CompanyName = c.CompanyName,
                        InvId = c.InvId,
                        InvNo = c.InvNo,
                        CompId = c.CompId,
                        BranchId = c.BranchId,
                        InvCreatedDate = Convert.ToDateTime(c.InvCreatedDate),
                        DueDate = Convert.ToDateTime(c.DueDate),
                        ChqAmount = c.ChqAmount,
                        ChqNo = c.ChqNo,
                        Emailid = c.Emailid,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChqSummaryForMonthlyListPvt", "Get Cheque Summary of previous month/Week " + "CompId:  " + CompId + "BranchId:  " + BranchId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChqSummaryForMonthlyList;
        }
        #endregion

        #region Get ChqSummary For Monthly Report
        /// <summary>
        /// Get ChqSummary For Monthly Report
        /// </summary>
        /// <returns></returns>
        public string GetChqSummaryForMonthlyReport(List<ChqSummaryForMonthlyModel> modelList, string MailFilePath)
        {
            string Table = string.Empty, TableList = string.Empty, msgHtml = string.Empty;
            double totalAmountValue = 0;
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChqSummaryForMonthlyReport", "Get ChqSummary For Monthly Report", "START", "");
                msgHtml = File.OpenText(MailFilePath).ReadToEnd().ToString();
                Table = "";
                TableList = "";
                Table += "<table style='border-collapse: collapse; width:100%; white-space:nowrap;'>";
                Table += "<thead><tr style='font-size:14px;font-weight: bold;'>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1'> INV.NO </th>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1'> INV.DT. </th>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1'> DUE DT. </th>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1'> AMT </th>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1'> CHQ.NO </th>";
                Table += "</tr></thead><tbody>";
                TableList += Table;

                foreach (var item in modelList)
                {
                    TableList += "<tr style='font-size:13px;text-align:center;color:black;'>";
                    TableList += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + item.InvNo + "</td>";
                    TableList += "<td style='border: 1px solid black;padding: 5px;' width='7%' border='1'>" + (item.InvCreatedDate != null ? Convert.ToDateTime(item.InvCreatedDate).ToString("dd-MM-yyyy") : "-") +
                             "</td>";
                    TableList += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + (item.DueDate != null ? Convert.ToDateTime(item.DueDate).ToString("dd-MM-yyyy") : "-") +
                             "</td>";
                    TableList += "<td style='text-align:center;border: 1px solid black;padding: 5px;word-wrap: break-word;' border='1'>" + item.ChqAmount +
                             "</td>";
                    TableList += "<td style='border: 1px solid black;padding: 5px;word-wrap: break-word;font-weight: bold;' border='1'>" + item.ChqNo +
                             "</td>";
                    TableList += "</tr>";
                    totalAmountValue = totalAmountValue + item.ChqAmount;
                }
                TableList += "</tbody><tfoot><tr><th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'></th><th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'></th><td style='border: 1px solid black;text-align:center;font-size:14px;font-weight: bold;' border='1'> TOT.AMT </td>" + "<td style='border: 1px solid black;text-align:center;font-size:13px;font-weight: bold;' border='1'>" + totalAmountValue + "</td><td style='border: 1px solid black;' border='1'></td><td></td><td></td></tr></tfoot></table></br></br>"; // footer section - TOT.AMT Added ChqAmount(AMT) wise

                if (TableList != "" && TableList != null)
                {
                    msgHtml = msgHtml.Replace("<!--MonthYearString-->", DateTime.Now.ToString("MMMM") + "-" + Convert.ToString(DateTime.Now.Year));
                    msgHtml = msgHtml.Replace("<!--CompanyNameString-->", modelList[0].CompanyName);
                    msgHtml = msgHtml.Replace("<!--MonthSchedulerTableString-->", TableList);
                }
                BusinessCont.SaveLog(0, 0, 0, "GetChqSummaryForMonthlyReport", "Get ChqSummary For Monthly Report", "END", "");
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChqSummaryForMonthlyReport", "Get ChqSummary For Monthly Report", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msgHtml;
        }
        #endregion

        #region Get Report Chq Summary For Sales Team List
        public List<ChqSummaryForSalesTeamModel> GetRptChqSummaryForSalesTeamList(int BranchId, int CompId)
        {
            return GetRptChqSummaryForSalesTeamListPvt(BranchId, CompId);
        }
        private List<ChqSummaryForSalesTeamModel> GetRptChqSummaryForSalesTeamListPvt(int BranchId, int CompId)
        {
            List<ChqSummaryForSalesTeamModel> chqSummaryForSalesTeamModel = new List<ChqSummaryForSalesTeamModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    chqSummaryForSalesTeamModel = _contextManager.usp_RptChqSummaryForSalesTeam(BranchId, CompId).Select(c => new ChqSummaryForSalesTeamModel
                    {
                        StockistId = c.StokistId,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        CityName = c.CityName,
                        TotalChqCount = Convert.ToInt64(c.TotalChqCount),
                        BlankChqs = Convert.ToInt32(c.BlankChqs)
                    }).OrderByDescending(x => x.StockistId).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetRptChqSummaryForSalesTeamListPvt", "Get Report Chq Summary For Sales Team List: " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return chqSummaryForSalesTeamModel;
        }
        #endregion

        #region Get Cheque Summary For Sales Team For Report
        public string GetChequeSummaryForSalesTeamForReport(List<ChqSummaryForSalesTeamModel> modelList, string MailFilePath)
        {
            string Table = string.Empty, TableList = string.Empty, msgHtml = string.Empty;
            try
            {
                BusinessCont.SaveLog(0,0, 0, "GetChequeSummaryForSalesTeam", "Get Cheque Summary For Sales Team", "START", "");
                msgHtml = File.OpenText(MailFilePath).ReadToEnd().ToString();
                Table = "";
                TableList = "";
                Table += "<table style='border-collapse: collapse; width: 60%; min-width: 400px; white-space:nowrap;'>";
                Table += "<thead><tr style = 'font-family: Verdana; font-size: 12px; font-weight: bold; background-color:#3c8dbc; color:white;'>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1' width='20%'> Stockiest Name </th>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1' width='15%'> City </th>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;' border='1' width='15%'> Balance Cheque Count </th>";
                Table += "</tr></thead><tbody>";
                TableList += Table;

                foreach (var item in modelList) 
                {
                    TableList += "<tr style = 'font-family: sans-serif; font-size: 12px; color:black;'>";
                    TableList += "<td style='border: 1px solid black;padding: 5px;text-align:center;' border='1'>" + item.StockistName + "</td>";
                    TableList += "<td style='border: 1px solid black;padding: 5px;text-align:center;' width='7%' border='1'>" + item.CityName + "</td>";
                    TableList += "<td style='border: 1px solid black;padding: 5px;text-align:center;' border='1'>" + item.TotalChqCount + "</td>";
                }
                TableList += "</tr></tbody></table>";
                if (TableList != "" && TableList != null)
                {                    
                    msgHtml = msgHtml.Replace("<!--SchedulerTableString-->", TableList);
                }
                BusinessCont.SaveLog(0, 0, 0, "GetChequeSummaryForSalesTeam", "Get Cheque Summary For Sales Team", "END", "");
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChequeSummaryForSalesTeamForReport", "Get Cheque Summary For Sales Team", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msgHtml;
        }
        #endregion

        #region Get Sales Team Email List
        public List<OfficerDetails> GetSalesTeamEmailList()
        {
            return GetSalesTeamEmailListPvt();
        }
        private List<OfficerDetails> GetSalesTeamEmailListPvt()
        {
            List<OfficerDetails> InternalTeamList = new List<OfficerDetails>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InternalTeamList = _contextManager.usp_GetOfficerDetails().Select(c => new OfficerDetails
                    {
                     EmployeeNo = c.EmployeeNo,
                     EmployeeName = c.EmployeeName,
                     MobileNo = Convert.ToDecimal(c.MobileNo),
                     Email = c.Email,
                     OfficerRole = c.OfficerRole
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetSalesTeamEmailListPvt", "Get Sales Team Email List " ,BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InternalTeamList;
        }
        #endregion

        #region Internal Audit Html
        public string InterAuditHtml(List<AuditDtls> auditDtls, string msgHtml)
        {
            string TableAuditList = string.Empty, TableAuditDtls = string.Empty;
            try
            {
                TableAuditList = "";
                TableAuditDtls = "";

               
                TableAuditList += "<table style='border-collapse: collapse; width:100%; white-space:nowrap;'>";
                TableAuditList += "<thead><tr style='font-size:14px;font-weight: bold;background-color:#3c8dbc; color:white;'>";
                //TableAuditList += "<th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'> Sr.No </th>";
                TableAuditList += "<th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'> Customer Code </th>";
                TableAuditList += "<th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'> Customer Name </th>";
                TableAuditList += "<th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'> City </th>";
                TableAuditList += "<th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'> Cheque No </th>";
                TableAuditList += "<th style='border: 1px solid black;padding: 5px;text-align:center;' border='1'> Remarks </th>";
                TableAuditList += "</tr></thead><tbody>";
                //int Count = 1;
                TableAuditDtls += TableAuditList;

                foreach (var item in auditDtls)
                {

                    TableAuditDtls += "<tr style='font-size:13px;text-align:center;'>";
                    //TableAuditDtls += "<td style='text-align:center;border: 1px solid grey;padding: 7px;' width='7%' border='1'>" + Count +
                    //         "</td>";
                    //TableAuditDtls += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + item.StockistNo + "</td>";
                    TableAuditDtls += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + item.StockistNo + "</td>";
                    TableAuditDtls += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + item.StockistName + "</td>";
                    TableAuditDtls += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + item.CityName + "</td>";
                    TableAuditDtls += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + item.ChqNo + "</td>";
                    TableAuditDtls += "<td style='border: 1px solid black;padding: 5px;' border='1'>" + item.ChqStatusText + "</td>";
                    TableAuditDtls += "</tr>";
                    // Count++;
                }
                TableAuditDtls += "</tbody></table>";
                if (TableAuditDtls != "" && TableAuditDtls != null)
                {
                    msgHtml = msgHtml.Replace("<!--TableAudit-->", TableAuditDtls);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InterAuditHtml", "Generate Internal Audit Html", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msgHtml;
        }
        #endregion
    }
}
