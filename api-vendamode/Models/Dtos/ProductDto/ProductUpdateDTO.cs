using System.Globalization;
using System.Reflection;
using api_vendace.Entities.Products;
using api_vendace.Enums;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Dtos.ProductDto.Stock;
using api_vendace.Utility;
using api_vendamode.Utility;
using Newtonsoft.Json;

namespace api_vendace.Models.Dtos.ProductDto;

public class ProductUpdateDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string StockTag { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Date { get; set; } = string.Empty;
    public IFormFile MainThumbnail { get; set; } = default!;
    public List<IFormFile> Thumbnail { get; set; } = [];
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsFake { get; set; }
    public Guid? BrandId { get; set; }
    public List<Guid>? FeatureValueIds { get; set; }
    public ProductScaleDTO? ProductScale { get; set; }
    public List<StockItemDTO>? StockItems { get; set; }
    public StatusType Status { get; set; }
    public DateTimeOffset? ParsedDate { get; set; }

    public static async ValueTask<ProductUpdateDTO?> BindAsync(HttpContext context,
                                                                 ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();
        var mainThumbnail = form.Files.GetFile("MainThumbnail");
        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var productId = Guid.Parse(form["Id"]!);
        var title = form["Title"];
        var stockTag = form["StockTag"];
        var isActive = bool.Parse(form["IsActive"]!);
        var categoryId = Guid.Parse(form["CategoryId"]!);
        var description = form["Description"];
        var isFake = bool.Parse(form["IsFake"]!);
        var brandId = string.IsNullOrEmpty(form["BrandId"]) ? null : (Guid?)Guid.Parse(form["BrandId"]!);
        var statusForm = form["Status"];
        var status = Enum.Parse<StatusType>(statusForm!);
        List<Guid> featureValueIds = new List<Guid>();
        foreach (var id in form["FeatureValueIds"])
        {
            var idParts = id?.Split(',');
            foreach (var part in idParts!)
            {
                if (Guid.TryParse(part, out Guid guid))
                {
                    featureValueIds.Add(guid);
                }
                else
                {
                    throw new FormatException($"Invalid GUID format: {part}");
                }
            }
        }
        var dateStr = form["Date"];
        var parsedDate = ConvertToDateTimeOffset(dateStr);
        var productScaleData = form["ProductScale"];
        var productScale = string.IsNullOrEmpty(productScaleData) ? null : JsonConvert.DeserializeObject<ProductScaleDTO>(productScaleData!);

        var stockItemsData = form["StockItems"];
        List<StockItemDTO> stockItems = new List<StockItemDTO>();
        if (!string.IsNullOrEmpty(stockItemsData))
        {
            var stockItemsTempList = JsonConvert.DeserializeObject<List<StockItemTempDTO>>(stockItemsData);
            foreach (var tempItem in stockItemsTempList!)
            {
                var thumbnailStock = form.Files.GetFile($"ImageStock_{tempItem.StockId}");
                var stockItem = new StockItemDTO
                {
                    StockId = tempItem.StockId,
                    Idx = tempItem.Idx,
                    IsHidden = tempItem.IsHidden,
                    ImageStock = thumbnailStock,
                    FeatureValueId = tempItem.FeatureValueId,
                    SizeId = tempItem.SizeId,
                    Quantity = tempItem.Quantity ?? 0,
                    Price = tempItem.Price ?? 0,
                    Discount = tempItem.Discount ?? 0,
                    PurchasePrice = tempItem.PurchasePrice,
                    Weight = tempItem.Weight,
                    OfferTime = tempItem.OfferTime ?? 0,
                    AdditionalProperties = tempItem.AdditionalProperties
                };
                stockItems.Add(stockItem);
            }
        }

        return new ProductUpdateDTO
        {
            Id = productId,
            MainThumbnail = mainThumbnail!,
            Thumbnail = thumbnail!,
            Title = title!,
            StockTag = stockTag,
            IsActive = isActive,
            ParsedDate = parsedDate,
            Date = dateStr,
            CategoryId = categoryId,
            Description = description!,
            IsFake = isFake,
            Status = status,
            BrandId = brandId,
            FeatureValueIds = featureValueIds,
            StockItems = stockItems,
            ProductScale = productScale
        };
    }
    public static DateTimeOffset? ConvertToDateTimeOffset(string input)
    {
        try
        {
            // جدا کردن زمان و تاریخ
            string[] parts = input.Split(" - ");
            string timePart = parts[0]; // "13:05:00"
            string datePart = parts[1]; // "1403/06/24"
            timePart = ConvertPersianNumbersToEnglish(timePart);
            datePart = ConvertPersianNumbersToEnglish(datePart);
            // جدا کردن اجزای تاریخ
            string[] dateParts = datePart.Split('/');
            int persianYear = int.Parse(dateParts[0]);
            int persianMonth = int.Parse(dateParts[1]);
            int persianDay = int.Parse(dateParts[2]);

            // جدا کردن اجزای زمان
            string[] timeParts = timePart.Split(':');
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);
            int second = int.Parse(timeParts[2]);

            // ایجاد یک PersianCalendar
            PersianCalendar persianCalendar = new PersianCalendar();

            // تبدیل تاریخ شمسی به میلادی
            DateTime gregorianDateTime = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, hour, minute, second, 0);

            // تنظیم منطقه زمانی (به عنوان مثال +03:30)
            TimeSpan offset = new TimeSpan(3, 30, 0);

            // ایجاد DateTimeOffset با تنظیم منطقه زمانی
            DateTimeOffset dateTimeOffset = new DateTimeOffset(gregorianDateTime, offset);

            return dateTimeOffset.ToUniversalTime(); ;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message, "exexexexex");
            // اگر خطایی رخ داد، مقدار null برگردانید
            return null;
        }
    }
    public static string ConvertPersianNumbersToEnglish(string input)
    {
        string[] persianNumbers = new string[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
        string[] englishNumbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        for (int i = 0; i < persianNumbers.Length; i++)
        {
            input = input.Replace(persianNumbers[i], englishNumbers[i]);
        }

        return input;
    }
}
