using Flowey.CORE.Result.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Abstract
{
    public interface IFileService
    {
        Task<IDataResult<string>> UploadAsync(IFormFile file);
    }
}
