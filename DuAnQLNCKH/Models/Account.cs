//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DuAnQLNCKH.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        public int IdAccount { get; set; }
        public string Email { get; set; }
        public string PassWord { get; set; }
        public int Access { get; set; }
        public int Status { get; set; }
    
        public virtual Information Information { get; set; }
    }
}