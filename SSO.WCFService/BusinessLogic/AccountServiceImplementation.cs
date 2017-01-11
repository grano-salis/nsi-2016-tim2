using SSO.WCFService.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SSO.WCFService.DataContracts;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using SSO.WCFService.Exceptions;
using System.Net;
using SSO.WCFService.Helpers;

namespace SSO.WCFService.BusinessLogic
{
    public class AccountServiceImplementation
    {
        private SSOContext _db;

        public AccountServiceImplementation(SSOContext _db)
        {
            this._db = _db;
            var r = new CustomRoleProvider(_db);
        }

        public ActionResult ChangePassword(User user)
        {
            /*
               SSOContext _db = new SSOContext();
               UserLuka l = new UserLuka();
               l.NAME = "luka lol";

               _db.UserLukas.Add(l);
               _db.SaveChanges();
           */
            // Succeful login
            // Make token
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] tokenB = new byte[40];
            rng.GetBytes(tokenB);

            //Convert to hex
            String tokenHex = BitConverter.ToString(tokenB).Replace("-", String.Empty);
            Claim claim = new Claim();
            claim.Token = tokenHex;
            claim.Valid = "1";
            claim.Created = DateTime.Now;
            claim.User = user;

            throw new SSOBaseException("This method is currently not implemented.", System.Net.HttpStatusCode.NotImplemented);
        }

        public Claim Login(LoginRequest loginModel)
        {
           
            if (loginModel == null)
            {
                throw new SSOBaseException("Login Model required.", HttpStatusCode.BadRequest);
            }

            var user = _db.Users.SingleOrDefault(u => u.Username.Equals(loginModel.Username));
            if (user == null)
            {
                throw new WrongCredentialsException();
            }

            byte[] saltB = Convert.FromBase64String(user.Salt);

            byte[] passwordB = System.Text.Encoding.UTF8.GetBytes(loginModel.Password);
            var hashAlgorithm = new System.Security.Cryptography.SHA256Cng();
            byte[] passwordHashB = hashAlgorithm.ComputeHash(passwordB.Concat(saltB).ToArray());
            var passwordHashS = Convert.ToBase64String(passwordHashB);

            // TODO change database password field to nvarchar
            // 44 is length of
            if (!passwordHashS.Equals(user.Password.Substring(0, 44)))
            {
                throw new WrongCredentialsException();
            }

            // Succeful login
            // Make token
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] tokenB = new byte[40];
            rng.GetBytes(tokenB);

            //Convert to hex
            String tokenHex = BitConverter.ToString(tokenB).Replace("-", String.Empty);
            Claim claim = new Claim();
            claim.Token = tokenHex;
            claim.Valid = "1";
            claim.Created = DateTime.Now;
            claim.User = user;

            _db.Claims.Add(claim);
            _db.SaveChanges();

            return claim;
            
        }

        public ActionResult Register(RegisterRequest registerModel)
        {
            if (registerModel == null)
                    throw new ArgumentNullException();
            //TODO check model validation and throw ModelValidatoinException if neede
            if (!checkPassword(registerModel.Password))
            {
                throw new WeakPasswordException();
            }
            if (_db.Users.SingleOrDefault(u => u.Username.Equals(registerModel.Username)) != null)
            {
                // User with same username already exists
                throw new UsernameExistsException(registerModel.Username);
            }
            if (_db.UserInfoes.SingleOrDefault(u => u.Email.Equals(registerModel.Email)) != null)
            {
                // User with same email already exists
                throw new EmailExistsException(registerModel.Email);
            }

            // Make salt
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Salt should be long at least as hash algorith output. Sha256 output iz 32 bytes long.
            byte[] saltB = new byte[32];
            rng.GetBytes(saltB);
            var saltS = Convert.ToBase64String(saltB);

            // Make hash with salt
            byte[] passwordB = System.Text.Encoding.UTF8.GetBytes(registerModel.Password);
            var hashAlgorithm = new System.Security.Cryptography.SHA256Cng();
            byte[] passwordHashB = hashAlgorithm.ComputeHash(passwordB.Concat(saltB).ToArray());
            var passwordHashS = Convert.ToBase64String(passwordHashB);

            // Make new user
            User newUser = new User();
            newUser.Username = registerModel.Username;
            newUser.Salt = saltS;
            newUser.Password = passwordHashS;

            UserInfo info = new UserInfo();
            info.Email = registerModel.Email;
            info.FirstName = registerModel.FirstName;
            info.LastName = registerModel.LastName;

            info.User = newUser;

            //Save user

            _db.UserInfoes.Add(info);
            _db.SaveChanges();

            return new ActionResult { Message = "Successfully registered." };
        }

        private async Task<ActionResult> asyncFinish(Task t, String m)
        {
            //bla bla nesto neovisno od t
            await t;
            return new ActionResult { Message = m };
        }

        private bool checkPassword(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            Boolean haveLowercase = false;
            Boolean haveUppercase = false;
            Boolean haveDigit = false;

            foreach (char letter in password)
            {
                if (Char.IsLower(letter))
                {
                    haveLowercase = true;
                }
                else if (Char.IsUpper(letter))
                {
                    haveUppercase = true;
                }
                else if (Char.IsDigit(letter))
                {
                    haveDigit = true;
                }

            }

            // Password is strong enought if it have at least one lowecare, uppercase letter 
            return haveLowercase && haveUppercase;
        }

    }
}