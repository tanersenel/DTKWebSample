using DTKWebSample.Interfaces;
using DTKWebSample.Models;
using ImageMagick;
using Microsoft.AspNetCore.Components.Forms;
using System.Drawing;
using System.IO;

namespace DTKWebSample.Services
{
    public class BufferedFileUploadLocalService : IBufferedFileUploadService
    {
        public async Task<ResponseModel> UploadFile(IFormFile file)
        {
            ResponseModel res = new ResponseModel();
            string path = "";
            string newFileName = DateTime.Now.ToString().Replace(" ", "").Replace("/","").Replace(":","").Replace("-","").Replace(".","").Trim() + Path.GetExtension(file.FileName);
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot/UploadedFiles"));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {

                        await file.CopyToAsync(fileStream);
                        using (var image = new MagickImage(fileStream.Name))
                        {
                            image.Resize(800, 800);
                            image.Strip();
                            image.Quality = 75;
                            image.Write(Path.Combine(path, newFileName));
                        }
                        fileStream.Close();
                        
                        File.Delete(fileStream.Name);
                        res.Response = Path.Combine(path, newFileName);
                        res.UploadedPhoto = newFileName;
                        res.Status = true;
                    }
                   


                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }
}
