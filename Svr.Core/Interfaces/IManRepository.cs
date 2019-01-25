using System;
using System.Collections.Generic;
using System.Text;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IManRepository : IRepository<Man>, IRepositoryAsync<Man>
    {
    }
}
