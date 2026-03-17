using Flowey.CORE.Result.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.Interfaces.Services
{
    public interface ILocalFileStorageService
    {
        Task<IDataResult<string>> UploadAsync(IFormFile file);
    }
}
