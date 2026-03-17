using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.SHARED.Enums
{
    public enum LinkType
    {
        RelatesTo = 1,  // Sadece ilişkili
        Blocks = 2,     // Source, Target'ı blokluyor
        IsBlockedBy = 3,// Source, Target tarafından bloklanıyor
        Duplicates = 4  // Kopyası
    }
}
