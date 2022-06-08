using DatabaseEntities;

namespace TCPConnectionAPI_C_sharp_
{
    public interface IExpertMethod
    {
        IRateable Rate(IRateable obj, Expert expert, float rate);
    }
}
