using IcardProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IcardProject.Controllers
{
    public class StudentController : Controller
    {
        DatabaseConnection _connection;
        IWebHostEnvironment _environment;
        public StudentController(DatabaseConnection connection, IWebHostEnvironment environment)
        {
            _connection = connection;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> StudentRegister(Student data, IFormFile ProfilePhoto)
        {
            string folder = Path.Combine(_environment.WebRootPath, "Images");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filename = Guid.NewGuid().ToString() + "-" + ProfilePhoto.FileName;

            string filePath = Path.Combine(folder, filename);

            var stream = new FileStream(filePath, FileMode.Create);

            await ProfilePhoto.CopyToAsync(stream);


            data.ProfilePicture = filename;

            _connection.Student.Add(data);
            _connection.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult ShowStudents()
        {
            var data = _connection.Student.Where(s => s.isDeleted == false).ToList();
            return View(data);
        }

        public IActionResult Icard(int id)
        {
            var data = _connection.Student.Find(id);

            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var data = _connection.Student.Find(id);

            var folder = Path.Combine(_environment.WebRootPath, "Images");
            var filepath = Path.Combine(folder, data.ProfilePicture);

            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }

            _connection.Student.Remove(data);
            _connection.SaveChanges();

            return RedirectToAction("ShowStudents");
        }

        public IActionResult Edit(int id)
        {
            var data = _connection.Student.Find(id);

            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Student data, IFormFile? ProfilePicture)
        {
            var olddata = _connection.Student.AsNoTracking().FirstOrDefault(x => x.Id == data.Id);
            if (ProfilePicture == null)
            {
                data.ProfilePicture = olddata.ProfilePicture;
                _connection.Student.Update(data);
                _connection.SaveChanges();

                return RedirectToAction("ShowStudents");
            }
            else
            {
                string folder = Path.Combine(_environment.WebRootPath, "Images");

                if (!string.IsNullOrEmpty(olddata.ProfilePicture))
                {
                    string oldfilepath = Path.Combine(folder, olddata.ProfilePicture);
                    if (System.IO.File.Exists(oldfilepath))
                    {
                        System.IO.File.Delete(oldfilepath);
                    }
                }

                string filename = Guid.NewGuid().ToString();
                string newfilepath = Path.Combine(folder, (filename + "-" + ProfilePicture.FileName));

                var stream = new FileStream(newfilepath, FileMode.Create);

                await ProfilePicture.CopyToAsync(stream);

                data.ProfilePicture = (filename + "-" + ProfilePicture.FileName);

                _connection.Student.Update(data);
                _connection.SaveChanges();

                return RedirectToAction("ShowStudents");
            }


        }

        public IActionResult SendInRecycle(int id)
        {
            var data = _connection.Student.Find(id);
            data.isDeleted = true;
            _connection.Student.Update(data);
            _connection.SaveChanges();
            return RedirectToAction("ShowStudents");
        }

        public IActionResult RecycleBin()
        {
            var data = _connection.Student.Where(s => s.isDeleted == true).ToList();
            return View(data);
        }

        public IActionResult Restore(int id)
        {
            var data = _connection.Student.Find(id);
            data.isDeleted = !data.isDeleted;
            _connection.Student.Update(data);
            _connection.SaveChanges();
            return RedirectToAction("RecycleBin");
        }
    }
}

