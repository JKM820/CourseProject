using ClassLibraryForTCPConnectionAPI_C_sharp_;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    class TCPClientProtocol : IUserProtocol
    {
        protected IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse
            (ConfigurationManager.AppSettings.Get("serverIP")),
            int.Parse(ConfigurationManager.AppSettings.Get("serverPort")));

        protected Socket connectionSocket
            = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        protected void SendServiceCommand(ServiceCommands command)
        {
            connectionSocket.Send(BitConverter.GetBytes((int)command));
        }

        protected ServiceCommands ReceiveServiceCommand()
        {
            byte[] buffer = new byte[4];
            connectionSocket.Receive(buffer);
            return (ServiceCommands)(BitConverter.ToInt32(buffer, 0));
        }

        public void GoToPreviousRoom()
        {
            SendCommand(CommandsToServer.PreviousRoom);
        }

        public bool Connect()
        {
            connectionSocket.Connect(serverEndPoint);
            return connectionSocket.Connected;
        }

        public AnswerFromServer ReceiveAnswerFromServer()
        {
            byte[] answerBuf = new byte[4];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(answerBuf);
            AnswerFromServer answer = (AnswerFromServer)(BitConverter.ToInt32(answerBuf, 0));
            return answer;
        }

        public List<T> ReceiveCollection<T>()
        {
            var sizeBuffer = new byte[sizeof(System.Int64)];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(sizeBuffer);
            var collectionBuffer = new byte[BitConverter.ToInt64(sizeBuffer, 0)];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(collectionBuffer);
            var formatter = new BinaryFormatter();
            List<T> collection = null;
            using (var ms = new MemoryStream(collectionBuffer))
            {
                collection = formatter.Deserialize(ms) as List<T>;
            }
            return collection;
        }

        public TypeOfUser ReceiveTypeOfUser()
        {
            byte[] typeBuf = new byte[4];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(typeBuf);
            TypeOfUser type = (TypeOfUser)(BitConverter.ToInt32(typeBuf, 0));
            return type;
        }

        public void SendCollection<T>(List<T> collection)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, collection);
                ReceiveServiceCommand();
                connectionSocket.Send(BitConverter.GetBytes(ms.Length));
                ReceiveServiceCommand();
                connectionSocket.Send(ms.ToArray());
            }
        }

        public void SendCommand(CommandsToServer command)
        {
            ReceiveServiceCommand();
            connectionSocket.Send(BitConverter.GetBytes((int)command));
        }

        public void SendLogin(string login)
        {
            ReceiveServiceCommand();
            connectionSocket.Send(BitConverter.GetBytes(login.Length));
            ReceiveServiceCommand();
            connectionSocket.Send(Encoding.UTF8.GetBytes(login));
        }

        public void SendPassword(string password)
        {
            ReceiveServiceCommand();
            connectionSocket.Send(BitConverter.GetBytes(password.Length));
            ReceiveServiceCommand();
            connectionSocket.Send(Encoding.UTF8.GetBytes(password));
        }

        public void SendTypeOfUser(TypeOfUser typeOfUser)
        {
            ReceiveServiceCommand();
            connectionSocket.Send(BitConverter.GetBytes((int)typeOfUser));
        }

        public string ReceiveString()
        {
            var sizeBuffer = new byte[4];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(sizeBuffer);
            byte[] loginBuffer = new byte[BitConverter.ToInt32(sizeBuffer, 0)];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(loginBuffer);
            return Encoding.Default.GetString(loginBuffer);
        }

        public void SendString(string str)
        {
            ReceiveServiceCommand();
            connectionSocket.Send(BitConverter.GetBytes(str.Length));
            ReceiveServiceCommand();
            connectionSocket.Send(Encoding.Default.GetBytes(str));
        }

        public T ReceiveObject<T>() where T : class
        {
            var sizeBuffer = new byte[8];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(sizeBuffer);
            var collectionBuffer = new byte[BitConverter.ToInt32(sizeBuffer, 0)];
            SendServiceCommand(ServiceCommands.Ready_to_receive);
            connectionSocket.Receive(collectionBuffer);
            var formatter = new BinaryFormatter();
            T buf = null;
            using (var ms = new MemoryStream(collectionBuffer))
            {
                buf = formatter.Deserialize(ms) as T;
            }
            return buf;
        }

        public void SendObject<T>(T obj) where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ms, obj);
                ReceiveServiceCommand();
                connectionSocket.Send(BitConverter.GetBytes(ms.Length));
                ReceiveServiceCommand();
                connectionSocket.Send(ms.ToArray());
            }
        }
    }
}
