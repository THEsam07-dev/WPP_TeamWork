using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library_CP.database;

namespace Library_CP.helpers
{
    public static class Session
    {
        public static Users CurrentUser { get; set; }
    }
}
