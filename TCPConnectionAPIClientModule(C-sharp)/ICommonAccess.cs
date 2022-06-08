using ClassLibraryForTCPConnectionAPI_C_sharp_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public interface ICommonAccess
    {
        TypeOfUser Authorization(string login, string password);
        AnswerFromServer Registration<T>(TypeOfUser type, T user) where T : class;
        void PreviousRoom();
    }
}
