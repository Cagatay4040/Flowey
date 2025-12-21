using Flowey.BUSINESS.DTO.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.Result.Abstract;

namespace Flowey.BUSINESS.Abstract
{
    public interface ICommentService
    {
        #region Get Methods

        Task<IDataResult<List<CommentGetDTO>>> GetByTaskIdAsync(Guid taskId);

        #endregion

        #region Insert Methods

        Task<IResult> AddAsync(CommentAddDTO dto);

        #endregion

        #region Update Methods

        Task<IResult> UpdateAsync(CommentUpdateDTO dto);

        #endregion

        #region Delete Methods

        Task<IResult> DeleteAsync(Guid id);

        #endregion
    }
}
