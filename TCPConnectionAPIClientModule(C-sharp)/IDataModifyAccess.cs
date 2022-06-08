using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public interface IDataModifyAccess : IDataViewAccess   
    {
        AnswerFromServer CreateCreditor(Creditor obj);
        AnswerFromServer ModifyCreditor(Creditor obj);
        AnswerFromServer DeleteCreditor(int id);

        AnswerFromServer CreateDetail(Detail obj);
        AnswerFromServer ModifyDetail(Detail obj);
        AnswerFromServer DeleteDetail(int id);
    }
}
