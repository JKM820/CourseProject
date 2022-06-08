using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;
using System.Collections.Generic;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public abstract class IAdminAccess : IUserModifyAccess, IDataModifyAccess, ICommonAccess
    {
        public abstract TypeOfUser Authorization(string login, string password);
        public abstract AnswerFromServer CreateCreditor(Creditor obj);
        public abstract AnswerFromServer CreateDetail(Detail obj);
        public abstract AnswerFromServer DeleteCreditor(int id);
        public abstract AnswerFromServer DeleteDetail(int id);
        public abstract List<Creditor> FindCreditorWithName(string model);
        public abstract List<Creditor> FindCreditorWithSumToLoan(decimal sum);
        public abstract List<Creditor> FindCreditorWithTotalRate(double rate);
        public abstract List<Creditor> FindCreditorWithUNP(int unp);
        public abstract List<Detail> FindDetailByCost(decimal cost);
        public abstract List<Detail> FindDetailByName(string name);
        public abstract List<Detail> FindDetailByVendorCode(int code);
        public abstract List<Creditor> GetAllCreditors();
        public abstract List<Detail> GetAllDetails();
        public abstract string GetReport();
        public abstract AnswerFromServer ModifyCreditor(Creditor obj);
        public abstract AnswerFromServer ModifyDetail(Detail obj);
        public abstract void PreviousRoom();
        public abstract AnswerFromServer Registration<T>(TypeOfUser type, T user) where T : class;
    }
}
