using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Web.Models.DirViewModels
{
    public class ItemViewModel : Dir
    {
        public string StatusMessage { get; set; }
    }
}
