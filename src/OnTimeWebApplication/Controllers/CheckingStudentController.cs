using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OnTimeWebApplication.Controllers
{
    //[Produces("application/json")]
    [Route("api/checking")]
    public class CheckingStudentController : Controller
    {
        [HttpGet("testget")]
        public IActionResult sendTest()
        {
            return StatusCode(200);
        }

        [HttpPost("checkstudents")]
        public async Task<IActionResult> CheckStudents([FromBody]CheckingStudentData[] datas)
        {
            if (datas == null || datas.Length == 0)
            {
                return StatusCode(400);
            }



            return StatusCode(200);
        }
    }

    public class CheckingStudentData
    {
        public string Id { get; set; }
        public DateTime AttendTime { get; set; }
    }
}