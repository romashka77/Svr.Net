using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Web.Controllers;
using Svr.Web.Interfaces;
using Svr.Web.Models;
using Svr.Web.Models.RegionsViewModels;
using Xunit;

namespace Svr.Web.Test
{
    public class RegionsControllerTests
    {
        [Fact]
        public void Create_ReturnsViewResult()
        {
            // Arrange
            //var region = new Region();
            //var mock = new Mock<IRegionRepository>();
            //mock.Setup(regionRepository => regionRepository.AddAsync(region)).Returns()

            //AddAsync(
            //var controller = new RegionsController(mock.Object);
            //var now = DateTime.UtcNow;
            //var newRegion = new RegionItemViewModel() { Id = 5, Name = "Test", Description = "Test", Code = "001", CreatedOnUtc = now, UpdatedOnUtc = now };
            //// Act
            //var result = await controller.Create(newRegion);
            //// Assert
            //var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
        }
        //[Fact]
        //public async void Details_ReturnsViewResult()
        //{
        //    // Arrange
        //    int testRegionId = 1;
        //    var mock = new Mock<IRegionRepository>();
        //    mock.Setup(regionRepository => regionRepository.GetByIdWithItemsAsync(testRegionId)).Returns(GetByIdWithItemsAsync(testRegionId));
        //    var controller = new RegionsController(mock.Object);
        //    // Act
        //    var result = await controller.Details(1);
        //    // Assert
        //    Assert.NotNull(result);
        //    //Является ли возвращаемый результат объектом ViewResult
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    //Передается ли в представление в качестве модели объект RegionIndexViewModel
        //    var model = Assert.IsAssignableFrom<Region>(viewResult.ViewData.Model);
        //    Assert.Equal(testRegionId, model.Id);
        //}
        //[Fact]
        //public async void Index_ReturnsViewResult()
        //{
        //    // Arrange
        //    var mock = new Mock<IRegionRepository>();
        //    mock.Setup(regionRepository => regionRepository.ListAllAsync()).Returns(GetTestRegions());
        //    var controller = new RegionsController(mock.Object);
        //    // Act
        //    var result = await controller.Index();
        //    // Assert
        //    Assert.NotNull(result);
        //    //Является ли возвращаемый результат объектом ViewResult
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    //Передается ли в представление в качестве модели объект RegionIndexViewModel
        //    var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
        //    //количество объектов в модели, которая передается в представление
        //    Assert.Equal(4, model.ItemViewModels.Count());
        //}

        //private async Task<List<Region>> GetTestRegions()
        //{
        //    var now = DateTime.UtcNow;
        //    var regions = new List<Region>
        //    {
        //        new Region { Id=1, Name="Region 1", Code="1",Description="Регион 1",CreatedOnUtc=now,UpdatedOnUtc=now},
        //        new Region { Id=2, Name="Region 2", Code="2",Description="Регион 2",CreatedOnUtc=now,UpdatedOnUtc=now},
        //        new Region { Id=3, Name="Region 3", Code="3",Description="Регион 3",CreatedOnUtc=now,UpdatedOnUtc=now},
        //        new Region { Id=4, Name="Region 4", Code="4",Description="Регион 4",CreatedOnUtc=now,UpdatedOnUtc=now}
        //    };
        //    return regions;
        //}
        //private async Task<Region> GetByIdWithItemsAsync(int? id)
        //{
        //    var now = DateTime.UtcNow;
        //    return new Region { Id = 1, Name = "Region 1", Code = "1", Description = "Регион 1", CreatedOnUtc = now, UpdatedOnUtc = now };
        //}
    }
}
