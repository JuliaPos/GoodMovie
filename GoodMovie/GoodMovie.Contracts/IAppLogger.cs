using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodMovie.Contracts
{
   public interface IAppLogger
    {
        void Info(string msg);
        void Error(string msg);

    }
}
