using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2.Common
{
    public class Util
    {
        public static int GetInt(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return 0;

            return int.Parse(number);
        }

        public static double GetDouble(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return 0.0;

            return double.Parse(number);
        }
    }
}
