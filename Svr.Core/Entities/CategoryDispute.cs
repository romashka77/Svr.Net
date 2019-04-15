﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Категория споров
    /// </summary>
    public class CategoryDispute : BaseEntityDescription
    {
        /// <summary>
        /// Группа исков
        /// </summary>
        [Display(Name = "Группы исков")]
        public virtual ICollection<GroupClaim> GroupClaims { get; set; } = new List<GroupClaim>();

        public override string ToString() => "Категория споров";
    }
}
