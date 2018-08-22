using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.ViewModels.Areas.Manage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public class FilesManagerController : Controller
    {
        //مسیر پوشه فایل‌ها
        protected string FilesFolder = Infrastructure.Constants.FilesManagerRootPath;

        protected string KendoFileType = "f";
        protected string KendoDirType = "d";

        protected readonly IHostingEnvironment HostingEnvironment;
        public FilesManagerController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public KendoFileViewModel CreateFolder(string name, string path)
        {
            //تمیز سازی امنیتی
            name = Path.GetFileName(name);
            path = GetSafeDirPath(path);
            var dirToCreate = Path.Combine(path, name);

            Directory.CreateDirectory(dirToCreate);

            return new KendoFileViewModel
            {
                Name = name,
                Type = KendoDirType
            };
        }

        [HttpPost]
        public IActionResult DestroyFile(string name, string path)
        {
            //تمیز سازی امنیتی
            name = Path.GetFileName(name);
            path = GetSafeDirPath(path);

            var pathToDelete = Path.Combine(path, name);

            var attr = System.IO.File.GetAttributes(pathToDelete);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Directory.Delete(pathToDelete, recursive: true);
            }
            else
            {
                System.IO.File.Delete(pathToDelete);
            }

            return Json(new object[0]);
        }

        [HttpGet]
        public IActionResult GetFile(string path)
        {
            path = GetSafeFileAndDirPath(path);
            return PhysicalFile(path, "image/png");
        }

        [HttpGet]
        public IEnumerable<KendoFileViewModel> GetFilesList(string path)
        {
            path = GetSafeDirPath(path);
            //if (!System.IO.Directory.Exists(path))
            //{
            //    path = "";
            //    path = GetSafeDirPath(path);
            //}
            
            var imagesList = new DirectoryInfo(path)
                                .GetFiles()
                                .Select(fileInfo => new KendoFileViewModel
                                {
                                    Name = fileInfo.Name,
                                    Size = fileInfo.Length,
                                    Type = KendoFileType
                                }).ToList();

            var foldersList = new DirectoryInfo(path)
                                .GetDirectories()
                                .Select(directoryInfo => new KendoFileViewModel
                                {
                                    Name = directoryInfo.Name,
                                    Type = KendoDirType
                                }).ToList();

            return imagesList.Union(foldersList);
        }

        [HttpPost]
        public async Task<KendoFileViewModel> UploadFile(IFormFile file, string path)
        {
            //تمیز سازی امنیتی
            var name = Path.GetFileName(file.FileName);
            path = GetSafeDirPath(path);
            var pathToSave = Path.Combine(path, name);

            using (var fileStream = new FileStream(pathToSave, FileMode.Create))
            {
                await file.CopyToAsync(fileStream).ConfigureAwait(false);
            }

            return new KendoFileViewModel
            {
                Name = name,
                Size = file.Length,
                Type = KendoFileType
            };
        }

        protected string GetSafeDirPath(string path)
        {
            // path = مسیر زیر پوشه‌ی وارد شده
            if (string.IsNullOrWhiteSpace(path))
            {
                return Path.Combine(HostingEnvironment.WebRootPath, FilesFolder);
            }

            //تمیز سازی امنیتی
            path = Path.GetDirectoryName(path);
            path = Path.Combine(HostingEnvironment.WebRootPath, FilesFolder, path);
            return path;
        }

        protected string GetSafeFileAndDirPath(string path)
        {
            // path = مسیر فایل و زیر پوشه‌ی وارد شده

            //تمیز سازی امنیتی
            var name = Path.GetFileName(path);
            var dir = string.Empty;
            if (!string.IsNullOrWhiteSpace(path))
            {
                dir = Path.GetDirectoryName(path);
            }

            path = Path.Combine(HostingEnvironment.WebRootPath, FilesFolder, dir, name);
            return path;
        }
    }


    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public class ImagesManagerController : FilesManagerController
    {
        public ImagesManagerController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment)
        {
            // بازنویسی مسیر پوشه‌ی فایل‌ها
            FilesFolder = Infrastructure.Constants.ImagesManagerRootPath;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "path" })]
        public IActionResult GetThumbnail(string path)
        {
            //todo: create thumb/ resize image

            path = GetSafeFileAndDirPath(path);
            //return PhysicalFile(path, "image/png");

            //if (!System.IO.Directory.Exists(path))
            //{
            //    path = System.IO.Path.GetFileName(path);
            //    path = GetSafeFileAndDirPath(path);
            //}

            var outputStream = new MemoryStream();

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(path))
            {
                image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new SixLabors.Primitives.Size(Infrastructure.Constants.EditorImageThumbWidthSize, Infrastructure.Constants.EditorImageThumbHeightSize),
                            Mode = ResizeMode.Max
                        }));

                image.Save(outputStream, new PngEncoder());
            }

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(path, out contentType);

            return new FileContentResult(outputStream.ToArray(), contentType ?? "application/octet-stream");
        }
    }
}
