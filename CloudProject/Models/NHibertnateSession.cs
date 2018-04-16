using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg;

namespace CloudProject.Models
{
    public class NHibertnateSession
    {
        public static ISession OpenSession()
        {
            var configuration = new Configuration();
            var configurationPath = HttpContext.Current.Server.MapPath(@"~\Models\Nhibernate\hibernate.cfg.xml");
            configuration.Configure(configurationPath);
            var userConfigurationFile = HttpContext.Current.Server.MapPath(@"~\Models\Nhibernate\User.hbm.xml");
            var documentConfigurationFile = HttpContext.Current.Server.MapPath(@"~\Models\Nhibernate\Document.hbm.xml");
            configuration.AddFile(userConfigurationFile);
            configuration.AddFile(documentConfigurationFile);
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            return sessionFactory.OpenSession();
        }
    }
}