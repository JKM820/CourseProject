using DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnectionAPI_C_sharp_
{
    public class RationalStructureByExpertAssessments : IExpertMethod
    {
          
        public IRateable Rate(IRateable obj, Expert expert, float rate)
        {
            return obj;
        }
    }
}
