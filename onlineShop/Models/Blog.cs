using System;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Markup { get; set; }

        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }

        public AppUser LastModifiedBy { get; set; }
        public string LastModifiedById { get; set; }


        public DateTime? LastModifiedOn { get; set; }

        [Display(Name = "Preview Size (characters)")]
        public int PreviewSize { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Show Creation Date")]
        public bool DisplayDate { get; set; }

        [Display(Name = "Pin to Top")]
        public bool IsPinned { get; set; }

    }
}
