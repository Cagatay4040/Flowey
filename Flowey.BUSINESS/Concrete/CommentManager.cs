using AutoMapper;
using Flowey.BUSINESS.Abstract;
using Flowey.BUSINESS.DTO.Comment;
using Flowey.BUSINESS.Extensions;
using Flowey.CORE.Constants;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.DATACCESS.Abstract;
using Flowey.DOMAIN.Model.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.BUSINESS.DTO.Notification;

namespace Flowey.BUSINESS.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly IUserNotificationService _userNotificationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public CommentManager(ICommentRepository commentRepository, ITaskRepository taskRepository, IMapper mapper, IUserNotificationService userNotificationService, ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
            _userNotificationService = userNotificationService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
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

            var cleanContent = dto.Content.ToSafeRichText();

            var comment = _mapper.Map<Comment>(dto);
            comment.Content = cleanContent;

            int effectedRow = await _commentRepository.AddAsync(comment);

            if (effectedRow > 0)
            {
                await SendMentionNotificationsAsync(cleanContent, dto.TaskId);
                return new Result(ResultStatus.Success, Messages.CommentAdded);
            }

            return new Result(ResultStatus.Error, Messages.CommentCreateError);
        }

        private async System.Threading.Tasks.Task SendMentionNotificationsAsync(string content, Guid taskId)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            var currentUserId = _currentUserService.GetUserId().Value;
            var senderUser = await _userRepository.GetByIdAsync(currentUserId);
            string senderName = senderUser != null ? $"{senderUser.Name} {senderUser.Surname}" : "System";

            var mentionedUserIds = ExtractMentionedUserIds(content);
            if (!mentionedUserIds.Any()) return;

            var existingTask = await _taskRepository.GetByIdAsync(taskId);
            string taskIdentifier = existingTask?.TaskKey != null ? $"task #{existingTask.TaskKey}" : "a task";

            foreach (var mentionedUserId in mentionedUserIds)
            {
                if (mentionedUserId != currentUserId)
                {
                    await _userNotificationService.AddUserNotificationAsync(new BUSINESS.DTO.Notification.UserNotificationAddDTO
                    {
                        UserId = mentionedUserId,
                        SenderId = currentUserId,
                        Title = Messages.NewMentionTitle,
                        Message = string.Format(Messages.NewMentionMessage, senderName, taskIdentifier),
                        ActionUrl = $"/board/{existingTask?.ProjectId}?taskId={taskId}"
                    });
                }
            }
        }

        private List<Guid> ExtractMentionedUserIds(string htmlContent)
        {
            var userIds = new List<Guid>();
            if (string.IsNullOrWhiteSpace(htmlContent)) return userIds;

            var regex = new System.Text.RegularExpressions.Regex(@"data-id=""([a-fA-F0-9\-]{36})""");
            var matches = regex.Matches(htmlContent);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups.Count > 1 && Guid.TryParse(match.Groups[1].Value, out Guid parsedId))
                {
                    if (!userIds.Contains(parsedId))
                    {
                        userIds.Add(parsedId);
                    }
                }
            }
            return userIds;
        }

        #endregion

        #region Update Methods

        public async Task<IResult> UpdateAsync(CommentUpdateDTO dto)
        {
            var existingComment = await _commentRepository.GetByIdAsync(dto.CommentId);

            if (existingComment == null)
                return new Result(ResultStatus.Error, Messages.CommentNotFound);

            var cleanContent = dto.Content.ToSafeRichText();

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

    }
}
