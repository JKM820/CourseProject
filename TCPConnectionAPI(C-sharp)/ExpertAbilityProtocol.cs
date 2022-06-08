using DatabaseEntities;
using System;
using System.Collections.Generic;


namespace TCPConnectionAPI_C_sharp_
{
    public class ExpertAbilityProtocol : IExpertAbilityProtocol
    {
        public IExpertMethod expertMethod { get; set; }

        public IDataModifyPermission DBconnection;

        public bool Rate(Creditor entity, Expert expert, float rate)
        {
            var ratedObj = expertMethod.Rate(entity, expert, rate) as Creditor;
            return DBconnection.UpdateCreditor(ratedObj);
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

        public ExpertAbilityProtocol()
        {
            DBconnection = new DatabaseContext();
        }

    }
}
