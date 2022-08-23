using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using CNF.Business.Model.Context;
using CNF.Business.Model.Login;
using CNF.Business.Model.OrderDispatch;
using CNF.Business.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;

namespace CNF.Business.Repositories
{
    public class OrderDispatchRepository : IOrderDispatchRepository
    {
        private CFADBEntities _contextManager;
        private LoginRepository _loginRepository;
        private ChequeAccountingRepository _ChequeAccountingRepository;
        //Inject DB Context instance from Unit of Work to avoid unwanted memory allocations.
        public OrderDispatchRepository(CFADBEntities contextManager)
        {
            _contextManager = contextManager;
        }

        #region PickListHeader list & AddEdit
        public string PickListHeaderAddEdit(PickListModel model)
        {
            return PickListHeaderAddEditPvt(model);
        }
        private string PickListHeaderAddEditPvt(PickListModel model)
        {

            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_PickListHeaderAddEdit(model.Picklistid, model.BranchId, model.CompId, model.PicklistDate, model.FromInv, model.ToInv, model.PicklistStatus, model.VerifiedBy, model.RejectReason, model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PickListHeaderAddEditPvt", "Picklist AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if(RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.FailStatus;
            }
        }
        #endregion

        #region Get Picklist By BranchId and CompanyId
        public List<PickListModel> GetPickLst(int BranchId, int CompId, DateTime PicklistDate)
        {
            return GetPickLstPvt(BranchId, CompId, PicklistDate);
        }
        private List<PickListModel> GetPickLstPvt(int BranchId,int CompId, DateTime PicklistDate)
        {
            List<PickListModel> PckList = new List<PickListModel>();

            try
            {
                PckList = _contextManager.usp_GetPickList(BranchId, CompId, PicklistDate).Select(c => new PickListModel
                {
                    BranchId = c.BranchId,
                    CompId = c.CompId,
                    Picklistid = c.Picklistid,
                    PicklistNo = c.PicklistNo,
                    PicklistDate = DateTime.Parse(c.PicklistDate.ToString()),
                    FromInv = c.FromInv,
                    ToInv = c.ToInv,
                    PicklistStatus = Convert.ToInt32(c.PicklistStatus),
                    VerifiedBy = Convert.ToInt32(c.VerifiedBy),
                    StatusText = c.StatusText,
                    OnPriority = c.OnPriority
                }).OrderByDescending(x => x.OnPriority).ToList();

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickLstPvt", "Get Picklist By BranchId and CompanyId List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PckList;
        }
        #endregion

        #region Get PickList Details By Id
        public List<PickListDetailsByIdModel> GetPickListDetailsById(int Picklistid)
        {
            return GetPickListDetailsByIdPvt(Picklistid);
        }
        private List<PickListDetailsByIdModel> GetPickListDetailsByIdPvt(int Picklistid)
        {
            List<PickListDetailsByIdModel> pickListDetailsByIdModel = new List<PickListDetailsByIdModel>();

            try
            {
                //pickListDetailsByIdModel = _contextManager.usp_PickListDetailsById(Picklistid).Select(c => new PickListDetailsByIdModel
                //{
                //    PicklistDtlsId = c.PicklistDtlsId,
                //    Picklistid = c.PicklistId,
                //    DivisionId = c.DivisionId,
                //    Addedby = c.Addedby,
                //    AddedOn = Convert.ToDateTime(c.AddedOn),
                //    LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn)
                //}).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListDetailsByIdPvt", "Get PickList Details By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickListDetailsByIdModel;
        }
        #endregion

        #region Send Email to Stockist
        public string sendEmail(string EmailId, string PicklistNo)
        {
            bool bResult = false;
            string msg = string.Empty, messageformat = string.Empty, Date = string.Empty, Subject = string.Empty, MailFilePath = string.Empty, CCEmail = string.Empty;
            EmailSend Email = new EmailSend();
            try
            {
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                CCEmail = ConfigurationManager.AppSettings["ToEmail"];
                Subject = ConfigurationManager.AppSettings["StockistSubject"] + Date + " ";
                msg = ConfigurationManager.AppSettings["Message"] + PicklistNo;
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailToStockiest.html");
                bResult = EmailNotification.SendEmails(EmailId, CCEmail, Subject, msg, MailFilePath);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "sendEmail", "Send Email to Stockist", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Send Email To Picker
        public string sendEmailToPicker(string EmailId, string PicklistNo)
        {
            bool bResult = false;
            string msg = string.Empty, messageformat = string.Empty, Date = string.Empty, Subject = string.Empty, MailFilePath = string.Empty, CCEmail = string.Empty;
            EmailSend Email = new EmailSend();
            try
            {              
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                CCEmail = ConfigurationManager.AppSettings["ToEmail"];
                Subject = ConfigurationManager.AppSettings["PickerSubject"] + Date + " ";
                msg = ConfigurationManager.AppSettings["Message"] + PicklistNo;
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailToPicker.html");
                bResult = EmailNotification.SendEmails(EmailId, CCEmail, Subject, msg, MailFilePath);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "sendEmailToPicker", "Send Email To Picker", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Picklist Allotment Add
        public string PicklistAllotmentAdd(PicklstAllotReallotModel model)
        {
            return PicklistAllotmentAddPvt(model);
        }
        private string PicklistAllotmentAddPvt(PicklstAllotReallotModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            _loginRepository = new LoginRepository(_contextManager);
            try
            {
                RetValue = _contextManager.usp_PicklistAllotmentAdd(model.BranchId, model.CompId, model.Picklistid, model.PicklistDtlsId, model.ProductCode, model.BatchNo, model.PickerId,
                    model.AllottedBy, obj);
                if (RetValue > 0)
                {
                    string[] values = model.PickerId.Split(',');
                    for (var i = 0; i < values.Length - 1; i++)
                    {
                    _loginRepository.SendNotification(model.BranchId, Convert.ToInt32(values[i]), BusinessCont.PickListNotificationHeader, BusinessCont.PickListNotificationMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PicklistAllotmentAddPvt", "Picklist Allotment Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if(RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.FailStatus;
            }
        }
        #endregion

        #region Picklist ReAllotment Add
        public string PicklistReAllotmentAdd(PicklstAllotReallotModel model)
        {
            return PicklistReAllotmentAddPvt(model);
        }
        private string PicklistReAllotmentAddPvt(PicklstAllotReallotModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            _loginRepository = new LoginRepository(_contextManager);

            try
            {
                RetValue = _contextManager.usp_PicklistReAllotmentAdd(model.BranchId, model.CompId, model.Picklistid,model.PicklistDtlsId, model.AllotmentId,model.ProductCode, model.BatchNo, model.PickerId,
                    model.ReAllottedBy, obj);

                if (RetValue > 0)
                {
                    string[] values = model.PickerId.Split(',');
                    for (var i = 0; i < values.Length - 1; i++)
                    {
                    _loginRepository.SendNotification(model.BranchId, Convert.ToInt32(values[i]), BusinessCont.PickListNotificationHeader, BusinessCont.PickListNotificationMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PicklistReAllotmentAddPvt", "Picklist ReAllotment Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Picklist Allotment Status
        public string PicklistAllotmentStatus(PicklistAllotmentStatusModel model)
        {
            return PicklistAllotmentStatusPvt(model);
        }
        private string PicklistAllotmentStatusPvt(PicklistAllotmentStatusModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_PicklistAllotmentStatus(model.AllotmentId, model.Picklistid, model.BranchId, model.AllotmentStatus, model.ReasonId, model.RejectRemark, model.UserId,model.StatusTime, obj);  //pass pickerid to userid
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PicklistAllotmentStatusPvt", "Picklist Allotment Status", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get InvoiceHeaderList
        public List<InvoiceLstModel> GetInvoiceHeaderLst(int BranchId, int CompId, Nullable<DateTime> FromDate, Nullable<DateTime> ToDate, int BillDrawerId)
        {
            return GetInvoiceHeaderLstPvt(BranchId, CompId, FromDate, ToDate, BillDrawerId);
        }
        private List<InvoiceLstModel> GetInvoiceHeaderLstPvt(int BranchId, int CompId, Nullable<DateTime> FromDate, Nullable<DateTime> ToDate, int BillDrawerId)
        {
            List<InvoiceLstModel> InvoiceLst = new List<InvoiceLstModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InvoiceLst = _contextManager.usp_InvoiceHeaderList(BranchId, CompId, FromDate, ToDate, BillDrawerId).Select(c => new InvoiceLstModel
                    {
                        InvId = c.InvId,
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        InvNo = c.InvNo,
                        InvPostingDate = Convert.ToDateTime(c.InvPostingDate),
                        InvCreatedDate = Convert.ToDateTime(c.InvCreatedDate),
                        IsColdStorage = c.IsColdStorage,
                        SoldTo_StokistId = c.SoldTo_StokistId,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        SoldTo_City = c.SoldTo_City,
                        CityName = c.CityName,
                        InvAmount = Convert.ToDouble(c.InvAmount),
                        PONo = c.PONo,
                        PODate = Convert.ToDateTime(c.PODate),
                        InvStatus = Convert.ToInt32(c.InvStatus),
                        StatusText = c.StatusText,
                        Addedby = c.Addedby,
                        AddedOn = Convert.ToDateTime(c.AddedOn),
                        LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn),
                        NoOfItems = Convert.ToInt32(c.NoOfItems),
                        CityCode = c.CityCode,
                        AcceptedBy = Convert.ToInt32(c.AcceptedBy),
                        BillDrawerName = c.BillDrawerName,
                        AcceptedDate = Convert.ToDateTime(c.AcceptedDate),
                        BillDrawnDate = Convert.ToDateTime(c.BillDrawnDate),
                        PackedBy = Convert.ToInt32(c.PackedBy),
                        PackedByName = c.PackedByName,
                        PackedDate = Convert.ToDateTime(c.PackedDate),
                        NoOfBox = Convert.ToInt32(c.NoOfBox),
                        InvWeight = Convert.ToInt32(c.InvWeight),
                        IsCourier = Convert.ToInt32(c.IsCourier),
                        PackingConcernDate = Convert.ToDateTime(c.PackingConcernDate),
                        PackingConcernId = Convert.ToInt32(c.PackingConcernId),
                        PackingConcernText = c.PackingConcernText,
                        PackingRemark = c.PackingRemark,
                        ReadyToDispatchBy = Convert.ToInt32(c.ReadyToDispatchBy),
                        ReadyToDispatchDate = Convert.ToDateTime(c.ReadyToDispatchDate),
                        ReadyToDispatchConcernId = Convert.ToInt32(c.ReadyToDispatchConcernId),
                        ReadyToDispatchRemark = c.ReadyToDispatchRemark,
                        CancelBy = Convert.ToInt32(c.CancelBy),
                        CancelDate = Convert.ToDateTime(c.CancelDate),
                        TotalInv = Convert.ToInt32(c.TotalInv),
                        OnPriority = Convert.ToInt32(c.OnPriority)
                    }).OrderBy(x => Convert.ToInt64(x.InvNo)).OrderByDescending(x => x.OnPriority).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceHeaderLstPvt", "Get Invoice Header List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvoiceLst;
        }
        #endregion

        #region Get Alloted Picklist For Picker
        public List<Picklstmodel> GetAllotedPickListForPicker(int BranchId, int CompId, int PickerId, DateTime PicklistDate)
        {
            return GetAllotedPickListForPickerPvt(BranchId, CompId, PickerId, PicklistDate);
        }
        private List<Picklstmodel> GetAllotedPickListForPickerPvt(int BranchId, int CompId,int PickerId, DateTime PicklistDate)
        {
            List<Picklstmodel> picklstmodels = new List<Picklstmodel>();
            try
            {
                picklstmodels = _contextManager.usp_GetAllotedPickListForPicker(BranchId, CompId, PickerId, PicklistDate)
                    .Select(c => new Picklstmodel
                {
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        PickerId = c.PickerId,
                        AllotmentId = c.AllotmentId,
                        Picklistid = c.Picklistid,
                        AllottedBy = Convert.ToInt32(c.AllottedBy),
                        AllottedDate = Convert.ToDateTime(c.AllottedDate),
                        AllotmentStatus = c.AllotmentStatus,
                        AllotmentStatusText = c.AllotmentStatusText,
                        PicklistNo = c.PicklistNo,
                        PicklistDate = Convert.ToDateTime(c.PicklistDate),
                        FromInv = c.FromInv,
                        ToInv = c.ToInv,
                        PicklistStatus = Convert.ToInt32(c.PicklistStatus),
                        PicklistStatusText = c.PicklistStatusText,
                        AcceptedDate = Convert.ToDateTime(c.AcceptedDate),
                        ReasonId = Convert.ToInt32(c.ReasonId),
                        ReasonText = c.ReasonText,
                        RejectRemark = c.RejectRemark,
                        PickedDate = Convert.ToDateTime(c.PickedDate),
                        PickerConcernId = Convert.ToInt32(c.PickerConcernId),
                        pickerconcerText = c.pickerconcerText,
                        PickerConcernRemark = c.PickerConcernRemark,
                        PickerConcernDate = Convert.ToDateTime(c.PickerConcernDate),
                        VerifiedBy = Convert.ToInt32(c.VerifiedBy),
                        VerifiedDate = Convert.ToDateTime(c.VerifiedDate),
                        VerifiedByName = c.VerifiedByName,
                        VerifiedConcernId = Convert.ToInt32(c.VerifiedConcernId),
                        VerifiedConcernText = c.VerifiedConcernText,
                        VerifiedConcernRemark = c.VerifiedConcernRemark,
                        OnPriority = c.OnPriority
                    }).OrderByDescending(x => x.OnPriority).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetAllotedPickListForPickerPvt", "Get Alloted Picklist For Picker", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return picklstmodels;
        }
        #endregion

        #region Invoice Header Status Update
        public string InvoiceHeaderStatusUpdate(InvoiceHeaderStatusUpdateModel model)
        {
            return InvoiceHeaderStatusUpdatePvt(model);
        }
        private string InvoiceHeaderStatusUpdatePvt(InvoiceHeaderStatusUpdateModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetVal", typeof(int));
            try
            {
                RetValue = _contextManager.usp_InvoiceHeaderStatusUpdate(model.InvId, model.BranchId, model.CompId, model.InvStatus, model.NoOfBox, model.InvWeight, model.IsColdStorage, model.IsCourier, model.ConcernId, model.PackedBy, model.Remark, model.Addedby, model.UpdateDate, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InvoiceHeaderStatusUpdatePvt", "Invoice Header Status Update " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Assign Transport Mode
        public string AssignTransportMode(AssignTransportModel model)
        {
            return AssignTransportModePvt(model);
        }
        private string AssignTransportModePvt(AssignTransportModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_AssignTransportMode(model.InvoiceId, model.TransportModeId, model.PersonName,
                    model.PersonAddress, model.PersonMobileNo, model.OtherDetails, model.TransporterId, model.Delivery_Remark,
                    model.CAId, model.CourierId, model.Addedby,model.AttachedInvId, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AssignTransportModePvt", "Assign Transport Mode", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (model.Action == "DELETE")
            {
                if (RetValue > 0)
                {
                    return BusinessCont.DeleteRecord;
                }
                else
                {
                    return BusinessCont.FailStatus;
                }
            }
            else
            {
                if (RetValue > 0)
                {
                    return BusinessCont.SuccessStatus;
                }
                else if (RetValue == -1)
                {
                    return BusinessCont.msg_exist;
                }
                else
                {
                    return BusinessCont.FailStatus;
                }
            }
        }
        #endregion

        #region Get Picklist Summary Data 
        public PickLstSummaryData GetPickListSummaryData(int BranchId, int CompId, int PickerId, DateTime PicklistDate)
        {
            return GetPickListSummaryDataPvt(BranchId, CompId, PickerId, PicklistDate);
        }
        private PickLstSummaryData GetPickListSummaryDataPvt(int BranchId,int CompId,int PickerId,DateTime PicklistDate)
        {
            PickLstSummaryData pickLstdDtls = new PickLstSummaryData();
            try
            {
                pickLstdDtls = _contextManager.usp_GetPicklistSummaryData(BranchId,CompId,PickerId,PicklistDate).Select(c => new PickLstSummaryData
                {
                    BranchId = c.BranchId,
                    CompId = c.CompId,
                    PicklistDate = Convert.ToDateTime(c.PicklistDate),
                    PickerId = c.PickerId,
                    Alloted = Convert.ToInt32(c.TotalPL),
                    Accepted = Convert.ToInt32(c.Accepted),
                    Pending = Convert.ToInt32(c.Pending),
                    Rejected  = Convert.ToInt32(c.Rejected),
                    Concern = Convert.ToInt32(c.Concern),
                    Completed = Convert.ToInt32(c.Completed),
                    Verified = Convert.ToInt32(c.Verified)
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListSummaryDataPvt", "Get Picklist Summary Data  ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickLstdDtls;
        }
        #endregion

        #region Invoice List For Assign Trans Mode
        public List<InvoiceLstModel> InvoiceListForAssignTransMode(int BranchId, int CompId)
        {
            return InvoiceListForAssignTransModePvt(BranchId, CompId);
        }
        private List<InvoiceLstModel> InvoiceListForAssignTransModePvt(int BranchId, int CompId)
        {
            List<InvoiceLstModel> InvoiceLst = new List<InvoiceLstModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InvoiceLst = _contextManager.usp_InvoiceHeaderListForAssignTransMode(BranchId, CompId).Select(c => new InvoiceLstModel
                    {
                        InvId = c.InvId,
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        InvNo = c.InvNo,
                        InvPostingDate = Convert.ToDateTime(c.InvPostingDate),
                        InvCreatedDate = Convert.ToDateTime(c.InvCreatedDate),
                        IsColdStorage = c.IsColdStorage,
                        SoldTo_StokistId = c.SoldTo_StokistId,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        SoldTo_City = c.SoldTo_City,
                        InvAmount = Convert.ToDouble(c.InvAmount),
                        PONo = c.PONo,
                        PODate = Convert.ToDateTime(c.PODate),
                        InvStatus = Convert.ToInt32(c.InvStatus),
                        StatusText = c.StatusText,
                        Addedby = c.Addedby,
                        AddedOn = Convert.ToDateTime(c.AddedOn),
                        LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn),
                        NoOfItems = Convert.ToInt32(c.NoOfItems),
                        CityCode = c.CityCode,
                        CityName = c.CityName,
                        IsCourier = c.IsCourier,
                        OnPriority = Convert.ToInt32(c.OnPriority)
                    }).OrderBy(x => Convert.ToInt64(x.InvNo)).OrderByDescending(x => x.OnPriority).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InvoiceListForAssignTransModePvt", "Invoice List For Assign Trans Mode " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvoiceLst;
        }
        #endregion

        #region Get PickList Generate New No
        public string GetPickListGenerateNewNo(int BranchId, int CompId, DateTime PicklistDate)
        {
            return GetPickListGenerateNewNoPvt(BranchId, CompId, PicklistDate);
        }
        private string GetPickListGenerateNewNoPvt(int BranchId, int CompId, DateTime PicklistDate)
        {
            string pickListNo = string.Empty;

            try
            {
                pickListNo = _contextManager.usp_PickListGenerateNewNo(BranchId, CompId, PicklistDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListGenerateNewNoPvt", "Get PickList Generate New No " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickListNo;
        }
        #endregion

        #region Get Invoice Details For Sticker
        public List<GetInvoiceDetailsForStickerModel> GetInvoiceDetailsForSticker(int BranchId, int CompId, int InvId)
        {
            return GetInvoiceDetailsForStickerPvt(BranchId, CompId, InvId);
        }
        private List<GetInvoiceDetailsForStickerModel> GetInvoiceDetailsForStickerPvt(int BranchId, int CompId, int InvId)
        {
            List<GetInvoiceDetailsForStickerModel> modelList = new List<GetInvoiceDetailsForStickerModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    modelList = _contextManager.usp_GetInvoiceDetailsForSticker(BranchId, CompId, InvId).Select(s => new GetInvoiceDetailsForStickerModel
                    {
                        InvId = s.InvId,
                        BranchId = s.BranchId,
                        CompId = s.CompId,
                        InvNo = s.InvNo,
                        StockistNo = s.StockistNo,
                        StockistName = s.StockistName,
                        MobNo = s.MobNo,
                        StockistAddress = s.StockistAddress,
                        CityCode = s.CityCode,
                        CityName = s.CityName,
                        TransportModeId = s.TransportModeId,
                        PersonName = s.PersonName,
                        PersonAddress = s.PersonAddress,
                        PersonMobNo = s.PersonMobNo,
                        OtherDetails = s.OtherDetails,
                        TransporterId = Convert.ToInt32(s.TransporterId),
                        TransporterNo = s.TransporterNo,
                        TransporterName = s.TransporterName,
                        CourierId = Convert.ToInt32(s.CourierId),
                        CourierName = s.CourierName,
                        Delivery_Remark = s.Delivery_Remark,
                        SoldTo_StokistId = s.SoldTo_StokistId,
                        InvAmount = Convert.ToDouble(s.InvAmount),
                        NoOfItems = Convert.ToInt32(s.NoOfItems),
                        InvStatus = Convert.ToInt32(s.InvStatus),
                        NoOfBox = Convert.ToInt32(s.NoOfBox),
                        InvWeight = Convert.ToDecimal(s.InvWeight),
                        IsCourier = Convert.ToInt32(s.IsCourier),
                        OnPriority = Convert.ToInt32(s.OnPriority)
                    }).OrderByDescending(x => x.OnPriority).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceDetailsForStickerPvt", "Get Invoice Details For Sticker " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion
  
        #region Get LR Data List
        public List<ImportLrDataModel> GetLRDataLst(int BranchId, int CompId, string LRDate)
        {
            return GetLRDataLstPvt(BranchId, CompId, LRDate);
        }
        private List<ImportLrDataModel> GetLRDataLstPvt(int BranchId, int CompId, string LRDate)
        {
            List<ImportLrDataModel> LRDataLst = new List<ImportLrDataModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    LRDataLst = _contextManager.usp_GetInvoiceListWithLRDetails(BranchId, CompId, Convert.ToDateTime(LRDate))    //need to update sp  
                        .Select(c => new ImportLrDataModel
                        {
                            InvId = c.InvId,
                            InvNo = c.InvNo,
                            StokistId = c.StokistId,
                            StockistNo = c.StockistNo,
                            StockistName=c.StockistName,
                            LRNo = c.LRNo,
                            LRDate = Convert.ToDateTime(c.LRDate).Date.ToString("dd-MM-yyyyy"),
                            LRDatestring = (Convert.ToString(c.LRDate) != "" ? Convert.ToString(c.LRDate) : "--"),
                            LRBox = Convert.ToInt32(c.LRBox),
                            LRWeightInKG = Convert.ToDecimal(c.LRWeightInKG),
                            NoOfBox = Convert.ToInt32(c.NoOfBox),
                            OnPriority = Convert.ToInt32(c.OnPriority)                          
                        }).OrderBy(x => Convert.ToInt64(x.InvNo)).OrderByDescending(x => x.OnPriority).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetLRDataLstPvt", "Get LR List " + "BranchId:  " + BranchId + "CompId:  " + CompId + "LRDate:  " + LRDate, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return LRDataLst;
        }
        #endregion

        #region Get Picklist Summary Data 
        public PickLstSummaryData GetPickListSummaryCounts(PickLstSummaryData model)
        {
            return GetPickListSummaryCountsPvt(model);
        }
        private PickLstSummaryData GetPickListSummaryCountsPvt(PickLstSummaryData model)
        {
            PickLstSummaryData pickLstdDtls = new PickLstSummaryData();
            try
            {
                pickLstdDtls = _contextManager.usp_GetPicklistSummaryCounts(model.BranchId, model.CompId, model.PicklistDate).Select(c => new PickLstSummaryData
                {
                    BranchId = c.BranchId,
                    CompId = c.CompId,
                    TotalPL = Convert.ToInt32(c.TotalPL),
                    OperatorRejected = Convert.ToInt32(c.OperatorRejected),
                    Pending = Convert.ToInt32(c.AllotmentPending),
                    Alloted = Convert.ToInt32(c.Alloted),
                    Accepted = Convert.ToInt32(c.Accepted),
                    Concern = Convert.ToInt32(c.Concern),
                    Completed = Convert.ToInt32(c.Completed),
                    CompletedVerified = Convert.ToInt32(c.CompletedVerified)
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListSummaryCountsPvt", "Get Picklist Summary Counts  ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickLstdDtls;
        }
        #endregion

        #region Get Picklist For Re-Allotment
        public List<PickListModel> GetPickListForReAllotment(PickListModel model)
        {
            return GetPickForReAllotmentPvt(model);
        }
        private List<PickListModel> GetPickForReAllotmentPvt(PickListModel model)
        {
            List<PickListModel> PickList = new List<PickListModel>();

            try
            {
                PickList = _contextManager.usp_GetPickListForReallotment(model.BranchId, model.CompId, model.PicklistDate).Select(c => new PickListModel
                {
                    BranchId = c.BranchId,
                    CompId = c.CompId,
                    Picklistid = c.Picklistid,
                    PicklistNo = c.PicklistNo,
                    PickerId = c.PickerId,
                    PicklistDate = DateTime.Parse(c.PicklistDate.ToString()),
                    FromInv = c.FromInv,
                    ToInv = c.ToInv,
                    PickerName = c.PickerName,
                    PickerNo = c.PickerNo,
                    PicklistStatus = Convert.ToInt32(c.PicklistStatus),
                    VerifiedBy = Convert.ToInt32(c.VerifiedBy),
                    StatusText = c.StatusText,
                    AllotmentId = c.AllotmentId,
                    AllottedDate = DateTime.Parse(c.AllottedDate.ToString()),
                    AllotmentStatus = c.AllotmentStatus.ToString(),
                    AllotmentStatusText = c.AllotmentStatusText,
                    RejectRemark = c.RejectRemark,
                    OnPriority = c.OnPriority
                }).OrderByDescending(x => x.OnPriority).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickLstPvt", "Get Picklist By BranchId and CompanyId List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PickList;
        }
        #endregion

        #region Get Invoice Summary Counts
        public InvCntModel InvoiceSummaryCounts(int BranchId, int CompId, DateTime InvDate)
        {
            return InvoiceSummaryCountsPvt(BranchId, CompId, InvDate);
        }
        private InvCntModel InvoiceSummaryCountsPvt(int BranchId,int CompId, DateTime InvDate)
        {
            InvCntModel InvCnts = new InvCntModel();

            try
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceSummaryCounts", "Get Invoice Summary Counts", "START", "");
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InvCnts = _contextManager.usp_GetInvoiceSummaryCounts(BranchId, CompId, InvDate).Select(c => new InvCntModel
                    {
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        TotalInv = Convert.ToInt32(c.TotalInv),
                        CancelInv = Convert.ToInt32(c.CancelInv),
                        AcceptedInv = Convert.ToInt32(c.AcceptedInv),
                        PendingForAcceptInv = Convert.ToInt32(c.PendingForAcceptInv),
                        InvoiceDrawn = Convert.ToInt32(c.InvoiceDrawn),
                        Packed = Convert.ToInt32(c.Packed),
                        ReadyToDispatch = Convert.ToInt32(c.ReadyToDispatch),
                        Concern = Convert.ToInt32(c.Concern),
                        GetpassGenerated = Convert.ToInt32(c.GetpassGenerated),
                        Dispatched = Convert.ToInt32(c.Dispatched)
                    }).FirstOrDefault();
                }
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceSummaryCounts", "Get Invoice Summary Counts", "END", "");
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceSummaryCounts", "Get Invoice Summary Counts", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvCnts;
        }
        #endregion

        #region PickList Header Delete
        public string PickListHeaderDelete(PickListModel model)
        {
            return PickListHeaderDeletePvt(model);
        }
        private string PickListHeaderDeletePvt(PickListModel model)
        {

            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_PickListHeaderDelete(model.Picklistid,obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PickListHeaderDeletePvt", "PickList Header Delete ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Invoice Header Status Update
        public List<InvSts> InvoiceStatusForMob()
        {
            return InvoiceStatusForMobPvt();
        }
        private List<InvSts> InvoiceStatusForMobPvt()
        {
            List<InvSts> InvSts = new List<InvSts>();
            try
            {
                InvSts = _contextManager.usp_InvStatusForMob().Select(s=> new InvSts
                {
                    Id = Convert.ToInt32(s.id),
                    StatusText = s.StatusText
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InvoiceStatusFroMobPvt", "Invoice Status FroMobPvt " + " " + 0 + " " + 0, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvSts;
        }
        #endregion

        #region Get Printer Details
        public List<PrinterDtls> GetPrinterDetails(int BranchId, int CompId)
        {
            return GetPrinterDetailsPvt(BranchId, CompId);
        }
        private List<PrinterDtls> GetPrinterDetailsPvt(int BranchId, int CompId)
        {
            List<PrinterDtls> printerDtls = new List<PrinterDtls>();
            try
            {
                printerDtls = _contextManager.usp_GetPrintDetails(BranchId, CompId).Select(c => new PrinterDtls
                {
                    CompanyName = c.CompanyName,
                    BranchName = c.BranchName,
                    PrinterId = c.PrinterId,
                    BranchId = c.BranchId,
                    CompId = c.CompId,
                    PrinterType = c.PrinterType,
                    PrinterIPAddress = c.PrinterIPAddress,
                    PrinterName = c.PrinterName,
                    PrinterPortNumber = c.PrinterPortNumber,
                    AddedBy = c.AddedBy,
                    LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn),
                }).OrderByDescending(x => x.PrinterType).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPrinterDetailsPvt", "Get Printer Details - BranchId: " + BranchId + "  CompanyId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return printerDtls;
        }
        #endregion

        #region Printer Log Add/Edit
        public string PrinterLogAddEdit(PrinterLogAddEditModel model)
        {
            return PrinterLogAddEditPvt(model);
        }
        private string PrinterLogAddEditPvt(PrinterLogAddEditModel model)
        {
            decimal RetValue = 0;
            try
            {
                var resultId = _contextManager.usp_PrinterLogAddEdit(model.PrinterLogID, model.PrinterLogFor, model.PrinterLogData, model.PrinterLogStatus, model.PrinterLogDatetime, model.PrinterLogException).FirstOrDefault();
                if (resultId != null)
                {
                    RetValue = resultId.Value;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PrinterLogAddEditPvt", "Printer Log Add/Edit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Printer PDF Data - Save PDF File Path
        public string PrinterPDFData(PrintPDFDataModel model)
        {
            return PrinterPDFDataPvt(model);
        }
        public string PrinterPDFDataPvt(PrintPDFDataModel model)
        {
            string msg = string.Empty;
            int RetValue = 0;
            ObjectParameter objRetValue = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_SavePrintPDFData(model.BranchId, model.CompId, model.InvId, model.GpId, model.Type, model.BoxNo, model.Flag, model.AddedBy, model.Action, objRetValue);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PrinterPDFDataPvt", "Printer PDF Data", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                msg = BusinessCont.SuccessStatus;
            }
            else if (RetValue == -1)
            {
                msg = BusinessCont.msg_exist;
            }
            else
            {
                msg = BusinessCont.FailStatus;
            }
            return msg;
        }
        #endregion

        #region Get PDF Print Data
        public List<PrintPDFDataModel> GetPrintPDFData(int BranchId, int CompId)
        {
            return GetPrintPDFDataPvt(BranchId, CompId);
        }
        private List<PrintPDFDataModel> GetPrintPDFDataPvt(int BranchId, int CompId)
        {
            List<PrintPDFDataModel> PrintDataList = new List<PrintPDFDataModel>();
            PrintPDFDataModel PrintData = null;
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPrintPDFDataPvt", "Get Print PDF Data", "START", "");

                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    var resultData = _contextManager.usp_GetPrintPDFData(BranchId, CompId).ToList();
                    if (resultData.Count > 0)
                    {
                        foreach (var c in resultData)
                        {
                            PrintData = new PrintPDFDataModel();
                            PrintData.pkId = c.pkId;
                            PrintData.BranchId = c.BranchId;
                            PrintData.CompId = c.CompId;
                            PrintData.InvId = c.InvId;
                            PrintData.GpId = c.GpId;
                            PrintData.Type = c.Type;
                            PrintData.BoxNo = c.BoxNo;
                            PrintData.Flag = c.Flag;
                            PrintData.AddedBy = c.AddedBy;
                            PrintData.LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn);
                            PrintDataList.Add(PrintData);
                        }
                    }
                    else
                    {
                        BusinessCont.SaveLog(0, 0, 0, "GetPrintPDFDataPvt", "Get Print PDF Data", "No Records", "");
                    }
                    BusinessCont.SaveLog(0, 0, 0, "GetPrintPDFDataPvt", "Get Print PDF Data", "END", "");
                    return PrintDataList;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPrintPDFDataPvt", "Get Print PDF Data", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PrintDataList;
        }
        #endregion

        #region Generate Gatepass Add Edit
        public string GenerateGatepasAddEdit(GatePassModel model)
        {
            return GenerateGatepasAddEditPvt(model);
        }
        private string GenerateGatepasAddEditPvt(GatePassModel model)
        {
            string RetValue = "";
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                var result = _contextManager.usp_GenerateGatepassAddEdit(model.GatepassId, model.BranchId, model.CompId,model.CAId, model.GatepassDate, model.VehicleNo, model.InvStr, model.GuardNameId,model.GuardNameText, model.DriverName, model.Addedby, model.Action, obj).ToList();
                var GatepassId = result[0];
                RetValue = Convert.ToString(GatepassId);

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GenerateGatepasAddEditPvt", "Generate Gatepass Add Edit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }

            return RetValue;
        }
        #endregion

        #region Gatepass Generate New No
        public string GatepassListGenerateNewNo(int BranchId,int CompId,DateTime GatepassDate)
        {
            return GatepassListGenerateNewNoPvt(BranchId, CompId, GatepassDate);
        }
        private string GatepassListGenerateNewNoPvt(int BranchId,int CompId,DateTime GatepassDate)
        {
            string GatepassNo = string.Empty;

            try
            {
                GatepassNo = _contextManager.usp_GatepassListGenerateNewNo(BranchId, CompId, GatepassDate).FirstOrDefault();
            }
            catch(Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GatepassListGenerateNewNoPvt", "Gatepass Generate New No", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return GatepassNo;
        }
        #endregion

        #region Get Gatepass Dtls For PDF
        public List<InvGatpassDtls> GetGatepassDtlsForPDF(int BranchId, int CompId, int GPid)
        {
            return GetGatepassDtlsForPDFPvt(BranchId, CompId, GPid);
        }
        private List<InvGatpassDtls> GetGatepassDtlsForPDFPvt(int BranchId,int CompId,int GPid)
        {
            List<InvGatpassDtls> invaGatepassDtls = new List<InvGatpassDtls>();

            try
            {
                invaGatepassDtls = _contextManager.usp_GetGatepassDtlsForPDF(BranchId, CompId, GPid).Select(s => new InvGatpassDtls
                {
                    GatepassId = s.GatepassId,
                    BranchId = s.BranchId,
                    CompId = s.CompId,
                    GatepassNo = s.GatepassNo,
                    GatepassDate = Convert.ToDateTime(s.GatepassDate),
                    VehicleNo = s.VehicleNo,
                    GuardNameId = Convert.ToInt32(s.GuardNameId),
                    EmpName = s.EmpName,
                    EmpNo = s.EmpNo,
                    DriverName = s.DriverName,
                    Addedby = s.Addedby,
                    AddedOn = Convert.ToDateTime(s.AddedOn),
                    UpdatedBy = s.UpdatedBy,
                    LastUpdatedOn = Convert.ToDateTime(s.LastUpdatedOn),
                    IsPrinted = Convert.ToInt32(s.IsPrinted),
                    GatepassDtlsId = s.GatepassDtlsId,
                    InvId = Convert.ToInt32(s.InvId),
                    AssignTransMId = s.AssignTransMId,
                    TransportModeId = s.TransportModeId,
                    TrasportModeText = s.TrasportModeText,
                    TransporterId = Convert.ToInt32(s.TransporterId),
                    TransporterNo = s.TransporterNo,
                    TransporterName = s.TransporterName,
                    CourierId = Convert.ToInt32(s.CourierId),
                    CourierName = s.CourierName,
                    Delivery_Remark = s.Delivery_Remark,
                    LRNo = s.LRNo,
                    InvoiceId = s.InvoiceId,
                    InvNo = s.InvNo,
                    InvCreatedDate = Convert.ToDateTime(s.InvCreatedDate),
                    IsColdStorage = s.IsColdStorage,
                    SoldTo_StokistId = s.SoldTo_StokistId,
                    StockistNo = s.StockistNo,
                    StockistName = s.StockistName,
                    MobNo = s.MobNo,
                    Emailid = s.Emailid,
                    SoldTo_City = s.SoldTo_City,
                    CityName = s.CityName,
                    InvAmount = Convert.ToDouble(s.InvAmount),
                    PONo = s.PONo,
                    PODate = Convert.ToDateTime(s.PODate),
                    NoOfItems = Convert.ToInt32(s.NoOfItems),
                    InvStatus = Convert.ToInt32(s.InvStatus),
                    NoOfBox = Convert.ToInt32(s.NoOfBox),
                    InvWeight = Convert.ToInt32(s.InvWeight),
                    IsCourier = Convert.ToInt32(s.IsCourier),
                    ReadyToDispatchBy = Convert.ToInt32(s.ReadyToDispatchBy),
                    CancelBy = Convert.ToInt32(s.CancelBy),
                    BranchCode = s.BranchCode,
                    BranchName = s.BranchName,
                    BranchAddress = s.BranchAddress,
                    BrCitycode = s.BrCitycode,
                    BrCityName = s.BrCityName,
                    brPin = s.brPin,
                    brContactNo = s.brContactNo,
                    brEmail = s.brEmail,
                    CompanyCode = s.CompanyCode,
                    CompanyName = s.CompanyName,
                    CompanyEmail = s.CompanyEmail,
                    CompContactNo = s.CompContactNo,
                    CompanyAddress = s.CompanyAddress,
                    CompanyCityCode = s.CompanyCityCode,
                    CompanyCityName = s.CompanyCityName,
                    CompanyPin = s.CompanyPin,
                    CAId = s.CAId,
                    CAName = s.CAName,
                    GuardNameText = s.GuardNameText
                }).OrderBy(i => i.InvoiceId).OrderByDescending(g => g.GatepassId).ToList();
            }
            catch(Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGatepassDtlsForPDFPvt", "Get Gatepass Dtls For PDF", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return invaGatepassDtls;
        }
        #endregion

        #region Get Gatepass Dtls For Mobile
        public List<GatepassDtls> GetGatepassDtlsForMobile(int BranchId, int CompId)
        {
            return GetGatepassDtlsForMobilePvt(BranchId, CompId);
        }
        private List<GatepassDtls> GetGatepassDtlsForMobilePvt(int BranchId, int CompId)
        {
            List<GatepassDtls> invaGatepassDtls = new List<GatepassDtls>();

            try
            {
                invaGatepassDtls = _contextManager.usp_GetGatepassDtlsForMob(BranchId, CompId).Select(s => new GatepassDtls
                {
                    GatepassId = s.GatepassId,
                    BranchId = s.BranchId,
                    CompId = s.CompId,
                    GatepassNo = s.GatepassNo,
                    GatepassDate = Convert.ToDateTime(s.GatepassDate),
                    VehicleNo = s.VehicleNo,
                    GuardNameId = Convert.ToInt32(s.GuardNameId),
                    EmpName = s.EmpName,
                    EmpNo = s.EmpNo,
                    DriverName = s.DriverName,
                    Addedby = s.Addedby,
                    AddedOn = Convert.ToDateTime(s.AddedOn),
                    UpdatedBy = s.UpdatedBy,
                    LastUpdatedOn = Convert.ToDateTime(s.LastUpdatedOn),
                    IsPrinted = Convert.ToInt32(s.IsPrinted),
                    NoOfInv = Convert.ToInt32(s.NoOfInv),
                    InvIdStr= s.InvIdStr,
                    CAId = s.CAId,
                    CAName = s.CAName,
                    GuardNameText = s.GuardNameText
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGatepassDtlsForMobilePvt", "Get Gatepass Dtls For Mobile", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return invaGatepassDtls;
        }
        #endregion

        #region Invoice Details For Mobile
        public List<InvDtlsForMob> GetInvoiceDtlsForMobile(int BranchId, int CompId, int InvStatus)
        {
            return GetInvoiceDtlsForMobilePvt(BranchId, CompId, InvStatus);
        }
        private List<InvDtlsForMob> GetInvoiceDtlsForMobilePvt(int BranchId, int CompId,int InvStatus)
        {
            List<InvDtlsForMob> invaGatepassDtls = new List<InvDtlsForMob>();

            try
            {
                invaGatepassDtls = _contextManager.usp_GetInvoiceListForMob(BranchId, CompId, InvStatus).Select(s => new InvDtlsForMob
                {
                    InvId = s.InvId,
                    BranchId = s.BranchId,
                    CompId = s.CompId,
                    InvNo = s.InvNo,
                    InvCreatedDate = Convert.ToDateTime(s.InvCreatedDate),
                    StockistNo = s.StockistNo,
                    StockistName = s.StockistName,
                    MobNo = s.MobNo,
                    StockistAddress = s.StockistAddress,
                    CityCode = s.CityCode,
                    CityName = s.CityName,
                    PersonName = s.PersonName,
                    PersonAddress = s.PersonAddress,
                    PersonMobNo = s.PersonMobNo,
                    OtherDetails = s.OtherDetails,
                    TransporterId = Convert.ToInt32(s.TransporterId),
                    TransporterNo = s.TransporterNo,
                    TransporterName = s.TransporterName,
                    CourierId = Convert.ToInt32(s.CourierId),
                    CourierName = s.CourierName,
                    Delivery_Remark = s.Delivery_Remark,
                    SoldTo_StokistId = s.SoldTo_StokistId,
                    InvAmount = Convert.ToDouble(s.InvAmount),
                    NoOfItems = Convert.ToInt32(s.NoOfItems),
                    InvStatus = Convert.ToInt32(s.InvStatus),
                    NoOfBox = Convert.ToInt32(s.NoOfBox),
                    InvWeight = Convert.ToDecimal(s.InvWeight),
                    IsCourier = Convert.ToInt32(s.IsCourier),
                    OnPriority = Convert.ToInt32(s.OnPriority),
                    TransportModeId = Convert.ToInt32(s.TransportModeId),
                    TransportModeText = s.TransportModeText,
                    AttachedInvId = Convert.ToInt32(s.AttachedInvId),
                    BrCityCode = s.BrCityCode
                }).OrderByDescending(x => x.InvId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceDtlsForMobilePvt", "Get Invoice Dtls For Mobile", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return invaGatepassDtls;
        }
        #endregion

        #region Gatepass Dtls For DeleteBy Id
        public string GatepassDtlsForDeleteById(int GatepassId)
        {
            return GatepassDtlsForDeleteByIdPvt(GatepassId);
        }
        private string GatepassDtlsForDeleteByIdPvt(int GatepassId)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_GatepassDtlsForDeleteById(GatepassId, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GatepassDtlsForDeleteById", "Gatepass Dtls For DeleteBy Id:  " + GatepassId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Send Email To Stockies For Dispatch Done
        public string sendEmailForDispatchDone(string Emailid, string StockistName, string PONo, DateTime PODate, string TransporterName,string CompanyName, int BranchId,int CompId)
        {
            bool bResult = false;
            string msg = string.Empty, messageformat = string.Empty, Date = string.Empty, Subject = string.Empty, MailFilePath = string.Empty, ToEmail = string.Empty,CCEmail = string.Empty, EmailCC= string.Empty;
            EmailSend Email = new EmailSend();
            List<CCEmailDtls> EmailCntDtls = new List<CCEmailDtls>();
            try
            {
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                var date = PODate.Date.ToString("dd-MM-yyyy");
                ToEmail = Emailid;
                Subject = ConfigurationManager.AppSettings["DispatchSubj"] + Date + " ";
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\Dispatch_Done.html");
                ChequeAccountingRepository _ChequeAccountingRepository = new ChequeAccountingRepository(_contextManager);
                EmailCntDtls = _ChequeAccountingRepository.GetCCEmailDtlsPvt(BranchId, CompId, 5);
                if (EmailCntDtls.Count > 0)
                {
                    for (int i = 0; i < EmailCntDtls.Count; i++)
                    {
                        CCEmail += ";" + EmailCntDtls[i].Email;
                    }

                    EmailCC = CCEmail.TrimStart(';');
                    bResult = EmailNotification.SendEmailForDispatchDone(ToEmail, EmailCC, Subject, StockistName, PONo, date, TransporterName, CompanyName,MailFilePath);
                }
                   
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "sendEmailForDispatchDone", "Dispatch Done Email", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get Stockiest Invoice Details For Email
        public List<InvDtlsForEmail> GetInvDtlsForEmail(int BranchId, int CompId,int GatepassId)
        {
            return GetInvDtlsForEmailPvt(BranchId, CompId, GatepassId);
        }
        private List<InvDtlsForEmail> GetInvDtlsForEmailPvt(int BranchId, int CompId, int GatepassId)
        {
            List<InvDtlsForEmail> invaGatepassDtls = new List<InvDtlsForEmail>();

            try
            {
                invaGatepassDtls = _contextManager.usp_GetInvDtlsForEmail(BranchId, CompId, GatepassId).Select(s => new InvDtlsForEmail
                {
                    PONo = s.PONo,
                    PODate = Convert.ToDateTime(s.PODate),
                    TransporterName =s.TransporterName,
                    LRNo = s.LRNo,
                    StockistName= s.StockistName,
                    Emailid = s.Emailid,
                    CompanyCode = s.CompanyCode,
                    CompanyName = s.CompanyName
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvDtlsForEmailPvt", "Get Inv Dtls For Email", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return invaGatepassDtls;
        }
        #endregion

        #region Print Details Add
        public string PrintDetailsAdd(PrinterDtls model)
        {
            return PrintDetailsAddPvt(model);
        }
        private string PrintDetailsAddPvt(PrinterDtls model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                using(CFADBEntities contextManager = new CFADBEntities())
                {
                    RetValue = contextManager.usp_PrintDetailsAdd(model.PrinterId, model.BranchId, model.CompId, model.PrinterType, model.PrinterIPAddress, model.PrinterPortNumber, model.Action, obj);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PrintDetailsAdd", "Print Details Add:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else if (RetValue == -1)
            {
                return BusinessCont.msg_exist;
            }
            else
            {
                return BusinessCont.FailStatus;
            }
        }
        #endregion

        #region Get Picklist By Picker Status
        public List<PickListModel> GetPicklistByPickerStatus(int BranchId, int CompId, DateTime PicklistDate)
        {
            return GetPicklistByPickerStatusPvt(BranchId, CompId, PicklistDate);
        }
        private List<PickListModel> GetPicklistByPickerStatusPvt(int BranchId, int CompId, DateTime PicklistDate)
        {
            List<PickListModel> PckList = new List<PickListModel>();
            List<PickListDetailsByPicker> PickListDetailsByPicker = new List<PickListDetailsByPicker>();

            try
            {
                PckList = _contextManager.usp_GetPickList(BranchId, CompId, PicklistDate).Select(c => new PickListModel
                {
                    BranchId = c.BranchId,
                    CompId = c.CompId,
                    Picklistid = c.Picklistid,
                    PicklistNo = c.PicklistNo,
                    PicklistDate = DateTime.Parse(c.PicklistDate.ToString()),
                    FromInv = c.FromInv,
                    ToInv = c.ToInv,
                    PicklistStatus = Convert.ToInt32(c.PicklistStatus),
                    VerifiedBy = Convert.ToInt32(c.VerifiedBy),
                    StatusText = c.StatusText,
                    OnPriority = c.OnPriority
                }).OrderByDescending(x => x.OnPriority).ToList();

                for (int i = 0; i < PckList.Count(); i++)
                {
                    PickListDetailsByPicker = _contextManager.usp_GetAllotmentDetailsPicklistWise(BranchId, CompId, PckList[i].Picklistid)
                        .Select(p => new PickListDetailsByPicker
                        {
                            Picklistid = p.Picklistid,
                            PickerId = p.PickerId,
                            PickerName = p.PickerName,
                            AllotmentId = p.AllotmentId,
                            AllotmentStatus = p.AllotmentStatus,
                            AllotmentStatusText = p.AllotmentStatusText,
                            ReasonId = Convert.ToInt32(p.ReasonId),
                            ReasonText = p.ReasonText,
                            RejectRemark = p.RejectRemark,
                            PickerConcernId = Convert.ToInt32(p.PickerConcernId),
                            pickerconcernText = p.pickerconcernText,
                            PickerConcernRemark = p.PickerConcernRemark,
                            VerifiedBy = Convert.ToInt32(p.VerifiedBy),
                            VerifiedConcernId = Convert.ToInt32(p.VerifiedConcernId),
                            VerifiedConcernText = p.VerifiedConcernText,
                            VerifiedConcernRemark = p.VerifiedConcernRemark,
                            AcceptedDate = Convert.ToString(p.AcceptedDate),
                            PickedDate = Convert.ToString(p.PickedDate),
                            PickerConcernDate = Convert.ToString(p.PickerConcernDate),
                            VerifiedDate = Convert.ToString(p.VerifiedDate)
                        }).ToList();
                    PckList[i].PickListByPicker = PickListDetailsByPicker;
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPicklistByPickerStatusPvt", "GetPicklistByPickerStatusPvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PckList;
        }
        #endregion

        #region Priority Invoice Flag Update
        public string PriorityInvoiceFlagUpdate(PriorityFlagUpdtModel model)
        {
            return PriorityInvoiceFlagUpdatePvt(model);
        }
        private string PriorityInvoiceFlagUpdatePvt(PriorityFlagUpdtModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetVal", typeof(int));
            try
            {
                using (CFADBEntities contextManager = new CFADBEntities())
                {
                    RetValue = contextManager.usp_InvoiceHeaderStatusPriority(model.InvId,model.PriorityFlag,model.Remark,model.Addedby,model.updateDate,obj);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PriorityInvoiceFlagUpdatePvt", "Priority Invoice Flag Update: " + model.InvId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Picklist Resolve Concern Add
        public string ResolveConcernAdd(PickListModel model)
        {
            return ResolveConcernAddPvt(model);
        }
        private string ResolveConcernAddPvt(PickListModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            _loginRepository = new LoginRepository(_contextManager);
            try
            {
                RetValue = _contextManager.usp_PicklistResolveConcern(model.AllotmentId, model.Picklistid, model.BranchId, model.CurrentStatus,
                    model.ResolveRemark, Convert.ToDateTime(model.StatusTime), model.UpdatedBy, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ResolveConcernAddPvt", "Resolve Concern Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                _loginRepository.SendNotification(model.BranchId, model.PickerId, BusinessCont.ResolveConcernHeader, BusinessCont.PickListResolveConcernMsg);
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.FailStatus;
            }
}
        #endregion

        #region Get Resolve Concern List
        public List<PickListModel> ResolveConcernLst(int BranchId, int CompId, DateTime PicklistDate)
        {
            return ResolveConcernLstPvt(BranchId, CompId, PicklistDate);
        }
        private List<PickListModel> ResolveConcernLstPvt(int BranchId, int CompId, DateTime PicklistDate)
        {
            List<PickListModel> resolveConcrn = new List<PickListModel>();

            try
            {
                resolveConcrn = _contextManager.usp_GetPicklistForResolveConcern(BranchId, CompId, PicklistDate).Select(c => new PickListModel
                {
                    Picklistid = c.Picklistid,
                    PicklistNo = c.PicklistNo,
                    PicklistDate = DateTime.Parse(c.PicklistDate.ToString()),
                    FromInv = c.FromInv,
                    ToInv = c.ToInv,
                    PicklistStatus = Convert.ToInt32(c.PicklistStatus),
                    PicklistStatusText = c.PicklistStatusText,
                    PickerId = c.PickerId,
                    PickerName = c.PickerName,
                    AllotmentId = c.AllotmentId,
                    AllotmentStatus = Convert.ToString(c.AllotmentStatus),
                    AllotmentStatusText = Convert.ToString(c.AllotmentStatusText),
                    ReasonId = Convert.ToInt32(c.ReasonId),
                    ReasonText = c.ReasonText,
                    pickerconcernText = c.pickerconcernText,
                    RejectRemark = c.RejectRemark,
                    PickerConcernId = Convert.ToInt32(c.PickerConcernId),
                    PickerConcernRemark = c.PickerConcernRemark,
                    VerifiedBy = Convert.ToInt32(c.VerifiedBy),
                    VerifiedConcernId = Convert.ToInt32(c.VerifiedConcernId),
                    VerifiedConcernText = c.VerifiedConcernText,
                    VerifiedConcernRemark = c.VerifiedConcernRemark,
                    AcceptedDate = Convert.ToDateTime(c.AcceptedDate),
                    PickedDate = Convert.ToDateTime(c.PickedDate),
                    PickerConcernDate = Convert.ToDateTime(c.PickerConcernDate),
                    VerifiedDate = Convert.ToDateTime(c.VerifiedDate),
                    OnPriority = c.OnPriority
                }).OrderByDescending(x => x.OnPriority).ToList();

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ResolveConcernLstPvt", "Resolve ConcernLst Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return resolveConcrn;
        }
        #endregion

        #region Invoice Resolve Concern Add
        public string ResolveInvConcernAdd(InvoiceLstModel model)
        {
            return ResolveInvConcernAddPvt(model);
        }
        private string ResolveInvConcernAddPvt(InvoiceLstModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetVal", typeof(int));
            _loginRepository = new LoginRepository(_contextManager);
            try
            {
                RetValue = _contextManager.usp_InvoiceHeaderResolveConcern(Convert.ToInt32(model.InvId), model.CurrentStatus, model.Remark, model.Addedby, model.updateDate, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ResolveConcernAddPvt", "Resolve Concern Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                _loginRepository.SendNotification(model.BranchId, model.AcceptedBy, BusinessCont.ResolveConcernHeader, BusinessCont.InvoiceResolveConcernMsg);
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.FailStatus;
            }
        }
        #endregion

        #region Get Invoice Header List for Resolve Convern
        public List<InvoiceLstModel> GetInvoiceHeaderLstResolveCnrn(int BranchId, int CompId, int BillDrawerId)
        {
            return GetInvoiceHeaderLstResolveCnrnPvt(BranchId, CompId, BillDrawerId);
        }
        private List<InvoiceLstModel> GetInvoiceHeaderLstResolveCnrnPvt(int BranchId, int CompId, int BillDrawerId)
        {
            List<InvoiceLstModel> InvoiceLst = new List<InvoiceLstModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InvoiceLst = _contextManager.usp_GetInvoiceHeaderResolveConcern(BranchId, CompId, BillDrawerId).Select(c => new InvoiceLstModel
                    {
                        InvId = c.InvId,
                        BranchId = c.BranchId,
                        CompId = c.CompId,
                        InvNo = c.InvNo,
                        InvPostingDate = Convert.ToDateTime(c.InvPostingDate),
                        InvCreatedDate = Convert.ToDateTime(c.InvCreatedDate),
                        IsColdStorage = c.IsColdStorage,
                        SoldTo_StokistId = c.SoldTo_StokistId,
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        CityName = c.CityName,
                        InvAmount = Convert.ToDouble(c.InvAmount),
                        PONo = c.PONo,
                        PODate = Convert.ToDateTime(c.PODate),
                        InvStatus = Convert.ToInt32(c.InvStatus),
                        StatusText = c.StatusText,
                        NoOfItems = Convert.ToInt32(c.NoOfItems),
                        CityCode = c.CityCode,
                        AcceptedBy = Convert.ToInt32(c.AcceptedBy),
                        BillDrawerName = c.BillDrawerName,
                        PackedBy = Convert.ToInt32(c.PackedBy),
                        PackedByName = c.PackedByName,
                        NoOfBox = Convert.ToInt32(c.NoOfBox),
                        InvWeight = Convert.ToInt32(c.InvWeight),
                        IsCourier = Convert.ToInt32(c.IsCourier),
                        PackingConcernDate = Convert.ToDateTime(c.PackingConcernDate),
                        PackingConcernId = Convert.ToInt32(c.PackingConcernId),
                        PackingConcernText = c.PackingConcernText,
                        PackingRemark = c.PackingRemark,
                        ReadyToDispatchBy = Convert.ToInt32(c.ReadyToDispatchBy),
                        ReadyToDispatchDate = Convert.ToDateTime(c.ReadyToDispatchDate),
                        ReadyToDispatchConcernId = Convert.ToInt32(c.ReadyToDispatchConcernId),
                        ReadyToDispatchRemark = c.ReadyToDispatchRemark,
                        DispatchByName=c.DispatchByName,
                        DispatchConcernText=c.DispatchConcernText,
                        OnPriority=Convert.ToInt32(c.OnPriority)
                    }).OrderBy(x => Convert.ToInt64(x.InvNo)).OrderByDescending(x => x.OnPriority).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceHeaderLstResolveCnrnPvt", "Get Invoice Header List for Resolve Convern " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvoiceLst;
        }
        #endregion

        #region Get Assigned Transporter List
        public List<AssignedTransportModel> GetAssignedTransporterList(int BranchId, int CompId)
        {
            return GetAssignedTransporterListPvt(BranchId, CompId);
        }
        private List<AssignedTransportModel> GetAssignedTransporterListPvt(int BranchId, int CompId)
        {
            List<AssignedTransportModel> AssignTransportLst = new List<AssignedTransportModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    AssignTransportLst = _contextManager.usp_GetAssignedTransporterList(BranchId, CompId).Select(c => new AssignedTransportModel
                    {
                        AssignTransMId = c.AssignTransMId,
                        InvoiceId = Convert.ToString(c.InvId),
                        InvNo = c.InvNo,
                        InvCreatedDate = Convert.ToDateTime(c.InvCreatedDate),
                        StockistNo = c.StockistNo,
                        StockistName = c.StockistName,
                        SoldTo_City = c.SoldTo_City,
                        CityName = c.CityName,
                        TransportModeId = c.TransportModeId,
                        PersonName = c.PersonName,
                        PersonAddress = c.PersonAddress,
                        PersonMobileNo = c.PersonMobNo,
                        OtherDetails = c.OtherDetails,
                        TransporterName = c.TransporterName,
                        TransporterId = Convert.ToInt32(c.TransporterId),
                        CourierName = c.CourierName,
                        CourierId = Convert.ToInt32(c.CourierId),
                        Delivery_Remark = c.Delivery_Remark,
                        LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn),
                        OnPriority = Convert.ToInt32(c.OnPriority)
                    }).OrderBy(x => Convert.ToInt64(x.InvNo)).OrderByDescending(x => x.OnPriority).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetAssignedTransporterListPvt", "Get Assigned Transporter List ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return AssignTransportLst;
        }
        #endregion

        #region Edit Assigned Transport Mode
        public string EditAssignedTransportMode(AssignedTransportModel model)
        {
            return EditAssignedTransportModePvt(model);
        }
        private string EditAssignedTransportModePvt(AssignedTransportModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_AssignTransportModeEdit(model.AssignTransMId, model.InvoiceId, model.TransportModeId, model.PersonName, model.PersonAddress, model.PersonMobileNo, model.OtherDetails, model.TransporterId, model.Delivery_Remark, model.CourierId, model.Addedby, obj);

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "EditAssignedTransportModePvt", "Edit Assigned Transport Mode", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

    }
}
