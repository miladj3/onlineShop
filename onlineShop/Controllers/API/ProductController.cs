using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Models;
using onlineShop.Models.ProductModels;
using onlineShop.Extensions;
using Microsoft.AspNetCore.Authorization;
using onlineShop.Repositories;
using Newtonsoft.Json.Linq;

namespace onlineShop.Controllers.API
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductRepository _productRepository;

        public ProductController(
            UserManager<AppUser> userManager,
            IProductRepository productRepository)
        {
            _userManager = userManager;
            _productRepository = productRepository;
        }

        [HttpPost("/API/Product/AvailabilityNotification/Add/{productId}/{guestEmail?}")]
        public IActionResult AddAvailabilityNotification([FromRoute] int productId, [FromRoute] string guestEmail = "")
        {
            var userId = _userManager.GetUserId(User);
            var product = _productRepository.GetProductById(productId);

            if (String.IsNullOrEmpty(userId) && String.IsNullOrEmpty(guestEmail))
                return BadRequest("User not found & email not specified.");

            if (product == null)
                return BadRequest("Product not found.");

            if (product.IsAvailable)
                return BadRequest("Product is already available.");

            ProductAvailabilityNotification notification;

            if (!String.IsNullOrEmpty(userId))
            {

                if (_productRepository.ProductNotificationExistsForUserId(productId, userId))
                    return BadRequest("Notification for user already exists.");

                notification = new ProductAvailabilityNotification()
                {
                    CustomerId = userId,
                    ProductId = productId
                };
            }
            else
            {
                if(!guestEmail.IsValidEmailAddress())
                    return BadRequest("Invalid email address.");

                if (_productRepository.ProductNotifExistsForEmail(productId, guestEmail))
                    return BadRequest("Failed to create notification.");

                notification = new ProductAvailabilityNotification()
                {
                    Email = guestEmail,
                    ProductId = productId
                };
            }

            _productRepository.AddProductNotification(notification);
            _productRepository.SaveChanges();

            return Ok();
        }

        [HttpPost("/API/Product/AvailabilityNotification/Remove/{productId}")]
        public IActionResult RemoveAvailabilityNotification([FromRoute] int productId)
        {
            var userId = _userManager.GetUserId(User);
            //var product = _productRepository.GetProductById(productId);

            if (String.IsNullOrEmpty(userId))
                return BadRequest("User not found");

            var notification = _productRepository.GetProductNotificationByUserId(productId, userId);

            if (notification == null)
                return BadRequest("Notification not found.");

            _productRepository.RemoveProductNotification(notification);
            _productRepository.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost("/API/Products/Comments/Delete/{commentId}")]
        public IActionResult DeleteComment([FromRoute] int commentId)
        {
            var comment = _productRepository.GetProductCommentById(commentId);

            if (comment == null)
                return BadRequest("Comment not found");

            _productRepository.RemoveProductComment(comment);
            _productRepository.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost("/API/Products/Comments/Publish/{commentId}")]
        public IActionResult PublishComment([FromRoute] int commentId)
        {
            var comment = _productRepository.GetProductCommentById(commentId);

            if (comment == null)
                return BadRequest("Comment not found");

            comment.IsPublished = true;
            _productRepository.SaveChanges();

            return Ok();
        }

        [HttpPost("/API/Products/Comments/Add/{productId}/{rating}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductComment([FromRoute] int productId, [FromRoute] int rating, [FromBody] JObject comment)
        {
            string guestUserName = "";
            string commentText = "";

            try
            {
                commentText = comment["commentText"].Value<string>();

                if (String.IsNullOrEmpty(commentText))
                    return BadRequest("Invalid comment text.");
            }
            catch
            {
                return BadRequest("Comment text is not provided.");
            }

            var user = await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);

            // check if account is restricted (read-only)
            if (user != null && user.IsBlocked)
            {
                return BadRequest("Your request can't be processed as you account has been restricted.");
            }

            //check presence of username for guest user
            if (user == null) {
                try
                {
                    guestUserName = comment["guestUserName"].Value<string>();

                    if (String.IsNullOrEmpty(guestUserName))
                        return BadRequest("Invalid guest username.");
                }
                catch
                {
                    return BadRequest("Guest username is not provided.");
                }
            }

            var isGuestUser = (user == null);

            var prod = _productRepository.GetProductById(productId);
            if (prod == null)
                return BadRequest($"Product with id {productId} not found.");

            // prevent user from setting wrong rating value or rate some product more than once
            if (rating > 6 || rating < 1 || _productRepository.ProductIsRatedByUserId(productId, userId))
                rating = 0;

            var comm = new ProductComment
            {
                CustomerId = (!isGuestUser) ? userId : null,
                GuestUserName = (isGuestUser) ? guestUserName : null,
                DateAdded = DateTime.UtcNow,
                Text = commentText,
                Product = prod,
                RatingValue = rating,
                IsPublished = false, // await moderation before publishing
                IsVerifiedPurchase = _productRepository.ProductWasOrderedByUser(productId, userId) // check if user previously purchased this item
            };

            _productRepository.AddProductComment(comm);
            _productRepository.SaveChanges();

            return Ok();
        }
    }
}
