using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUserOrRoleManager.ViewModels
{
    /// <summary>
    /// webapi返回的信息
    /// </summary>
    public class VMregisterInfomation
    {
        /// <summary>
        /// webapi注册信息：成功、失败、无效等
        /// </summary>
        public String message { get; set; }
    }
}
