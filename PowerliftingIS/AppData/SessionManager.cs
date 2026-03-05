using PowerliftingIS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerliftingIS.AppData
{
    public static class SessionManager
    {
        public static Coaches CurrentCoach { get; set; }
        public static bool IsAdmin { get; set; }
    }
}