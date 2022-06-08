using DatabaseEntities;
using System;
using System.Collections.Generic;

namespace TCPConnectionAPI_C_sharp_
{
    public class ClientAbilityProtocol : IClientAbilityProtocol
    {
        IDataViewPermision DBconnection;

        public ClientAbilityProtocol()
        {
            DBconnection = new DatabaseContext();
        }

        public void Dispose()
        {
            DBconnection.Dispose();
        }

        public List<Creditor> FindCreditorWhere(Func<Creditor, bool> comparer)
        {
            return DBconnection.FindCreditorWhere(comparer);
        }

        public List<Detail> FindDetailsWhere(Func<Detail, bool> comparer)
        {
            return DBconnection.FindDetailsWhere(comparer);
        }
    }
}
