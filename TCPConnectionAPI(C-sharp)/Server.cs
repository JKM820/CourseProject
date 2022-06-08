using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TCPConnectionAPI_C_sharp_
{
    public class Server
    {
        protected IServerProtocol _protocol;

        protected ICollection<ConnectedUserInfo> _connectedUsers;

        protected IUserModifyPermission serverAccessPermission;

        protected virtual int Registration(ref ConnectedUserInfo user)
        {
            user.Type = _protocol.ReceiveTypeOfUser(user.ConnectionSocket);
            switch (user.Type)
            {
                case TypeOfUser.Admin:
                    {
                        var obj = _protocol.ReceiveObject<Admin>(user.ConnectionSocket);
                        return serverAccessPermission.CreateAdmin(obj);
                    }
                case TypeOfUser.Client:
                    {
                        var obj = _protocol.ReceiveObject<Client>(user.ConnectionSocket);
                        return serverAccessPermission.CreateClient(obj);
                    }
                case TypeOfUser.Expert:
                    {
                        var obj = _protocol.ReceiveObject<Expert>(user.ConnectionSocket);
                        return serverAccessPermission.CreateExpert(obj);
                    }
                case TypeOfUser.Undefined:
                    return 0;
                default:
                    return 0;
            }
        }

        protected virtual int Authorization(ref ConnectedUserInfo user)
        {
            string login = _protocol.ReceiveLogin(user.ConnectionSocket);
            string password = _protocol.ReceivePassword(user.ConnectionSocket);

            var admins = serverAccessPermission.FindAdminsWhere(c => c.Login == login);
            if (admins.Count == 0) { user.Type = TypeOfUser.Undefined; }
            else
            {
                var admin = admins.First();
                if (admin.Id > 0 && admin.Password == password && admin.UserStatus == Status.NotBanned)
                { user.Type = TypeOfUser.Admin; return admin.Id; }
            }

            var clients = serverAccessPermission.FindClientsWhere(c => c.Login == login);
            if (clients.Count == 0) { user.Type = TypeOfUser.Undefined; }
            else
            {
                var client = clients.First();
                if (client.Id > 0 && client.Password == password && client.UserStatus == Status.NotBanned)
                { user.Type = TypeOfUser.Client; return client.Id; }
            }


            var experts = serverAccessPermission.FindExpertsWhere(c => c.Login == login);
            if (experts.Count == 0) { user.Type = TypeOfUser.Undefined; }
            else
            {
                var expert = experts.First();
                if (expert.Id > 0 && expert.Password == password && expert.UserStatus == Status.NotBanned)
                { user.Type = TypeOfUser.Expert; return expert.Id; }
                user.Type = TypeOfUser.Undefined;
                return 0;
            }
            user.Type = TypeOfUser.Undefined;
            return 0;
        }

        protected virtual int Validation(ref ConnectedUserInfo user)
        {
            while (true)
            {
                switch (_protocol.ReceiveCommand(user.ConnectionSocket))
                {
                    case CommandsToServer.Registration:
                        {
                            int id = Registration(ref user);
                            if (id > 0)
                            {
                                _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                return id;
                            }
                            else
                            {
                                _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                continue;
                            }
                        }
                    case CommandsToServer.Authorization:
                        {
                            int id = Authorization(ref user);
                            _protocol.SendTypeOfUser(user.Type, user.ConnectionSocket);
                            if (id > 0)
                            {
                                return id;
                            }
                            else
                            {
                                continue;
                            }

                        }
                    case CommandsToServer.PreviousRoom:
                        {
                            return 0;
                        }
                    default:
                        {
                            _protocol.SendAnswerFromServer(AnswerFromServer.UnknownCommand, user.ConnectionSocket);
                            return 0;
                        }
                }
            }
        }

        protected virtual void UserHandler(object client)
        {
            ConnectedUserInfo user = client as ConnectedUserInfo;
            try
            {
                while (true)
                {
                    user.DB_Id = Validation(ref user);
                    if (user.DB_Id <= 0) { Console.WriteLine("Пользователь отключился"); user.ConnectionSocket.Close(); return; }
                    switch (user.Type)
                    {
                        case TypeOfUser.Admin:
                            { AdminHandler(user); break; }
                        case TypeOfUser.Client:
                            { ClientHandler(user); break; }
                        case TypeOfUser.Expert:
                            { ExpertHandler(user); break; }
                        default:
                            break;
                    }
                }
            }
            catch(System.Net.Sockets.SocketException)
            {
                Console.WriteLine("Пользователь отключился");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        protected virtual void ClientHandler(ConnectedUserInfo user)
        {
            var client = serverAccessPermission.FindClientsWhere(c => c.Id == user.DB_Id).First();
            client.IsOnline = true;
            serverAccessPermission.UpdateClient(client);
            using (IClientAbilityProtocol clientProtocol = new ClientAbilityProtocol())
            {
                while (true)
                {
                    switch (_protocol.ReceiveCommand(user.ConnectionSocket))
                    {
                        case CommandsToServer.FindCreditorByRate:
                            {
                                var param = float.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(clientProtocol.FindCreditorWhere(c => c.TotalRate == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorByName:
                            {
                                var param = _protocol.ReceiveString(user.ConnectionSocket);
                                _protocol.SendCollection(clientProtocol.FindCreditorWhere(c => c.Name == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorByUNP:
                            {
                                var param = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(clientProtocol.FindCreditorWhere(c => c.UNP == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorBySumToLoan:
                            {
                                var param = decimal.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(clientProtocol.FindCreditorWhere(c => c.SumToLoan == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllCreditors:
                            {
                                _protocol.SendCollection(clientProtocol.FindCreditorWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByName:
                            {
                                var param = _protocol.ReceiveString(user.ConnectionSocket);
                                _protocol.SendCollection(clientProtocol.FindDetailsWhere(c => c.Name == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByCost:
                            {
                                var param = decimal.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(clientProtocol.FindDetailsWhere(c => c.Cost == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByVendorCode:
                            {
                                var param = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(clientProtocol.FindDetailsWhere(c => c.VendorCode == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllDetails:
                            {
                                _protocol.SendCollection(clientProtocol.FindDetailsWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.PreviousRoom:
                            {
                                var disconnectingUser = serverAccessPermission.FindClientsWhere(c => c.Id == user.DB_Id).First();
                                disconnectingUser.IsOnline = false;
                                serverAccessPermission.UpdateClient(disconnectingUser);
                                return;
                            }
                        default:
                            {
                                _protocol.SendAnswerFromServer(AnswerFromServer.UnknownCommand, user.ConnectionSocket);
                                break;
                            }
                    }
                }
            }
        }

        protected virtual void AdminHandler(ConnectedUserInfo user)
        {
            var admin = serverAccessPermission.FindAdminsWhere(c => c.Id == user.DB_Id).First();
            admin.IsOnline = true;
            serverAccessPermission.UpdateAdmin(admin);
            using (IAdminAbilityProtocol adminProtocol = new AdminAbilityProtocol())
            {
                while (true)
                {
                    switch (_protocol.ReceiveCommand(user.ConnectionSocket))
                    {
                        case CommandsToServer.CreateCreditor:
                            {
                                if (adminProtocol.CreateCreditor(_protocol.ReceiveObject<Creditor>(user.ConnectionSocket)))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.CreateDetail:
                            {
                                if (adminProtocol.CreateDetail(_protocol.ReceiveObject<Detail>(user.ConnectionSocket)))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.ModifyCreditor:
                            {
                                if (adminProtocol.ModifyCreditor(_protocol.ReceiveObject<Creditor>(user.ConnectionSocket)))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.ModifyDetail:
                            {
                                if (adminProtocol.ModifyDetail(_protocol.ReceiveObject<Detail>(user.ConnectionSocket)))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.ModifyClient:
                            {
                                if (adminProtocol.ModifyClient(_protocol.ReceiveObject<Client>(user.ConnectionSocket)))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.ModifyExpert:
                            {
                                if (adminProtocol.ModifyExpert(_protocol.ReceiveObject<Expert>(user.ConnectionSocket)))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.DeleteCreditor:
                            {
                                int id = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                if (adminProtocol.DeleteCreditorsWhere(c => c.Id == id))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.DeleteDetail:
                            {
                                int id = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                if (adminProtocol.DeleteCreditorsWhere(c => c.Id == id))
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else
                                    _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorByRate:
                            {
                                var param = float.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(adminProtocol.FindCreditorWhere(c => c.TotalRate == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorByName:
                            {
                                var param = _protocol.ReceiveString(user.ConnectionSocket);
                                _protocol.SendCollection(adminProtocol.FindCreditorWhere(c => c.Name == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorByUNP:
                            {
                                var param = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(adminProtocol.FindCreditorWhere(c => c.UNP == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorBySumToLoan:
                            {
                                var param = decimal.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(adminProtocol.FindCreditorWhere(c => c.SumToLoan == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllCreditors:
                            {
                                _protocol.SendCollection(adminProtocol.FindCreditorWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByName:
                            {
                                var param = _protocol.ReceiveString(user.ConnectionSocket);
                                _protocol.SendCollection(adminProtocol.FindDetailsWhere(c => c.Name == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByCost:
                            {
                                var param = decimal.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(adminProtocol.FindDetailsWhere(c => c.Cost == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByVendorCode:
                            {
                                var param = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(adminProtocol.FindDetailsWhere(c => c.VendorCode == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllDetails:
                            {
                                _protocol.SendCollection(adminProtocol.FindDetailsWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllClients:
                            {
                                _protocol.SendCollection(adminProtocol.FindClientsWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllExperts:
                            {
                                _protocol.SendCollection(adminProtocol.FindExpertsWhere(c => c != null), user.ConnectionSocket);

                                break;
                            }
                        case CommandsToServer.RegisterNewAdmin:
                            {
                                var obj = _protocol.ReceiveObject<Admin>(user.ConnectionSocket);
                                if (serverAccessPermission.CreateAdmin(obj) > 0) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.RegisterNewClient:
                            {
                                var obj = _protocol.ReceiveObject<Client>(user.ConnectionSocket);
                                if (serverAccessPermission.CreateClient(obj) > 0) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.RegisterNewExpert:
                            {
                                var obj = _protocol.ReceiveObject<Expert>(user.ConnectionSocket);
                                if (serverAccessPermission.CreateExpert(obj) > 0) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.BanClient:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                var res = adminProtocol.BanClientsWhere(a => a.Login == login);
                                if (res) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.BanExpert:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                var res = adminProtocol.BanExpertsWhere(a => a.Login == login);
                                if (res) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.UnbanClient:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                var res = adminProtocol.UnbanClientsWhere(a => a.Login == login);
                                if (res) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.UnbanExpert:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                var res = adminProtocol.UnbanExpertsWhere(a => a.Login == login);
                                if (res) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.DeleteClient:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                var res = adminProtocol.DeleteClientsWhere(a => a.Login == login);
                                if (res) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.DeleteExpert:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                var res = adminProtocol.DeleteExpertsWhere(a => a.Login == login);
                                if (res) _protocol.SendAnswerFromServer(AnswerFromServer.Successfully, user.ConnectionSocket);
                                else _protocol.SendAnswerFromServer(AnswerFromServer.Error, user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.CreateReport:
                            {
                                _protocol.SendString(adminProtocol.CreateReport(), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindAdminByLogin:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                _protocol.SendCollection(adminProtocol.FindAdminsWhere(c => c.Login == login), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllAdmins:
                            {
                                _protocol.SendCollection(serverAccessPermission.FindAdminsWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindClientByLogin:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                _protocol.SendCollection(adminProtocol.FindClientsWhere(c => c.Login == login), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindExpertByLogin:
                            {
                                var login = _protocol.ReceiveLogin(user.ConnectionSocket);
                                _protocol.SendCollection(adminProtocol.FindExpertsWhere(c => c.Login == login), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.PreviousRoom:
                            {
                                var adm = serverAccessPermission.FindAdminsWhere(c => c.Id == user.DB_Id).First();
                                adm.IsOnline = false;
                                serverAccessPermission.UpdateAdmin(adm);
                                return;
                            }
                        default:
                            {
                                _protocol.SendAnswerFromServer(AnswerFromServer.UnknownCommand, user.ConnectionSocket);
                                break;
                            }
                    }
                }
            }
        }

        protected virtual void ExpertHandler(ConnectedUserInfo user)
        {
            var expert = serverAccessPermission.FindExpertsWhere(c => c.Id == user.DB_Id).First();
            expert.IsOnline = true;
            serverAccessPermission.UpdateExpert(expert);
            using (IExpertAbilityProtocol expertProtocol = new ExpertAbilityProtocol())
            {
                while (true)
                {
                    switch (_protocol.ReceiveCommand(user.ConnectionSocket))
                    {
                        case CommandsToServer.FindCreditorByRate:
                            {
                                var param = float.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(expertProtocol.FindCreditorWhere(c => c.TotalRate == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorByName:
                            {
                                var param = _protocol.ReceiveString(user.ConnectionSocket);
                                _protocol.SendCollection(expertProtocol.FindCreditorWhere(c => c.Name == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorByUNP:
                            {
                                var param = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(expertProtocol.FindCreditorWhere(c => c.UNP == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindCreditorBySumToLoan:
                            {
                                var param = decimal.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(expertProtocol.FindCreditorWhere(c => c.SumToLoan == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllCreditors:
                            {
                                _protocol.SendCollection(expertProtocol.FindCreditorWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByName:
                            {
                                var param = _protocol.ReceiveString(user.ConnectionSocket);
                                _protocol.SendCollection(expertProtocol.FindDetailsWhere(c => c.Name == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByCost:
                            {
                                var param = decimal.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(expertProtocol.FindDetailsWhere(c => c.Cost == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.FindDetailByVendorCode:
                            {
                                var param = int.Parse(_protocol.ReceiveString(user.ConnectionSocket));
                                _protocol.SendCollection(expertProtocol.FindDetailsWhere(c => c.VendorCode == param), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.GetAllDetails:
                            {
                                _protocol.SendCollection(expertProtocol.FindDetailsWhere(c => c != null), user.ConnectionSocket);
                                break;
                            }
                        case CommandsToServer.PreviousRoom:
                            {
                                var client = serverAccessPermission.FindExpertsWhere(c => c.Id == user.DB_Id).First();
                                client.IsOnline = false;
                                serverAccessPermission.UpdateExpert(client);
                                return;
                            }
                        default:
                            {
                                _protocol.SendAnswerFromServer(AnswerFromServer.UnknownCommand, user.ConnectionSocket);
                                break;
                            }
                    }
                }
            }
        }

        public Server()
        {
            _protocol = new TCPServerProtocol();
            _connectedUsers = new List<ConnectedUserInfo>();
            serverAccessPermission = new DatabaseContext();
            serverAccessPermission.CreateAdmin(new Admin("admin", "admin"));
        }

        public void openConnection()
        {
            while (true)
            {
                ConnectedUserInfo connectedUserInfo = new ConnectedUserInfo();
                connectedUserInfo.ConnectionSocket = _protocol.AcceptConnectionRequest();
                Console.WriteLine("Подключился новый пользователь!");
                ThreadPool.QueueUserWorkItem(UserHandler, connectedUserInfo);
            }
        }
    }
}
