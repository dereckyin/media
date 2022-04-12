using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace media.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetImagesController : Controller
    {
        private readonly ILogger<GetImagesController> _logger;
        private IMemoryCache _cache;

        public GetImagesController(ILogger<GetImagesController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet(Name = "GetImages")]
        public IActionResult GetImages(string? sc = "noImage", string? height = "290", string? width = "195", string? refresh = "n", string? fill = "n", string? scale = "w", string? rotata = "n")
        {
            // check if sc all charators are numbers
            if (!Regex.IsMatch(sc, @"^\d+$") || sc.Equals("noImage", StringComparison.OrdinalIgnoreCase))
            {
                return File(ImageTools.ToByteArray(ImageTools.GetNoImage()), "image/jpeg");
            }

            
            // default size
            int _height = 290;
            int _width = 195;

            int.TryParse(height, out _height);
            int .TryParse(width, out _width);

            if (_width > 1500)
                _width = 1500;

            if (_width <= 0)
                _width = 195;

            if (_height > 500)
                _height = 500;

            if (_height <= 0)
                _height = 290;

        
            // Get From File cache
            //Utility.SaveThumbNails(sc);

            // Get From DB
            var output = DbTools.getImageFromDbByProdId_Test(sc);

            if (fill.Equals("f"))
            {
                output = ImageTools.ResizeImage(output, _width, _height);
            }
            else
            {
                double inputRatio = 0;

                if (scale.Equals("w"))
                {
                    if (output.Width > _width)
                    {
                        inputRatio = (_width * output.Height) / output.Width;
                        output = ImageTools.ResizeImage(output, _width, Convert.ToInt32(inputRatio));
                    }
                }
                else if (scale.Equals("h"))
                {
                    if (output.Height > _height)
                    {
                        inputRatio = (_height * output.Width) / output.Height;
                        output = ImageTools.ResizeImage(output, Convert.ToInt32(inputRatio), _height);
                    }
                }
            }

            

            return File(ImageTools.ToByteArray(output), "image/jpeg");
        }
    }
}
