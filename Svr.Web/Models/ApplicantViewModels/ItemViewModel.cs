using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Svr.Core.Entities;

namespace Svr.Web.Models.ApplicantViewModels
{
    public class ItemViewModel : Applicant
    {
        public string StatusMessage { get; set; }
    }
}
