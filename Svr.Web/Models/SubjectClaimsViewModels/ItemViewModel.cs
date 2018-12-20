using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Web.Models.SubjectClaimsViewModels
{
    public class ItemViewModel : SubjectClaim
    {
        public string StatusMessage { get; set; }
    }
}
