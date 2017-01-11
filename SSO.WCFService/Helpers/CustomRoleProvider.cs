using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SSO.WCFService.Helpers
{
    public class CustomRoleProvider : RoleProvider
    {
        private SSOContext _db { get; set; }
        private string _applicationName;

        public CustomRoleProvider()
        {
            _db = new SSOContext();
        }

        public CustomRoleProvider(SSOContext db)
        {
            _db = db;            
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            if (config["applicationName"] != null)
                ApplicationName = config["applicationName"];
        }

        public override string ApplicationName
        {
            get
            {
                return _applicationName;
            }

            set
            {
                _applicationName = value;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var userIds = new List<int>();
            var roleIds = new List<int>();

            for (int i = 0; i < roleNames.Length; i++)
            {
                var role = _db.Roles.SingleOrDefault(r => r.Name.Equals(roleNames[i]));
                if (role == null)
                    throw new Exception("Role with specified name does not exist.");
                roleIds.Add(role.ID);
            }

            for (int i = 0; i < usernames.Length; i++)
            {
                var user = _db.Users.SingleOrDefault(u => u.Username.Equals(usernames[i]));
                if (user == null)
                    throw new Exception("User with specified username does not exist.");
                userIds.Add(user.ID);
            }

            for (int i = 0; i < userIds.Count; i++)
            {
                for (int j = 0; j < roleIds.Count; j++)
                    _db.ManageRoles.Add(new ManageRole { RoleID = roleIds[j], UserID = userIds[i] });
            }
        }

        public void AddUsersToRoles (int[] userIds, int[] roleIds)
        {
            for (int i = 0; i < roleIds.Length; i++)
            {
                var role = _db.Roles.SingleOrDefault(r => r.ID.Equals(roleIds[i]));
                if (role == null)
                    throw new Exception("Role with specified id does not exist.");

            }

            for (int i = 0; i < userIds.Length; i++)
            {
                var user = _db.Users.SingleOrDefault(u => u.ID.Equals(userIds[i]));
                if (user == null)
                    throw new Exception("User with specified id does not exist.");
            }

            for (int i = 0; i < userIds.Length; i++)
            {
                for (int j = 0; j < roleIds.Length; j++)
                    _db.ManageRoles.Add(new ManageRole { RoleID = roleIds[j], UserID = userIds[i] });
            }

        }

        public override void CreateRole(string roleName)
        {
            Roles r = new Roles();
            r.Name = roleName;
            _db.Roles.Add(r);
            _db.SaveChanges();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            Roles r = _db.Roles.SingleOrDefault(role => role.Name.Equals(roleName));

            if (r == null)
                throw new Exception("Role with that name doesn't exist.");
             
            if (throwOnPopulatedRole)
            {
                bool exists = _db.ManageRoles.Any(mr => mr.RoleID == r.ID);
                if (exists)
                    throw new Exception("There are users assigned to this role.");
            }
            List<ManageRole> mngroles = _db.ManageRoles.Where(m => m.RoleID.Equals(r.ID)).ToList();
            _db.ManageRoles.RemoveRange(mngroles);
            _db.Roles.Remove(r);
            _db.SaveChanges();
            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            Roles r = _db.Roles.SingleOrDefault(role => role.Name.Equals(roleName));

            if (r == null)
                throw new Exception("Role with that name doesn't exist.");

            return _db.ManageRoles.Include("User").Where(m => m.RoleID.Equals(r.ID) && m.User.Username.Contains(usernameToMatch)).Select(m => m.User.Username).ToArray();
        }

        public List<User> FindUsersInRole(string roleName)
        {
            Roles r = _db.Roles.SingleOrDefault(role => role.Name.Equals(roleName));

            if (r == null)
                throw new Exception("Role with that name doesn't exist.");

            return _db.ManageRoles.Include("User").Where(m => m.RoleID.Equals(r.ID)).Select(m => m.User).ToList();
        }

        public override string[] GetAllRoles()
        {
            return _db.Roles.Select(r => r.Name).ToArray();

        }

        public override string[] GetRolesForUser(string username)
        {
            var user = _db.Users.SingleOrDefault(u => u.Username.Equals(username));
            if (user == null)
                throw new Exception("User with specified username does not exist.");
            return _db.ManageRoles.Include("Role").Where(mr => mr.UserID == user.ID).Select(mr => mr.Role.Name).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var role = _db.Roles.SingleOrDefault(r => r.Name.Equals(roleName));
            if (role == null)
                throw new Exception("Role with specified name does not exist.");
            return _db.ManageRoles.Include("User").Where(mr => mr.RoleID == role.ID).Select(mr => mr.User.Username).ToArray();

        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var user = _db.Users.SingleOrDefault(u => u.Username.Equals(username));
            if (user == null)
                throw new Exception("User with specified username does not exist.");

            var role = _db.Roles.SingleOrDefault(r => r.Name.Equals(roleName));
            if (role == null)
                throw new Exception("Role with specified name does not exist.");

            return _db.ManageRoles.Any(mr => mr.RoleID == role.ID && mr.UserID == user.ID);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            List<ManageRole> mrs = _db.ManageRoles.Include("User").Include("Role").Where(mr => usernames.Contains(mr.User.Username) && roleNames.Contains(mr.Role.Name)).ToList();
            _db.ManageRoles.RemoveRange(mrs);
        }

        public override bool RoleExists(string roleName)
        {
            var role = _db.Roles.SingleOrDefault(r => r.Name.Equals(roleName));
            if (role == null)
                return false;
            return true;
        }
    }
}