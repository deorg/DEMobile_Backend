using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.Management
{
    public class m_Member
    {
        public int member { get; set; }
        public int registeredMember { get; set; }
        public int signedInMember { get; set; }
    }
}