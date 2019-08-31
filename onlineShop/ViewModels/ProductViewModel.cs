using Microsoft.AspNetCore.Http;
using onlineShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace onlineShop.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            this.PicturesToUpload = new List<IFormFile>();
            this.Pictures = new List<FilePath>();
            this.ProductDescriptionItems = new List<ProductDescriptionItem>();
            this.Comments = new List<ProductComment>();
            this.ChangeHistory = new List<ProductChangeLog>();
        }

        [Display(Name = "Remove all existing images")]
        public bool RemoveExistingImages { get; set; }

        public List<IFormFile> PicturesToUpload { get; set; }


        // =========================================
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(75)]
        [Display(Name ="Producer Code")]
        public string ProducerCode { get; set; }

        [Required]
        [MaxLength(75)]
        [Display(Name = "Catalog Code")]
        public string CatalogCode { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Sale Price")]
        public double SalePrice { get; set; }

        [Required]
        [Range(0, 100000)]
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
        public string LastModifiedById { get; set; }
        public string LastModifiedByName { get; set; }

        public int SubcategoryId { get; set; }

        public string ExtendedDescriptionMarkup { get; set; }

        public List<ProductDescriptionItem> ProductDescriptionItems { get; set; }
        public List<FilePath> Pictures { get; set; }
        public List<ProductComment> Comments { get; set; }
        public List<ProductChangeLog> ChangeHistory { get; set; }

        [Display(Name="Status")]
        public bool IsActive { get; set; }
        public bool IsAvailable { get => this.NumberInStock > 0; }

        public bool IsRatedByUser { get; set; }
        public bool IsWatchedByUser { get; set; }
        public bool NoUserAccount { get; set; }

        public double RatingOverall
        {
            get
            {
                return Math.Round((this.Comments.Where(c => c.IsPublished).Sum(r => r.RatingValue) / (float)this.Comments.Where(c => c.RatingValue != 0 && c.IsPublished).Count()), 2);
            }
        }
        public bool IsRated
        {
            get
            {
                return (this.Comments.Where(c => c.RatingValue != 0 && c.IsPublished).Count() > 0);
            }
        }
        public int RatingCount
        {
            get
            {
                return (this.Comments.Where(c => c.RatingValue != 0 && c.IsPublished).Count());
            }
        }

        public int PublishedCommentsCount { get { return this.Comments.Where(c => c.IsPublished).Count(); } }

    }
}
