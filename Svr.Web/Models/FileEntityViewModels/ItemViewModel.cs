using Microsoft.AspNetCore.Http;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Models.FileEntityViewModels
{
    public class ItemViewModel : FileEntity
    {
        [Display(Name = "Выберите файл для загрузки")]
        public IFormFile UploadedFile { get; set; }
        public string StatusMessage { get; set; }
    }
}
