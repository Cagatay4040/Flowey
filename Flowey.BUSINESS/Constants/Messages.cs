using System;

namespace Flowey.BUSINESS.Constants
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
        public const string ProjectRemoveUser = "User removed from project";
        public const string ProjectRemoveUserError = "Could not remove user";
        public const string ProjectStepsNotFound = "No steps found for this project.";

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
        public const string UserOldPasswordIncorrect = "The old password is incorrect";
        public const string UserPasswordUpdateFailed = "Could not change password";
        public const string InvalidCredentials = "Invalid email or password";
        public const string LoginSuccessful = "Login successful.";

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

        public const string UnauthorizedAccess = "Unauthorized";
        public const string ProjectIdMissing = "ProjectId could not be found in request parameters or body";
        public const string TaskIdMissing = "TaskId could not be found in request parameters or body";
        public const string StepIdMissing = "StepId could not be found in request parameters or body";
        public const string UserNotProjectMember = "You are not a member of this project";
        public const string InsufficientPermissions = "You do not have permission for this action";
    }
}
