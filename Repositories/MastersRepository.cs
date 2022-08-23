using CNF.Business.BusinessConstant;
using CNF.Business.Model.Context;
using CNF.Business.Model.Master;
using CNF.Business.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;

namespace CNF.Business.Repositories
{
    public class MastersRepository : IMastersRepository
    {
        private CFADBEntities _contextManager;

        //Inject DB Context instance from Unit of Work to avoid unwanted memory allocations.
        public MastersRepository(CFADBEntities contextManager)
        {
            _contextManager = contextManager;
        }

        #region General Master List
        public GeneralMasterList GetGeneralMaster(string CategoryName, string Status)
        {
            return GetGeneralMasterPvt(CategoryName, Status);
        }
        private GeneralMasterList GetGeneralMasterPvt(string CategoryName, string Status)
        {
            GeneralMasterList generalmasterlist = new GeneralMasterList();
            try
            {
                using (CFADBEntities ContextManager = new CFADBEntities())
                {
                    generalmasterlist.GeneralMasterParameter = ContextManager.usp_GeneralMasterList(CategoryName, Status).Select(x => new GeneralMasterDetail()
                    {
                        pkId = Convert.ToInt32(x.pkId),
                        CategoryName = x.CategoryName,
                        MasterName = x.MasterName,
                        DescriptionText = x.DescriptionText,
                        isActive = x.IsActive
                    }).OrderByDescending(x => x.pkId).ToList();
                }

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGeneralMasterPvt", "Get General Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return generalmasterlist;
        }
        #endregion

        #region Get State List
        public GetStateList GetStateList(string Flag)
        {
            return GetStateDetailsPvt(Flag);
        }
        private GetStateList GetStateDetailsPvt(string Flag)
        {
            GetStateList getstateModel = new GetStateList();
            try
            {
                using (CFADBEntities ContextManager = new CFADBEntities())
                {
                    getstateModel.GetStateParameter = ContextManager.usp_GetStateList(Flag).Select(x => new GetStateDtls()
                    {
                        StateCode = x.StateCode,
                        StateName = x.StateName,
                        ActiveFlag = x.ActiveFlag,
                        LastUpdateBy = x.LastUpdateBy,
                        LastUpdateTime = Convert.ToDateTime(x.LastUpdateTime)
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStateDetailsPvt", "Get State Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return getstateModel;
        }
        #endregion

        #region Get Branch List
        public List<BranchList> GetBranchList(string Status)
        {
            return GetBranchListPvt(Status);
        }
        private List<BranchList> GetBranchListPvt(string Status)
        {
            List<BranchList> modelList = new List<BranchList>();
            try
            {
                modelList = _contextManager.usp_BranchMasterList(Status).Select(b => new BranchList
                {
                    BranchId = b.BranchId,
                    BranchCode = b.BranchCode,
                    BranchName = b.BranchName,
                    BranchAddress = b.BranchAddress,
                    City = b.City,
                    Pin = b.Pin,
                    ContactNo = b.ContactNo,
                    Email = b.Email,
                    Pan = b.Pan,
                    GSTNo = b.GSTNo,
                    IsActive = b.IsActive,
                    Addedby = b.Addedby,
                    CityName = b.CityName,
                    AddedOn = Convert.ToDateTime(b.AddedOn),
                    LastUpdatedOn = Convert.ToDateTime(b.LastUpdatedOn)
                }).OrderByDescending(x => x.BranchId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetBranchListPvt", "Get Branch List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get City List
        public GetCityList GetCityList(string StateCode, string districtCode, string Flag)
        {
            return GetCityDetailsPvt(StateCode, districtCode, Flag);
        }
        private GetCityList GetCityDetailsPvt(string StateCode, string districtCode, string Flag)
        {
            GetCityList getcityModel = new GetCityList();
            getcityModel.GetCityParameter = new List<GetCityDtls>();
            try
            {
                using (CFADBEntities ContextManager = new CFADBEntities())
                {
                    getcityModel.GetCityParameter = ContextManager.usp_GetCityList(StateCode, districtCode, Flag).Select(x => new GetCityDtls()
                    {
                        CityCode = x.CityCode,
                        CityName = x.CityName,
                        StateName = x.StateName,
                        StateCode = x.StateCode,
                        ActiveFlag = x.ActiveFlag,
                        LastUpdateBy = x.LastUpdateBy,
                        LastUpdateTime = Convert.ToDateTime(x.LastUpdateTime)
                    }).OrderByDescending(x => x.CityCode).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCityDetailsPvt", "Get City Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return getcityModel;
        }
        #endregion

        #region  Add Edit Branch Master
        public string BranchMasterAddEdit(BranchList model)
        {
            return BranchMasterAddEditPvt(model);
        }
        private string BranchMasterAddEditPvt(BranchList model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_BranchMasterAddEdit(model.BranchId, model.BranchCode, model.BranchName, model.BranchAddress, model.City, model.Pin, model.ContactNo, model.Email, model.Pan, model.GSTNo, model.IsActive, model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "BranchMasterAddEditPvt", "Branch Master AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Company Details
        public List<CompanyDtls> CompanyDtls(string Status)
        {
            return CompanyDtlsPvt(Status);
        }
        private List<CompanyDtls> CompanyDtlsPvt(string Status)
        {
            List<CompanyDtls> modelList = new List<CompanyDtls>();
            try
            {
                modelList = _contextManager.usp_CompanyMasterList(Status).Select(c => new CompanyDtls
                {
                    CompanyId = c.CompanyId,
                    CompanyCode = c.CompanyCode,
                    CompanyName = c.CompanyName,
                    CompanyEmail = c.CompanyEmail,
                    ContactNo = c.ContactNo,
                    CompanyAddress = c.CompanyAddress,
                    CompanyCity = c.CompanyCity,
                    CityName = c.CityName,
                    Pin = c.Pin,
                    CompanyPAN = c.CompanyPAN,
                    GSTNo = c.GSTNo,
                    IsPicklistAvailable = c.IsPicklistAvailable,
                    IsActive = c.IsActive,
                    Addedby = c.Addedby,
                    AddedOn = Convert.ToDateTime(c.AddedOn)
                }).OrderByDescending(x => x.CompanyId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "CompanyDtlsPvt", "Get Company Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        public string CompanyDtlsAddEdit(CompanyDtls model)
        {
            return CompanyDtlsAddEditPvt(model);
        }
        private string CompanyDtlsAddEditPvt(CompanyDtls model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_CompanyMasterAddEdit(model.CompanyId, model.CompanyCode, model.CompanyName, model.CompanyEmail, model.ContactNo, model.CompanyAddress, model.CompanyCity, model.Pin, model.CompanyPAN, model.GSTNo, model.IsPicklistAvailable, model.IsActive, model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.CompanyId, "CompanyDtlsAddEditPvt", "Add Company Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Api for Update Employee and User Activation
        public string EmployeeMasterActivate(EmployeeActiveModel model)
        {
            return EmployeeMasterActivatePvt(model);
        }
        private string EmployeeMasterActivatePvt(EmployeeActiveModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_EmployeeMasterActivate(model.EmpId, model.IsActive, model.Addedby, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.EmpId, "EmployeeMasterActivatePvt", "Employee Activate", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Add Employee masters
        public int AddEmployeeDtls(AddEmployeeModel model)
        {
            return AddEmployeeDtlsPvt(model);
        }
        private int AddEmployeeDtlsPvt(AddEmployeeModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_EmployeeMasterAdd(model.BranchId, model.EmpNo, model.EmpName, model.EmpPAN, model.EmpEmail,
                    model.EmpMobNo, model.EmpAddress, model.CityCode, model.DesignationId, model.BloodGroup, model.AadharNo, model.companyStr, model.Addedby,obj);

                RetValue = Convert.ToInt32(obj.Value);

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEmployeeDtlsPvt", "Add Employee", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Edit Employee Masters
        public int EditEmployeeDtls(AddEmployeeModel model)
        {
            return EditEmployeeDtlsPvt(model);
        }
        private int EditEmployeeDtlsPvt(AddEmployeeModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_EmployeeMasterEdit(model.EmpId, model.BranchId, model.EmpName, model.EmpPAN, model.EmpEmail,
                    model.EmpMobNo, model.EmpAddress, model.CityCode, model.DesignationId, model.BloodGroup, model.AadharNo, model.companyStr,  model.Addedby, obj);

                RetValue = Convert.ToInt32(obj.Value);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "EditEmployeeDtlsPvt", "Edit Employee", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Employee Detalis
        public List<EmployeeDtls> GetEmployeeDtls(int EmpId)
        {
            return GetEmployeeDtlsPvt(EmpId);
        }
        private List<EmployeeDtls> GetEmployeeDtlsPvt(int EmpId)
        {
            List<EmployeeDtls> EmpLst = new List<EmployeeDtls>();
            try
            {
                EmpLst = _contextManager.usp_EmployeeCompanyDetails(EmpId).Select(c => new EmployeeDtls
                {
                    EmpId = c.EmpId,
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    BranchCode = c.BranchCode,
                    BranchName = c.BranchName,
                    CityCode = c.CityCode,
                    CityName = c.CityName,
                    CompanyCode = c.CompanyCode
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetEmployeeDtlsPvt", "Get Employee Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return EmpLst;
        }
        #endregion

        #region Get Category List
        public GetCategoryList GetCategoryList()
        {
            return GetCategoryListPvt();
        }
        private GetCategoryList GetCategoryListPvt()
        {
            GetCategoryList CategoryList = new GetCategoryList();
            try
            {
                using (CFADBEntities ContextManager = new CFADBEntities())
                {
                    CategoryList.CategoryParameter = ContextManager.usp_GetcategoryList().Select(x => new GetCategoryDetails()
                    {
                        CatId = Convert.ToInt32(x.CatId),
                        CategoryName = x.CategoryName,
                        isActive = x.isActive,
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCategoryList", "Get Category List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return CategoryList;
        }
        #endregion

        #region Add Edit Division Master
        public string AddEditDivisionMaster(DivisionMasterLst model)
        {
            return AddEditDivisionMasterPvt(model);
        }
        private string AddEditDivisionMasterPvt(DivisionMasterLst model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_DivisionMasterAddEdit(model.BranchId,model.CompanyId,model.DivisionId, model.DivisionCode, model.DivisionName, model.FloorName, model.IsColdStorage, model.IsActive, model.AddedBy, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.DivisionId, "AddEditDivisionMaster", "Add Edit Division Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get Division Master List
        public List<DivisionMasterLst> GetDivisionMasterList(string Status)
        {
            return GetDivisionMasterListPvt(Status);
        }
        private List<DivisionMasterLst> GetDivisionMasterListPvt(string Status)
        {
            List<DivisionMasterLst> divisionmasterList = new List<DivisionMasterLst>();
            try
            {
                divisionmasterList = _contextManager.usp_DivisionMasterList(Status).Select(x => new DivisionMasterLst
                {
                    BranchId = x.BranchId,
                    DivisionId = x.DivisionId,
                    DivisionCode = x.DivisionCode,
                    DivisionName = x.DivisionName,
                    FloorName = x.FloorName,
                    IsColdStorage = Convert.ToInt32(x.IsColdStorage),
                    SupplyType = x.SupplyType,
                    IsActive = x.IsActive,
                    AddedBy = x.AddedBy,
                    AddedOn = Convert.ToDateTime(x.AddedOn),
                    LastUpdatedOn = Convert.ToDateTime(x.LastUpdatedOn)
                }).OrderByDescending(x => x.DivisionId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetDivisionMasterList", "Get Division Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return divisionmasterList;
        }
        #endregion

        #region Add Edit General Master
        public string AddEditGeneralMaster(GeneralMasterLst model)
        {
            return AddEditGeneralMasterPvt(model);
        }
        private string AddEditGeneralMasterPvt(GeneralMasterLst model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_GeneralMasterAddEdit(model.pkId, model.CategoryName, model.MasterName, model.DescriptionText, model.IsActive, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditGeneralMaster", "Add Edit General Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Add Edit Transporter Master
        public string AddEditTransporterMaster(TransporterMasterLst model)
        {
            return AddEditTransporterMasterPvt(model);
        }
        private string AddEditTransporterMasterPvt(TransporterMasterLst model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_TransporterMasterAddEdit(model.TransporterId, model.BranchId, model.TransporterNo, model.TransporterName, model.TransporterEmail, model.TransporterMobNo, model.TransporterAddress, model.CityCode, model.StateCode, model.DistrictCode, model.IsActive, model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditTransporterMaster", "Add Edit Transporter Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get Transporter Master List
        public List<TransporterMasterLst> GetTransporterMasterList(string DistrictCode, string Status)
        {
            return GetTransporterMasterListPvt(DistrictCode, Status);
        }
        private List<TransporterMasterLst> GetTransporterMasterListPvt(string DistrictCode, string Status)
        {
            List<TransporterMasterLst> transportermasterList = new List<TransporterMasterLst>();
            try
            {
                transportermasterList = _contextManager.usp_TransporterMasterList(DistrictCode, Status).Select(x => new TransporterMasterLst
                {
                    TransporterId = x.TransporterId,
                    BranchId = Convert.ToInt32(x.BranchId),
                    TransporterNo = x.TransporterNo,
                    TransporterName = x.TransporterName,
                    TransporterEmail = x.TransporterEmail,
                    TransporterMobNo = x.TransporterMobNo,
                    TransporterAddress = x.TransporterAddress,
                    CityCode = x.CityCode,
                    CityName = x.CityName,
                    StateCode = x.StateCode,
                    StateName = x.StateName,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictName,
                    IsActive = x.IsActive
                }).OrderByDescending(x => x.TransporterId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetTransporterMasterList", "Get Transporter Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return transportermasterList;
        }
        #endregion

        #region Get Role List
        public List<RoleModel> GetRoleLst()
        {
            return GetRoleLstPvt();
        }
        private List<RoleModel> GetRoleLstPvt()
        {
            List<RoleModel> RoleLst = new List<RoleModel>();
            try
            {
                RoleLst = _contextManager.usp_GetRoleList().Select(c => new RoleModel
                {
                    RoleId = c.RoleId,
                    RoleName = c.RoleName,
                    ActiveStatus = c.ActiveStatus
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetRoleLstPvt", "Get Role List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RoleLst;
        }
        #endregion

        #region Stockist
        public List<StockistModel> GetStockistLst(int BranchId,int CompanyId, string Status)
        {
            return GetStockistPvt(BranchId,CompanyId, Status);
        }
        private List<StockistModel> GetStockistPvt(int BranchId, int CompanyId, string Status)
        {
            List<StockistModel> StockLst = new List<StockistModel>();
            try
            {
                StockLst = _contextManager.usp_StockistMasterList(BranchId,CompanyId, Status).Select(c => new StockistModel
                {
                    StockistId = c.StockistId,
                    StockistNo = c.StockistNo,
                    StockistName = c.StockistName,
                    StockistPAN = c.StockistPAN,
                    Emailid = c.Emailid,
                    MobNo = c.MobNo,
                    StockistAddress = c.StockistAddress,
                    CityCode = c.CityCode,
                    CityName = c.CityName,
                    GSTNo = c.GSTNo,
                    BankId = Convert.ToInt16(c.BankId),
                    IFSCCode = c.IFSCCode,
                    BankAccountNo = c.BankAccountNo,
                    LocationId = c.LocationId,
                    MasterName = c.LocationName,
                    Pincode = c.Pincode,
                    DLNo = c.DLNo,
                    DLExpDate = DateTime.Parse(c.DLExpDate.ToString()), // Convert.ToDateTime(c.DLExpDate),
                    FoodLicNo = c.FoodLicNo,
                    FoodLicExpDate = DateTime.Parse(c.FoodLicExpDate.ToString()), // Convert.ToDateTime(c.FoodLicExpDate),
                    IsActive = c.IsActive,
                    Addedby = c.Addedby,
                    AddedOn = Convert.ToDateTime(c.AddedOn),
                    LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn)
                }).OrderByDescending(x => x.StockistId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockiestPvt", "Get Role List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockLst;
        }

        public string StockistDtlsAddEdit(StockistModel model)
        {
            return StockistDtlsAddEditPvt(model);
        }
        private string StockistDtlsAddEditPvt(StockistModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            List<BankModel> BankList = new List<BankModel>();

            for (int i = 0; i < model.BnkDtls.Count; i++)
            {
                BankModel bankmodel = new BankModel();
                bankmodel.StockistId = model.StockistId;
                bankmodel.BankId = model.BnkDtls[i].BankId;
                bankmodel.IFSCCode = model.BnkDtls[i].IFSCCode;
                bankmodel.AccountNo = model.BnkDtls[i].AccountNo;
                BankList.Add(bankmodel);
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("StockistId");
            dt.Columns.Add("BankId");
            dt.Columns.Add("IFSCCode");
            dt.Columns.Add("AccountNo");

            foreach (var item in BankList)
            {
                dt.Rows.Add(item.StockistId, item.BankId, item.IFSCCode, item.AccountNo);
            }

            try
            {
                using (var db = new CFADBEntities())
                {
                    SqlConnection connection = (SqlConnection)db.Database.Connection;
                    SqlCommand cmd = new SqlCommand("CFA.usp_StockistMasterAddEdit", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter StockistIdParameter = cmd.Parameters.AddWithValue("@StockistId", model.StockistId);
                    StockistIdParameter.SqlDbType = SqlDbType.Int;
                    SqlParameter StockistNoParameter = cmd.Parameters.AddWithValue("@StockistNo", model.StockistNo);
                    StockistNoParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter StockistNameParameter = cmd.Parameters.AddWithValue("@StockistName", model.StockistName);
                    StockistNameParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter StockistPANParameter = cmd.Parameters.AddWithValue("@StockistPAN", model.StockistPAN);
                    StockistPANParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter EmailidParameter = cmd.Parameters.AddWithValue("@Emailid", model.Emailid);
                    EmailidParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter MobNoParameter = cmd.Parameters.AddWithValue("@MobNo", model.MobNo);
                    MobNoParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter StockistAddressParameter = cmd.Parameters.AddWithValue("@StockistAddress", model.StockistAddress);
                    StockistAddressParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter CityCodeParameter = cmd.Parameters.AddWithValue("@CityCode", model.CityCode);
                    CityCodeParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter GSTNoParameter = cmd.Parameters.AddWithValue("@GSTNo", model.GSTNo);
                    GSTNoParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter PincodeParameter = cmd.Parameters.AddWithValue("@Pincode", model.Pincode);
                    PincodeParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter DLNoParameter = cmd.Parameters.AddWithValue("@DLNo", model.DLNo);
                    DLNoParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter DLExpDateParameter = cmd.Parameters.AddWithValue("@DLExpDate", model.DLExpDate);
                    DLExpDateParameter.SqlDbType = SqlDbType.DateTime;
                    SqlParameter FoodLicNoParameter = cmd.Parameters.AddWithValue("@FoodLicNo", model.FoodLicNo);
                    FoodLicNoParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter FoodLicExpDateParameter = cmd.Parameters.AddWithValue("@FoodLicExpDate", model.FoodLicExpDate);
                    FoodLicExpDateParameter.SqlDbType = SqlDbType.DateTime;
                    SqlParameter IsActiveParameter = cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                    IsActiveParameter.SqlDbType = SqlDbType.Char;
                    SqlParameter AddedbyParameter = cmd.Parameters.AddWithValue("@Addedby", model.Addedby);
                    AddedbyParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter BnkDtlsParameter = cmd.Parameters.AddWithValue("@BnkDtls", dt);
                    BnkDtlsParameter.SqlDbType = SqlDbType.Structured;
                    SqlParameter ActionParameter = cmd.Parameters.AddWithValue("@Action", model.Action);
                    ActionParameter.SqlDbType = SqlDbType.NVarChar;
                    SqlParameter RetValueParameter = cmd.Parameters.AddWithValue("@RetValue", 0);
                    RetValueParameter.Direction = ParameterDirection.Output;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    RetValue = cmd.ExecuteNonQuery();
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.CompanyId, "StockistDtlsAddEditPvt", "Add Edit Stockiest Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get Stockist by Id
        public List<BankModel> GetStockistBankList(int StockistId)
        {
            return GetStockistBankListPvt(StockistId);
        }
        private List<BankModel> GetStockistBankListPvt(int StockistId)
        {
            List<BankModel> BankModel = new List<BankModel>();
            try
            {
                using (CFADBEntities ContextManager = new CFADBEntities())
                {
                    BankModel = ContextManager.usp_StockistBankListById(StockistId).Select(x => new BankModel()
                    {
                        StockistId = x.StockistId,
                        BankId = x.BankId,
                        BankName = x.BankName,
                        AccountNo = x.AccountNo,
                        IFSCCode = x.IFSCCode
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistBankListPvt", "Get Stockist Bank List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return BankModel;
        }
        #endregion

        #region Get Stokist Transport Mapping List
        public List<StokistTransportModel> GetStokistTransportMappingList(int CompanyId)
        {
            return GetStokistTransportMappingListPvt(CompanyId);
        }
        private List<StokistTransportModel> GetStokistTransportMappingListPvt(int CompanyId)
        {
            List<StokistTransportModel> StockMapLst = new List<StokistTransportModel>();
            try
            {
                StockMapLst = _contextManager.usp_StokistTransportMappingList(CompanyId).Select(c => new StokistTransportModel
                {
                    Mappingid = c.Mappingid,
                    BranchId = c.BranchId,
                    CompanyId = c.CompanyId,
                    StockistId = c.StockistId,
                    StockistNo = c.StockistNo,
                    StockistName = c.StockistName,
                    Emailid = c.Emailid,
                    MobNo = c.MobNo,
                    CityCode = c.CityCode,
                    LocationId = Convert.ToInt32(c.LocationId),
                    TransporterId = c.TransporterId,
                    TransporterNo = c.TransporterNo,
                    TransporterName = c.TransporterName,
                    TransitDays = c.TransitDays,
                    Addedby = c.Addedby,
                    AddedOn = Convert.ToDateTime(c.AddedOn),
                    SupplyTypeId = c.SupplyTypeId,
                    MasterName = c.MasterName

                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStokistTransportMappingListPvt", "Get Stokist Transport Mapping List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockMapLst;
        }
        #endregion

        #region Stokist Transport Mapping AddEdit
        public string StokistTransportMappingAddEdit(StokistTransportModel model)
        {
            return StokistTransportMappingAddEditPvt(model);
        }
        private string StokistTransportMappingAddEditPvt(StokistTransportModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_StokistTransportMappingAddEdit(model.BranchId, model.CompanyId, model.StockistId, model.TransporterId, model.TransitDays, model.SupplyTypeId, model.Addedby, model.AddedOn, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "StokistTransportMappingAddEditPvt", "Stokist Transport Mapping AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Carting Agent Details
        public List<cartingAgentmodel> GetCartingAgentLst(string Status, int BranchId)
        {
            return GetCartingAgentLstPvt(Status, BranchId);
        }
        private List<cartingAgentmodel> GetCartingAgentLstPvt(string Status, int BranchId)
        {
            List<cartingAgentmodel> CAList = new List<cartingAgentmodel>();

            try
            {
                CAList = _contextManager.usp_CartingAgentMasterList(Status, BranchId).Select(c => new cartingAgentmodel
                {
                    CAId = c.CAId,
                    BranchId = Convert.ToInt32(c.BranchId),
                    CAName = c.CAName,
                    CAMobNo = c.CAMobNo,
                    CAEmail = c.CAEmail,
                    CAPan = c.CAPan,
                    GSTNo = c.GSTNo,
                    CAAddress = c.CAAddress,
                    StateCode = c.StateCode,
                    StateName = c.StateName,
                    DistrictCode = c.DistrictCode,
                    DistrictName = c.DistrictName,
                    CityCode = c.CityCode,
                    CityName = c.CityName,
                    IsActive = c.IsActive,
                    Addedby = c.Addedby,
                    AddedOn = Convert.ToDateTime(c.AddedOn),
                    LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn)
                }).OrderByDescending(x => x.CAId).ToList();

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCartingAgentLstPvt", "Get carting Agent List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return CAList;
        }

        public string CartingAgentMasterAddEdit(cartingAgentmodel model)
        {
            return CartingAgentMasterAddEditPvt(model);
        }
        private string CartingAgentMasterAddEditPvt(cartingAgentmodel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_CartingAgentMasterAddEdit(model.CAId, model.BranchId, model.CAName, model.CAMobNo, model.CAEmail, model.CAPan, model.GSTNo, model.CAAddress, model.StateCode, model.DistrictCode, model.CityCode, model.IsActive, model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "CartingAgentMasterAddEditPvt", "Carting Agent AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get District List
        public GetDistrictList GetDistrictList(string StateCode, string Flag)
        {
            return GetDistrictDtlsPvt(StateCode, Flag);
        }
        private GetDistrictList GetDistrictDtlsPvt(string StateCode, string Flag)
        {
            GetDistrictList getdistModel = new GetDistrictList();
            try
            {
                using (CFADBEntities ContextManager = new CFADBEntities())
                {
                    getdistModel.GetDistrictParameter = ContextManager.usp_GetDistrictList(StateCode, Flag).Select(x => new GetDistrictDtls()
                    {
                        DistrictCode = x.DistrictCode,
                        DistrictName = x.DistrictName,
                        StateName = x.StateName,
                        StateCode = x.StateCode,
                        ActiveFlag = x.ActiveFlag,
                        LastUpdateBy = x.LastUpdateBy,
                        LastUpdateTime = Convert.ToDateTime(x.LastUpdateTime)
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetDistrictDtlsPvt", "Get District List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return getdistModel;
        }
        #endregion

        #region Get Employee Master List
        public List<EmployeeMasterList> GetEmployeeMasterList(int BranchId, string Status)
        {
            return GetEmployeeMasterListPvt(BranchId, Status);
        }
        private List<EmployeeMasterList> GetEmployeeMasterListPvt(int BranchId, string Status)
        {
            List<EmployeeMasterList> objList = new List<EmployeeMasterList>();

            try
            {
                objList = _contextManager.usp_EmployeeMasterList(BranchId, Status).Select(e => new EmployeeMasterList
                {
                    EmpId = e.EmpId,
                    BranchId = Convert.ToInt32(e.BranchId),
                    EmpNo = e.EmpNo,
                    BranchCode = e.BranchCode,
                    BranchName = e.BranchName,
                    EmpName = e.EmpName,
                    EmpPAN = e.EmpPAN,
                    EmpEmail = e.EmpEmail,
                    EmpMobNo = e.EmpMobNo,
                    EmpAddress = e.EmpAddress,
                    CityCode = e.CityCode,
                    CityName = e.CityName,
                    DesignationId = Convert.ToInt32(e.DesignationId),
                    DesignationName = e.DesignationName,
                    pkId = Convert.ToInt32(e.pkId),
                    BloodGroupName = e.BloodGroupName,
                    AadharNo = e.AadharNo,
                    IsUser = e.IsUser,
                    IsActive = e.IsActive,
                    Addedby = e.Addedby,
                    AddedOn = Convert.ToDateTime(e.AddedOn).ToString(BusinessCont.DateFormat),
                    LastUpdatedOn = Convert.ToDateTime(e.LastUpdatedOn).ToString(BusinessCont.DateFormat),
                    RoleId = Convert.ToInt32(e.RoleId),
                    UserName = e.UserName,
                    Password = e.Password,
                    UserStatus = e.UserStatus
                }).OrderByDescending(x => x.EmpId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetEmployeeMasterListPvt", "Get Employee Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return objList;
        }
        #endregion

        #region Api for Update User Activation
        public string UserActiveDeactive(EmployeeActiveModel model)
        {
            return UserActiveDeactivePvt(model);
        }
        private string UserActiveDeactivePvt(EmployeeActiveModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_UserActiveDeactive(model.EmpId, model.IsActive, model.Addedby, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.EmpId, "UserActiveDeactivePvt", "Update User Activate", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Courier Master
        public string AddEditCourierMaster(CourierMasterLst model)
        {
            return AddEditCourierMasterPvt(model);
        }
        private string AddEditCourierMasterPvt(CourierMasterLst model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_CourierMasterAddEdit(model.CourierId, model.BranchId, model.CourierName, model.CourierEmail, model.CourierMobNo, model.CourierAddress, model.CityCode, model.StateCode, model.DistrictCode, model.IsActive, model.Addedby, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditCourierMaster", "Add Edit Courier Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        public List<CourierMasterLst> GetcourierMasterList(int BranchId,string DistrictCode, string Status)
        {
            return GetcourierMasterListPvt(BranchId,DistrictCode, Status);
        }
        private List<CourierMasterLst> GetcourierMasterListPvt(int BranchId,string DistrictCode, string Status)
        {
            List<CourierMasterLst> CourierList = new List<CourierMasterLst>();
            try
            {
                CourierList = _contextManager.usp_CourierMasterList(BranchId,DistrictCode,Status).Select(x => new CourierMasterLst
                {
                    CourierId = x.CourierId,
                    BranchId = Convert.ToInt32(x.BranchId),
                    CourierName = x.CourierName,
                    CourierEmail = x.CourierEmail,
                    CourierMobNo = x.CourierMobNo,
                    CourierAddress = x.CourierAddress,
                    CityCode = x.CityCode,
                    CityName = x.CityName,
                    StateCode = x.StateCode,
                    StateName = x.StateName,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictName,
                    IsActive = x.IsActive
                }).OrderByDescending(x=> x.CourierId).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "usp_CourierMasterList", "Get courier Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return CourierList;
        }
        #endregion

        #region Get User Details
        public UserDtls GetUserDtls(int UserId)
        {
            return GetUserDtlsPvt(UserId);
        }
        private UserDtls GetUserDtlsPvt(int UserId)
        {
            UserDtls UserLst = new UserDtls();
            try
            {
                UserLst = _contextManager.usp_GetUserDetailsForChangePwd(UserId).Select(x => new UserDtls
                {
                    UserId = x.UserId,
                    EmpId = x.EmpId,
                    DisplayName = x.DisplayName,
                    UserName = x.UserName,
                    EmpEmail= x.EmpEmail,
                    Password = x.Password,
                    EncryptPassword = x.EncryptPassword,
                    Designation = x.Designation,
                    BloodGroup = x.BloodGroup,
                    AadharNo = x.AadharNo,
                    EmpMobNo = x.EmpMobNo,
                    IsActive = x.IsActive,
                    EmpNo = x.EmpNo
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetUserDtlsPvt", " Get User Dtls", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return UserLst;
        }
        #endregion

        #region Get Stockist By Id
        public StockistModel GetStockistById(int StockistId)
        {
            return GetStockistByIdPvt(StockistId);
        }
        private StockistModel GetStockistByIdPvt(int StockistId)
        {
            StockistModel StockList = new StockistModel();
            try
            {
                //StockList = _contextManager.usp_StockistById(StockistId).Select(c => new StockistModel
                //{
                //    StockistId = c.StockistId,
                //    BranchId = c.BranchId,
                //    CompanyId = c.CompanyId,
                //    StockistNo = c.StockistNo,
                //    StockistName = c.StockistName,
                //    StockistPAN = c.StockistPAN,
                //    Emailid = c.Emailid,
                //    MobNo = c.MobNo,
                //    StockistAddress = c.StockistAddress,
                //    CityCode = c.CityCode,
                //    CityName = c.CityName,
                //    GSTNo = c.GSTNo,
                //    LocationId = c.LocationId,
                //    MasterName = c.MasterName,
                //    Pincode = c.Pincode,
                //    DLNo = c.DLNo,
                //    DLExpDate = Convert.ToDateTime(c.DLExpDate),
                //    FoodLicNo = c.FoodLicNo,
                //    FoodLicExpDate = Convert.ToDateTime(c.FoodLicExpDate),
                //    IsActive = c.IsActive,
                //    Addedby = c.Addedby,
                //    AddedOn = Convert.ToDateTime(c.AddedOn),
                //    LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn)
                //}).SingleOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistByIdPvt", "Get Stockist By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockList;
        }
        #endregion

        #region Get Transporter By Id
        public TransporterMasterLst GetTransporterById(int TransporterId)
        {
            return GetTransporterByIdPvt(TransporterId);
        }
        private TransporterMasterLst GetTransporterByIdPvt(int TransporterId)
        {
            TransporterMasterLst transporterMasterList = new TransporterMasterLst();
            try
            {
                transporterMasterList = _contextManager.usp_TransporterById(TransporterId).Select(x => new TransporterMasterLst
                {
                    TransporterId = x.TransporterId,
                    BranchId = Convert.ToInt32(x.BranchId),
                    TransporterNo = x.TransporterNo,
                    TransporterName = x.TransporterName,
                    TransporterEmail = x.TransporterEmail,
                    TransporterMobNo = x.TransporterMobNo,
                    TransporterAddress = x.TransporterAddress,
                    CityCode = x.CityCode,
                    CityName = x.CityName,
                    StateCode = x.StateCode,
                    StateName = x.StateName,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictName,
                    IsActive = x.IsActive
                }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetTransporterByIdPvt", "Get Transporter By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return transporterMasterList;
        }
        #endregion

        #region Get Branch Details By BranchId
        public List<BranchIdDtls> GetBranchByIdDtls(int BranchId)
        {
            return GetBranchByIdDtlsPvt(BranchId);
        }
        private List<BranchIdDtls> GetBranchByIdDtlsPvt(int BranchId)
        {
            List<BranchIdDtls> model = new List<BranchIdDtls>();
            try
            {
                model = _contextManager.usp_GetBranchById(BranchId).Select(c => new BranchIdDtls
                {
                    BranchId = c.BranchId,
                    BranchName = c.BranchName
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, BranchId, "GetBranchByIdDtlsPvt", "Get Branch Details By BranchId", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Add Stockist Company Relation
        public string AddEditStockistCompanyRelation(StockistRelation model)
        {
            return AddEditStockistCompanyRelationPvt(model);
        }
        private string AddEditStockistCompanyRelationPvt(StockistRelation model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_StockiestCompanyRelationAddEdit(model.pkid, model.Stockieststr, model.CompId,  model.AddedBy, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditStockistCompanyRelation", "Add Edit Stockist Company Relation", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Add Stockist Branch Relation
        public string AddEditStockistBranchRelation(StockistRelation model)
        {
            return AddEditStockistBranchRelationPvt(model);
        }
        private string AddEditStockistBranchRelationPvt(StockistRelation model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_StockiestBranchRelationAddEdit(model.pkid, model.Stockieststr, model.BranchId, model.AddedBy, model.Action, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditStockistBranchRelation", "Add Edit Stockist Branch Relation", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        #region Get Branch Stockist Relation List
        public List<StockistRelation> GetStockistBranchRelationList(int BranchId)
        {
            return GetStockistBranchRelationListPvt(BranchId);
        }
        private List<StockistRelation> GetStockistBranchRelationListPvt(int BranchId)
        {
            List<StockistRelation> model = new List<StockistRelation>();
            try
            {
                model = _contextManager.usp_StockistBranchRelationList(BranchId).Select(c => new StockistRelation
                {
                    pkid = c.PkId,
                    StockistId = c.StockiestId,
                    StockistName = c.StockistName,
                    BranchId = c.BranchId,
                    BranchName = c.BranchName,
                    StockistNo = c.StockistNo,
                    BranchCode = c.BranchCode
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, BranchId, "GetStockistBranchListPvt", "Get Stockist Branch Relation List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Stockist Company Relation List
        public List<StockistRelation> GetStockistCompanyRelationList(int CompId)
        {
            return GetStockistCompanyRelationListPvt(CompId);
        }
        private List<StockistRelation> GetStockistCompanyRelationListPvt(int CompId)
        {
            List<StockistRelation> model = new List<StockistRelation>();
            try
            {
                model = _contextManager.usp_StockistCompRelationList(CompId).Select(c => new StockistRelation
                {
                    pkid = c.PkId,
                    StockistId = c.StockiestId,
                    StockistName = c.StockistName,
                    CompId = c.CompId,
                    CompName = c.CompanyName,
                    StockistNo = c.StockistNo,
                    CompanyCode= c.CompanyCode

                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, CompId, "GetStockistCompanyRelationListPvt", "Get Stockist Company Relation List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Check Username Available
        public CheckUsernameAvailableModel GetCheckUsernameAvailable(string Username)
        {
            return GetCheckUsernameAvailablePvt(Username);
        }

        private CheckUsernameAvailableModel GetCheckUsernameAvailablePvt(string Username)
        {
            CheckUsernameAvailableModel model = new CheckUsernameAvailableModel();

            try
            {
                model = _contextManager.usp_checkUsernameAvailable(Username).Select(c => new CheckUsernameAvailableModel
                {
                    UserId = c.UserId,
                    BranchId = Convert.ToInt32(c.BranchId),
                    RoleId = c.RoleId,
                    EmpId = c.EmpId,
                    DisplayName = c.DisplayName,
                    UserName = c.UserName,
                    Password = c.Password,
                    EncryptPassword = c.EncryptPassword,
                    IsActive = c.IsActive,
                    AddedBy = c.Addedby,
                    AddedOn = Convert.ToDateTime(c.AddedOn),
                    LastUpdatedOn = Convert.ToDateTime(c.LastUpdatedOn)
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckUsernameAvailablePvt", "Get Check Username Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Stockist List By Branch 
        public List<StockistModel> GetStockistListByBranch(int BranchId,string Status)
        {
            return GetStockisListByBranchPvt(BranchId,Status);
        }
        private List<StockistModel> GetStockisListByBranchPvt(int BranchId, string Status)
        {
            List<StockistModel> model = new List<StockistModel>();
            try
            {
                model = _contextManager.usp_StockistMasterListByBranchId(BranchId,Status).Select(c => new StockistModel
                {
                 
                  StockistId = c.StockistId,
                  StockistNo = c.StockistNo,
                  StockistName = c.StockistName,
                  Checked = c.Checked
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, BranchId, "GetStockisListByBranchPvt", "Get Stockist List By Branch ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Stockist List By Company 
        public List<StockistModel> GetStockistListByCompany(int CompanyId, string Status)
        {
            return GetStockistListByCompanyPvt(CompanyId, Status);
        }
        private List<StockistModel> GetStockistListByCompanyPvt(int CompanyId, string Status)
        {
            List<StockistModel> model = new List<StockistModel>();
            try
            {
                model = _contextManager.usp_StockistMasterListByCompanyId(CompanyId, Status).Select(c => new StockistModel
                {

                    StockistId = c.StockistId,
                    StockistNo = c.StockistNo,
                    StockistName = c.StockistName,
                    Checked = c.Checked
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, CompanyId, "GetStockistListByCompanyPvt", "Get Stockist List By Company", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Roles Detalis
        public List<RolesModel> GetRolesls(int EmpId)
        {
            return GetRoleDtlsPvt(EmpId);
        }
        private List<RolesModel> GetRoleDtlsPvt(int EmpId)
        {
            List<RolesModel> RoleLst = new List<RolesModel>();
            try
            {
                RoleLst = _contextManager.usp_UserRoleDetails(EmpId).Select(c => new RolesModel
                {
                    EmpId = c.EmpId,
                    RoleId = Convert.ToInt32(c.RoleId),
                    RoleName = c.RoleName
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetRoleDtlsPvt", "Get Roles Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RoleLst;
        }
        #endregion

        #region Get Guard Details
        public List<GuardDetails> GetGuardDetails(int BranchId, int CompId)
        {
            return GetGuardDetailsPvt(BranchId, CompId);
        }
        private List<GuardDetails> GetGuardDetailsPvt(int BranchId, int CompId)
        {
            List<GuardDetails> guardDetails = new List<GuardDetails>();
            try
            {
                guardDetails = _contextManager.usp_GetGuardDetails(BranchId, CompId).Select(c => new GuardDetails
                {
                    EmpId = c.EmpId,
                    EmpName = c.EmpName
                }).ToList();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGuardDetailsPvt", "Get Guard Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return guardDetails;
        }
        #endregion

        #region Create User
        public string CreateUser(CreateUserModel model)
        {
            return CreateUserPvt(model);
        }
        private string CreateUserPvt(CreateUserModel model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));
            try
            {
                RetValue = _contextManager.usp_UserAdd(model.BranchId, model.EmpId, model.RoleIdStr, model.UserName,
                    model.Password, model.EncryptPassword, model.Addedby, obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "CreateUserPvt", "Create User", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            if (RetValue > 0)
            {
                return BusinessCont.SuccessStatus;
            }
            else
            {
                return BusinessCont.msg_exist;
            }
        }
        #endregion

        #region Get Check Stockist Already Available
        public StockistModel GetStockistNoAvailables(string StockistNo)
        {
            return GetStockistNoAvailablesPvt(StockistNo);
        }
        private StockistModel GetStockistNoAvailablesPvt(string StockistNo)
        {
            StockistModel model = new StockistModel();
            try
            {
                model = _contextManager.usp_checkStockistNo(StockistNo).Select(c => new StockistModel
                {                                        
                    StockistNo = c.StockistNo,
                    StockistName = c.StockistName,
                    Flag = c.Flag
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistNoAvailablesPvt", "Get Check StocksitNo Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Check TransporterNo is Available
        public TransporterMasterLst GetTransporterNoAvailables(string TransporterNo)
        {
            return GetTransporterNoAvailablesPvt(TransporterNo);
        }
        private TransporterMasterLst GetTransporterNoAvailablesPvt(string TransporterNo)
        {
            TransporterMasterLst model = new TransporterMasterLst();
            try
            {
                model = _contextManager.usp_checkTransporterMaster(TransporterNo).Select(c => new TransporterMasterLst
                {
                    TransporterNo = c.TransporterNo,
                    TransporterName = c.TransporterName,
                    Flag = c.Flag
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetTransporterNoAvailablesPvt", "Get Check StockistNo Already Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Check Employee Number,Email,Mobile Available
        public AddEmployeeModel GetCheckEmployeeNumberAvilable(int EmpId, string EmpNo, string EmpEmail, string EmpMobNo)
        {
            return GetCheckEmployeeNumberAvilablePvt(EmpId, EmpNo, EmpEmail, EmpMobNo);
        }
        private AddEmployeeModel GetCheckEmployeeNumberAvilablePvt(int EmpId , string EmpNo, string EmpEmail, string EmpMobNo)
        {
            AddEmployeeModel model = new AddEmployeeModel();
            try
            {
                model = _contextManager.usp_checkEmpNo(EmpId, EmpNo, EmpEmail, EmpMobNo).Select(c => new AddEmployeeModel
                {
                    flag = c.Flag,
                    EmpId = c.Empid,
                    EmpNo = c.EmpNo,
                    EmpName = c.EmpName
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckEmployeeNumberAvilablePvt", "Get Check Employee Number Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion
        
        #region Get Check carting Agent Name
        public cartingAgentmodel GetCheckCartingAgentAvilable(string CAName)
        {
            return GetCheckCartingAgentAvilablePvt(CAName);
        }
        private cartingAgentmodel GetCheckCartingAgentAvilablePvt(string CAName)
        {
            cartingAgentmodel model = new cartingAgentmodel();
            try
            {
                model = _contextManager.usp_checkCAName(CAName).Select(c => new cartingAgentmodel
                {
                    flag = c.Flag,
                    CAId = c.CAId,
                    CAName = c.CAName
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckCartingAgentAvilablePvt", "Get check Carting Agent Name", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Check Courier Name
        public CourierMasterLst GetCheckCourierNameAvilable(string CourierName)
        {
            return GetCheckCourierNameAvilablePvt(CourierName);
        }
        private CourierMasterLst GetCheckCourierNameAvilablePvt(string CourierName)
        {
            CourierMasterLst model = new CourierMasterLst();
            try
            {
                model = _contextManager.usp_checkCourierName(CourierName).Select(c => new CourierMasterLst
                {
                    flag = c.Flag,
                    CourierId = c.CourierId,
                }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckCourierNameAvilablePvt", "Get check Courier Name", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Add Edit City Master
        public string AddEditCityMaster(CityMaster model)
        {
            return AddEditCityMasterPvt(model);
        }
        private string AddEditCityMasterPvt(CityMaster model)
        {
            int RetValue = 0;
            ObjectParameter obj = new ObjectParameter("RetValue", typeof(int));

            try
            {
                RetValue = _contextManager.usp_CityMastersAddEdit(model.CityCode,model.StateCode,model.CityName,model.ActiveFlag,model.Action,model.AddedBy,obj);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditCityMaster", "Add Edit City Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
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

        }

    }










