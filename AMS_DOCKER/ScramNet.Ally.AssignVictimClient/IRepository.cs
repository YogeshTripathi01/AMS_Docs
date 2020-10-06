using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScramNet.Ally.AssignVictimClient
{
    public interface IRepository<T>
    {
        Task InsertRecord(T record);
    }
}
