using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.DAL;
using WpfUserOrRoleManager.Method;

namespace WpfUserOrRoleManager.BLL
{
    public class SignUp
    {
        public void Sign(string Account, string Password)
        {
            try
            {
                string PasswordMD5 = CreateMD5.EncryptWithMD5(Password);
                var Signupd = new SignUpD();
                Signupd.SignUP(Account, PasswordMD5);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
