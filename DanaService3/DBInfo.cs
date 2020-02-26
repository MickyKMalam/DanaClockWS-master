using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DanaService3
{
    public class DBInfo
    {
        public string sConnStr
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["DanaDB"].ConnectionString;
            }
        }
        public int CurrentOvedID;
        public List<OvdimPermitted> ovdimPermitted;
        public DanaRequestResult requestResult;
        public DBInfo(Guid token)
        {
            CurrentOvedID = GetOvedIDByToken(token);
            ovdimPermitted = GetOvdimPermitted();
        }
        public DBInfo()
        {
            //should be used only upon login - for creating token
        }

        private static string MapToList(string sClass, string ColName)
        {
            //It is quite a common situation that DB fields ending with "ID" are spelled "Id". If we reached this point, 
            //this may be the case. So:
            string sDBColName = ColName;
            if (ColName.EndsWith("Id")) sDBColName = ColName.Substring(0, ColName.Length - 2) + "ID";

            if (sClass == "TimeSheet")
            {
                if (sDBColName == "TimeSheetID" || sDBColName == "ClockDate" || sDBColName == "IsMonthApproved"
                        || sDBColName == "InTime" || sDBColName == "OutTime" || sDBColName == "TeurHeadrut"
                        || sDBColName == "Hearot" || sDBColName == "SugYom" || sDBColName == "SugChag"
                        || sDBColName == "TeurSugYom" || sDBColName == "TeurSugChag")
                    return sDBColName;
                if (sDBColName == "SugDivuach") return "KodHeadrut";
            }
            else if (sClass == "TimeSheetMonthly")
            {
                if (sDBColName == "ClockDate" || sDBColName == "TeurHeadrut"
                        || sDBColName == "SugYom" || sDBColName == "SugChag"
                        || sDBColName == "TeurSugYom" || sDBColName == "TeurSugChag"
                        || sDBColName == "TotalWorkTime" || sDBColName == "TotalTaskTime" || sDBColName == "Hefresh"
                        || sDBColName == "DayDescription" || sDBColName == "ErrorDescription")
                    return sDBColName;
                if (sDBColName == "SugDivuach") return "KodHeadrut";
            }
            else if (sClass == "OvedTasks")
            {
                if (sDBColName == "ProjectID" || sDBColName == "ProjectName"
                        || sDBColName == "ERPLakoachID" || sDBColName == "LakoachName"
                        || sDBColName == "LakoachNum" || sDBColName == "ProjectShlavID"
                        || sDBColName == "ShlavTeur" || sDBColName == "TaskTeur")
                    return sDBColName;
                if (sDBColName == "SugProjectShlav") return "nShlavID";
                if (sDBColName == "ProjectModulTaskID") return "nTaskID";
                if (sDBColName == "ProjectModulTaskOvedId") return "ID";
                if (sDBColName == "SugTifuli_bool") return "bIsTifuli";
                if (sDBColName == "HearotYN_bool") return "bMustHaveComments";
            }
            else if (sClass == "OvdimPermitted")
            {
                if (sDBColName == "OvedID" || sDBColName == "ShemPrati" || sDBColName == "ShemMishpacha")
                    return sDBColName;
            }
            else if (sClass == "TimeSheetProject")
            {
                if (sDBColName == "OvedID" || sDBColName == "ProjectID" || sDBColName == "ProjectShlavID"
                        || sDBColName == "ClockDate" || sDBColName == "Shaot"
                        || sDBColName == "FromTime" || sDBColName == "ToTime"
                        || sDBColName == "ERPLakoachID" || sDBColName == "LakoachName")
                    return sDBColName;
                if (sDBColName == "TimeSheetProjectID") return "ID";
                if (sDBColName == "ProjectTeur") return "ProjectName";
                if (sDBColName == "SugProjectShlav") return "nShlavID";
                if (sDBColName == "ProjectShlavTeur") return "ShlavTeur";
                if (sDBColName == "TSPHearot") return "Hearot";
                if (sDBColName == "ProjectModulTaskID") return "nTaskID";
                if (sDBColName == "ProjectModulTaskTeur") return "TaskTeur";
                if (sDBColName == "SugTifuli_bool") return "bIsTifuli";
                if (sDBColName == "MustHaveComments_bool") return "bMustHaveComments";
            }
            else if (sClass == "MonthlySummary")
            {
                if (sDBColName == "Chodesh" || sDBColName == "TotalHours" || sDBColName == "ExpectedHours"
                        || sDBColName == "TimeDiff" || sDBColName == "AhuzTeken" || sDBColName == "OdefHoserSign"
                        || sDBColName == "TotalHoursFormatted" || sDBColName == "ExpectedHoursFormatted"
                        || sDBColName == "IsMonthApproved" || sDBColName == "IsMonthApprovedText")
                    return sDBColName;
                if (sDBColName == "OvedID") return "nOvedID";
                if (sDBColName == "Formatted") return "HefreshFormatted";
            }
            else if (sClass == "MonthlyAbsence")
            {
                if (sDBColName == "KodHeadrut" || sDBColName == "TeurHeadrut" || sDBColName == "CountDays")
                    return sDBColName;
            }
            else if (sClass == "MonthlyTasks")
            {
                if (sDBColName == "ProjectID" || sDBColName == "ShaotFloat"
                        || sDBColName == "ERPLakoachID" || sDBColName == "LakoachName" || sDBColName == "LakoachNum")
                    return sDBColName;
                if (sDBColName == "ProjectTeur") return "ProjectName";
                if (sDBColName == "SugProjectShlav") return "nShlavID";
                if (sDBColName == "ProjectShlavTeur") return "ShlavTeur";
                if (sDBColName == "ProjectModulTaskID") return "nTaskID";
                if (sDBColName == "ProjectModulTaskTeur") return "TaskTeur";
                if (sDBColName == "ProjectShlavID") return "ProjectShlavID";
            }
            else if (sClass == "OvedData")
            {
                //public int OvedID { get; set; }
                //public string ShemMishpacha { get; set; }
                //public string ShemPrati { get; set; }
                //public int MivneIrgunID { get; set; }
                //public int MerkazAlut { get; set; }
                //public string MerkazAlutName { get; set; }
                //public int SugOved { get; set; }
                //public string SugOvedTeur { get; set; }
                //public string Email { get; set; }

                if (sDBColName == "ShemMishpacha" || sDBColName == "ShemPrati"
                        || sDBColName == "MivneIrgunID" || sDBColName == "MerkazAlut" || sDBColName == "MerkazAlutName"
                        || sDBColName == "SugOved" || sDBColName == "SugOvedTeur" || sDBColName == "Email")
                    return sDBColName;
                if (sDBColName == "OvedID") return "nOvedID";
            }
            else if (sClass == "Domains")
            {
                if (sDBColName == "DomainName")
                    return sDBColName;
                if (sDBColName == "DomainID")
                    return "nDomainID";
            }
            return String.Empty;
        }
        private static void AddBoolYNColumn(DataTable dt, string sColumn)
        {
            string sColNew = sColumn + "_bool";
            dt.Columns.Add(sColNew, typeof(bool));
            foreach (DataRow dr in dt.Rows)
                dr[sColNew] = (dr[sColumn].ToString() == "1" ? true : false);
        }

        private int GetOvedIDByToken(Guid token)
        {
            int nOvedID = -1;
            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileTokenCheck", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@Token", token));
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string sErrorCode = reader["ErrorCode"].ToString();
                string sErrorDescription = reader["ErrorDescription"].ToString();
                requestResult = new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
                string sOvedID = reader["OvedID"].ToString();
                if (!Int32.TryParse(sOvedID, out nOvedID))
                    return -1;
            }
            connection.Close();
            return nOvedID;
        }

        public DanaRequestResult Login(string sDomain, string sUserName, string sPassword, out Guid token, out int nOvedID)
        {
            token = Guid.Empty;
            nOvedID = CheckUserInAD(sDomain, sUserName, sPassword);
            if (nOvedID <= 0)
            {
                return new DanaRequestResult(99, "Not Authorized");
            }
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileTokenAdd", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                if (Guid.TryParse(reader["token"].ToString(), out token))
                {
                    connection.Close();
                    return new DanaRequestResult();
                }
            }
            connection.Close();
            return new DanaRequestResult(999, "Unspecified error");
        }

        private int CheckUserInAD(string sDomain, string sUserID, string sPassword)
        {
            //returns ovedID if authorized
            int nOvedID = -1;
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileOvedSearchByUserName", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@Domain", sDomain));
            command.Parameters.Add(new SqlParameter("@LoginName", sUserID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string sOvedID = reader["OvedID"].ToString();
                if (!Int32.TryParse(sOvedID, out nOvedID))
                {
                    connection.Close();
                    return -1;
                }
            }
            connection.Close();
            return nOvedID;
        }

        public List<Domains> GetDomains()
        {
            AddToLogTable("MobileApp: GetDomains");
            List<Domains> lstDomains = new List<Domains>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileGetDomainList", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", -1));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                lstDomains = ConvertDataTable<Domains>(dt);
            }
            else
            {
                AddToLogTable("No results");
            }
            connection.Close();

            return lstDomains;
        }

        public List<OvdimPermitted> GetOvdimPermitted()
        {
            AddToLogTable("MobileApp: GetOvdimPermitted");
            List<OvdimPermitted> lstOvdimPermitted = new List<OvdimPermitted>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileOvedGetPermitted", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                lstOvdimPermitted = ConvertDataTable<OvdimPermitted>(dt);
            }
            else
            {
                AddToLogTable("No results");
            }
            connection.Close();

            return lstOvdimPermitted;
        }

        public bool IsOvedPermitted(int nOvedID)
        {
            if (ovdimPermitted != null)
            {
                OvdimPermitted op = ovdimPermitted.Find(o => o.OvedID == nOvedID);
                if (op != null) return true;
            }
            return false;
        }

        public List<Headrut> GetAbsences(int nKod)
        {
            List<Headrut> lstHeadrut = new List<Headrut>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileHeadrutGet", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            Headrut headrut = null;

            while (reader.Read())
            {
                headrut = new Headrut();
                headrut.Kod = int.Parse(reader["Kod"].ToString());
                headrut.Shem = reader["Shem"].ToString();
                headrut.bCanUseWithWorkHours = bool.Parse(reader["CanUseWithWorkHours"].ToString());
                lstHeadrut.Add(headrut);
            }
            connection.Close();
            if (nKod > 0)
                return lstHeadrut.Where(a => a.Kod == nKod).ToList<Headrut>();
            return lstHeadrut;
        }

        public List<OvedData> GetOvedData(int nOvedID)
        {
            AddToLogTable("MobileApp: GetOvedData");
            List<OvedData> lstOvedData = new List<OvedData>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspOvdimGet", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedId", nOvedID));
            command.Parameters.Add(new SqlParameter("@OvedIdCurrent", CurrentOvedID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                lstOvedData = ConvertDataTable<OvedData>(dt);
            }
            else
            {
                AddToLogTable("No results");
            }
            connection.Close();

            return lstOvedData;
        }

        public List<TimeSheet> GetTimeSheet(int nOvedID, DateTime dtFrom, DateTime dtTo)
        {
            //@OvedID INT, @DateFrom DATETIME, @DateTo DATETIME, @CurrentOvedID INT
            AddToLogTable("MobileApp: GetTimeSheet");
            List<TimeSheet> lstTimeSheet = new List<TimeSheet>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileTimeSheetGet", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@DateFrom", dtFrom));
            command.Parameters.Add(new SqlParameter("@DateTo", dtTo));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                lstTimeSheet = ConvertDataTable<TimeSheet>(dt);
            }
            else
            {
                AddToLogTable("No results");
            }
            connection.Close();

            return lstTimeSheet;
        }

        public List<TimeSheetMonthly> GetTimeSheetMonthly(int nOvedID, DateTime dtFrom, DateTime dtTo)
        {
            //@OvedID INT, @DateFrom DATETIME, @DateTo DATETIME, @CurrentOvedID INT
            AddToLogTable("MobileApp: GetTimeSheetMonthly");
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileTimeSheetSummary", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@DateFrom", dtFrom));
            command.Parameters.Add(new SqlParameter("@DateTo", dtTo));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                lstTimeSheetMonthly = ConvertDataTable<TimeSheetMonthly>(dt);
            }
            else
            {
                AddToLogTable("No results");
            }
            connection.Close();

            return lstTimeSheetMonthly;
        }

        public DanaRequestResult InsertMonthApproval(int nOvedID, DateTime dtClockDate, out List<TimeSheetMonthly> lstTimeSheetMonthly)
        {
            //CREATE PROC dbo.aspMobileMonthApproval(@OvedID INT, @ClockDate DATETIME, @CurrentOvedID INT)

            lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileMonthApproval", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@ClockDate", dtClockDate));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(ds);

            if (ds == null)
            {
                AddToLogTable("No dataset");
            }
            else if (ds.Tables.Count < 2)
            {
                AddToLogTable("Tables missing");
            }
            else
            {
                lstTimeSheetMonthly = ConvertDataTable<TimeSheetMonthly>(ds.Tables[0]);

                if (ds.Tables[1].Rows.Count < 1)
                {
                    AddToLogTable("Result - no rows");
                }
                else
                {
                    DataRow dr = ds.Tables[1].Rows[0];
                    string sErrorCode = dr["ErrorCode"].ToString();
                    string sErrorDescription = dr["ErrorDescription"].ToString();
                    connection.Close();
                    return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
                }
            }
            connection.Close();
            return new DanaRequestResult(999, "Unspecified error");
        }

        public List<OvedTasks> GetOvedTasks(int nOvedID)
        {
            //@OvedID INT, @DateFrom DATETIME, @DateTo DATETIME, @CurrentOvedID INT
            AddToLogTable("MobileApp: GetOvedTasks");
            List<OvedTasks> lstOvedTasks = new List<OvedTasks>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspProjectOvedGetAllTasks", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                AddBoolYNColumn(dt, "SugTifuli");
                AddBoolYNColumn(dt, "HearotYN");
                lstOvedTasks = ConvertDataTable<OvedTasks>(dt);
                foreach (OvedTasks ovedTasks in lstOvedTasks)
                    ovedTasks.nOvedID = nOvedID;
            }
            else
            {
                AddToLogTable("No results");
            }
            connection.Close();

            return lstOvedTasks;
        }
        public DanaRequestResult InsertAbsencePeriod(int nOvedID, int nKodHeadrut, DateTime dtFrom, DateTime dtTo)
        {
            //ALTER PROC dbo.aspMobileAbsenceInsert(@OvedID INT, @SugDivuach INT, @DateFrom DATETIME, @DateTo DATETIME, @CurrentOvedID INT)
            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileAbsenceInsert", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@SugDivuach", nKodHeadrut));
            command.Parameters.Add(new SqlParameter("@DateFrom", dtFrom));
            command.Parameters.Add(new SqlParameter("@DateTo", dtTo));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string sErrorCode = reader["ErrorCode"].ToString();
                string sErrorDescription = reader["ErrorDescription"].ToString();
                connection.Close();
                return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
            }
            connection.Close();
            return new DanaRequestResult(999, "Unspecified error");
        }

        public DanaRequestResult InsertUpdateTimeSheet(int nOvedID, int nTimeSheetID, DateTime dtClockDate, int nKodHeadrut, DateTime tFrom, DateTime tTo, string Hearot, 
            out List<TimeSheet> lstTimeSheet, out List<TimeSheetMonthly> lstTimeSheetMonthly)
        {
            //ALTER PROC dbo.aspMobileTimeSheetInsertUpdate 
            //(@TimeSheetID INT, @OvedID INT, @ClockDate DATETIME, @SugDivuach INT, @TimeFrom DATETIME, @TimeTo DATETIME, @CurrentOvedID INT)

            lstTimeSheet = new List<TimeSheet>();
            lstTimeSheetMonthly = new List<TimeSheetMonthly>();

            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileTimeSheetInsertUpdate", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@TimeSheetID", nTimeSheetID));
            command.Parameters.Add(new SqlParameter("@SugDivuach", nKodHeadrut));
            command.Parameters.Add(new SqlParameter("@ClockDate", dtClockDate));
            command.Parameters.Add(new SqlParameter("@TimeFrom", tFrom));
            command.Parameters.Add(new SqlParameter("@TimeTo", tTo));
            command.Parameters.Add(new SqlParameter("@Hearot", Hearot));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(ds);

            string sDSError = "Unspecified error";
            if (ds == null)
            {
                sDSError="No dataset";
            }
            else if (ds.Tables.Count < 3)
            {
                sDSError = "Tables missing";
            }
            else
            {
                lstTimeSheet = ConvertDataTable<TimeSheet>(ds.Tables[0]);
                lstTimeSheetMonthly = ConvertDataTable<TimeSheetMonthly>(ds.Tables[1]);
                if (ds.Tables[2].Rows.Count < 1)
                {
                    sDSError = "Result - no rows";
                }
                else
                {//OK
                    DataRow dr = ds.Tables[2].Rows[0];
                    string sErrorCode = dr["ErrorCode"].ToString();
                    string sErrorDescription = dr["ErrorDescription"].ToString();
                    connection.Close();
                    return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
                }
            }
            AddToLogTable(sDSError);
            connection.Close();
            return new DanaRequestResult(999, sDSError);
        }

        public DanaRequestResult InsertUpdateTimeSheetProject(int nOvedID, int nTimeSheetProjectID, DateTime dtClockDate,
            DateTime tFrom, DateTime tTo, DateTime Shaot, int nProjectModulTaskID, string Hearot, 
            out List<TimeSheetProject> lstTimeSheetProject, out List<TimeSheetMonthly> lstTimeSheetMonthly)
        {
            //CREATE PROC dbo.aspMobileTimeSheetProjectInsertUpdate(@TimeSheetProjectID INT, @OvedID INT, @ClockDate DATETIME,
            //    @TimeFrom DATETIME, @TimeTo DATETIME, @Shaot DATETIME, @ProjectModulTaskID INT, @Hearot VARCHAR(512), @CurrentOvedID INT)

            lstTimeSheetProject = new List<TimeSheetProject>();
            lstTimeSheetMonthly = new List<TimeSheetMonthly>();

            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileTimeSheetProjectInsertUpdate", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@TimeSheetProjectID", nTimeSheetProjectID));
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@ClockDate", dtClockDate));
            command.Parameters.Add(new SqlParameter("@TimeFrom", tFrom));
            command.Parameters.Add(new SqlParameter("@TimeTo", tTo));
            command.Parameters.Add(new SqlParameter("@Shaot", Shaot));
            command.Parameters.Add(new SqlParameter("@ProjectModulTaskID", nProjectModulTaskID));
            command.Parameters.Add(new SqlParameter("@Hearot", Hearot));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(ds);

            string sDSError = "Unspecified error";
            if (ds == null)
            {
                sDSError = "No dataset";
            }
            else if (ds.Tables.Count < 3)
            {
                sDSError = "Tables missing";
            }
            else
            {
                DataTable dtTSP = ds.Tables[0];
                AddBoolYNColumn(dtTSP, "SugTifuli");
                AddBoolYNColumn(dtTSP, "MustHaveComments");
                lstTimeSheetProject = ConvertDataTable<TimeSheetProject>(dtTSP);
                if (ds.Tables[2].Rows.Count < 1)
                {
                    sDSError = "Result - no rows";
                }
                else
                {
                    lstTimeSheetMonthly = ConvertDataTable<TimeSheetMonthly>(ds.Tables[1]);
                    DataRow dr = ds.Tables[2].Rows[0];
                    string sErrorCode = dr["ErrorCode"].ToString();
                    string sErrorDescription = dr["ErrorDescription"].ToString();
                    connection.Close();
                    return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
                }
            }
            AddToLogTable(sDSError);
            connection.Close();
            return new DanaRequestResult(999, sDSError);
        }

        public DanaRequestResult DeleteTSP(int nID, bool bIsTimeSheetProject, out List<TimeSheetMonthly> lstTimeSheetMonthly)
        {
            lstTimeSheetMonthly = new List<TimeSheetMonthly>();

            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand(bIsTimeSheetProject ? "aspMobileTimeSheetProjectDelete" : "aspMobileTimeSheetDelete", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter(bIsTimeSheetProject ? "@TimeSheetProjectID" : "@TimeSheetID", nID));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(ds);

            string sDSError = "Unspecified error";
            if (ds == null)
            {
                sDSError = "No dataset";
            }
            else if (ds.Tables.Count < 2)
            {
                sDSError = "Tables missing";
            }
            else
            {
                if (ds.Tables[1].Rows.Count < 1)
                {
                    sDSError = "Result - no rows";
                }
                else
                {
                    lstTimeSheetMonthly = ConvertDataTable<TimeSheetMonthly>(ds.Tables[0]);
                    DataRow dr = ds.Tables[1].Rows[0];
                    string sErrorCode = dr["ErrorCode"].ToString();
                    string sErrorDescription = dr["ErrorDescription"].ToString();
                    connection.Close();
                    return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
                }
            }
            AddToLogTable(sDSError);
            connection.Close();
            return new DanaRequestResult(999, sDSError);
        }

        public DanaRequestResult InsertUpdateTimeSheet(int nOvedID, int nTimeSheetID, DateTime dtClockDate, int nKodHeadrut, DateTime tFrom, DateTime tTo)
        {//NOT USED
            //ALTER PROC dbo.aspMobileTimeSheetInsertUpdate 
            //(@TimeSheetID INT, @OvedID INT, @ClockDate DATETIME, @SugDivuach INT, @TimeFrom DATETIME, @TimeTo DATETIME, @CurrentOvedID INT)

            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileTimeSheetInsertUpdate", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@TimeSheetID", nTimeSheetID));
            command.Parameters.Add(new SqlParameter("@SugDivuach", nKodHeadrut));
            command.Parameters.Add(new SqlParameter("@ClockDate", dtClockDate));
            command.Parameters.Add(new SqlParameter("@TimeFrom", tFrom));
            command.Parameters.Add(new SqlParameter("@TimeTo", tTo));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string sErrorCode = reader["ErrorCode"].ToString();
                string sErrorDescription = reader["ErrorDescription"].ToString();
                connection.Close();
                return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
            }
            connection.Close();
            return new DanaRequestResult(999, "Unspecified error");
        }

        public DanaRequestResult InsertUpdateTimeSheetProject(int nOvedID, int nTimeSheetProjectID, DateTime dtClockDate,
            DateTime tFrom, DateTime tTo, DateTime Shaot, int nProjectModulTaskID, string Hearot)
        {//NOT USED
            //CREATE PROC dbo.aspMobileTimeSheetProjectInsertUpdate(@TimeSheetProjectID INT, @OvedID INT, @ClockDate DATETIME,
            //    @TimeFrom DATETIME, @TimeTo DATETIME, @Shaot DATETIME, @ProjectModulTaskID INT, @Hearot VARCHAR(512), @CurrentOvedID INT)

            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileTimeSheetProjectInsertUpdate", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@TimeSheetID", nTimeSheetProjectID));
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@ClockDate", dtClockDate));
            command.Parameters.Add(new SqlParameter("@TimeFrom", tFrom));
            command.Parameters.Add(new SqlParameter("@TimeTo", tTo));
            command.Parameters.Add(new SqlParameter("@Shaot", Shaot));
            command.Parameters.Add(new SqlParameter("@ProjectModulTaskID", nProjectModulTaskID));
            command.Parameters.Add(new SqlParameter("@Hearot", Hearot));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string sErrorCode = reader["ErrorCode"].ToString();
                string sErrorDescription = reader["ErrorDescription"].ToString();
                connection.Close();
                return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
            }
            connection.Close();
            return new DanaRequestResult(999, "Unspecified error");
        }

        public DanaRequestResult DeleteTimeSheet(int nTimeSheetID)
        {//NOT USED - Use DeleteTSP whth flag instead
            //CREATE PROC dbo.aspMobileTimeSheetDelete(@TimeSheetID INT, @CurrentOvedID INT) 

            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileTimeSheetDelete", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@TimeSheetID", nTimeSheetID));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string sErrorCode = reader["ErrorCode"].ToString();
                string sErrorDescription = reader["ErrorDescription"].ToString();
                connection.Close();
                return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
            }
            connection.Close();
            return new DanaRequestResult(999, "Unspecified error");
        }
        public DanaRequestResult DeleteTimeSheetProject(int nTimeSheetProjectID)
        {//NOT USED - Use DeleteTSP whth flag instead
            //CREATE PROC dbo.aspMobileTimeSheetProjectDelete(@TimeSheetID INT, @CurrentOvedID INT) 

            SqlConnection connection = new SqlConnection(sConnStr);
            SqlCommand command = new SqlCommand("aspMobileTimeSheetProjectDelete", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@TimeSheetID", nTimeSheetProjectID));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string sErrorCode = reader["ErrorCode"].ToString();
                string sErrorDescription = reader["ErrorDescription"].ToString();
                connection.Close();
                return new DanaRequestResult(Int32.Parse(sErrorCode), sErrorDescription);
            }
            connection.Close();
            return new DanaRequestResult(999, "Unspecified error");
        }

        public List<TimeSheetProject> GetTimeSheetProject(int nOvedID, DateTime dtFrom, DateTime dtTo)
        {
            //@OvedID INT, @DateFrom DATETIME, @DateTo DATETIME
            AddToLogTable("MobileApp: GetTimeSheetProject");
            List<TimeSheetProject> lstTimeSheetProject = new List<TimeSheetProject>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspTimeSheetProjectGet", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@ClockDateFrom", dtFrom));
            command.Parameters.Add(new SqlParameter("@ClockDateTo", dtTo));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                lstTimeSheetProject = ConvertDataTable<TimeSheetProject>(dt);
            }
            else
            {
                AddToLogTable("No results");
            }
            connection.Close();

            return lstTimeSheetProject;
        }

        public void GetDailyActivity(int nOvedID, DateTime dtFrom, DateTime dtTo, out List<TimeSheet> lstTimeSheet, 
            out List<TimeSheetProject> lstTimeSheetProject, out List<TimeSheetMonthly> lstTimeSheetMonthly)
        {
            //@OvedID INT, @DateFrom DATETIME, @DateTo DATETIME
            AddToLogTable("MobileApp: GetDailyActivity");
            lstTimeSheet = new List<TimeSheet>();
            lstTimeSheetProject = new List<TimeSheetProject>();
            lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileDailyActivityGet", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@DateFrom", dtFrom));
            command.Parameters.Add(new SqlParameter("@DateTo", dtTo));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(ds);

            if (ds == null)
            {
                AddToLogTable("No dataset");
            }
            else if (ds.Tables.Count == 0)
            {
                AddToLogTable("No tables");
            }
            else
            {
                lstTimeSheet = ConvertDataTable<TimeSheet>(ds.Tables[0]);
                if (ds.Tables.Count <3)
                {
                    AddToLogTable("TSP - no table found");
                }
                else
                {
                    DataTable dtTSP = ds.Tables[1];
                    AddBoolYNColumn(dtTSP, "SugTifuli");
                    AddBoolYNColumn(dtTSP, "MustHaveComments");
                    lstTimeSheetProject = ConvertDataTable<TimeSheetProject>(dtTSP);

                    lstTimeSheetMonthly = ConvertDataTable<TimeSheetMonthly>(ds.Tables[2]);

                }
            }
            connection.Close();
        }

        public List<MonthlySummary> GetMonthlySummary(int nOvedID, DateTime Chodesh)
        {
            List<MonthlySummary> lstMonthlySummary = new List<MonthlySummary>();
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspTimeSheetGetByTekenSikumHodshi", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@Chodesh", Chodesh));
            command.Parameters.Add(new SqlParameter("@OvedIDCurrent", this.CurrentOvedID));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            if (reader.HasRows)
            {
                dt.Load(reader);
                lstMonthlySummary = ConvertDataTable<MonthlySummary>(dt);
            }
            else
            {
                AddToLogTable("No results from aspTimeSheetGetByTekenSikumHodshi");
            }
            connection.Close();

            return lstMonthlySummary;
        }

        public void GetMonthlyActivity(int nOvedID, DateTime dtFrom, DateTime dtTo, 
            out float WorkTime, out int CountWorkDays,
            out List<MonthlyAbsence> lstMonthlyAbsence, out List<MonthlyTasks> lstMonthlyTasks)
        {
            //@OvedID INT, @DateFrom DATETIME, @DateTo DATETIME
            AddToLogTable("MobileApp: GetMonthlyActivity");
            lstMonthlyAbsence = new List<MonthlyAbsence>();
            lstMonthlyTasks = new List<MonthlyTasks>();
            WorkTime = 0;
            CountWorkDays = 0;
            SqlConnection connection = new SqlConnection(sConnStr);

            SqlCommand command = new SqlCommand("aspMobileMonthlyActivityGet", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OvedID", nOvedID));
            command.Parameters.Add(new SqlParameter("@DateFrom", dtFrom));
            command.Parameters.Add(new SqlParameter("@DateTo", dtTo));
            command.Parameters.Add(new SqlParameter("@CurrentOvedID", CurrentOvedID));

            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.Fill(ds);

            if (ds == null)
            {
                AddToLogTable("No dataset");
            }
            else if (ds.Tables.Count == 0)
            {
                AddToLogTable("No tables");
            }
            else
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (!float.TryParse(ds.Tables[0].Rows[0]["WorkTime"].ToString(), out WorkTime))
                        WorkTime = 0;
                    if (!int.TryParse(ds.Tables[0].Rows[0]["WorkDays"].ToString(), out CountWorkDays))
                        CountWorkDays = 0;
                }
                if (ds.Tables.Count < 3)
                {
                    AddToLogTable("Tables missing");
                }
                else
                {
                    lstMonthlyAbsence = ConvertDataTable<MonthlyAbsence>(ds.Tables[1]);
                    lstMonthlyTasks = ConvertDataTable<MonthlyTasks>(ds.Tables[2]);
                }
            }
            connection.Close();
        }

        public static void AddToLogTable(string s)
        {
            string ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DanaDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(ConnString);
            SqlCommand command = new SqlCommand("insert into dbo.zzLog(Txt) values('" + s.Replace("'", "''") + "')", connection);
            command.CommandType = System.Data.CommandType.Text;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToLower() == MapToList(temp.Name, column.ColumnName).ToLower())
                    {
                        if (dr[column.ColumnName] != System.DBNull.Value)
                            pro.SetValue(obj, dr[column.ColumnName], null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}