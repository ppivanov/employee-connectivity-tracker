using EctBlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Behaviour
{
    public static class UserBehaviour
    {
        public static Task<EctUser> GetExistingEctUserOrNewWrapper(string userId, HttpClient client, EctDbContext dbContext)
        {
            return GetExistingEctUserOrNew(userId, client, dbContext);
        }

        private static async Task<EctUser> GetExistingEctUserOrNew(string userId, HttpClient client, EctDbContext dbContext)
        {
            try
            {
                EctUser userForUserIdParm = dbContext.Users.First(user => user.Email.Equals(userId));
                return userForUserIdParm;
            }
            catch (Exception)
            {
                EctUser addUserResult = await dbContext.AddUser(userId, client);
                return addUserResult;
            }
        }
    }
}
