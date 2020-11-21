using CodersAcademy.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodersAcademy.API.Repository
{
    public class UserRepository
    {
        private MusicContext context { get; set; }
        public UserRepository(MusicContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(User user)
        {
            await this.context.User.AddAsync(user);
            await this.context.SaveChangesAsync();
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            return await this.context.User.Include(x => x.FavoriteMusics)
                .ThenInclude(x => x.Music)
                .ThenInclude(x => x.Album)
                .Where(x => x.Email == email && x.Password == password)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await this.context.User
                .Include(x => x.FavoriteMusics)
                .ThenInclude(x => x.Music)
                .ThenInclude(x => x.Album)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(User user)
        {
            this.context.User.Update(user);
            await this.context.SaveChangesAsync();
        }

        public async Task<IList<User>> GetAllAsync()
        {
            return await this.context.User
                .Include(x => x.FavoriteMusics)
                .ThenInclude(x => x.Music)
                .ThenInclude(x => x.Album)
                .ToListAsync();
        }

        public async Task RemoveAsync(User user)
        {
            this.context.User.Remove(user);
            await this.context.SaveChangesAsync();
        }
    }
}
