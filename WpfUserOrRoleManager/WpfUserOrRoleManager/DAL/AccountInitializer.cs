using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.Models;

namespace WpfUserOrRoleManager.DAL
{
    class AccountInitializer :
       DropCreateDatabaseIfModelChanges<AccountContext>
    {
        protected override void Seed(AccountContext context)
        {
            /*  var sysUsers = new List<SysUser>
              {
                  new SysUser{ID=1,Account="123",Password="abc"}
              };
              sysUsers.ForEach(s => context.SysUsers.Add(s));
              */

            var sysRole = new List<SysRole>
            {
                new SysRole {RoleName ="学生",RoleDec ="登录学生窗口"},
               new SysRole {RoleName ="教师",RoleDec ="登录教师窗口"},
               new SysRole{RoleName="admin",RoleDec="admin登录用户管理界面" }
            };
            sysRole.ForEach(s => context.SysRoles.Add(s));
            context.SaveChanges();
        }
    }
}



