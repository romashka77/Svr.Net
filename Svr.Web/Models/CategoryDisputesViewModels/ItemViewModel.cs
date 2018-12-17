using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.CategoryDisputesViewModels
{
    public class ItemViewModel: CategoryDispute
    {
        public string StatusMessage { get; set; }
    }
}
