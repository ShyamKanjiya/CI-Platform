using CI_platform.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.GenericRepository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        ICityRepository City { get; }
        ICountryRepository Country { get; }
        ICommentRepository Comment { get; }
        IFavouriteMissionRepository FavouriteMission { get; }
        IMissionApplicationRepository MissionApplication { get; }
        IMissionInviteRepository MissionInvite { get; }
        IMissionRepository Mission { get; }
        IMissionThemeRepository MissionTheme { get; }
        IPasswordResetRepository PasswordReset { get; }
        IStoryMediaRepository StoryMedia { get; }
        IStoryRepository Story { get; }
        IUserRepository User { get; }
        IMissionRatingRepository MissionRating { get; }
        IStoryInviteRepository StoryInvite { get; }
        int Save();
    }
}
