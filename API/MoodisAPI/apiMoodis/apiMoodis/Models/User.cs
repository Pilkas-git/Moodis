//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace apiMoodis.Models
{
    using System.Collections.Generic;
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.ImageInfos = new HashSet<ImageInfo>();
        }
    
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string GroupName { get; set; }
        public string PersonGroupId { get; set; }
        public string PersonId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageInfo> ImageInfos { get; set; }
    }
}
