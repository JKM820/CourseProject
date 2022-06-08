namespace ClassLibraryForTCPConnectionAPI_C_sharp_
{
    public enum CommandsToServer
    {
        Registration,
        Authorization,
        PreviousRoom,
        //Admin commands
        RegisterNewAdmin,
        RegisterNewClient,
        RegisterNewExpert,
        BanClient,
        BanExpert,
        UnbanClient,
        UnbanExpert,
        DeleteClient,
        DeleteExpert,
        ModifyClient,
        ModifyExpert,
        FindClientByLogin,
        GetAllClients,
        GetAllAdmins,
        FindExpertByLogin,
        GetAllExperts,
        FindAdminByLogin,
        CreateCreditor,
        CreateDetail,
        ModifyCreditor,
        ModifyDetail,
        DeleteCreditor,
        DeleteDetail,
        CreateReport,
        //Expert commands
        RateCreditor, 
        //Client commands
        FindCreditorByName,
        FindCreditorByUNP,
        FindCreditorByRate,
        FindCreditorBySumToLoan,
        GetAllCreditors,

        FindDetailByName,
        FindDetailByCost,
        FindDetailByVendorCode,
        GetAllDetails,
    }
}
