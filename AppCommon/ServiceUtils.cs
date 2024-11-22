using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon
{
    public class ServiceUtils
    {
        public static CancellationToken GetCancellationToken()
        {
            return new CancellationTokenSource().Token;
        }
    }
}
