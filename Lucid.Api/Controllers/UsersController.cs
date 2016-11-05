using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Database;
using Lucid.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lucid.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
	    private readonly IUserRepository _userRepository;

	    public UsersController(IUserRepository userRepository)
	    {
			_userRepository = userRepository;
	    }
		
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
	        return await _userRepository.GetList();
        }
		
		[HttpGet("{id}")]
		public async Task<User> Get(int id)
		{
			return await _userRepository.Get(id);
		}

		// GET api/values/5
		//[HttpGet("{id}")]
		//public string Get(int id)
		//{
		//    return "value";
		//}

		//// POST api/values
		//[HttpPost]
		//public void Post([FromBody]string value)
		//{
		//}

		//// PUT api/values/5
		//[HttpPut("{id}")]
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//// DELETE api/values/5
		//[HttpDelete("{id}")]
		//public void Delete(int id)
		//{
		//}
	}
}
