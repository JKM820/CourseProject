using DatabaseEntities;
using System.Collections.Generic;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public abstract class IUserViewAccess
    {
        public abstract List<Client> GetAllClients();
        public abstract List<Expert> GetAllExperts();
        public abstract List<Admin> GetAllAdmins();
        public abstract Client FindClientByLogin(string login);
        public abstract Expert FindExpertByLogin(string login);
        public abstract Admin FindAdminByLogin(string login);
    }
}
