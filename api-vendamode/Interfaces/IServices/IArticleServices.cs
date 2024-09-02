using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.ArticleDto;

namespace api_vendamode.Interfaces.IServices;

public interface IArticleServices
{
    Task<ServiceResponse<Guid>> UpsertArticle(ArticleUpsertDTO articleUpsert);
    Task<ServiceResponse<bool>> DeleteArticle(Guid id);
    Task<ServiceResponse<Pagination<ArticleDto>>> GetAllArticles(RequestQuery requestQuery);
    Task<ServiceResponse<bool>> DeleteTrashAsync(Guid id);
    Task<ServiceResponse<bool>> RestoreArticleAsync(Guid id);
    Task<ServiceResponse<ArticleDto>> GetArticle(Guid id);
    Task<ServiceResponse<ArticleDto>> GetBy(Guid? id);
    Task<ServiceResponse<ArticleDto>> GetBy(string? id);

}