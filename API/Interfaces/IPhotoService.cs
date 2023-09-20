using API.Entities;
using CloudinaryDotNet.Actions;

namespace API;

public interface IPhotoService
{
  Task<ImageUploadResult>AddPhotoAsync(IFormFile file) ; 
  Task<DeletionResult>DeletePhotoAsync(string publicId) ; 

   
}