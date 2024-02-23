using ClosedXML.Excel;
using JWTAuthentication.NET6._0.Auth;
using JWTAuthentication.NET6._0.Helpter;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Services;
using JWTAuthentication.NET6._0.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace JWTAuthentication.NET6._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        public CommonController(IEmailService emailService, ApplicationDbContext context, IProductService productService, ICategoryService categoryService)
        {
            _emailService = emailService;
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail([FromBody] EmailModel email)
        {
            try
            {
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = email.emailModel;
                mailrequest.Subject = "Welcome to The Vu";
                mailrequest.Body = _emailService.GetHtmlcontent(email.emailModel);
                await _emailService.SendEmailAsync(mailrequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File not selected");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok("File uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("multiupload")]
        public async Task<IActionResult> MultiUpload([FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return Content("files not selected");

                foreach (var file in files)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                return Ok("upload files success");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ExportExcel")]
        public ActionResult ExportExcel()
        {
            var _productdata = GetProductdata();
            var _categorydata = GetCategorydata();
            using (XLWorkbook wb = new XLWorkbook())
            {
                var sheet1 = wb.AddWorksheet(_productdata, "Product Records");
                wb.AddWorksheet(_categorydata);

                sheet1.Column(1).Style.Font.FontColor = XLColor.Red;
                sheet1.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                sheet1.Columns(2, 4).Style.Font.FontColor = XLColor.Blue;

                sheet1.Row(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.Black;
                //sheet1.Row(1).Cells(1,3).Style.Fill.BackgroundColor = XLColor.Yellow;
                sheet1.Row(1).Style.Font.FontColor = XLColor.White;

                sheet1.Row(1).Style.Font.Bold = true;
                /*sheet1.Row(1).Style.Font.Shadow = true;
                sheet1.Row(1).Style.Font.Underline = XLFontUnderlineValues.Single;
                sheet1.Row(1).Style.Font.VerticalAlignment = XLFontVerticalTextAlignmentValues.Superscript;*/
                sheet1.Row(1).Style.Font.Italic = true;

               /* sheet1.Rows(2, 3).Style.Font.FontColor = XLColor.AshGrey;*/

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sample.xlsx");
                }
            }
        }

        [NonAction]
        private DataTable GetProductdata()
        {
            DataTable dt = new DataTable();
            dt.TableName = "Productdata";
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));

            List<ProductDTO> products = _productService.GetProducts();
            if (products.Count > 0)
            {
                products.ForEach(product =>
                {
                    dt.Rows.Add(product.ProductId, product.ProductName, product.ProductDescription, product.ProductPrice);
                });
            }

            return dt;
        }

        [NonAction]
        private DataTable GetCategorydata()
        {
            DataTable dt = new DataTable();
            dt.TableName = "Categorydata";
            dt.Columns.Add("Category Id", typeof(int));
            dt.Columns.Add("Category Name", typeof(string));

            List<CategoryDTO> categories = _categoryService.GetCategories();
            if (categories.Count > 0)
            {
                categories.ForEach(item =>
                {
                    dt.Rows.Add(item.CategoryId, item.CategoryName);
                });
            }

            return dt;
        }

    }
}
