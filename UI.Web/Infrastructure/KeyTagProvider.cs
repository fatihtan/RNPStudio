using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Infrastructure
{
    public class KeyTagProvider
    {
        public static Dictionary<string, List<string>> Keys = new Dictionary<string, List<string>>()
        {
            {
                "User/Detail", 
                new List<string>{
                    "tk.job.title",
                    "tk.industry"
                }
            },
            {
                "Customer/Detail",
                new List<string>{
                    "tk.job.title",
                    "tk.industry"
                }
            }
        };

        public enum TKList
        {
            TKJobTitle = 1,
            TKIndustry = 2
        }
    }
}