﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestSite.Startup))]
namespace TestSite
{
    /// <summary>
    /// Main startup class for app
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configures the application
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
