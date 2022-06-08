using ClassLibraryForTCPConnectionAPI_C_sharp_;
using DatabaseEntities;
using System.Collections.Generic;
using System.Linq;

namespace TCPConnectionAPIClientModule_C_sharp_
{
    public class ClientConnectionModule : IAdminAccess, IExpertAccess, IClientAccess
    {
        protected IUserProtocol protocol;

        protected static int amountOfObjects;

        public bool Connected { get; }

        public ClientConnectionModule()
        {
            protocol = new TCPClientProtocol();
            if (amountOfObjects == 0)
            {
                Connected = protocol.Connect();
            }
            amountOfObjects++;
        }

        public override TypeOfUser Authorization(string login, string password)
        {
            protocol.SendCommand(CommandsToServer.Authorization);
            protocol.SendLogin(login);
            protocol.SendPassword(password);
            return protocol.ReceiveTypeOfUser();
        }

        public override AnswerFromServer Registration<T>(TypeOfUser type, T user)
        {
            protocol.SendCommand(CommandsToServer.Registration);
            protocol.SendTypeOfUser(type);
            protocol.SendObject<T>(user);
            return protocol.ReceiveAnswerFromServer();
        }

        public override void PreviousRoom()
        {
            protocol.GoToPreviousRoom();
        }

        public override Client FindClientByLogin(string login)
        {
            protocol.SendCommand(CommandsToServer.FindClientByLogin);
            protocol.SendLogin(login);
            var received = protocol.ReceiveCollection<Client>();
            if (received.Count == 0 || received.Count > 1)
            {
                return new Client();
            }
            else
            {
                return received.First();
            }
        }

        public override Expert FindExpertByLogin(string login)
        {
            protocol.SendCommand(CommandsToServer.FindExpertByLogin);
            protocol.SendLogin(login);
            var received = protocol.ReceiveCollection<Expert>();
            if (received.Count == 0 || received.Count > 1)
            {
                return new Expert();
            }
            else
            {
                return received.First();
            }
        }

        public override Admin FindAdminByLogin(string login)
        {
            protocol.SendCommand(CommandsToServer.FindAdminByLogin);
            protocol.SendLogin(login);
            var received = protocol.ReceiveCollection<Admin>();
            if (received.Count == 0 || received.Count > 1)
            {
                return new Admin();
            }
            else
            {
                return received.First();
            }
        }

