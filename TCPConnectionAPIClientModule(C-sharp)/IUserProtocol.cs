using ClassLibraryForTCPConnectionAPI_C_sharp_;
using System.Collections.Generic;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public interface IUserProtocol
    {
        bool Connect();
        string ReceiveString();
        void SendString(string str);
        void SendLogin(string login);
        void SendPassword(string password);
        void SendCollection<T>(List<T> collection);
        List<T> ReceiveCollection<T>();
        T ReceiveObject<T>() where T : class;
        void SendObject<T>(T obj) where T : class;
        void SendCommand(CommandsToServer command);
        AnswerFromServer ReceiveAnswerFromServer();
        TypeOfUser ReceiveTypeOfUser();
        void GoToPreviousRoom();
        void SendTypeOfUser(TypeOfUser type);
    }
}
