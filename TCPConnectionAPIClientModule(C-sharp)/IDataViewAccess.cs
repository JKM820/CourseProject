using DatabaseEntities;
using System.Collections.Generic;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public interface IDataViewAccess
    {
        List<Creditor> FindCreditorWithName(string model);
        List<Creditor> FindCreditorWithTotalRate(double rate);
        List<Creditor> FindCreditorWithUNP(int unp);
        List<Creditor> FindCreditorWithSumToLoan(decimal sum);
        List<Creditor> GetAllCreditors();

        List<Detail> FindDetailByName(string name);
        List<Detail> FindDetailByCost(decimal cost);
        List<Detail> FindDetailByVendorCode(int code);
        List<Detail> GetAllDetails();
    }
}