        public override AnswerFromServer RegisterNewAdmin(Admin admin)
        {
            protocol.SendCommand(CommandsToServer.RegisterNewAdmin);
            protocol.SendObject(admin);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer RegisterNewClient(Client client)
        {
            protocol.SendCommand(CommandsToServer.RegisterNewClient);
            protocol.SendObject(client);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer RegisterNewExpert(Expert expert)
        {
            protocol.SendCommand(CommandsToServer.RegisterNewExpert);
            protocol.SendObject(expert);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer BanClientWith(string login)
        {
            protocol.SendCommand(CommandsToServer.BanClient);
            protocol.SendLogin(login);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer BanExpertWith(string login)
        {
            protocol.SendCommand(CommandsToServer.BanExpert);
            protocol.SendLogin(login);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer UnbanExpertWith(string login)
        {
            protocol.SendCommand(CommandsToServer.UnbanExpert);
            protocol.SendLogin(login);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer UnbanClientWith(string login)
        {
            protocol.SendCommand(CommandsToServer.UnbanExpert);
            protocol.SendLogin(login);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer DeleteExpertWith(string login)
        {
            protocol.SendCommand(CommandsToServer.DeleteExpert);
            protocol.SendLogin(login);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer DeleteClientWith(string login)
        {
            protocol.SendCommand(CommandsToServer.DeleteClient);
            protocol.SendLogin(login);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer CreateCreditor(Creditor obj)
        {
            protocol.SendCommand(CommandsToServer.CreateCreditor);
            protocol.SendObject(obj);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer ModifyCreditor(Creditor obj)
        {
            protocol.SendCommand(CommandsToServer.ModifyCreditor);
            protocol.SendObject(obj);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer DeleteCreditor(int id)
        {
            protocol.SendCommand(CommandsToServer.DeleteCreditor);
            protocol.SendString(id.ToString());
            return protocol.ReceiveAnswerFromServer();
        }

        public override string GetReport()
        {
            protocol.SendCommand(CommandsToServer.CreateReport);
            return protocol.ReceiveString();
        }

        public override List<Client> GetAllClients()
        {
            protocol.SendCommand(CommandsToServer.GetAllClients);
            return protocol.ReceiveCollection<Client>();
        }

        public override List<Expert> GetAllExperts()
        {
            protocol.SendCommand(CommandsToServer.GetAllExperts);
            return protocol.ReceiveCollection<Expert>();
        }

        public override List<Creditor> GetAllCreditors()
        {
            protocol.SendCommand(CommandsToServer.GetAllCreditors);
            return protocol.ReceiveCollection<Creditor>();
        }

        public override AnswerFromServer ModifyClient(Client client)
        {
            protocol.SendCommand(CommandsToServer.ModifyClient);
            protocol.SendObject(client);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer ModifyExpert(Expert expert)
        {
            protocol.SendCommand(CommandsToServer.ModifyExpert);
            protocol.SendObject(expert);
            return protocol.ReceiveAnswerFromServer();
        }

        public AnswerFromServer Rate(int planeId, float expertRate)
        {
            protocol.SendCommand(CommandsToServer.RateCreditor);
            protocol.SendString(planeId.ToString());
            protocol.SendString(expertRate.ToString());
            return protocol.ReceiveAnswerFromServer();
        }

        public override List<Creditor> FindCreditorWithName(string name)
        {
            protocol.SendCommand(CommandsToServer.FindCreditorByName);
            protocol.SendString(name);
            return protocol.ReceiveCollection<Creditor>();
        }

        public override List<Creditor> FindCreditorWithTotalRate(double rate)
        {
            protocol.SendCommand(CommandsToServer.FindCreditorByRate);
            protocol.SendString(rate.ToString());
            return protocol.ReceiveCollection<Creditor>();
        }

        public override List<Admin> GetAllAdmins()
        {
            protocol.SendCommand(CommandsToServer.GetAllAdmins);
            return protocol.ReceiveCollection<Admin>();
        }

        public override AnswerFromServer CreateDetail(Detail obj)
        {
            protocol.SendCommand(CommandsToServer.CreateDetail);
            protocol.SendObject(obj);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer ModifyDetail(Detail obj)
        {
            protocol.SendCommand(CommandsToServer.ModifyDetail);
            protocol.SendObject(obj);
            return protocol.ReceiveAnswerFromServer();
        }

        public override AnswerFromServer DeleteDetail(int id)
        {
            protocol.SendCommand(CommandsToServer.DeleteDetail);
            protocol.SendObject(id.ToString());
            return protocol.ReceiveAnswerFromServer();
        }

        public override List<Creditor> FindCreditorWithUNP(int unp)
        {
            protocol.SendCommand(CommandsToServer.FindCreditorByUNP);
            protocol.SendObject(unp.ToString());
            return protocol.ReceiveCollection<Creditor>();
        }

        public override List<Creditor> FindCreditorWithSumToLoan(decimal sum)
        {
            protocol.SendCommand(CommandsToServer.FindCreditorBySumToLoan);
            protocol.SendObject(sum.ToString());
            return protocol.ReceiveCollection<Creditor>();
        }

        public override List<Detail> FindDetailByName(string name)
        {
            protocol.SendCommand(CommandsToServer.FindDetailByName);
            protocol.SendString(name);
            return protocol.ReceiveCollection<Detail>();
        }

        public override List<Detail> FindDetailByCost(decimal cost)
        {
            protocol.SendCommand(CommandsToServer.FindDetailByCost);
            protocol.SendString(cost.ToString());
            return protocol.ReceiveCollection<Detail>();
        }

        public override List<Detail> FindDetailByVendorCode(int code)
        {
            protocol.SendCommand(CommandsToServer.FindDetailByVendorCode);
            protocol.SendString(code.ToString());
            return protocol.ReceiveCollection<Detail>();
        }

        public override List<Detail> GetAllDetails()
        {
            protocol.SendCommand(CommandsToServer.GetAllDetails);
            return protocol.ReceiveCollection<Detail>();
        }
    }
}
