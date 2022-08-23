using CNF.Business.Model.Master;
using System.Collections.Generic;

namespace CNF.Business.Repositories.Repository
{
    public interface IMastersRepository
    {
        GeneralMasterList GetGeneralMaster(string CategoryName, string Status);
        GetStateList GetStateList(string Flag);
        GetCityList GetCityList(string StateCode, string districtCode, string Flag);
        List<BranchList> GetBranchList(string Status);
        string BranchMasterAddEdit(BranchList model);
        List<CompanyDtls> CompanyDtls(string Status);
        string CompanyDtlsAddEdit(CompanyDtls model);
        string EmployeeMasterActivate(EmployeeActiveModel model);
        int AddEmployeeDtls(AddEmployeeModel model);
        int EditEmployeeDtls(AddEmployeeModel model);
        List<EmployeeDtls> GetEmployeeDtls(int EmpId);
        GetCategoryList GetCategoryList();
        string AddEditDivisionMaster(DivisionMasterLst model);
        List<DivisionMasterLst> GetDivisionMasterList(string Status);
        string AddEditGeneralMaster(GeneralMasterLst model);
        string AddEditTransporterMaster(TransporterMasterLst model);
        List<TransporterMasterLst> GetTransporterMasterList(string DistrictCode, string Status);
        List<RoleModel> GetRoleLst();
        List<StockistModel> GetStockistLst(int BranchId, int CompanyId, string Status);
        string StockistDtlsAddEdit(StockistModel model);
        List<BankModel> GetStockistBankList(int StockistId);
        List<StokistTransportModel> GetStokistTransportMappingList(int CompanyId);
        string StokistTransportMappingAddEdit(StokistTransportModel model);
        GetDistrictList GetDistrictList(string StateCode, string Flag);
        List<cartingAgentmodel> GetCartingAgentLst(string Status, int BranchId);
        string CartingAgentMasterAddEdit(cartingAgentmodel model);
        List<EmployeeMasterList> GetEmployeeMasterList(int BranchId, string Status);
        string UserActiveDeactive(EmployeeActiveModel model);

        string AddEditCourierMaster(CourierMasterLst model);
        List<CourierMasterLst> GetcourierMasterList(int BranchId,string DistrictCode, string Status);
        UserDtls GetUserDtls(int UserId);
        //StockistModel GetStockistById(int StockistId);
        TransporterMasterLst GetTransporterById(int StockistId);
        List<BranchIdDtls> GetBranchByIdDtls(int BranchId);
        string AddEditStockistCompanyRelation(StockistRelation model);
        string AddEditStockistBranchRelation(StockistRelation model);
        List<StockistRelation> GetStockistBranchRelationList(int BranchId);
        List<StockistRelation> GetStockistCompanyRelationList(int CompId);
        CheckUsernameAvailableModel GetCheckUsernameAvailable(string Username);
        List<StockistModel> GetStockistListByBranch(int BranchId, string Status);
        List<StockistModel> GetStockistListByCompany(int CompanyId, string Status);
        List<RolesModel> GetRolesls(int EmpId);
        List<GuardDetails> GetGuardDetails(int BranchId, int CompId);
        string CreateUser(CreateUserModel model);
        StockistModel GetStockistNoAvailables(string StockistNo);
        TransporterMasterLst GetTransporterNoAvailables(string TransporterNo);

        AddEmployeeModel GetCheckEmployeeNumberAvilable( int EmpId, string EmpNo, string EmpEmail, string EmpMobNo);

        cartingAgentmodel GetCheckCartingAgentAvilable(string CAName);

        CourierMasterLst GetCheckCourierNameAvilable(string CAName);
        string AddEditCityMaster(CityMaster model);
    } 
}
 