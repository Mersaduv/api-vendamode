
using api_vendace.Const;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.ProductDto;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class SliderEndpoints
{
    public static IEndpointRouteBuilder MapSliderApi(this IEndpointRouteBuilder apiGroup)
    {
        var sliderGroup = apiGroup.MapGroup(Constants.Slider);

        apiGroup.MapGet(Constants.Sliders, GetAllSliders);

        apiGroup.MapGet($"main/{Constants.Sliders}", GetMainSliders);

        sliderGroup.MapPost(string.Empty, CreateSlider)
        .Accepts<SliderCreateDto>("multipart/form-data");

        sliderGroup.MapPost("update", UpdateSlider)
        .Accepts<SliderUpdateDto>("multipart/form-data");

        sliderGroup.MapPut(string.Empty, UpdateSlider);

        sliderGroup.MapGet("{id:guid}", GetSlider);

        sliderGroup.MapDelete("{id:guid}", DeleteSlider);

        return apiGroup;
    }
    private static async Task<Ok<ServiceResponse<bool>>> CreateSlider(ISliderServices sliderServices,
               SliderCreateDto slider, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Slider");

        var result = await sliderServices.AddSlider(slider);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateSlider(ISliderServices sliderServices, SliderUpdateDto slider, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Slider");

        var result = await sliderServices.UpdateSlider(slider);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteSlider(ISliderServices sliderServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Slider");

        var result = await sliderServices.DeleteSlider(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<SliderDto>>> GetSlider(ISliderServices sliderServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Slider");

        var result = await sliderServices.GetSliderBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<SliderDto>>>> GetAllSliders(ISliderServices sliderServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Sliders");

        var result = await sliderServices.GetSliders();

        return TypedResults.Ok(result);
    }

        private async static Task<Ok<ServiceResponse<IReadOnlyList<SliderDto>>>> GetMainSliders(ISliderServices sliderServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting Main Sliders");

        var result = await sliderServices.GetMainSliders();

        return TypedResults.Ok(result);
    }
}
