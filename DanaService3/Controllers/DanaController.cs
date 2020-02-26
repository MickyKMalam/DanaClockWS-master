using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace DanaService3.Controllers
{
    [RoutePrefix("api/dana")]
    public class DanaController : ApiController
    {
        // POST api/dana/login
        [Route("login")]
        [HttpPost]
        public dynamic Login(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            DBInfo _dbInfo = new DBInfo();
            Guid Token = Guid.Empty;
            int nOvedID = -1;
            try 
            {
                rslt = _dbInfo.Login(formData.sDomain.ToString(), formData.sUserName.ToString(), formData.sPassword.ToString(), out Token, out nOvedID);
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.token = Token;
            result.currentOvedID = nOvedID;
           
            return result;
        }

        // POST api/dana/absences
        [Route("absences")]
        [HttpPost]
        public dynamic AbsencesByParam(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<Headrut> lstHeadrut = new List<Headrut>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                else
                {
                    lstHeadrut = _dbInfo.GetAbsences((int)formData.nCode);
                }
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.absences = JObject.FromObject(new { lstHeadrut });

            return result;
        }


        // POST api/dana/timesheet
        [Route("ovedData")]
        [HttpPost]
        public dynamic GetOvedData(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<OvedData> lstOvedData = new List<OvedData>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                else
                {
                    if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                    {
                        rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                    }
                    else
                    {
                        lstOvedData = _dbInfo.GetOvedData((int)formData.nOved);

                    }
                }
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.ovedData = JObject.FromObject(new { lstOvedData });

            return result;
        }

        // POST api/dana/timesheet
        [Route("timesheet")]
        [HttpPost]
        public dynamic TimeSheet(dynamic formData)
        {
            //Guid token, int nOved, DateTime dtFrom, DateTime dtTo, out List<TimeSheet> lstTimeSheet
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheet> lstTimeSheet = new List<TimeSheet>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                else
                {
                    if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                    {
                        rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                    }
                    else
                    {
                        lstTimeSheet = _dbInfo.GetTimeSheet((int)formData.nOved, (DateTime)formData.dtFrom, (DateTime)formData.dtTo);
                    }
                }
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.timesheet = JObject.FromObject(new { lstTimeSheet });

            return result;
        }

        // POST api/dana/insertAbsencePeriod
        [Route("insertAbsencePeriod")]
        [HttpPost]
        public dynamic InsertAbsencePeriod(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                if (_dbInfo.requestResult.ResultCode == 0)
                {
                    rslt = _dbInfo.InsertAbsencePeriod((int)formData.nOved, (int)formData.nKodHeadrut, (DateTime)formData.dtFrom, (DateTime)formData.dtTo);
                }
                else
                {
                    rslt = _dbInfo.requestResult;
                }
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);

            return result;
        }

        // POST api/dana/insertUpdateTimeSheet
        [Route("insertUpdateTimeSheet")]
        [HttpPost]
        public dynamic InsertUpdateTimeSheet(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheet> lstTimeSheet = new List<TimeSheet>();
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                if (_dbInfo.requestResult.ResultCode == 0)
                {
                    rslt = _dbInfo.InsertUpdateTimeSheet((int)formData.nOved, 
                            (formData.nTimeSheetID == null ? -1 : (int)formData.nTimeSheetID), 
                            (formData.dtClockDate == null ? new DateTime(1899,1,1) : (DateTime)formData.dtClockDate), 
                            (formData.nKodHeadrut == null ? -1 : (int)formData.nKodHeadrut),
                            (formData.tFrom == null ? new DateTime(1899, 1, 1) : (DateTime)formData.tFrom),
                            (formData.tTo == null ? new DateTime(1899, 1, 1) : (DateTime)formData.tTo),
                            (formData.Hearot) == null ? String.Empty : (string)formData.Hearot,
                        out lstTimeSheet, out lstTimeSheetMonthly);
                }
                else
                {
                    rslt = _dbInfo.requestResult;
                }
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.timeSheet = JObject.FromObject(new { lstTimeSheet });
            result.timeSheetMonthly = JObject.FromObject(new { lstTimeSheetMonthly });
            result.danaResult = JObject.FromObject(rslt);

            return result;
        }

        // POST api/dana/insertUpdateTimeSheetProject
        [Route("insertUpdateTimeSheetProject")]
        [HttpPost]
        public dynamic InsertUpdateTimeSheetProject(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheetProject> lstTimeSheetProject = new List<TimeSheetProject>();
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                if (_dbInfo.requestResult.ResultCode == 0)
                {
                    rslt = _dbInfo.InsertUpdateTimeSheetProject((int)formData.nOved, 
                        (formData.nTimeSheetProjectID == null ? -1 : (int)formData.nTimeSheetProjectID),
                        (formData.dtClockDate == null ? new DateTime(1899, 1, 1) : (DateTime)formData.dtClockDate),
                        (formData.tFrom == null ? new DateTime(1899, 1, 1) : (DateTime)formData.tFrom),
                        (formData.tTo == null ? new DateTime(1899, 1, 1) : (DateTime)formData.tTo),
                        (formData.Shaot == null ? new DateTime(1899, 1, 1) : (DateTime)formData.Shaot),
                        (int)formData.nTaskID,
                        (formData.Hearot) == null ? String.Empty : (string)formData.Hearot, 
                        out lstTimeSheetProject, out lstTimeSheetMonthly);
                }
                else
                {
                    rslt = _dbInfo.requestResult;
                }
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.timeSheetProject = JObject.FromObject(new { lstTimeSheetProject });
            result.timeSheetMonthly = JObject.FromObject(new { lstTimeSheetMonthly });
            result.danaResult = JObject.FromObject(rslt);

            return result;
        }

        // POST api/dana/deleteTimeSheet
        [Route("deleteTimeSheet")]
        [HttpPost]
        public dynamic DeleteTimeSheet(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                //if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                //{
                //    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                //}
                if (_dbInfo.requestResult.ResultCode == 0)
                {
                    //rslt = _dbInfo.DeleteTimeSheet((int)formData.nTimeSheetID);
                    rslt = _dbInfo.DeleteTSP((int)formData.nTimeSheetID, false, out lstTimeSheetMonthly);
                }
                else
                {
                    rslt = _dbInfo.requestResult;
                }
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.timeSheetMonthly = JObject.FromObject(new { lstTimeSheetMonthly });

            return result;
        }

        // POST api/dana/deleteTimeSheetProject
        [Route("deleteTimeSheetProject")]
        [HttpPost]
        public dynamic DeleteTimeSheetProject(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                //if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                //{
                //    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                //}
                if (_dbInfo.requestResult.ResultCode == 0)
                {
                    //rslt = _dbInfo.DeleteTimeSheetProject((int)formData.nTimeSheetProjectID);
                    rslt = _dbInfo.DeleteTSP((int)formData.nTimeSheetProjectID, true, out lstTimeSheetMonthly);
                }
                else
                {
                    rslt = _dbInfo.requestResult;
                }
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.timeSheetMonthly = JObject.FromObject(new { lstTimeSheetMonthly });

            return result;
        }

        // POST api/dana/ovedTasks
        [Route("ovedTasks")]
        [HttpPost]
        public dynamic OvedTasks(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<OvedTasks> lstOvedTasks = new List<OvedTasks>();

            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                lstOvedTasks = _dbInfo.GetOvedTasks((int)formData.nOved);
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.ovedTasks = JObject.FromObject(new { lstOvedTasks });

            return result;
        }

        // Post api/dana/ovdimPermitted
        [Route("ovdimPermitted")]
        [HttpPost]
        public dynamic OvdimPermitted(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<OvdimPermitted> lstOvdimPermitted = new List<OvdimPermitted>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                lstOvdimPermitted = _dbInfo.GetOvdimPermitted();
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.ovdimPermitted = JObject.FromObject(new { lstOvdimPermitted });

            return result;
        }

        // Post api/dana/getDomains
        [Route("getDomains")]
        [HttpPost]
        public dynamic GetDomains(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<Domains> lstDomains = new List<Domains>();
            try
            {
                //DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                DBInfo _dbInfo = new DBInfo();
                lstDomains = _dbInfo.GetDomains();
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.domains = JObject.FromObject(new { lstDomains });

            return result;
        }

        // Post api/dana/timeSheetProject
        [Route("timeSheetProject")]
        [HttpPost]
        public dynamic TimeSheetProject(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheetProject> lstTimeSheetProject = new List<TimeSheetProject>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                lstTimeSheetProject = _dbInfo.GetTimeSheetProject((int)formData.nOved, (DateTime)formData.dtFrom, (DateTime)formData.dtTo);
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.timeSheetProject = JObject.FromObject(new { lstTimeSheetProject });

            return result;
        }
        // Post api/dana/dailyActivity
        [Route("dailyActivity")]
        [HttpPost]
        public dynamic GetDailyActivity(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheet> lstTimeSheet = new List<TimeSheet>();
            List<TimeSheetProject> lstTimeSheetProject = new List<TimeSheetProject>();
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                else if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                else
                {
                    _dbInfo.GetDailyActivity((int)formData.nOved, (DateTime)formData.dtFrom, (DateTime)formData.dtTo, 
                        out lstTimeSheet, out lstTimeSheetProject, out lstTimeSheetMonthly);
                }
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.timeSheet = JObject.FromObject(new { lstTimeSheet });
            result.timeSheetProject = JObject.FromObject(new { lstTimeSheetProject });
            result.timeSheetMonthly = JObject.FromObject(new { lstTimeSheetMonthly });

            return result;
        }
        // POST api/dana/timesheet
        [Route("timeSheetMonthly")]
        [HttpPost]
        public dynamic GetTimeSheetMonthly(dynamic formData)
        {
            //Guid token, int nOved, DateTime dtFrom, DateTime dtTo, out List<TimeSheet> lstTimeSheet
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                else
                {
                    if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                    {
                        rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                    }
                    else
                    {
                        lstTimeSheetMonthly = _dbInfo.GetTimeSheetMonthly((int)formData.nOved, (DateTime)formData.dtFrom, (DateTime)formData.dtTo);
                    }
                }
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.timeSheetMonthly = JObject.FromObject(new { lstTimeSheetMonthly });

            return result;
        }

        // POST api/dana/timesheet
        [Route("monthlySummary")]
        [HttpPost]
        public dynamic GetMonthlySummary(dynamic formData)
        {
            //Guid token, int nOved, DateTime dtFrom, DateTime dtTo, out List<TimeSheet> lstTimeSheet
            DanaRequestResult rslt = new DanaRequestResult();
            List<MonthlySummary> lstMonthlySummary = new List<MonthlySummary>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                else
                {
                    if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                    {
                        rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                    }
                    else
                    {
                        lstMonthlySummary = _dbInfo.GetMonthlySummary((int)formData.nOved, (DateTime)formData.Chodesh);
                    }
                }
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.monthlySummary = JObject.FromObject(new { lstMonthlySummary });

            return result;
        }

        // Post api/dana/monthlyActivity
        [Route("monthlyActivity")]
        [HttpPost]
        public dynamic GetMonthlyActivity(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            int nWorkDays = 0;
            float WorkTime = 0;
            List<MonthlyAbsence> lstMonthlyAbsence = new List<MonthlyAbsence>();
            List<MonthlyTasks> lstMonthlyTasks = new List<MonthlyTasks>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                else if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                else
                {
                    _dbInfo.GetMonthlyActivity((int)formData.nOved, (DateTime)formData.dtFrom, (DateTime)formData.dtTo, 
                        out WorkTime, out nWorkDays, out lstMonthlyAbsence, out lstMonthlyTasks);
                }
            }
            catch (Exception ex)
            {
                DBInfo.AddToLogTable(ex.StackTrace);
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.workTime = WorkTime;
            result.workDays = nWorkDays;
            result.monthlyAbsence = JObject.FromObject(new { lstMonthlyAbsence });
            result.monthlyTasks = JObject.FromObject(new { lstMonthlyTasks });

            return result;
        }
        
        // Post api/dana/monthlyApproval
        [Route("monthlyApproval")]
        [HttpPost]
        public dynamic InsertMonthlyApproval(dynamic formData)
        {
            DanaRequestResult rslt = new DanaRequestResult();
            List<TimeSheetMonthly> lstTimeSheetMonthly = new List<TimeSheetMonthly>();
            try
            {
                DBInfo _dbInfo = new DBInfo((Guid)formData.token);
                if (_dbInfo.requestResult.ResultCode != 0)
                {
                    rslt = _dbInfo.requestResult;
                }
                if (!_dbInfo.IsOvedPermitted((int)formData.nOved))
                {
                    rslt = new DanaRequestResult(RequestResultError.NotPermittedForOved);
                }
                if (_dbInfo.requestResult.ResultCode == 0)
                {
                    rslt = _dbInfo.InsertMonthApproval((int)formData.nOved, (DateTime)formData.dtClockDate, out lstTimeSheetMonthly);
                }
                else
                {
                    rslt = _dbInfo.requestResult;
                }
            }
            catch (Exception ex)
            {
                rslt = new DanaRequestResult(-1, ex.Message);
            }
            dynamic result = new JObject();
            result.danaResult = JObject.FromObject(rslt);
            result.timeSheetMonthly = JObject.FromObject(new { lstTimeSheetMonthly });

            return result;
        }
    }
}
