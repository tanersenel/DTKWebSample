using DTKWebSample.Models;

namespace DTKWebSample.Interfaces
{
    public interface IBufferedFileUploadService
    {
        Task<ResponseModel> UploadFile(IFormFile file);
    }
}
