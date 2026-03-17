using System;

namespace Flowey.SHARED.Constants
{
    public static class Messages
    {
        public const string CommentAdded = "Comment added successfully";
        public const string CommentUpdated = "Comment updated successfully";
        public const string CommentDeleted = "Comment deleted successfully";
        public const string CommentNotFound = "Comment not found";
        public const string CommentListed = "Comments listed successfully";
        public const string CommentCreateError = "Could not create comment";
        public const string CommentUpdateError = "Could not update comment";
        public const string CommentDeleteError = "Could not delete comment";

        public const string TaskAdded = "Task {0} has been created";
        public const string TaskUpdated = "Task updated successfully";
        public const string TaskDeleted = "Task deleted successfully";
        public const string TaskNotFound = "Task not found";
        public const string TaskListed = "Tasks listed successfully";
        public const string TaskCreateError = "Could not create task";
        public const string TaskUpdateError = "Could not update task";
        public const string TaskDeleteError = "Could not delete task";
        public const string TaskAssignedSuccessfully = "Task successfully assigned to the user";
        public const string TaskAssignError = "Could not assign task";
        public const string TaskStepUpdatedSuccessfully = "Task step successfully updated";
        public const string TaskStepUpdateFailed = "Failed to update task step";
        public const string TaskIsBlocked = "This task is blocked by one or more incomplete tasks and cannot be moved forward.";

        public const string CannotLinkTaskToItself = "A task cannot be linked to itself.";
        public const string TaskLinkAlreadyExists = "This exact link between the tasks already exists.";
        public const string TasksLinkedSuccessfully = "Tasks have been linked successfully.";
        public const string TaskLinkFailed = "An error occurred while linking the tasks.";

        public const string ProjectAdded = "Project added successfully";
        public const string ProjectUpdated = "Project updated successfully";
        public const string ProjectDeleted = "Project deleted successfully";
        public const string ProjectNotFound = "Project not found";
        public const string ProjectListed = "Projects listed successfully";
        public const string ProjectCreateError = "Could not create project";
        public const string ProjectUpdateError = "Could not update project";
        public const string ProjectDeleteError = "Could not delete project";
        public const string ProjectAlreadyAssignedToUser = "User is already assigned to this project";
        public const string ProjectAssigned = "User assigned to project successfully";
        public const string ProjectAssignError = "Could not assign user to project";
        public const string ProjectUserNotFound = "User is not assigned to this project";
        public const string ProjectStepsNotFound = "No steps found for this project";

        public const string ProjectRemoveUser = "User removed from project";
        public const string ProjectRemoveUserError = "Could not remove user";
        public const string UserRoleUpdated = "User role has been successfully updated.";
        public const string UserRoleUpdateFailed = "An error occurred while updating the user role.";

        public const string ProjectOwnershipTransferred = "Project ownership has been successfully transferred.";
        public const string ProjectOwnershipTransferFailed = "An error occurred while transferring project ownership.";
        public const string CannotTransferOwnershipToYourself = "You cannot transfer project ownership to yourself.";
        public const string UnauthorizedToTransferOwnership = "You are not authorized to transfer the ownership of this project.";

        public const string UserAdded = "User added successfully";
        public const string UserUpdated = "User updated successfully";
        public const string UserDeleted = "User deleted successfully";
        public const string UserNotFound = "User not found";
        public const string UserListed = "Users listed successfully";
        public const string UserCreateError = "Could not create user";
        public const string UserUpdateError = "Could not update user";
        public const string UserDeleteError = "Could not delete user";
        public const string UserEmailAlreadyUsed = "This email address is already in use";
        public const string UserPasswordChangeSuccess = "Password changed successfully";
        public const string PasswordChangeFailed = "An error occurred while changing your password. Please try again later.";
        public const string UserOldPasswordIncorrect = "The old password is incorrect";
        public const string UserPasswordUpdateFailed = "Could not change password";
        public const string InvalidCredentials = "Invalid email or password";
        public const string LoginSuccessful = "Login successful";
        public const string EmailRequired = "Email address is required";
        public const string InvalidEmailFormat = "A valid email address is required";
        public const string PasswordMinLength = "Password must be at least {MinLength} characters";
        public const string PasswordsDoNotMatch = "Passwords do not match";
        public const string NewPasswordCannotBeSame = "New password cannot be the same as the old password";

