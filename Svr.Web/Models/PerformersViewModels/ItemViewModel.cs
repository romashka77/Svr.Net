using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Web.Models.PerformersViewModels
{
    public class ItemViewModel : Performer
    {
        public string StatusMessage { get; set; }
    }
}
