using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Svr.api.Controllers
{
    [Route("api/[controller]")]
    public class ClaimsController : Controller
    {
        public class Claim
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }





        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Claim> Index()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Claim
            {
                Id = rng.Next(1,100),
                Name = "test"
            });
        }

        // GET api/<controller>/5
        //[HttpGet("{id}")]
        //public Claim Get(int id)
        //{
        //    return new Claim()
        //    {
        //        Id = 1,
        //        Name = "test1"
        //    };
        //}


        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
