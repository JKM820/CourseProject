using DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TCPConnectionAPI_C_sharp_
{
    class DatabaseContext : IMainDBPermission
    {
        static private EntityModelContainer1 DBconnection;

        static private int amountOfCommections;

        public DatabaseContext()
        {
            if (amountOfCommections == 0)
            {
                DBconnection = new EntityModelContainer1();
            }
            amountOfCommections++;
        }

        virtual protected bool CheckIfLoginExists(string login)
        {
            var admins = DBconnection.AdminSet.Where(c => c.Login == login).ToList();
            if (admins.Count > 0) return true;
            var clients = DBconnection.ClientSet.Where(c => c.Login == login).ToList();
            if (clients.Count > 0) return true;
            var experts = DBconnection.ExpertSet.Where(c => c.Login == login).ToList();
            if (experts.Count > 0) return true;
            return false;
        }

        virtual public int CreateAdmin(Admin newAdmin)
        {
            if (CheckIfLoginExists(newAdmin.Login)) { return 0; }
            DBconnection.Configuration.ProxyCreationEnabled = true;
            DBconnection.AdminSet.Add(newAdmin);
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return newAdmin.Id;
        }

        virtual public int CreateClient(Client newClient)
        {
            if (CheckIfLoginExists(newClient.Login)) { return 0; }
            DBconnection.Configuration.ProxyCreationEnabled = true;
            DBconnection.ClientSet.Add(newClient);
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return newClient.Id;
        }

        virtual public int CreateExpert(Expert newExpert)
        {
            if (CheckIfLoginExists(newExpert.Login)) { return 0; }
            DBconnection.Configuration.ProxyCreationEnabled = true;
            DBconnection.ExpertSet.Add(newExpert);
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return newExpert.Id;
        }

        virtual public int CreateCreditor(Creditor obj)
        {
            DBconnection.Configuration.ProxyCreationEnabled = true;
            DBconnection.CreditorSet.Add(obj);
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return obj.Id;
        }

        virtual public bool DeleteClientsWhere(Func<Client, bool> comparer)
        {
            var buf = DBconnection.ClientSet.Where(comparer).ToList();
            DBconnection.Configuration.ProxyCreationEnabled = true;
            if (buf.Count == 0) { return false; }
            else
            {
                DBconnection.ClientSet.RemoveRange(buf); DBconnection.SaveChanges(); DBconnection.Configuration.ProxyCreationEnabled = false;
                return true;
            }
        }

        virtual public bool DeleteExpertsWhere(Func<Expert, bool> comparer)
        {
            var buf = FindExpertsWhere(comparer);
            if (buf.Count == 0) { return false; }
            else
            {
                DBconnection.Configuration.ProxyCreationEnabled = true;
                DBconnection.ExpertSet.RemoveRange(buf);
                DBconnection.Configuration.ProxyCreationEnabled = false;
                DBconnection.SaveChanges();
                return true;
            }
        }

        virtual public bool DeleteCreditorWhere(Func<Creditor, bool> comparer)
        {
            var planeSet = FindCreditorWhere(comparer);
            if (planeSet.Count == 0) { return false; }
            else
            {
                DBconnection.Configuration.ProxyCreationEnabled = true;
                DBconnection.CreditorSet.RemoveRange(planeSet);
                DBconnection.Configuration.ProxyCreationEnabled = false;
                DBconnection.SaveChanges();
                return true;
            }
        }

        virtual public void Dispose()
        {
            amountOfCommections--;
            if (amountOfCommections == 0)
            {
                DBconnection.Dispose();
            }
        }

        virtual public List<Admin> FindAdminsWhere(Func<Admin, bool> comparer)
        {
            return DBconnection.AdminSet.Where(comparer).ToList();
        }

        virtual public List<Client> FindClientsWhere(Func<Client, bool> comparer)
        {
            return DBconnection.ClientSet.Where(comparer).ToList();
        }

        virtual public List<Expert> FindExpertsWhere(Func<Expert, bool> comparer)
        {
            return DBconnection.ExpertSet
                .Where(comparer)
                .ToList();
        }

        virtual public List<Creditor> FindCreditorWhere(Func<Creditor, bool> comparer)
        {
            return DBconnection.CreditorSet
                .Include("RateSet")
                .Where(comparer)
                .ToList();
        }

        virtual public bool UpdateCreditor(Creditor newVersion)
        {
            DBconnection.Configuration.ProxyCreationEnabled = true;
            var buf = DBconnection.CreditorSet.Find(newVersion.Id);
            if (buf == null) return false;
            else
            {
                buf.Id = newVersion.Id;
                buf.Name = newVersion.Name;
                buf.TotalRate = newVersion.TotalRate;
                buf.UNP = newVersion.UNP;
                buf.SumToLoan = newVersion.SumToLoan;
                DBconnection.SaveChanges();
                DBconnection.Configuration.ProxyCreationEnabled = false;
                return true;
            }
        }

        virtual public bool UpdateClient(Client newVersion)
        {
            var buf = DBconnection.ClientSet.Find(newVersion.Id);
            if (buf == null) return false;
            DBconnection.Configuration.ProxyCreationEnabled = true;
            buf.Login = newVersion.Login;
            buf.Password = newVersion.Password;
            buf.UserStatus = newVersion.UserStatus;
            buf.IsOnline = newVersion.IsOnline;
            buf.LastOnline = newVersion.LastOnline;
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return true;
        }

        virtual public bool UpdateExpert(Expert newVersion)
        {
            var buf = DBconnection.ExpertSet.Find(newVersion.Id);
            if (buf == null) return false;
            DBconnection.Configuration.ProxyCreationEnabled = true;
            buf.Login = newVersion.Login;
            buf.Password = newVersion.Password;
            buf.UserStatus = newVersion.UserStatus;
            buf.IsOnline = newVersion.IsOnline;
            buf.LastOnline = newVersion.LastOnline;
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return true;
        }

        virtual public bool UpdateAdmin(Admin newVersion)
        {
            var buf = DBconnection.AdminSet.Find(newVersion.Id);
            if (buf == null) return false;
            DBconnection.Configuration.ProxyCreationEnabled = true;
            buf.Login = newVersion.Login;
            buf.Password = newVersion.Password;
            buf.UserStatus = newVersion.UserStatus;
            buf.IsOnline = newVersion.IsOnline;
            buf.LastOnline = newVersion.LastOnline;
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return true;
        }

        public int CreateDetail(Detail obj)
        {
            DBconnection.Configuration.ProxyCreationEnabled = true;
            DBconnection.DetailSet.Add(obj);
            DBconnection.SaveChanges();
            DBconnection.Configuration.ProxyCreationEnabled = false;
            return obj.Id;
        }

        public bool DeleteDetailWhere(Func<Detail, bool> comparer)
        {
            var buf = FindDetailsWhere(comparer);
            if (buf.Count == 0) { return false; }
            else
            {
                DBconnection.Configuration.ProxyCreationEnabled = true;
                DBconnection.DetailSet.RemoveRange(buf);
                DBconnection.Configuration.ProxyCreationEnabled = false;
                DBconnection.SaveChanges();
                return true;
            }
        }

        public bool UpdateDetail(Detail newVersion)
        {
            DBconnection.Configuration.ProxyCreationEnabled = true;
            var buf = DBconnection.DetailSet.Find(newVersion.Id);
            if (buf == null) return false;
            else
            {
                buf.Id = newVersion.Id;
                buf.Name = newVersion.Name;
                buf.Cost = newVersion.Cost;
                buf.VendorCode = newVersion.VendorCode;
                DBconnection.SaveChanges();
                DBconnection.Configuration.ProxyCreationEnabled = false;
                return true;
            }
        }

        public List<Detail> FindDetailsWhere(Func<Detail, bool> comparer)
        {
            return DBconnection.DetailSet.Where(comparer).ToList();
        }
    }
}
