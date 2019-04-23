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
            var sysUsers = new List<SysUser>
            {
                new SysUser{ID=1,Account="123",Password="abc"}
            };
            sysUsers.ForEach(s => context.SysUsers.Add(s));
        }
    }
}



