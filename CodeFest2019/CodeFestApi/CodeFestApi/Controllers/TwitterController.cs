using CodeFest.Api.Entities;
using CodeFestTwitter.Api.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFest.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TwitterController : ControllerBase
	{
		private readonly CodeFestDBContext _dbContext;

		public TwitterController(CodeFestDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Twitter>>> Get()
		{
			var list = await _dbContext.Twitter.ToListAsync();
			return list;
		}

		[HttpGet("top")]
		public async Task<ActionResult<IEnumerable<Twitter>>> Get([FromQuery]int take, [FromQuery] OrderType orderType)
		{
			List<Twitter> list = null;

			if (orderType == OrderType.asc)
				list = await _dbContext.Twitter.OrderBy(t => t.Score).Take(take).ToListAsync();
			else if (orderType == OrderType.desc)
				list = await _dbContext.Twitter.OrderByDescending(t => t.Score).Take(take).ToListAsync();
			else
				list = new List<Twitter>();

			return list;
		}

		[HttpGet("funniest")]
		public ActionResult<Twitter> GetFunniest()
		{
			// Get maximun value once they have been ordered (desc)
			var funniest = _dbContext.Twitter.OrderByDescending(t => t.Score).Take(1);
			return Ok(funniest);
		}

		[HttpGet("saddest")]
		public ActionResult<Twitter> GetSaddest()
		{
			// Get minimun value once they have been ordered (asc)
			var saddest = _dbContext.Twitter.OrderBy(t => t.Score).Take(1);
			return Ok(saddest);
		}
	}
}
