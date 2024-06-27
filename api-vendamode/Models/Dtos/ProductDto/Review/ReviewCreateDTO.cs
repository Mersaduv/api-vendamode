using api_vendace.Entities.Products;
using api_vendamode.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace api_vendace.Models.Dtos.ProductDto.Review
{
    public class ReviewCreateDTO
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public List<Points>? PositivePoints { get; set; }
        public List<Points>? NegativePoints { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<IFormFile> ProductThumbnails { get; set; } = new List<IFormFile>();

        public static async ValueTask<ReviewCreateDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            var form = await context.Request.ReadFormAsync();

            var thumbnailFiles = form.Files.GetFiles("ProductThumbnails");
            var productThumbnails = thumbnailFiles.Any() ? thumbnailFiles.ToList() : new List<IFormFile>();

            var productId = Guid.TryParse(form["ProductId"], out var productIdParsed) ? productIdParsed : Guid.Empty;

            var positivePoints = form["PositivePoints"].ToList();
            var negativePoints = form["NegativePoints"].ToList();
            
             var positivePointsObject = positivePoints != null ? ParseHelper.ParseData<Points>(positivePoints) : null;
            var negativePointsObject = negativePoints != null ? ParseHelper.ParseData<Points>(negativePoints) : null;

            var rating = int.TryParse(form["Rating"], out var ratingParsed) ? ratingParsed : 0;
            var comment = form["Comment"];

            if (productId == Guid.Empty || rating == 0 || string.IsNullOrEmpty(comment))
            {
                return null;
            }

            return new ReviewCreateDTO
            {
                ProductThumbnails = productThumbnails,
                PositivePoints =positivePointsObject,
                NegativePoints =negativePointsObject,
                Rating = rating,
                Comment = comment!,
                ProductId = productId
            };
        }
    }
}
