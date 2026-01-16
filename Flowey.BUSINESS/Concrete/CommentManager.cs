using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Comment;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public CommentManager(ICommentRepository commentRepository, ITaskRepository taskRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<IDataResult<List<CommentGetDTO>>> GetByTaskIdAsync(Guid taskId)
        {
            var comments = await _commentRepository.GetList(c => c.TaskId == taskId && c.IsActive, includes: x => x.User);
            var commentDtos = _mapper.Map<List<CommentGetDTO>>(comments).OrderByDescending(c => c.CreatedDate).ToList();
            return new DataResult<List<CommentGetDTO>>(ResultStatus.Success, Messages.CommentListed, commentDtos);
        }

        #endregion

        #region Insert Methods

        public async Task<IResult> AddAsync(CommentAddDTO dto)
        {
            var existingTask = await _taskRepository.AnyAsync(x => x.Id == dto.TaskId);

            if (!existingTask)
                return new Result(ResultStatus.Error, Messages.TaskNotFound);

            var cleanContent = SanitizeContent(dto.Content);

            var comment = _mapper.Map<Comment>(dto);
            comment.Content = cleanContent;

            int effectedRow = await _commentRepository.AddAsync(comment);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.CommentAdded);

            return new Result(ResultStatus.Error, Messages.CommentCreateError);
        }

        #endregion

        #region Update Methods

        public async Task<IResult> UpdateAsync(CommentUpdateDTO dto)
        {
            var existingComment = await _commentRepository.GetByIdAsync(dto.CommentId);

            if (existingComment == null)
                return new Result(ResultStatus.Error, Messages.CommentNotFound);

            var cleanContent = SanitizeContent(dto.Content);

            _mapper.Map(dto, existingComment);

            existingComment.Content = cleanContent;

            int effectedRow = await _commentRepository.UpdateAsync(existingComment);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.CommentUpdated);

            return new Result(ResultStatus.Error, Messages.CommentUpdateError);
        }

        #endregion

        #region Delete Methods

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var existingStep = await _commentRepository.GetByIdAsync(id);

            if (existingStep == null) 
                return new Result(ResultStatus.Error, Messages.CommentNotFound);

            int effectedRow =  await _commentRepository.SoftDeleteAsync(existingStep);

            if (effectedRow > 0)
                return new Result(ResultStatus.Success, Messages.CommentDeleted);

            return new Result(ResultStatus.Error, Messages.CommentDeleteError);
        }

        #endregion

        #region Helper Methods

        private string SanitizeContent(string rawContent)
        {
            if (string.IsNullOrEmpty(rawContent)) return rawContent;

            var sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Add("table");
            sanitizer.AllowedTags.Add("tr");
            sanitizer.AllowedTags.Add("td");
            sanitizer.AllowedTags.Add("img");

            return sanitizer.Sanitize(rawContent);
        }

        #endregion
    }
}
