using System.Drawing.Imaging;
using System.Drawing;
using DTK.LPR;
using DTK.Video;
using DTKWebSample.Models;

namespace DTKWebSample.Services
{
    public static class Recognizer
    {
        private static LPREngine engine;
        private static LPRParams lprParams = new LPRParams();
        
        public static ResponseModel Worker(string countryCodes, int minPlateWidth, int maxPlateWidth,string fileName)
        {
            ResponseModel responseModel = new ResponseModel();
           

            try
            {
                lprParams.MinPlateWidth = minPlateWidth;
                lprParams.MaxPlateWidth = maxPlateWidth;
                lprParams.Countries = countryCodes;
                lprParams.FormatPlateText =false;

                engine = new LPREngine(lprParams, false, null);

                Bitmap image = (Bitmap)Image.FromFile(fileName);

                    List<LicensePlate> plates = null;

                    int processingTime = 0;

                    string method = "file"; // "memfile" or "buffer"
                if (method == "file")
                {
                    // read from file located on disk
                    plates = engine.ReadFromFile(fileName, out processingTime);
                }
                else if (method == "memfile")
                {
                    // read from file located in memory
                    byte[] fileData = File.ReadAllBytes(fileName);
                    unsafe
                    {
                        fixed (byte* p = fileData)
                        {
                            plates = engine.ReadFromMemFile((IntPtr)p, fileData.Length, out processingTime);
                        }
                    }
                }
                else if (method == "buffer")
                {
                    // read from image buffer
                    BitmapData bmpdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                    if (image.PixelFormat == PixelFormat.Format24bppRgb)
                        plates = engine.ReadFromImageBuffer(bmpdata.Scan0, image.Width, image.Height, bmpdata.Stride, PIXFMT.RGB24, out processingTime);
                    else if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                        plates = engine.ReadFromImageBuffer(bmpdata.Scan0, image.Width, image.Height, bmpdata.Stride, PIXFMT.GRAYSCALE, out processingTime);
                    else
                    {
                        responseModel.Error = "unsupported pixel format";
                        plates = new List<LicensePlate>(); // unsupported pixel format 
                    }
                       
                }
                else
                {
                    responseModel.Error = "Plaka Okunamadı";
                    responseModel.Status = false;
                }
                    string result = "";
                    Image plates_img = null;
                foreach (LicensePlate plate in plates)
                {
                    if (result.Length > 0) result += ",";
                    result += plate.Text;

                    Rectangle bbox = new Rectangle(plate.X, plate.Y, plate.Width, plate.Height);
                    Image plateImage = bbox.IsEmpty ? GetDemoPlateImage(plate.Text) : cropImage(image, bbox);

                    if (plates_img == null)
                    {
                        plates_img = plateImage;
                        string path = "";
                        string newFileName = DateTime.Now.ToString().Replace(" ", "").Replace("/", "").Replace(":", "").Replace("-", "").Trim() + ".png";
                        path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot/UploadedFiles"));
                        var fileStream = new FileStream(Path.Combine(path, newFileName), FileMode.Create);
                        plates_img.Save(fileStream, ImageFormat.Png);
                        responseModel.PlatePhoto = "/UploadedFiles/" + newFileName;
                    }
                        
                    else
                    {
                        Image image1 = new Bitmap(plates_img.Width + plateImage.Width, Math.Max(plates_img.Height, plateImage.Height));
                        Graphics g = Graphics.FromImage(image1);
                        g.DrawImage(plates_img, 0, 0);
                        g.DrawImage(plateImage, plates_img.Width, 0);
                        plates_img = image1;

                        string path = "";
                        string newFileName = DateTime.Now.ToString().Replace(" ", "").Replace("/", "").Replace(":", "").Replace("-", "").Trim() + ".png";
                        path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot/UploadedFiles"));
                        var fileStream = new FileStream(Path.Combine(path, newFileName), FileMode.Create);
                        plates_img.Save(fileStream, ImageFormat.Png);
                        responseModel.PlatePhoto = "/UploadedFiles/" + newFileName;

                    }
                }
               
                responseModel.Status = true;
                responseModel.Plate = result;
                if (result.Length == 0)
                {
                    responseModel.Status = false;
                    responseModel.Error = "Plaka Okunamadı";
                }
                
                responseModel.UploadedPhoto = fileName;  
                
                engine.Dispose();
                return responseModel;
            }
            catch (Exception ex)
            {
                engine.Dispose();
                responseModel.Status = false;
                responseModel.Error = ex.Message;
                return responseModel;
            }
        }
        private static Image GetDemoPlateImage(string text)
        {
            Bitmap bmp = new Bitmap(200, 50);
            Graphics gr = Graphics.FromImage(bmp);
            gr.DrawRectangle(Pens.Black, new Rectangle(2, 2, 196, 46));
            gr.DrawString(text, new Font("Arial", 22), Brushes.Black, new Rectangle(5, 5, 190, 40));
            gr.Dispose();
            return bmp;
        }

        private static Image cropImage(Image img, Rectangle cropArea)
        {
            return ((Bitmap)img).Clone(cropArea, img.PixelFormat);
        }

    }
}
