using CNF.Business.Model.OrderDispatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNF.Business.Repositories.Repository
{
   public interface IOrderDispatchRepository
    {
        string PickListHeaderAddEdit(PickListModel model);
        List<PickListModel> GetPickLst(int BranchId, int CompId, DateTime PicklistDate);
        List<PickListDetailsByIdModel> GetPickListDetailsById(int Picklistid);
        string sendEmail(string EmailId, string PicklistNo);
        string sendEmailToPicker(string EmailId, string PicklistNo);
        string PicklistAllotmentAdd(PicklstAllotReallotModel model);
        string PicklistReAllotmentAdd(PicklstAllotReallotModel model);
        string PicklistAllotmentStatus(PicklistAllotmentStatusModel model);
        List<InvoiceLstModel> GetInvoiceHeaderLst(int BranchId, int CompId, Nullable<DateTime> FromDate, Nullable<DateTime> ToDate, int BillDrawerId);
        List<Picklstmodel> GetAllotedPickListForPicker(int BranchId, int CompId, int PickerId, DateTime PicklistDate);
        string InvoiceHeaderStatusUpdate(InvoiceHeaderStatusUpdateModel model);
        string AssignTransportMode(AssignTransportModel model);
        PickLstSummaryData GetPickListSummaryData(int BranchId, int CompId, int PickerId, DateTime PicklistDate);
        List<InvoiceLstModel> InvoiceListForAssignTransMode(int BranchId, int CompId);
        string GetPickListGenerateNewNo(int BranchId, int CompId, DateTime PicklistDate);
        List<GetInvoiceDetailsForStickerModel> GetInvoiceDetailsForSticker(int BranchId, int CompId, int InvId);
        List<ImportLrDataModel> GetLRDataLst(int BranchId, int CompId, string LRDate);
        PickLstSummaryData GetPickListSummaryCounts(PickLstSummaryData model);
        List<PickListModel> GetPickListForReAllotment(PickListModel model);
        InvCntModel InvoiceSummaryCounts(int BranchId, int CompId, DateTime InvDate);
        string PickListHeaderDelete(PickListModel model);
        List<InvSts> InvoiceStatusForMob();
        string GenerateGatepasAddEdit(GatePassModel model);
        List<PrinterDtls> GetPrinterDetails(int BranchId, int CompId);
        string PrinterLogAddEdit(PrinterLogAddEditModel model);
        string PrinterPDFData(PrintPDFDataModel model);
        List<PrintPDFDataModel> GetPrintPDFData(int BranchId, int CompId);
        string GatepassListGenerateNewNo(int BranchId, int CompId, DateTime GatepassDate);
        List<InvGatpassDtls> GetGatepassDtlsForPDF(int BranchId, int CompId, int GPid);
        List<GatepassDtls> GetGatepassDtlsForMobile(int BranchId, int CompId);
        List<InvDtlsForMob> GetInvoiceDtlsForMobile(int BranchId, int CompId,int InvStatus);
        string GatepassDtlsForDeleteById(int GatepassId);
        string sendEmailForDispatchDone(string Emailid, string StockistName, string PONo, DateTime PODate, string TransporterName,string CompanyName,int BranchId, int CompId);
        List<InvDtlsForEmail> GetInvDtlsForEmail(int BranchId, int CompId, int GatepassId);
        string PrintDetailsAdd(PrinterDtls model);
        List<PickListModel> GetPicklistByPickerStatus(int BranchId, int CompId, DateTime PicklistDate);
        string PriorityInvoiceFlagUpdate(PriorityFlagUpdtModel model);
        string ResolveConcernAdd(PickListModel model);
        List<PickListModel> ResolveConcernLst(int BranchId, int CompId, DateTime PicklistDate);
        List<InvoiceLstModel> GetInvoiceHeaderLstResolveCnrn(int BranchId, int CompId, int BillDrawerId);
        string ResolveInvConcernAdd(InvoiceLstModel model);
        List<AssignedTransportModel> GetAssignedTransporterList(int BranchId, int CompId);
        string EditAssignedTransportMode(AssignedTransportModel model);
    }
}
