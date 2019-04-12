using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Samples.SqliteGenerator;
using ZNetCS.AspNetCore.ResumingFileResults.Extensions;


namespace Samples.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlobsController : Controller
    {
        [HttpGet("generate/{name}/{rows}/")]
        public async Task<IActionResult> Generate(string name, int rows)
        {
            await Generator.CreateSqlite(name, rows);
            return this.Ok();
        }


        [HttpGet("download")]
        public IActionResult Download() => this.ResumingFile("", "");


        [Authorize]
        [HttpGet("downloadwithauth")]
        public async Task<IActionResult> DownloadWithAuth() => this.ResumingFile("", "");


        [HttpGet("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // TODO: with resume offset
            return this.Ok();
        }
    }
}
