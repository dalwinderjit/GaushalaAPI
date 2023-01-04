using GaushalAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.DBContext
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromForm] Photo photo)
        {
            var c = photo.formFile.FileName;
            var supportedTypes = new[] { "jpg","jpeg","png" };
            string extension = System.IO.Path.GetExtension(photo.formFile.FileName);
            if (photo.formFile.Length > 0)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "images");
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var filePath = Path.GetTempFileName();
                int i = 0;
                string fileName_ = photo.formFile.FileName;
                Console.WriteLine(fileName_);
                string fileNameWithPath = Path.Combine(path, fileName_);
                if (System.IO.File.Exists(fileNameWithPath)) {
                    fileName_ = photo.formFile.FileName+i;
                    
                    Console.Write(fileName_);
                }
                Console.WriteLine(filePath);
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    photo.formFile.CopyTo(stream);
                }
                /*using (var stream = System.IO.File.Create(fileNameWithPath, (int)FileMode.Create))
                {
                    await photo.formFile.CopyToAsync(stream);
                }*/
            }
            return Ok();
        }
    }
}