        public const string UserNotificationAdded = "User notification added successfully";
        public const string UserNotificationCreateError = "Could not create user notification";
        public const string UserNotificationNotFound = "User notification not found";
        public const string UserNotificationUpdated = "User notification updated successfully";
        public const string UserNotificationUpdateError = "Could not update user notification";
        public const string UserNotificationsMarkedAsRead = "All notifications have been marked as read.";
        public const string NoUnreadNotifications = "There are no unread notifications.";

        public const string NewTaskAssignedTitle = "New Task Assignment";
        public const string NewTaskAssignedMessage = "User {0} assigned a new task (#{1}) to you.";
        public const string TaskReassignedTitle = "Task Assignment Updated";
        public const string TaskReassignedMessage = "User {0} assigned you to {1}.";
        public const string NewMentionTitle = "New Mention";
        public const string NewMentionMessage = "User {0} mentioned you in {1}.";

        public const string StepAdded = "Step added successfully";
        public const string StepUpdated = "Step updated successfully";
        public const string StepDeleted = "Step deleted successfully";
        public const string StepNotFound = "Step not found";
        public const string StepListed = "Steps listed successfully";
        public const string StepCreateError = "Could not create step";
        public const string StepUpdateError = "Could not update step";
        public const string StepDeleteError = "Could not delete step";
        public const string StepListEmpty = "Step list cannot be empty";
        public const string BulkStepProjectMismatch = "All steps must belong to the same project for bulk insertion";
        public const string StepsCreatedSuccess = "{0} steps created successfully";
        public const string StepsCreateFailed = "Could not create steps";
        public const string UpdateListEmpty = "Update list cannot be empty";
        public const string BulkUpdateAborted = "Some records were not found. Bulk update aborted";
        public const string StepsUpdated = "{0} steps updated successfully";
        public const string StepsUpdateFailed = "Could not update steps";
        public const string CannotDeleteLastRequiredCategoryStep = "This step cannot be deleted because a project must have at least one starting (To Do) and one completing (Done) step.";
        public const string MustSelectTargetStep = "This step contains active tasks. A target step must be selected.";
        public const string CannotDeleteLastRequiredCategoryStepBulk = "Bulk update aborted: The requested changes would leave a project without at least one 'To Do' and one 'Done' step.";

        public const string UnauthorizedAccess = "Unauthorized";
        public const string ProjectIdMissing = "ProjectId could not be found in request parameters or body";
        public const string ProjectIdOrStepIdMissing = "ProjectId or StepId could not be found in request parameters or body";
        public const string ProjectIdOrTaskIdMissing = "ProjectId or TaskId could not be found in request parameters or body";
        public const string CommentIdOrTaskIdMissing = "CommentId or TaskId could not be found in request parameters or body";
        public const string TaskIdMissing = "TaskId could not be found in request parameters or body";
        public const string StepIdMissing = "StepId could not be found in request parameters or body";
        public const string UserNotProjectMember = "You are not a member of this project";
        public const string InsufficientPermissions = "You do not have permission for this action";

        public const string FileUploadSuccessfull = "File uploaded successfully";

        public const string RequiredField = "{PropertyName} is required";
        public const string MaxLengthExceeded = "{PropertyName} must not exceed {MaxLength} characters";
        public const string MinLengthError = "{PropertyName} must be at least {MinLength} characters";
        public const string HtmlTagsNotAllowed = "{PropertyName} cannot contain HTML tags";
        public const string ValidationFailed = "Validation Failed";
        public const string FileRequired = "Please select a file";
        public const string FileTooLarge = "File size must not exceed {0} MB";
        public const string InvalidFileType = "Invalid file type. Allowed types: {0}";
        public const string DeadlineCannotBeInThePast = "Deadline cannot be in the past.";

        public const string SubscriptionFailed = "Failed to process the subscription. Please try again.";
        public const string PaymentSuccessful = "Payment successful. Premium activated.";
        public const string NoInvoicesFound = "No invoices found for this account.";
    }
}
