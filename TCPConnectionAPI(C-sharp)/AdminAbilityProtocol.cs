using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;
using System;
using System.Collections.Generic;

namespace TCPConnectionAPI_C_sharp_
{
    public class AdminAbilityProtocol : IAdminAbilityProtocol
    {
        protected IMainDBPermission dbContext =
            new DatabaseContext();

        public bool BanClientsWhere(Func<Client, bool> comparer)
        {
            var buf = dbContext.FindClientsWhere(comparer);
            if (buf.Count == 0) return false;
            else
            {
                foreach (var item in buf)
                {
                    item.UserStatus = Status.Banned;
                    dbContext.UpdateClient(item);
                }
                return true;
            }
        }

        public bool BanExpertsWhere(Func<Expert, bool> comparer)
        {
            var buf = dbContext.FindExpertsWhere(comparer);
            if (buf.Count == 0) return false;
            else
            {
                foreach (var item in buf)
                {
                    item.UserStatus = Status.Banned;
                    dbContext.UpdateExpert(item);
                }
                return true;
            }
        }

        public bool CreateCreditor(Creditor obj)
        {
            dbContext.CreateCreditor(obj);
            if (obj.Id > 0) return true;
            else return false;
        }

        public bool DeleteClientsWhere(Func<Client, bool> comparer)
        {
            return dbContext.DeleteClientsWhere(comparer);
        }

        public bool DeleteCreditorsWhere(Func<Creditor, bool> sampler)
        {
            return dbContext.DeleteCreditorWhere(sampler);
        }

        public bool DeleteExpertsWhere(Func<Expert, bool> comparer)
        {
            return dbContext.DeleteExpertsWhere(comparer);
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public List<Admin> FindAdminsWhere(Func<Admin, bool> comparer)
        {
            return dbContext.FindAdminsWhere(comparer);
        }

        public List<Client> FindClientsWhere(Func<Client, bool> comparer)
        {
            return dbContext.FindClientsWhere(comparer);
        }

        public List<Expert> FindExpertsWhere(Func<Expert, bool> comparer)
        {
            return dbContext.FindExpertsWhere(comparer);
        }

        public List<Creditor> FindCreditorsWhere(Func<Creditor, bool> comparer)
        {
            return dbContext.FindCreditorWhere(comparer);
        }

        public bool ModifyCreditor(Creditor newVesrion)
        {
            return dbContext.UpdateCreditor(newVesrion);
        }

        public bool UnbanClientsWhere(Func<Client, bool> comparer)
        {
            var buf = dbContext.FindClientsWhere(comparer);
            if (buf == null) return false;
            else
            {
                foreach (var item in buf)
                {
                    if (item.UserStatus == Status.Banned)
                    {
                        item.UserStatus = Status.NotBanned;
                    }
                }
                return true;
            }
        }

        public bool UnbanExpertsWhere(Func<Expert, bool> comparer)
        {
            var buf = dbContext.FindExpertsWhere(comparer);
            if (buf == null) return false;
            else
            {
                foreach (var item in buf)
                {
                    if (item.UserStatus == Status.Banned)
                    {
                        item.UserStatus = Status.NotBanned;
                    }
                }
                return true;
            }
        }

        public string CreateReport()
        {
            return ReportCreator.CreateReportAboutVehicles();
        }

        public bool ModifyExpert(Expert newVersion)
        {
            return dbContext.UpdateExpert(newVersion);
        }

        public bool ModifyClient(Client newVersion)
        {
            return dbContext.UpdateClient(newVersion);
        }

        public int RegisterNewClient(Client obj)
        {
            return dbContext.CreateClient(obj);
        }

        public int RegisterNewAdmin(Admin obj)
        {
            return dbContext.CreateAdmin(obj);
        }

        public int RegisterNewExpert(Expert obj)
        {
            return dbContext.CreateExpert(obj);
        }

        public bool CreateDetail(Detail obj)
        {
            if (dbContext.CreateDetail(obj) > 0) return true;
            else return false;
        }

        public bool ModifyDetail(Detail newVesrion)
        {
            return dbContext.UpdateDetail(newVesrion);
        }

        public bool DeleteDetailsWhere(Func<Detail, bool> sampler)
        {
            return dbContext.DeleteDetailWhere(sampler);
        }

        public List<Creditor> FindCreditorWhere(Func<Creditor, bool> comparer)
        {
            return dbContext.FindCreditorWhere(comparer);
        }

        public List<Detail> FindDetailsWhere(Func<Detail, bool> comparer)
        {
            return dbContext.FindDetailsWhere(comparer);
        }
    }
}
