using System.Data.Common;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context=context;
        }


        [HttpGet]
        public async Task<ActionResult> getUsers(){

            return Ok( await _context.Users.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> getUserById(int id){

            return Ok (await _context.Users.FindAsync(id));
        }
    }
}