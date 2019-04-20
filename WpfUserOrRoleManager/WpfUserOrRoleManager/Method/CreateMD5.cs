using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfUserOrRoleManager.Method
{
    public class CreateMD5
    {
        public static string EncryptWithMD5(string source) //MD5加密
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            var md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            var strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            return strbul.ToString();
        }
    }

}
