using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelog.Core.Abstractions;
using Travelog.Core.Models;
using Travelog.DataAccess.Models;

namespace Travelog.DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly TravelogDbContext _context;
        private readonly IMapper _mapper;
        public UsersRepository(TravelogDbContext context, IMapper mapper)
        {
            this._context = context;
            _mapper = mapper;
        }

        public async Task<Guid> AddAsync(User user)
        {
            var entity = _mapper.Map<UserEntity>(user);
            entity.Id = Guid.NewGuid();
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null) return null;
            return _mapper.Map<User>(entity);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var entity = await _context.Users.FindAsync(user.Id);
            if (entity == null)
            {
                throw new Exception("Пользователь");
            }
            entity.UserName = user.UserName;
            entity.Email = user.Email;
            entity.PasswordHash = user.PasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var entity= await _context.Users.FirstOrDefaultAsync(u=>u.Email.Equals(email));
            return _mapper.Map<User>(entity);
        }

        public async Task<IEnumerable<User>> SearchUsersByUserNameAsync(string username)
        {
            var usersEntity = await _context.Users.Where(u => u.UserName.Contains(username)).ToListAsync();
            return _mapper.Map<List<User>>(usersEntity);
        }



        public async Task<bool> IsUserNameTakenAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName== userName);
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.UserName==username);
            return _mapper.Map<User>(userEntity);
        }
    }
}
