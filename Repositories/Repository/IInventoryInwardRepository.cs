using CNF.Business.Model.InventoryInward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNF.Business.Repositories.Repository
{
    public interface IInventoryInwardRepository
    {
        int InsuranceClaimAddEdit(InsuranceClaimModel model);
        List<InsuranceClaimModel> GetInsuranceClaimList(int BranchId, int CompId);
        List<InvoiceListModel> GetInvoiceList(int BranchId, int CompId);
        List<InsuranceClaimModel> GetClaimTypeList(int BranchId, int CompanyId);
        List<InsuranceClaimModel> GetInsuranceClaimInvByIdList(int BranchId, int CompanyId,string InvoiceId);
        List<InsuranceClaimModel> GetInsuranceClmDtlsForEmail(int BranchId, int CompId,int ClaimId);
        string InsuranceClaimforApproval(List<InsuranceClaimModel> modelList, string MailFilePath);
        string UpdateMailForApproval(int BranchId, int CompId, long ClaimId, bool IsEmailSend);
        int MapInwardVehicleAddEditForMob(MapInwardVehicleForMobModel model);
        List<MapInwardVehicleForMobModel> GetMapInwardVehicleListForMobs(int BranchId, int CompId);
        int VehicleCheckListAddEdits(VehicleChecklistModel model);
        List<InvInVehicleCheckListmodel> GetInvInVehicleCheckList(int BranchId, int CompId);
        int UpdateVerifyApproveVehicleIssues(VerifyCheckListApproveModel model);
        List<ImportTransitListModel> GetTransitDataLst(int BranchId, int CompId);
        string UpdateInvInwardRaiseRequestById(InvInwardRaiseRequestByIdForModel model);
        string AddEditDeleteLrDetails(LRDetailsModel model);
        List<LRDetailsModel> GetLRDetailsList(int BranchId, int CompId);
        List<InvInVehicleChecklistMaster> GetInvInVehicleCheckListMaster(int BranchId, int CompId, string ChecklistType);
    }
}
