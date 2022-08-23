using CNF.Business.BusinessConstant;
using CNF.Business.Model.Context;
using CNF.Business.Model.InventoryInward;
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

namespace CNF.Business.Repositories
{
    public class InventoryInwardRepository: IInventoryInwardRepository
    {
        private CFADBEntities _contextManager;

        //Inject DB Context instance from Unit of Work to avoid unwanted memory allocations.
        public InventoryInwardRepository(CFADBEntities contextManager)
        {
            _contextManager = contextManager;
        }

        #region InsuranceClaim AddEdit AddEdit
        public int InsuranceClaimAddEdit(InsuranceClaimModel model)
        {
            return InsuranceClaimAddEditPvt(model);
        }
        private int InsuranceClaimAddEditPvt(InsuranceClaimModel model)
        {

            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_InsuranceClaimAddEdit(model.ClaimId,model.BranchId, model.CompId,model.InvoiceId, 
                    model.ClaimNo, model.ClaimDate,model.ClaimAmount,model.ClaimType, model.DebitNote,model.DebitDate,
                    model.DebitAmount,model.Remark, model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InsuranceClaimAddEditPvt", "InsuranceClaim AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Insurance Claim List
        public List<InsuranceClaimModel> GetInsuranceClaimList(int BranchId , int CompId)
        {
            return GetGetInsuranceClaimListPvt(BranchId,CompId);
        }
        private List<InsuranceClaimModel> GetGetInsuranceClaimListPvt(int BranchId, int CompId)
        {
            List<InsuranceClaimModel> modelList = new List<InsuranceClaimModel>();
            try
            {
                modelList = _contextManager.usp_GetInsuranceClaimList(BranchId, CompId).Select(i => new InsuranceClaimModel
                {
                    BranchId = i.BranchId,
                    ClaimId = i.ClaimId,
                    InvoiceId = i.InvoiceId,
                    InvNo = i.InvNo,
                    ClaimNo = i.ClaimNo,
                    ClaimDate = Convert.ToDateTime(i.ClaimDate),
                    ClaimAmount = i.ClaimAmount,
                    DebitAmount = i.DebitAmount,
                    ClaimType = i.ClaimType,   
                    ClaimTypeId = Convert.ToInt64(i.ClaimTypeId),
                    DebitDate = Convert.ToDateTime(i.DebitDate),
                    DebitNote = i.DebitNote,
                    ClaimStatus = i.ClaimStatus,
                    Remark =i.Remark,                    
                    AddedBy = Convert.ToInt32(i.AddedBy),
                    AddedOn = Convert.ToDateTime(i.AddedOn),
                    LastUpdatedDate = Convert.ToDateTime(i.LastUpdatedDate),
                    IsEmail= Convert.ToBoolean(i.IsEmail)
                }).OrderByDescending(x => x.ClaimId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGetInsuranceClaimListPvt", "Get Insurance Claim List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get Invoice List
        public List<InvoiceListModel> GetInvoiceList(int BranchId,int CompId)
        {
            return GetInvoiceListPvt(BranchId,CompId);
        }
        private List<InvoiceListModel> GetInvoiceListPvt(int BranchId, int CompId)
        {
            List<InvoiceListModel> modelList = new List<InvoiceListModel>();
            try
            {
                modelList = _contextManager.usp_GetInsuranceClaimInvoiceList(BranchId, CompId).Select(b => new InvoiceListModel
                {
                    BranchId = b.BranchId,
                    CompId = b.CompId,
                    InvId = b.InvId,
                    InvNo = b.InvNo
                }).OrderByDescending(x => x.InvNo).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceListPvt", "Get Invoice List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get Claim Types List
        public List<InsuranceClaimModel> GetClaimTypeList(int BranchId, int CompanyId)
        {
            return GetClaimTypeListPvt(BranchId, CompanyId);
        }
        private List<InsuranceClaimModel> GetClaimTypeListPvt(int BranchId, int CompanyId)
        {
            List<InsuranceClaimModel> modelList = new List<InsuranceClaimModel>();
            try
            {
                modelList = _contextManager.uspGetInsuranceClaimTypeList(BranchId, CompanyId).Select(b => new InsuranceClaimModel
                {
                  ClaimTypeId=b.ClaimTypeId,
                  ClaimType=b.ClaimType
                }).OrderByDescending(x => x.ClaimTypeId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetClaimTypeListPvt", "Get Claim Type List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get Insurance Claim Invoice By Id List
        public List<InsuranceClaimModel> GetInsuranceClaimInvByIdList(int BranchId, int CompanyId,string InvoiceId)
        {
            return GetInsuranceClaimInvByIdListPvt(BranchId, CompanyId, InvoiceId);
        }
        private List<InsuranceClaimModel> GetInsuranceClaimInvByIdListPvt(int BranchId, int CompanyId,string InvoiceId)
        {
            List<InsuranceClaimModel> modelList = new List<InsuranceClaimModel>();
            try
            {
                modelList = _contextManager.usp_GetInsuranceClaimInvById(BranchId, CompanyId, InvoiceId).Select(b => new InsuranceClaimModel
                {
                   BranchId = b.BranchId,
                   CompId=b.CompId,
                   InvoiceId=b.InvoiceId,
                   ClaimId=b.ClaimId,
                   ClaimNo=b.ClaimNo,
                   ClaimDate=Convert.ToDateTime(b.ClaimDate),
                   ClaimAmount=b.ClaimAmount,
                }).OrderByDescending(x => x.InvoiceId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetClaimTypeListPvt", "Get Claim Type List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Send Email For approval Update Alert
        public List<InsuranceClaimModel> GetInsuranceClmDtlsForEmail(int BranchId, int CompId, int ClaimId)
        {
            return GetInsuranceClmDtlsForEmailPvt(BranchId, CompId, ClaimId);
        }
        private List<InsuranceClaimModel> GetInsuranceClmDtlsForEmailPvt(int BranchId, int CompId, int ClaimId)
        {
            List<InsuranceClaimModel> EmailDtls = new List<InsuranceClaimModel>();
            try
            {
                EmailDtls = _contextManager.usp_GetInsuranceClaimByIdApprovalEmail(BranchId, CompId, ClaimId).Select(i => new InsuranceClaimModel
                {
                    BranchId = i.BranchId,
                    CompId=i.CompId,
                    ClaimId = i.ClaimId,
                    InvoiceId = i.InvoiceId,
                    InvNo = i.InvNo,
                    ClaimNo = i.ClaimNo,
                    ClaimDate = Convert.ToDateTime(i.ClaimDate),
                    ClaimAmount = i.ClaimAmount,
                    DebitAmount = i.DebitAmount,
                    ClaimType = i.ClaimType,
                    DebitDate = Convert.ToDateTime(i.DebitDate),
                    DebitNote = i.DebitNote,
                    ClaimStatus = i.ClaimStatus,
                    Remark = i.Remark,
                    EmailId=i.EmailId,
                    AddedBy = Convert.ToInt32(i.AddedBy),
                    AddedOn = Convert.ToDateTime(i.AddedOn),
                    LastUpdatedDate = Convert.ToDateTime(i.LastUpdatedDate),
                    IsEmail= Convert.ToBoolean(i.IsEmail)
                }).OrderByDescending(x => x.InvoiceId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInsuranceClmDtlsForEmailPvt", "Get Insurance Clm Dtls For Email Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return EmailDtls;
        }
        #endregion

        #region Get Insurance Claim Approval Report Or mail Table format     
        public string InsuranceClaimforApproval(List<InsuranceClaimModel> modelList, string MailFilePath)
        {
            string Table = string.Empty, TableList = string.Empty, msgHtml = string.Empty;
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "InsuranceClaimforApproval", "Get Insurance Claim Approval Report ", "START", "");
                msgHtml = File.OpenText(MailFilePath).ReadToEnd().ToString();
                Table = "";
                TableList = "";
                Table += "<table style='border-collapse: collapse;width: 52%; min-width: 400px;; white-space:nowrap;'>";
                Table += "<thead><tr style = 'font-family: Verdana; font-size: 12px; font-weight: bold; background-color:#3c8dbc; color:white;'>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;'  width='10%' border='1'> Claim No. </th>";
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;'  width='15%' border='1'> Claim Date </th>";               
                Table += "<th style='border: 1px solid black;padding: 2px;text-align:center;'  width='10%' border='1'> Claim Amount </th>";
                Table += "</tr></thead><tbody>";
                TableList += Table;

                foreach (var item in modelList)
                {
                    TableList += "<tr style='font-size:13px;text-align:center;color:black;'>";
                    TableList += "<td style='border: 1px solid black;padding: 3px;' border='1'>" + item.ClaimNo + "</td>";
                    TableList += "<td style='border: 1px solid black;padding: 10px;' width='10%' border='1'>" + item.ClaimDate +
                             "</td>";
                    TableList += "<td style='border: 1px solid black;padding: 3px;' border='1'>" + item.ClaimAmount + "</td>";
                    TableList += "</tr>";
                }
                TableList += "</tbody></table></br></br>"; 

                if (TableList != "" && TableList != null)
                {
                    msgHtml = msgHtml.Replace("<!--ApprovalTableString-->", TableList);
                }
                BusinessCont.SaveLog(0, 0, 0, "InsuranceClaimforApproval", "Get Insurance Claim Approval Report", "END", "");
            }
            catch (Exception ex)
            { 
                BusinessCont.SaveLog(0, 0, 0, "InsuranceClaimforApproval", "Get Insurance Claim Approval Report", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msgHtml;
        }
        #endregion

        #region Update Email Insurance Claim Approval Update
        public string UpdateMailForApproval(int BranchId, int CompId, long ClaimId, bool IsEmailSend)
        {
            return UpdateMailForApprovalPvt(BranchId, CompId, ClaimId, IsEmailSend);
        }
        private string UpdateMailForApprovalPvt(int BranchId, int CompId, long ClaimId, bool IsEmailSend)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_UpdateInsuranceClaimByIdApprovalEmail(BranchId, CompId, ClaimId, IsEmailSend, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "UpdateMailForApprovalPvt", "Update Mail For Approval Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Add/Edit Map Inward Vehicle for Mobile
        public int MapInwardVehicleAddEditForMob(MapInwardVehicleForMobModel model)
        {
            return MapInwardVehicleAddEditForMobPvt(model);
        }
        private int MapInwardVehicleAddEditForMobPvt(MapInwardVehicleForMobModel model)
        {

            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_MapInwardVehicleAddEditForMob(model.BranchId, model.CompId,model.InvId,model.InvoiceDate,model.InwardDate ,
                    model.TransporterId,model.MobileNo,model.DriverName,model.VehicleNo,model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "MapInwardVehicleAddEditForMobPvt", "MapInward Vehicle AddEdit For Mob Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Maping Inward Vehicle List For Mob
        public List<MapInwardVehicleForMobModel> GetMapInwardVehicleListForMobs(int BranchId, int CompId)
        {
            return GetMapInwardVehicleListPvt(BranchId, CompId);
        }
        private List<MapInwardVehicleForMobModel> GetMapInwardVehicleListPvt(int BranchId, int CompId)
        {
            List<MapInwardVehicleForMobModel> modelList = new List<MapInwardVehicleForMobModel>();
            try
            {
                modelList = _contextManager.usp_GetMapInwardVehicleListForMob(BranchId, CompId).Select(i => new MapInwardVehicleForMobModel
                {
                    BranchId = i.BranchId,
                    CompId = i.CompId,
                    InvId = i.InvId,
                    InvoiceDate = Convert.ToDateTime(i.InvoiceDate),
                    InwardDate = Convert.ToDateTime(i.InwardDate),
                    TransporterId = Convert.ToInt32(i.TransporterId),
                    MobileNo = i.MobileNo,
                    DriverName = i.DriverName,
                    VehicleNo = i.VehicleNo,
                    InvNo=i.InvNo,
                    TransporterNo=i.TransporterNo,
                    TransporterName=i.TransporterName,
                    Addedby = i.Addedby,
                    AddedOn = Convert.ToDateTime(i.AddedOn),
                    LastUpdatedOn = Convert.ToDateTime(i.LastUpdatedOn)
                }).OrderByDescending(x => x.InvId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetMapInwardVehicleListPvt", "Get Map InwardVehicle List Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Inv In Vehicle Checklist Add Edit For Mob
        public int VehicleCheckListAddEdits(VehicleChecklistModel model)
        {
            return VehicleCheckListAddEditsPvt(model);
        }
        private int VehicleCheckListAddEditsPvt(VehicleChecklistModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_InvInVehicleChecklistAddEditForMob(model.BranchId, model.CompId,model.ChecklistTypeId,
                    model.ChecklistType,model.InvId,model.InvoiceDate,model.TransporterId,model.VehicleNo,model.IsColdStorage,
                    model.Remarks,model.SealNumber,model.Comments,model.Action,model.AddedBy, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "VehicleCheckListAddEditsPvt", "Vehicle CheckList AddEdits Pvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Invoice In Vehicle Check List For Mob
        public List<InvInVehicleCheckListmodel> GetInvInVehicleCheckList(int BranchId, int CompId)
        {
            return GetInvInVehicleCheckListPvt(BranchId, CompId);
        }
        private List<InvInVehicleCheckListmodel> GetInvInVehicleCheckListPvt(int BranchId, int CompId)
        {
            List<InvInVehicleCheckListmodel> InvInVehicleCheckLst = new List<InvInVehicleCheckListmodel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InvInVehicleCheckLst = _contextManager.usp_GetInvInVehicleCheckListForMob(BranchId, CompId) 
                       .Select(c => new InvInVehicleCheckListmodel
                       {
                           pkId=c.pkId,
                           BranchId = Convert.ToInt32(c.BranchId),
                           CompId = Convert.ToInt32(c.CompId),
                           ChecklistTypeId = Convert.ToInt32(c.ChecklistTypeId),//
                           ChecklistType = c.ChecklistType,
                           InvId = c.InvId,
                           InvNo = c.InvNo,
                           InvoiceDate = c.InvoiceDate,
                           TransporterId = c.TransporterId,
                           TransporterNo = c.TransporterNo,
                           TransporterName = c.TransporterName,
                           VehicleNo = c.VehicleNo,
                           IsColdStorage = c.IsColdStorage,
                           Status = c.Status,
                           Remarks = c.Remarks,
                           SealNumber = c.SealNumber,
                           Comments = c.Comments,
                           AddedBy = c.AddedBy,
                           AddedOn = Convert.ToDateTime(c.AddedOn),
                           LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn),
                           IsApprove=c.IsApprove
                       }).OrderBy(x => x.VehicleNo).ToList();

                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvInVehicleCheckListPvt", " Get Invoice In Vehicle Check List" + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvInVehicleCheckLst;
        }
        #endregion

        #region Update InvInVerify Approve Vehicle Issue
        public int UpdateVerifyApproveVehicleIssues(VerifyCheckListApproveModel model)
        {
            return VerifyApproveVehicleIssueEditPvt(model);
        }
        private int VerifyApproveVehicleIssueEditPvt(VerifyCheckListApproveModel model)
        {

            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_UpdateInvInVerifyApproveVehicleIssue(model.pkId, model.BranchId, model.CompId,
                    model.IsApprove, model.IsApproveBy, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "VerifyApproveVehicleIssueEditPvt", "Verify Approve Vehicle Issue EditPvt", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion


        #region Get Transit Data List
        public List<ImportTransitListModel> GetTransitDataLst(int BranchId, int CompId)
        {
            return GetTransitDataLstPvt(BranchId, CompId);
        }
        private List<ImportTransitListModel> GetTransitDataLstPvt(int BranchId, int CompId)
        {
            List<ImportTransitListModel> TransitDataLst = new List<ImportTransitListModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    TransitDataLst = _contextManager.usp_GetTransitDataList(BranchId, CompId) 
                        .Select(c => new ImportTransitListModel
                        {

                            BranchId = c.BranchId,
                            CompId = c.CompId,
                            DeliveryNo = c.DeliveryNo,
                            ActualGIDate = Convert.ToDateTime(c.ActualGIDate).Date.ToString("dd-MM-yyyy"),
                            RecPlant = c.RecPlant,
                            RecPlantDesc = c.RecPlantDesc,
                            DispPlant = c.DispPlant,
                            DispPlantDesc = c.DispPlantDesc,
                            InvoiceNo = c.InvNo,
                            InvoiceDate = Convert.ToDateTime(c.InvoiceDate).Date.ToString("dd-MM-yyyy"),
                            MaterialNo = c.MaterialNo,
                            MatDesc = c.MatDesc,
                            UoM = c.UoM,
                            BatchNo = c.BatchNo,
                            Quantity = c.Quantity,
                            TransporterId=Convert.ToInt32(c.TransporterId),
                            TransporterNo = c.TransporterNo,
                            //TransporterCode = c.TransporterCode,
                            TransporterName = c.TransporterName,
                            LrNo = c.LrNo,
                            LrDate = Convert.ToDateTime(c.LrDate).Date.ToString("dd-MM-yyyy"),
                            TotalCaseQty = c.TotalCaseQty,
                            VehicleNo = c.VehicleNo,
                            AddedBy = Convert.ToInt32(c.AddedBy),
                            AddedOn = Convert.ToDateTime(c.AddedOn).ToString()
                        }).OrderBy(x => x.InvoiceNo).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetTransitDataLstPvt", "Get Transit Data List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return TransitDataLst;
        }
        #endregion

        #region Invoice Inward Raise Request By Id For Mobile
        public string UpdateInvInwardRaiseRequestById(InvInwardRaiseRequestByIdForModel model)
        {
            return UpdateInvInwardRaiseRequestByIdPvt(model);
        }
        private string UpdateInvInwardRaiseRequestByIdPvt(InvInwardRaiseRequestByIdForModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_UpdateInvInwardRaiseRequestByIdForMob(model.BranchId, model.CompId, model.InvId, model.Remarks, model.RaiseRequestBy, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "UpdateInvInwardRaiseRequestByIdPvt", "Invoice Inward Raise Request By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region  Add/Edit and Delete LR Details  List For Mob

        public string AddEditDeleteLrDetails(LRDetailsModel model)
        {
            return AddEditDeleteLrDetailsPvt(model);
        }
        public string AddEditDeleteLrDetailsPvt(LRDetailsModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_InvInLRDetailsAddEditForMob(model.BranchId, model.CompId, model.InvId, model.InvoiceDate, model.LRNo, model.LRDate, model.NoOfCase, model.ActualNoOfCase, model.Remarks, model.Action, model.AddedBy, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditDeleteLrDetails", "Add Edit Delete Lr Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (model.Action == "STATUS")
            {
                if (RetValue > 0)
                {
                    return BusinessCont.msg_stsChange;
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

        #region Get LR List Details  List For Mob
        public List<LRDetailsModel> GetLRDetailsList(int BranchId, int CompId)
        {
            return GetLRDetailsListPvt(BranchId, CompId);
        }
        private List<LRDetailsModel> GetLRDetailsListPvt(int BranchId, int CompId)
        {
            List<LRDetailsModel> LRDetailsLst = new List<LRDetailsModel>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    LRDetailsLst = _contextManager.usp_GetInvInLRDetailsForMob(BranchId, CompId)    //need to update sp  
                       .Select(c => new LRDetailsModel
                       {
                           BranchId = c.BranchId,
                           CompId = c.CompId,
                           InvId = c.InvId,
                           InvoiceDate = c.InvoiceDate,
                           LRNo = c.LRNo,
                           LRDate = c.LRDate,
                           NoOfCase = c.NoOfCase,
                           ActualNoOfCase = c.ActualNoOfCase,
                           Remarks = c.ActualNoOfCase,
                           AddedBy = c.AddedBy,
                           AddedOn = Convert.ToDateTime(c.AddedOn),
                           LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn)
                       }).OrderBy(x => x.LRNo).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetLRDetailsListPvt", "Get LR List Details" + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return LRDetailsLst;
        }
        #endregion       

        #region Get Invoice In Vehicle Check List Master
        public List<InvInVehicleChecklistMaster> GetInvInVehicleCheckListMaster(int BranchId, int CompId, string ChecklistType)
        {
            return GetInvInVehicleCheckListMasterPvt(BranchId, CompId, ChecklistType);
        }
        private List<InvInVehicleChecklistMaster> GetInvInVehicleCheckListMasterPvt(int BranchId, int CompId, string ChecklistType)
        {
            List<InvInVehicleChecklistMaster> InvInVehicleMasterLst = new List<InvInVehicleChecklistMaster>();
            try
            {
                using (CFADBEntities _contextManager = new CFADBEntities())
                {
                    _contextManager.Database.CommandTimeout = 1000;
                    InvInVehicleMasterLst = _contextManager.usp_GetInvInVehicleCheckListMaster(BranchId, CompId, ChecklistType)    //need to update sp  
                       .Select(c => new InvInVehicleChecklistMaster
                       {
                           ChecklistTypeId = c.ChecklistTypeId,
                           BranchId = Convert.ToInt32(c.BranchId),
                           CompId = Convert.ToInt32(c.CompId),
                           ChecklistType = c.ChecklistType,
                           Section = c.Section,
                           QuestionName = c.QuestionName,
                           IsActive = c.IsActive,
                           AddedBy = c.AddedBy,
                           AddedOn = Convert.ToDateTime(c.AddedOn)
                       }).ToList();

                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvInVehicleCheckListMasterPvt", " Get Invoice In Vehicle Check List Master" + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvInVehicleMasterLst;
        }
        #endregion

    }
}
