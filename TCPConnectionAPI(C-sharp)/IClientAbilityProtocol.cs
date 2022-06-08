using DatabaseEntities;
using System;
using System.Collections.Generic;

namespace TCPConnectionAPI_C_sharp_
{
    public interface IClientAbilityProtocol : IDisposable
    {
        List<Creditor> FindCreditorWhere(Func<Creditor, bool> comparer);
        List<Detail> FindDetailsWhere(Func<Detail, bool> comparer);
    }
}
