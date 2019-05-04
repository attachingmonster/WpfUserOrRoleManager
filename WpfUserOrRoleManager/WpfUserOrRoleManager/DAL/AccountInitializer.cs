using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.Models;

namespace WpfUserOrRoleManager.DAL
{
    class AccountInitializer:
        DropCreateDatabaseIfModelChanges<AccountContext>
    {
        protected override void Seed(AccountContext context)
        {
            var sysRole = new List<SysRole>
            {
                new SysRole {RoleName ="管理员",RoleDec ="administrtors have full authorization to perform systea administration."},
               new SysRole {RoleName ="普通用户",RoleDec ="general users an access the shared data they are suthorized for."}
            };
            sysRole.ForEach(s => context.SysRoles.Add(s));
            context.SaveChanges();
        }
    }
}
