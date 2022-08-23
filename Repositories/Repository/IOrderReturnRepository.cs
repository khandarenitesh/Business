using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CNF.Business.Model.OrderReturn.OrderReturn;

namespace CNF.Business.Repositories.Repository
{
    public interface IOrderReturnRepository
    {
        string GetNewGeneratedGatepassNo(int BranchId, int CompId, DateTime ReceiptDate);
        string AddEditInwardGatepass(InwardGatepassModel model);
        List<StokistDtlsModel> GetStockistDtlsForEmail(int BranchId, int CompId, int GatepassId);
        int AddSendEmailFlag(int BranchId, int CompId, int GatepassId, int Flag);
        List<InwardGatepassModel> GetInwardGatepassList(int BranchId, int CompId);
        string SendEmailForConsignmentReceived(string Emailid, string StockistName, string LRNumber, DateTime LRDate, DateTime ReceiptDate, int BranchId, int CompId);
        List<InwardGatepassModel> GetMissingClaimFormList(int BranchId, int CompId, int Flag);
        string SendEmailForMissingClaimForm(string Emailid, int BranchId, int CompId, string StockistName);
        int PhysicalCheck1AddEdit(PhysicalCheck1 model);
        List<PhysicalCheck1> GetPhysicalCheck1List(int BranchId, int CompId);
        int PhysicalCheck1Concern(PhysicalCheck1 model);
        List<SRSClaimListForVerifyModel> GetSRSClaimListForVerifyList(int BranchId, int CompId);
        int AuditorCheckCorrection(AuditorCheckCorrectionModel model);
        List<ImportCNData> GetImportCNDataList(int BranchId, int CompId);
        List<CNDataForEmail> GetImportCNDataForEmail(int BranchId, int CompId);
        int ClaimSRSMappingAddEdits(AddClaimSRSMappingModel model);
        List<AddClaimSRSMappingModel> GetClaimSRSMappingLists(int BranchId, int CompId, int PhyChkId);
        List<GetClaimNoListModel> GetClaimNoLists(int BranchId, int CompId);
        List<AddClaimSRSMappingModel> GetSRSClaimMappedLists(int BranchId, int CompId);
        int UpdateCNDelayReason(UpdateCNDelayReason model);
        List<LRmisMatchModel> GetLrMisMatchLists(int BranchId, int CompId);
    }
}
