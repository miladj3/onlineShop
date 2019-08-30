using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace onlineShop.Models
{
    public class Product
    {
        public Product()
        {
            this.Pictures = new List<FilePath>();
            this.ProductDescriptionItems = new List<ProductDescriptionItem>();
            this.Comments = new List<ProductComment>();
            this.ChangeHistory = new List<ProductChangeLog>();
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name ="Producer Code")]
        public string ProducerCode { get; set; }

        [Required]
        [Display(Name = "Catalog Code")]
        public string CatalogCode { get; set; }

        [Required]
        [Display(Name = "Sale Price")]
        public double SalePrice { get; set; }

        [Required]
        [Display(Name = "Number In Stock")]
        public int NumberInStock { get; set; }

        [Required]
        [Display(Name = "Added On")]
        public DateTime AddedOn { get; set; }

        [Display(Name = "Added By")]
        public string AddedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime? LastModifiedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public AppUser LastModifiedBy { get; set; }
        public string LastModifiedById { get; set; }


        public Subcategory Subcategory { get; set; }

        [Required]
        public int SubcategoryId { get; set; }

        public string ExtendedDescriptionMarkup { get; set; }

        public List<ProductDescriptionItem> ProductDescriptionItems { get; set; }
        public virtual List<FilePath> Pictures { get; set; }
        public List<ProductComment> Comments { get; set; }
        public List<ProductChangeLog> ChangeHistory { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public bool IsAvailable { get => this.NumberInStock > 0; }

        public double RatingOverall
        {
            get
            {
                return Math.Round((this.Comments.Sum(r => r.RatingValue) / (float)this.Comments.Where(c => c.RatingValue != 0).Count()), 2);
            }
        }
        public bool IsRated
        {
            get
            {
                return (this.Comments.Where(c => c.RatingValue != 0).Count() > 0);
            }
        }
        public int RatingCount
        {
            get
            {
                return (this.Comments.Where(c => c.RatingValue != 0).Count());
            }
        }

    }
}
