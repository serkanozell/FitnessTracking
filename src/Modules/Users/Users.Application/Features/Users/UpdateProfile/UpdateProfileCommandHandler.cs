using Users.Application.Errors;

namespace Users.Application.Features.Users.UpdateProfile
{
    internal sealed class UpdateProfileCommandHandler(
        IUserRepository _userRepository,
        IUsersUnitOfWork _unitOfWork) : ICommandHandler<UpdateProfileCommand, Result>
    {
        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.NotFound(request.UserId));

            user.UpdateProfile(request.FirstName, request.LastName);

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
