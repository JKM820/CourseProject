using ClassLibraryForTCPConnectionAPI_C_sharp_;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TCPConnectionAPI_C_sharp_
{
    public interface IServerProtocol
    {
        Socket AcceptConnectionRequest();
        string ReceiveString(Socket from);
        void SendString(string str, Socket destination);
        string ReceiveLogin(Socket from);
        string ReceivePassword(Socket from);
        void SendCollection<T>(List<T> collection, Socket destination);
        List<T> ReceiveCollection<T>(Socket from);
        T ReceiveObject<T>(Socket from) where T : class;
        void SendObject<T>(T obj, Socket destination) where T : class;
        CommandsToServer ReceiveCommand(Socket from);
        void SendAnswerFromServer(AnswerFromServer answer, Socket desination);
        void SendTypeOfUser(TypeOfUser typeOfUser, Socket destination);
        TypeOfUser ReceiveTypeOfUser(Socket from);
        void CloseConnection();
    }
}
