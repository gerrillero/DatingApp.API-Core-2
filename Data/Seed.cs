using System.Collections.Generic;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context) => this._context = context;
        
        public void Seedusers() {
           // this._context.Users.RemoveRange(_context.Users);
           // this._context.SaveChanges();
           
           // Seed Users
           var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
           var users = JsonConvert.DeserializeObject<List<User>>(userData);
           
           foreach (var user in users)
           {
               byte[] passwordHash, passworSalt;
               CreatePasswordHash("password", out passwordHash, out passworSalt);
               
               user.PasswordHash = passwordHash;
               user.PasswordSalt = passworSalt;
               user.Username = user.Username.ToLower();
               
              this._context.Add(user);
           }
           
           this._context.SaveChanges();
        }
        
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
           using (var hmac = new System.Security.Cryptography.HMACSHA512())
           {
               passwordSalt = hmac.Key;
               passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           }
        }
    }
}