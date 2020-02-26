using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DanaService3
{
    public enum RequestResultError
    {
        Ok = 0,
        TokenNotFound = 1,
        EmployeeNotFound = 2,
        TokenExpired = 3,
        PermissionDenied = 4,
        NotPermittedForOved = 5
    }
    public class DanaRequestResult
    {
        public int ResultCode { get; set; }
        public string ErrorDescription { get; set; }
        public DanaRequestResult()
        {
            ResultCode = (int)RequestResultError.Ok;
            ErrorDescription = String.Empty;
        }
        public DanaRequestResult(int _code, string _description)
        {
            ResultCode = _code;
            ErrorDescription = _description;
        }
        public DanaRequestResult(RequestResultError e)
        {
            switch (e)
            {
                case RequestResultError.Ok:
                    ErrorDescription = String.Empty;
                    break;
                case RequestResultError.TokenNotFound:
                    ErrorDescription = "Token not found";
                    break;
                case RequestResultError.EmployeeNotFound:
                    ErrorDescription = "Employee not found";
                    break;
                case RequestResultError.TokenExpired:
                    ErrorDescription = "Token expired";
                    break;
                case RequestResultError.PermissionDenied:
                    ErrorDescription = "Permission denied";
                    break;
                case RequestResultError.NotPermittedForOved:
                    ErrorDescription = "Employee permission denied";
                    break;
                default:
                    ErrorDescription = "Unknown Code";
                    break;
            }
            ResultCode = (int)e;
        }
    }

    public class Headrut
    {
        public int Kod { get; set; }
        public string Shem { get; set; }
        public bool bCanUseWithWorkHours { get; set; }
    }
    public class TimeSheet
    {
        public int TimeSheetID { get; set; }
        public DateTime ClockDate { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public int KodHeadrut { get; set; }
        public string TeurHeadrut { get; set; }
        public string Hearot { get; set; }
        public int SugYom { get; set; }
        public string TeurSugYom { get; set; }
        public int SugChag { get; set; }
        public string TeurSugChag { get; set; }
        public int IsMonthApproved { get; set; }
    }
    public class OvedTasks
    {
        public int nOvedID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int ERPLakoachID { get; set; }
        public string LakoachName { get; set; }
        public string LakoachNum { get; set; }
        public int ProjectShlavID { get; set; }
        public int nShlavID { get; set; }
        public string ShlavTeur { get; set; }
        public int nTaskID { get; set; }
        public string TaskTeur { get; set; }
        public int ID { get; set; }
        public bool bIsTifuli { get; set; }
        public bool bMustHaveComments { get; set; }
        //ProjectID INT, ProjectName VARCHAR(256), ERPLakoachId INT, LakoachName VARCHAR(256), LakoachNum VARCHAR(64), 
        //ProjectShlavId INT, SugProjectShlav INT, ShlavTeur VARCHAR(128), ProjectModulTaskId INT, TaskTeur VARCHAR(128), 
        //ProjectModulTaskOvedId INT, SugTifuli INT, HearotYN INT
    }
    public class OvdimPermitted
    {
        public int OvedID { get; set; }
        public string ShemPrati { get; set; }
        public string ShemMishpacha { get; set; }
    }
    public class TimeSheetProject
    {
        public int ID { get; set; }
        public int OvedID { get; set; }
        public DateTime ClockDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public DateTime Shaot { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int ERPLakoachID { get; set; }
        public string LakoachName { get; set; }
        public int ProjectShlavID { get; set; }
        public int nShlavID { get; set; }
        public string ShlavTeur { get; set; }
        public int nTaskID { get; set; }
        public string TaskTeur { get; set; }
        public string Hearot { get; set; }
        public bool bIsTifuli { get; set; }
        public bool bMustHaveComments { get; set; }
    }
    public class TimeSheetMonthly
    {
        public DateTime ClockDate { get; set; }
        public int KodHeadrut { get; set; }
        public string TeurHeadrut { get; set; }
        public int SugYom { get; set; }
        public string TeurSugYom { get; set; }
        public int SugChag { get; set; }
        public string TeurSugChag { get; set; }
        public DateTime TotalWorkTime { get; set; }
        public DateTime TotalTaskTime { get; set; }
        public string Hefresh { get; set; } //signed
        public string DayDescription { get; set; }
        public string ErrorDescription { get; set; }
    }
    public class MonthlySummary
    {
        public int nOvedID { get; set; }
        public DateTime Chodesh { get; set; }
        public DateTime TotalHours { get; set; }
        public DateTime ExpectedHours { get; set; }
        public DateTime TimeDiff { get; set; }
        public int AhuzTeken { get; set; }
        public string OdefHoserSign { get; set; }
        public string TotalHoursFormatted { get; set; }
        public string ExpectedHoursFormatted { get; set; }
        public string HefreshFormatted { get; set; }
        public int IsMonthApproved { get; set; }
        public string IsMonthApprovedText { get; set; }
    }
    public class MonthlyAbsence
    {
        public int KodHeadrut { get; set; }
        public string TeurHeadrut { get; set; }
        public int CountDays { get; set; }
    }
    public class MonthlyTasks
    {
        public double ShaotFloat { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int ERPLakoachID { get; set; }
        public string LakoachName { get; set; }
        public string LakoachNum { get; set; }
        public int ProjectShlavID { get; set; }
        public int nShlavID { get; set; }
        public string ShlavTeur { get; set; }
        public int nTaskID { get; set; }
        public string TaskTeur { get; set; }
    }
    public class OvedData
    {
        public int nOvedID { get; set; }
        public string ShemMishpacha { get; set; }
        public string ShemPrati { get; set; }
        public int MivneIrgunID { get; set; }
        public int MerkazAlut { get; set; }
        public string MerkazAlutName { get; set; }
        public int SugOved { get; set; }
        public string SugOvedTeur { get; set; }
        public string Email { get; set; }
    }
    public class Domains
    {
        public int nDomainID { get; set; }
        public string DomainName { get; set; }
    }
}