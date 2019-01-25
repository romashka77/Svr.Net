using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Web.Models;
using Svr.Web.Models.RegionsViewModels;

namespace Svr.Web.Interfaces
{
    public interface IRegionService : IRegionRepository
    {
        Task<IndexViewModel> GetRegionItems(SortState sortOrder= SortState.NameAsc, string name=null, int pageIndex=1, int itemsPage=10);

    }
}
