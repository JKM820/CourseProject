using System;
using DatabaseEntities;
namespace TCPConnectionAPI_C_sharp_
{
    public interface IDataModifyPermission : IDataViewPermision
    {
        int CreateCreditor(Creditor obj);
        int CreateDetail(Detail obj);
        bool DeleteCreditorWhere(Func<Creditor, bool> comparer);
        bool DeleteDetailWhere(Func<Detail, bool> comparer);
        bool UpdateCreditor(Creditor newVersion);
        bool UpdateDetail(Detail newVersion);
    }
}
