using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public abstract class IUserModifyAccess : IUserViewAccess   
    {
        public abstract AnswerFromServer RegisterNewAdmin(Admin admin);
        public abstract AnswerFromServer RegisterNewClient(Client client);
        public abstract AnswerFromServer RegisterNewExpert(Expert expert);
        public abstract AnswerFromServer BanClientWith(string login);
        public abstract AnswerFromServer BanExpertWith(string login);
        public abstract AnswerFromServer UnbanExpertWith(string login);
        public abstract AnswerFromServer UnbanClientWith(string login);
        public abstract AnswerFromServer DeleteExpertWith(string login);
        public abstract AnswerFromServer DeleteClientWith(string login);
        public abstract AnswerFromServer ModifyClient(Client client);
        public abstract AnswerFromServer ModifyExpert(Expert expert);
    }
}
