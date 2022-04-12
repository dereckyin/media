using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System.Drawing;

namespace media.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "TestConnection")]
        //public IEnumerable<Members> Get(string? sr)
        public IActionResult Get(string? sr)
        {

            /*
             
            IEnumerable<Members> members = null;
            using (OracleConnection conn = new OracleConnection(DbTools.GetConnectionString()))
            {
                string strSql = "select Id, UserName, IBLOB from MEMBERS";
                var results = conn.Query(strSql).ToList();

                members = results
                .Select(o =>
                    new Members
                    {
                        Id = o.ID,
                        UserName = o.USERNAME,
                        Img = o.IBLOB != null ? (Bitmap)Bitmap.FromStream(new MemoryStream(o.IBLOB)) : null,
                    })
                .ToArray();
            }

            return members;
            */

            return File(ImageTools.ToByteArray(ImageTools.GetNoImage()), "image/jpeg");

        }
    }
}
