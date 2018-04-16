using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudProject.Models
{
    public class User
    {
        public virtual int ID { get; set; }
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
    }
}