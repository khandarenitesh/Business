using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using CNF.Business.Model.Context;
using CNF.Business.Model.Login;
using CNF.Business.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CNF.Business.Model.OrderReturn.OrderReturn;

namespace CNF.Business.Repositories
{
    public class OrderReturnRepository : IOrderReturnRepository
    {
        private CFADBEntities _contextManager;

       //Inject DB Context instance from Unit of Work to avoid unwanted memory allocations.
        public OrderReturnRepository(CFADBEntities contextManager)
        {
            _contextManager = contextManager;
        }

        #region Get New Generated Gatepass No
        public string GetNewGeneratedGatepassNo(int BranchId, int CompId, DateTime ReceiptDate)
        {
            return GetNewGeneratedGatepassNoPvt(BranchId, CompId, ReceiptDate);
        }
        private string GetNewGeneratedGatepassNoPvt(int BranchId, int CompId, DateTime ReceiptDate)
        {
            string gatepassNo = string.Empty;

            try
            {
                gatepassNo = _contextManager.usp_InwardGatepassNewNo(BranchId, CompId, ReceiptDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetNewGeneratedGatepassNoPvt", "Get New Generated Gatepass No " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return gatepassNo;
        }
        #endregion

        #region Generate Inward Gatepass Add Edit - for Mobile
        public string AddEditInwardGatepass(InwardGatepassModel model)
        {
            return AddEditInwardGatepassPvt(model);
        }
        private string AddEditInwardGatepassPvt(InwardGatepassModel model)
        {
            string RetValue = "";
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
               var result = _contextManager.usp_InwardGatepassAddEdit(model.GatepassId, model.BranchId, model.CompId, Convert.ToDateTime(model.ReceiptDate), model.StockistId, model.City, model.TransporterId, model.CourierId, model.OtherTransport, model.LRNumber, Convert.ToDateTime(model.LRDate), model.NoOfBoxes, model.AmountPaid, model.IsClaimAvilable, model.Addedby,model.Action, obj).ToList();
               var GatepassId = result[0];
               RetValue = Convert.ToString(GatepassId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditInwardGatepass", "Add Edit Inward Gatepass", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Stockiest Details For Email
        public List<StokistDtlsModel> GetStockistDtlsForEmail(int BranchId, int CompId, int GatepassId)
        {
            return GetStockistDtlsForEmailPvt(BranchId, CompId, GatepassId);
        }
        private List<StokistDtlsModel> GetStockistDtlsForEmailPvt(int BranchId, int CompId, int GatepassId)
        {
            List<StokistDtlsModel> stockistDtls = new List<StokistDtlsModel>();
            try
            {
                stockistDtls = _contextManager.usp_GetStockistDtlsForSendEmail(BranchId, CompId, GatepassId).Select(s => new StokistDtlsModel
                {
                    BranchId = s.BranchId,
                    CompId = s.CompId,
                    LRDate = Convert.ToDateTime(s.LRDate),
                    LRNumber = s.LRNo,
                    ReceiptDate = Convert.ToDateTime(s.ReceiptDate),
                    StockistNo = s.StockistNo,
                    StockistName = s.StockistName,
                    Emailid = s.Emailid
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistDtlsForEmailPvt", "Get Stockist Dtls For Email", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return stockistDtls;
        }
        #endregion

        #region Send Email To Stockist For Consignment Recieved
        public string SendEmailForConsignmentReceived(string Emailid, string StockistName, string LRNumber, DateTime LRDate, DateTime ReceiptDate, int BranchId, int CompId)
        {
            bool bResult = false;
            string msg = string.Empty, messageformat = string.Empty, Date = string.Empty, Subject = string.Empty, MailFilePath = string.Empty, ToEmail = string.Empty, CCEmail = string.Empty, EmailCC = string.Empty;
            EmailSend Email = new EmailSend();
            List<CCEmailDtls> EmailCntDtls = new List<CCEmailDtls>();
            try
            {
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                var date = LRDate.Date.ToString("dd-MM-yyyy");
                ToEmail = Emailid;
                Subject = ConfigurationManager.AppSettings["ConsignmentReceived"] + Date + " ";
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailToStkstForConsignmentRecieved.html");
                ChequeAccountingRepository _ChequeAccountingRepository = new ChequeAccountingRepository(_contextManager);
                EmailCntDtls = _ChequeAccountingRepository.GetCCEmailDtlsPvt(BranchId, CompId, 5);
                if (EmailCntDtls.Count > 0)
                {
                    for (int i = 0; i < EmailCntDtls.Count; i++)
                    {
                        CCEmail += ";" + EmailCntDtls[i].Email;
                    }

                    EmailCC = CCEmail.TrimStart(';');

                }
              // bResult = EmailNotification.SendEmailForConsignmentReceived(ToEmail, EmailCC, Subject, StockistName, LRNumber, LRDate, ReceiptDate, MailFilePath);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForConsignmentRecieved", "ConsignmentRecieved", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get Inward Gatepass List - For Mobile
        public List<InwardGatepassModel> GetInwardGatepassList(int BranchId, int CompId)
        {
            return GetInwardGatepassListPvt(BranchId, CompId);
        }
        private List<InwardGatepassModel> GetInwardGatepassListPvt(int BranchId, int CompId)
        {
            List<InwardGatepassModel> GatepassLst = new List<InwardGatepassModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    GatepassLst = _contextManager.usp_GetInwradGatepassList(BranchId, CompId).Select(c => new InwardGatepassModel
                    {
                        GatepassId = c.GatepassId,
                        GatepassNo = c.GatepassNo,
                        StockistId = c.StockistId,
                        StockistName = c.StockistName,
                        StockistNo = c.StockistNo,
                        City = Convert.ToInt32(c.City),
                        CityName = c.CityName,
                        TransporterId = Convert.ToInt32(c.TransporterId),
                        TransCourName = c.TransCourName,
                        TransporterName = c.TransporterName,
                        CourierId = Convert.ToInt32(c.CourierId),
                        CourierName = c.CourierName,
                        OtherTransport = c.OtherTrasport,
                        LRNumber = c.LRNo,
                        LRDate = Convert.ToDateTime(c.LRDate),
                        ReceiptDate = Convert.ToDateTime(c.ReceiptDate),
                        NoOfBoxes = Convert.ToInt32(c.NoOfBox),
                        AmountPaid = Convert.ToInt32(c.AmountPaid)
                    }).OrderByDescending(x => x.AddedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInwardGatepassListPvt", "Get Inward Gatepass List Pvt ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return GatepassLst;
        }
        #endregion

        #region Get Missing Claim Form List
        public List<InwardGatepassModel> GetMissingClaimFormList(int BranchId, int CompId, int Flag)
        {
            return GetMissingClaimFormListPvt(BranchId, CompId, Flag);
        }
        private List<InwardGatepassModel> GetMissingClaimFormListPvt(int BranchId, int CompId, int Flag)
        {
            List<InwardGatepassModel> MissingClaimFormLst = new List<InwardGatepassModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    MissingClaimFormLst = _contextManager.usp_GetmissingClaimFormList(BranchId, CompId, Flag).Select(c => new InwardGatepassModel
                    {
                        GatepassId = c.GatepassId,
                        GatepassNo = c.GatepassNo,
                        StockistId = c.StockistId,
                        StockistName = c.StockistName,
                        StockistNo = c.StockistNo,
                        TransporterId = Convert.ToInt32(c.TransporterId),
                        TransCourName = c.TransCourName,
                        TransporterName = c.TransporterName,
                        CourierId = Convert.ToInt32(c.CourierId),
                        CourierName = c.CourierName,
                        LRNumber = c.LRNo,
                        LRDate = Convert.ToDateTime(c.LRDate),
                        ReceiptDate = Convert.ToDateTime(c.ReceiptDate),
                        NoOfBoxes = Convert.ToInt32(c.NoOfBox),
                        AmountPaid = Convert.ToInt32(c.AmountPaid)
                    }).OrderByDescending(x => x.AddedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetMissingClaimFormListPvt", "Get Missing Claim Form List Pvt ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return MissingClaimFormLst;
        }
        #endregion

        #region Send Email To Stockiest For Missing Claim Form
        public string SendEmailForMissingClaimForm(string Emailid, int BranchId, int CompId, string StockistName)
        {
            bool bResult = false;
            string msg = string.Empty, messageformat = string.Empty, Date = string.Empty, Subject = string.Empty, MailFilePath = string.Empty, ToEmail = string.Empty, CCEmail = string.Empty, ClaimBodyText = string.Empty, EmailCC = string.Empty;
            EmailSend Email = new EmailSend();
            List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
            CCEmailDtls CCEmailModel = new CCEmailDtls();

            try
            {
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                ToEmail = Emailid;
                Subject = ConfigurationManager.AppSettings["ClaimSubj"] + Date + " ";
                ClaimBodyText = ConfigurationManager.AppSettings["ClaimBodyText"];
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailForMissingClaimForm.html");
                ChequeAccountingRepository _ChequeAccountingRepository = new ChequeAccountingRepository(_contextManager);
                CCEmailList = _ChequeAccountingRepository.GetCCEmailDtlsPvt(BranchId, CompId, 3);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }

                    EmailCC = CCEmail.TrimStart(';');
                }
                //bResult = EmailNotification.SendEmailMissingClaimForm(ToEmail, EmailCC, Subject, StockistName, ClaimBodyText, MailFilePath);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForMissingClaimForm", "Send Email For Missing Claim Form", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Add Send Email Flag
        public int AddSendEmailFlag(int BranchId, int CompId, int GatepassId, int Flag)
        {
            return AddSendEmailFlagPvt(BranchId,CompId, GatepassId,Flag);
        }
        private int AddSendEmailFlagPvt(int BranchId, int CompId, int GatepassId, int Flag)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_AddSendEmailFlag(BranchId, CompId, GatepassId,Flag, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddSendEmailFlagPvt", "Add Send Email Flag", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Physical check 1 List
        public List<PhysicalCheck1> GetPhysicalCheck1List(int BranchId, int CompId)
        {
            return GetPhysicalCheck1ListPvt(BranchId, CompId);
        }
        private List<PhysicalCheck1> GetPhysicalCheck1ListPvt(int BranchId, int CompId)
        {
            List<PhysicalCheck1> PhysicalCheck1List = new List<PhysicalCheck1>();
            try
            {
                PhysicalCheck1List = _contextManager.usp_PhysicalCheck1List(BranchId, CompId).Select(p => new PhysicalCheck1
                {
                    PhyChkId = p.PhyChkId,
                    BranchId = p.BranchId,
                    CompId = p.CompId,
                    GatepassId = Convert.ToInt32(p.GatepassId),
                    GatepassNo = p.GatepassNo,
                    StockistId = p.StockistId,
                    StockistNo = p.StockistNo,
                    StockistName = p.StockistName,
                    LRNo = p.LRNo,
                    ReturnCatId = Convert.ToInt32(p.ReturnCatId),
                    RetCatName = p.RetCatName,
                    ClaimNo = p.ClaimNo,
                    ClaimDate = Convert.ToDateTime(p.ClaimDate),
                    ClaimStatus = Convert.ToInt32(p.ClaimStatus),
                    ConcernId = Convert.ToInt32(p.ConcernId),
                    ConcernText = p.ConcernText,
                    ConcernDate = Convert.ToDateTime(p.ConcernDate),
                    ConcernBy = Convert.ToInt32(p.ConcernBy),
                    ConcernByName = p.ConcernByName,
                    ResolveConcernBy = Convert.ToInt32(p.ResolveConcernBy),
                    ResolveConcernByName = p.ResolveConcernByName,
                    ResolveConcernDate = Convert.ToDateTime(p.ResolveConcernDate),
                    ResolveRemark = p.ResolveRemark
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPhysicalCheck1ListPvt", "Get Physical Check1 List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PhysicalCheck1List;
        }
        #endregion

        #region 1st Physical Check Add Edit
        public int PhysicalCheck1AddEdit(PhysicalCheck1 model)
        {
            return PhysicalCheck1AddEditPvt(model);
        }
        private int PhysicalCheck1AddEditPvt(PhysicalCheck1 model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_PhysicalCheck1AddEdit(model.PhyChkId, model.BranchId, model.CompId, model.GatepassId, model.ReturnCatId, model.ClaimNo, model.ClaimDate, model.AddedBy,
                    model.AddedOn, model.ClaimStatus, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PhysicalCheck1AddEditPvt", "Physical Check Add Edit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Physical check 1 Concern
        public int PhysicalCheck1Concern(PhysicalCheck1 model)
        {
            return PhysicalCheck1ConcernPvt(model);
        }
        public int PhysicalCheck1ConcernPvt(PhysicalCheck1 model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                if (model.Action == "RAISECONCERN")
                {
                    RetValue = _contextManager.usp_PhysicalCheck1Concern(model.PhyChkId, model.GatepassId, model.ClaimStatus, model.ConcernId,
                        model.ConcernRemark, model.ConcernDate, model.ConcernBy, model.ResolveConcernBy,
                        DateTime.Now, model.ResolveRemark, model.Action, obj);
                }
                else
                {
                    RetValue = _contextManager.usp_PhysicalCheck1Concern(model.PhyChkId, model.GatepassId, model.ClaimStatus, model.ConcernId,
                        model.ConcernRemark, DateTime.Now, model.ConcernBy, model.ResolveConcernBy,
                        model.ResolveConcernDate, model.ResolveRemark, model.Action, obj);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PhysicalCheck1Concern", "Physical Check 1 Concern", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Auditor Check - Verify and Correction List
        public List<SRSClaimListForVerifyModel> GetSRSClaimListForVerifyList(int BranchId, int CompId)
        {
            return GetSRSClaimListForVerifyListPvt(BranchId, CompId);
        }
        private List<SRSClaimListForVerifyModel> GetSRSClaimListForVerifyListPvt(int BranchId, int CompId)
        {
            List<SRSClaimListForVerifyModel> SRSClaimListForVerifyList = new List<SRSClaimListForVerifyModel>();
            try
            {
                SRSClaimListForVerifyList = _contextManager.usp_GetSRSClaimListForVerify(BranchId, CompId).Select(s => new SRSClaimListForVerifyModel
                {
                    BranchId = Convert.ToInt32(s.BranchId),
                    CompId = Convert.ToInt32(s.CompId),
                    PhyChkId = Convert.ToInt64(s.PhyChkId),
                    ClaimNo = s.ClaimNo,
                    ClaimDate = Convert.ToDateTime(s.ClaimDate),
                    ClaimStatus = Convert.ToInt32(s.ClaimStatus),
                    DocDate = Convert.ToDateTime(s.DocDate),
                    BaseUOM = s.BaseUOM,
                    DocStatus = s.DocStatus,
                    SalesDocNo = s.SalesDocNo,
                    SalesDocType = s.SalesDocType,
                    SRSId = s.SRSId,
                    SRSStatus = Convert.ToInt32(s.SRSStatus),
                    PONo = s.PONo,
                    SoldtoPartyId = Convert.ToInt32(s.SoldtoPartyId),
                    ReturnCatId = Convert.ToInt32(s.ReturnCatId),
                    ReturnCatName = s.ReturnCatName,
                    LRNo = s.LRNo,
                    StockistId = Convert.ToInt32(s.StockistId),
                    StockistName = s.StockistName,
                    StockistNo = s.StockistNo,
                    IsVerified = s.IsVerified,
                    IsCorrectionReq = s.IsCorrectionReq,
                    CorrectionReqRemark = s.CorrectionReqRemark,
                    VerifyCorrectionBy = Convert.ToInt32(s.VerifyCorrectionBy),
                    VerifyCorrectionDate = Convert.ToDateTime(s.VerifyCorrectionDate),
                    Netvalue = s.Netvalue
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetSRSClaimListForVerifyList", "Get Auditor Check - Verify and Correction List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return SRSClaimListForVerifyList;
        }
        #endregion

        #region Auditor Check - Verify and Correction Required(Remark)
        public int AuditorCheckCorrection(AuditorCheckCorrectionModel model)
        {
            return AuditorCheckCorrectionPvt(model);
        }
        public int AuditorCheckCorrectionPvt(AuditorCheckCorrectionModel model)
        {
            int RetValue = 0;
            ObjectParameter objRetValue = new ObjectParameter("RetValue", typeof(string));
            try
            {
                if (model.Action == "VERIFY")
                {
                    RetValue = _contextManager.usp_AuditorCheckUpdate(model.BranchId, model.CompId, model.SRSId, model.Action, model.ActionBy, model.ActionDate, "", objRetValue);
                }
                else
                {
                    RetValue = _contextManager.usp_AuditorCheckUpdate(model.BranchId, model.CompId, model.SRSId, model.Action, model.ActionBy, model.ActionDate, model.CorrectionReqRemark, objRetValue);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AuditorCheckCorrection", "Auditor Check - Verify and Correction Required(Remark)", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Import CN Details List
        public List<ImportCNData> GetImportCNDataList(int BranchId, int CompId)
        {
            return GetImportCNDataListPvt(BranchId, CompId);
        }
        private List<ImportCNData> GetImportCNDataListPvt(int BranchId, int CompId)
        {
            List<ImportCNData> ImportCNDataList = new List<ImportCNData>();
            try
            {
                ImportCNDataList = _contextManager.usp_GetImportCNDataList(BranchId, CompId).Select(c => new ImportCNData
                {
                    BranchId= Convert.ToInt32(c.BranchId),
                    CompId = Convert.ToInt32(c.CompId),
                    CompanyCode = c.CompanyCode,
                    DistChannel = c.DistChannel,
                    Division = c.Division,
                    SalesOrderNo = c.SalesOrderNo,
                    SalesOrderDate = Convert.ToString(c.SalesOrderDate),
                    CrDrNoteNo= c.CrDrNoteNo,
                    CRDRCreationDate = Convert.ToString(c.CRDRCreationDate),
                    CrDrAmt = Convert.ToString(c.CrDrAmt),
                    StockistNo = c.StockistNo,
                    StockistName = c.StockistName,
                    CityName = c.CityName,
                    OrderReason = c.OrderReason,
                    LRNo = c.LRNo,
                    LRDate = Convert.ToString(c.LRDate),
                    CFAGRDate = Convert.ToString(c.CFAGRDate),
                    MaterialNumber = c.MaterialNumber,
                    BatchNo = c.BatchNo,
                    BillingQty = Convert.ToString(c.BillingQty),
                    AddedBy = Convert.ToInt32(c.AddedBy),
                    LastUpdateDate = Convert.ToDateTime(c.LastUpdateDate)
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetImportCNDataList", "Get Import CN Data List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ImportCNDataList;
        }
        #endregion

        #region Get Import CN Data For Email
        public List<CNDataForEmail> GetImportCNDataForEmail(int BranchId, int CompId)
        {
            return GetImportCNDataForEmailPvt(BranchId, CompId);
        }
        private List<CNDataForEmail> GetImportCNDataForEmailPvt(int BranchId, int CompId)
        {
            List<CNDataForEmail> ImportCNDataList = new List<CNDataForEmail>();
            try
            {
                ImportCNDataList = _contextManager.usp_ImportCNDetaForEmail(BranchId, CompId).Select(c => new CNDataForEmail
                {
                    BranchId = Convert.ToInt32(c.BranchId),
                    CompId = Convert.ToInt32(c.CompId),
                    CrDrNoteNo = c.CrDrNoteNo,
                    StockistName = c.StockistName,
                    Emailid = c.Emailid
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetImportCNDataForEmailPvt", "Get CN Data For Email", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ImportCNDataList;
        }
        #endregion

        #region Add Claim - SRS Mapping
        public int ClaimSRSMappingAddEdits(AddClaimSRSMappingModel model)
        {
            return ClaimSRSMappingAddEditPvt(model);
        }
        private int ClaimSRSMappingAddEditPvt(AddClaimSRSMappingModel model)
        {

            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_ClaimSRSMapping(model.BranchId, model.CompId, model.PhyChkId, model.SRSId, model.ClaimNo, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ClaimSRSMappingAddEditPvt", "Claim SRS Mapping AddEditPvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Claim - SRS Mapping List
        public List<AddClaimSRSMappingModel> GetClaimSRSMappingLists(int BranchId, int CompId, int PhyChkId)
        {
            return GetClaimSRSMappingListsPvt(BranchId, CompId, PhyChkId);
        }
        private List<AddClaimSRSMappingModel> GetClaimSRSMappingListsPvt(int BranchId, int CompId, int PhyChkId)
        {
            List<AddClaimSRSMappingModel> modelList = new List<AddClaimSRSMappingModel>();
            try
            {
                modelList = _contextManager.ups_GetSRSHeaderListForClaimMapping(BranchId, CompId, PhyChkId).Select(i => new AddClaimSRSMappingModel
                {
                    BranchId = Convert.ToInt32(i.BranchId),
                    CompId = Convert.ToInt32(i.CompId),
                    SRSId = Convert.ToString(i.SRSId),
                    SalesDocNo = i.SalesDocNo,
                    SalesDocType = i.SalesDocType,
                    DocDate = Convert.ToDateTime(i.DocDate),
                    PONo = i.PONo,
                    SoldtoPartyId = Convert.ToInt32(i.SoldtoPartyId),
                    BaseUOM = i.BaseUOM,
                    Netvalue = i.Netvalue,
                    Division = i.Division,
                    DocStatus = i.DocStatus,
                    OrderReason = i.OrderReason,
                    StockistId = Convert.ToInt32(i.StockistId),
                    StockistName = Convert.ToString(i.StockistName),
                    StockistNo = Convert.ToString(i.StockistNo),
                    CityCode = i.CityCode,
                    PhyChkId = Convert.ToInt32(i.PhyChkId)
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetClaimSRSMappingListsPvt", "Get Claim SRS Mapping Lists Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get ClaimNo List
        public List<GetClaimNoListModel> GetClaimNoLists(int BranchId, int CompId)
        {
            return GetClaimNoListPvt(BranchId, CompId);
        }
        private List<GetClaimNoListModel> GetClaimNoListPvt(int BranchId, int CompId)
        {
            List<GetClaimNoListModel> modelList = new List<GetClaimNoListModel>();
            try
            {
                modelList = _contextManager.usp_PhysicalCheck1List(BranchId, CompId).Select(b => new GetClaimNoListModel
                {
                    PhyChkId = Convert.ToInt32(b.PhyChkId),
                    BranchId = b.BranchId,
                    CompId = b.CompId,
                    GatepassId = Convert.ToInt32(b.GatepassId),
                    GatepassNo = b.GatepassNo,
                    ReceiptDate = Convert.ToDateTime(b.ReceiptDate),
                    StockistId = b.StockistId,
                    StockistNo = b.StockistNo,
                    StockistName = b.StockistName,
                    RetCatName = b.RetCatName,
                    ReturnCatId = Convert.ToInt32(b.ReturnCatId),
                    ClaimNo = b.ClaimNo,
                    ClaimDate = Convert.ToDateTime(b.ClaimDate),
                    ClaimStatus = Convert.ToInt32(b.ClaimStatus),
                    ConcernId = Convert.ToInt32(b.ConcernId),
                    ConcernText = b.ConcernText,
                    ConcernRemark = b.ConcernRemark,
                    ConcernDate = Convert.ToDateTime(b.ConcernDate),
                    ConcernBy = Convert.ToInt32(b.ConcernBy),
                    ConcernByName = b.ConcernByName,
                    ResolveConcernBy = Convert.ToInt32(b.ResolveConcernBy),
                    ResolveConcernByName = b.ResolveConcernByName,
                    ResolveConcernDate = Convert.ToDateTime(b.ResolveConcernDate),
                    ResolveRemark = Convert.ToString(b.ResolveRemark),
                    CityName = Convert.ToString(b.CityName),
                    LRDate = Convert.ToDateTime(b.LRDate),
                    NoOfBox = Convert.ToInt32(b.NoOfBox),
                    AmountPaid = Convert.ToInt32(b.AmountPaid),
                    ClaimFormAvailable = b.ClaimFormAvailable,
                    LRNo = b.LRNo
                }).OrderByDescending(x => x.ClaimNo).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetClaimNoListPvt", "Get ClaimNo List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get SRS Claim Mapped List
        public List<AddClaimSRSMappingModel> GetSRSClaimMappedLists(int BranchId, int CompId)
        {
            return GetSRSClaimMappedListPvt(BranchId, CompId);
        }
        private List<AddClaimSRSMappingModel> GetSRSClaimMappedListPvt(int BranchId, int CompId)
        {
            List<AddClaimSRSMappingModel> modelList = new List<AddClaimSRSMappingModel>();
            try
            {
                modelList = _contextManager.usp_GetClaimSRSMappedList(BranchId, CompId).Select(b => new AddClaimSRSMappingModel
                {
                    BranchId = Convert.ToInt32(b.BranchId),
                    CompId = Convert.ToInt32(b.CompId),
                    ClaimNo = b.ClaimNo,
                    SRSId = Convert.ToString(b.SRSId),
                    PhyChkId = Convert.ToInt32(b.PhyChkId),
                    ReturnCatId = Convert.ToInt32(b.ReturnCatId),
                    LRNo = b.LRNo,
                    StockistId = Convert.ToInt32(b.StockistId),
                    StockistName = b.StockistName,
                    StockistNo = b.StockistNo,
                    ClaimDate = Convert.ToDateTime(b.ClaimDate),
                    ClaimStatus = Convert.ToInt32(b.ClaimStatus),
                    SalesDocNo = b.SalesDocNo,
                    DocDate = Convert.ToDateTime(b.DocDate),
                    SalesDocType = b.SalesDocType
                }).OrderByDescending(x => x.ClaimNo).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetClaimNoListPvt", "Get ClaimNo List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region  Add Delay Reason Of Pending CN
        public int UpdateCNDelayReason(UpdateCNDelayReason model)
        {
            return AddDelayReasonOfPendingCNPvt(model);
        }
        public int AddDelayReasonOfPendingCNPvt(UpdateCNDelayReason model)
        {
            int RetValue = 0;
            ObjectParameter objRetValue = new ObjectParameter("RetValue", typeof(string));
            try
            {
                RetValue = _contextManager.usp_UpdateCNDelayReason(model.BranchId, model.CompId, model.SRSId, model.CNDelayReasonId, model.CNDelayRemark, model.AddedBy, objRetValue);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddDelayReasonOfPendingCN", "Add Delay Reason Of Pending CN", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get LR Mismatch List
        public List<LRmisMatchModel> GetLrMisMatchLists(int BranchId, int CompId)
        {
            return GetLrMisMatchListPvt(BranchId, CompId);
        }
        private List<LRmisMatchModel> GetLrMisMatchListPvt(int BranchId, int CompId)
        {
            List<LRmisMatchModel> modelList = new List<LRmisMatchModel>();
            try
            {
                modelList = _contextManager.usp_GetLrMisMatchList(BranchId, CompId).Select(b => new LRmisMatchModel
                {
                    BranchId = Convert.ToInt32(b.BranchId),
                    CompId = Convert.ToInt32(b.CompId),
                    StockistId = b.StockistId,
                    StockistNo = b.StockistNo,
                    StockistName = b.StockistName,
                    LRNo = b.LRNo,
                    LRDate = Convert.ToDateTime(b.LRDate),
                    AmountPaid = Convert.ToInt32(b.AmountPaid),
                    CityName = b.CityName,
                    TransporterId = Convert.ToInt32(b.TransporterId),
                    TransporterName = b.TransporterName,
                    TransporterNo = b.TransporterNo,
                }).OrderByDescending(x => x.LRNo).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetLrMisMatchListPvt", "Get Lr MisMatch List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

    }
}
