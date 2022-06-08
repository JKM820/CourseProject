using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEntities
{
    public interface IRateable
    {
        double TotalRate { get; set; }
    }
}
