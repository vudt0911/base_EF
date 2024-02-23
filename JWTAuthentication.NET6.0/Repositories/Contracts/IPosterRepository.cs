using JWTAuthentication.NET6._0.Models.Entities;
using JWTAuthentication.NET6._0.Models.Models;

namespace JWTAuthentication.NET6._0.Repositories.Contracts
{
    public interface IPosterRepository
    {
        List<PosterEntity> GetAll();
        PosterEntity? GetPosterById(int posterId);
        PosterEntity AddPoster(PosterRequest posterRequest);
        bool UpdatePoster(PosterRequest posterRequest);
        bool DeletePoster(PosterEntity poster);
    }
}
