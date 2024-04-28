using day_1.Data;
using day_1.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace day_1.Controllers
{
    public class StudentController : Controller
    {

        private readonly ApplicationDbContext obj;
        public StudentController(ApplicationDbContext dbContext)
        {
            obj = dbContext;
        }
        public IActionResult Index()
        {
            List<student> list = obj.students.ToList();
            return View(list);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(student studentData, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate a unique file name to prevent overwriting existing files
                string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string extension = Path.GetExtension(imageFile.FileName);
                fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

                // Specify the directory where you want to save the uploaded images
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(),"Uploads"); // Assuming the uploads directory is in wwwroot
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Save the uploaded image to the specified directory
                string filePath = Path.Combine(uploadPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Read the saved image file and convert it to byte array
                byte[] imageData;
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fileStream.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }
                var imageEntity = new student
                {
                    Imagedata = imageData,
                    Name=studentData.Name,
                    Age=studentData.Age,
                    Gender=studentData.Gender
                };

                // Save the ImageEntity object to the database
                obj.students.Add(imageEntity);
                await obj.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public IActionResult DisplayImage(int id)
        {
            var imageEntity = obj.students.Find(id);
            if (imageEntity == null)
            {
                return NotFound();
            }

            return File(imageEntity.Imagedata, "image/jpeg");
        }
        [HttpGet]
        public IActionResult edit(int id)
        {
            var list = obj.students.Where(x=>x.Id==id).FirstOrDefault();
            return View(list);
        }
        [HttpPost]
        public IActionResult edit(student std)
        {
            var update = obj.students.Find(std.Id);
            update.Name = std.Name;
            update.Age = std.Age;
            update.Gender = std.Gender;
            obj.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult delete(student std)
        {
            var find1 = obj.students.Find(std.Id);
            obj.students.Remove(find1);
            obj.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult details(int id)
        {
            var list = obj.students.Where(x => x.Id == id).FirstOrDefault();
            return View(list);
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult img()
        {
            return View();
        }
    }
}
