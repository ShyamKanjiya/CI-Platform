using CI_platform.Entities;
using CI_platform.Entities.DataModels;
using CI_platform.Repositories.GenericRepository.Interface;
using CI_platform.Repositories.Repository;
using CI_platform.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.GenericRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CIDbContext _db;
        public UnitOfWork(CIDbContext db)
        {
            _db = db;
            City = new CityRepository(_db);
            Country = new CountryRepository(_db);
            Comment = new CommentRepository(_db);
            FavouriteMission = new FavouriteMissionRepository(_db);
            MissionApplication = new MissionApplicationRepository(_db);
            MissionInvite = new MissionInviteRepository(_db);
            Mission = new MissionRepository(_db);
            MissionTheme = new MissionThemeRepository(_db);
            PasswordReset = new PasswordResetRepository(_db);
            StoryMedia = new StoryMediaRepository(_db);
            Story = new StoryRepository(_db);
            User = new UserRepository(_db);
            MissionRating = new MissionRatingRepository(_db);
            StoryInvite = new StoryInviteRepository(_db);
        }
        public ICityRepository City { get; private set; }

        public ICountryRepository Country { get; private set; }

        public ICommentRepository Comment { get; private set; }

        public IFavouriteMissionRepository FavouriteMission { get; private set; }

        public IMissionApplicationRepository MissionApplication { get; private set; }

        public IMissionInviteRepository MissionInvite { get; private set; }

        public IMissionRepository Mission { get; private set; }

        public IMissionThemeRepository MissionTheme { get; private set; }

        public IPasswordResetRepository PasswordReset { get; private set; }

        public IStoryMediaRepository StoryMedia { get; private set; }

        public IStoryRepository Story { get; private set; }

        public IUserRepository User { get; private set; }

        public IMissionRatingRepository MissionRating { get; private set; }

        public IStoryInviteRepository StoryInvite { get; private set; }

        public int Save()
        {
            return _db.SaveChanges();
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
