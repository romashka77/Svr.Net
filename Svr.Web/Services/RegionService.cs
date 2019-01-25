using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Web.Interfaces;
using Svr.Web.Models;
using Svr.Web.Models.RegionsViewModels;

namespace Svr.Web.Services
{
    public class RegionService //: IRegionService
    {
        #region поля
        private readonly ILogger<RegionService> _logger;
        private readonly IRegionRepository _itemRepository;
        //private readonly IUriComposer _uriComposer;
        #endregion
        #region конструктор
        public RegionService(ILoggerFactory loggerFactory,
            IRegionRepository itemRepository)
        {
            _logger = loggerFactory.CreateLogger<RegionService>();
            _itemRepository = itemRepository;
        }
        #endregion
        #region методы
        public Region Add(Region entity) => _itemRepository.Add(entity);

        public int Add(IEnumerable<Region> entities) => _itemRepository.Add(entities);

        public Task<Region> AddAsync(Region entity) => _itemRepository.AddAsync(entity);

        public Task<int> AddAsync(IEnumerable<Region> entities) => _itemRepository.AddAsync(entities);

        public void Delete(Region entity) => _itemRepository.Delete(entity);

        public Task DeleteAsync(Region entity) => _itemRepository.DeleteAsync(entity);

        public bool EntityExists(long id) => _itemRepository.EntityExists(id);

        public Task<bool> EntityExistsAsync(long id) => _itemRepository.EntityExistsAsync(id);

        public Region GetById(long? id) => _itemRepository.GetById(id);

        public Task<Region> GetByIdAsync(long? id) => _itemRepository.GetByIdAsync(id);

        public Region GetByIdWithItems(long? id) => _itemRepository.GetByIdWithItems(id);

        public Task<Region> GetByIdWithItemsAsync(long? id) => _itemRepository.GetByIdWithItemsAsync(id);

        public async Task<IndexViewModel> GetRegionItems(SortState sortOrder = SortState.NameAsc, string searchString = null, int pageIndex = 1, int itemsPage = 10)
        {
            _logger.LogInformation("GetRegionItems вызван.");
            var root = _itemRepository.ListAll();
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                root = root.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper()) || p.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            //сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    root = root.OrderByDescending(p => p.Name);
                    break;
                case SortState.CodeAsc:
                    root = root.OrderBy(p => p.Code);
                    break;
                case SortState.CodeDesc:
                    root = root.OrderByDescending(p => p.Code);
                    break;
                default:
                    root = root.OrderBy(p => p.Name);
                    break;
            }

            //пагинация
            var totalItems = root.Count();
            var itemsOnPage = root.Skip((pageIndex - 1) * itemsPage).Take(itemsPage).ToList();
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    //Districts=i.Districts,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc
                }),
                PageViewModel = new PageViewModel(totalItems, pageIndex, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString)
            };
            return indexModel;
        }

        public Region GetSingleBySpec(ISpecification<Region> spec) => _itemRepository.GetSingleBySpec(spec);

        public IEnumerable<Region> List(ISpecification<Region> spec) => _itemRepository.List(spec);

        public IEnumerable<Region> ListAll() => _itemRepository.ListAll();

        public Task<List<Region>> ListAllAsync() => _itemRepository.
        ListAllAsync();

        public Task<List<Region>> ListAsync(ISpecification<Region> spec) => _itemRepository.ListAsync(spec);

        public IQueryable<Region> Table()
        {
            throw new NotImplementedException();
        }

        public void Update(Region entity) => _itemRepository.Update(entity);

        public Task UpdateAsync(Region entity) => _itemRepository.UpdateAsync(entity);
        #endregion
    }
}
