﻿using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IFileEntityRepository : IRepository<FileEntity>, IRepositoryAsync<FileEntity>, ISort<FileEntity>
    {
        FileEntity GetByIdWithItems(long? id);
        Task<FileEntity> GetByIdWithItemsAsync(long? id);
    }
}
