
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;

namespace Mhrm.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Session_token { get; set; }
        public string Ip_address { get; set; }
        public string User_agent  { get; set; }
        
        public DateTime Login_at  { get; set; }
        public DateTime Logout_at { get; set; }
        public DateTime Expirees_at { get; set; }

        
    }
}
