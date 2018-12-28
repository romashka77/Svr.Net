﻿using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IApplicantRepository : IRepository<Applicant>, IAsyncRepository<Applicant>
    {
        Applicant GetByIdWithItems(long? id);
        Task<Applicant> GetByIdWithItemsAsync(long? id);
    }
}