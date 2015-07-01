using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface ITransactionRepository
    {
        bool UpdateStatus(string requestNumber, string status, string updatedBy);
    }
}
