using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudProject.Models
{
    public class Document
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Author { get; set; }
        public virtual string Link { get; set; }
        public virtual string ContentPath { get; set; }
    }
}