

using AutoMapper;
using JWTAuthentication.NET6._0.Auth;
using JWTAuthentication.NET6._0.Data;
using JWTAuthentication.NET6._0.Models.Entities;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Repositories.Contracts;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace JWTAuthentication.NET6._0.Repositories
{
    public class PosterRepository : BaseRepository, IPosterRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public PosterRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public PosterEntity AddPoster(PosterRequest posterRequest)
        {
            PosterEntity posterEntity = _mapper.Map<PosterEntity>(posterRequest);
            _context.Posters.Add(posterEntity);
            this.SaveChanges();
            return posterEntity;
        }

        public bool DeletePoster(PosterEntity poster)
        {
            throw new NotImplementedException();
        }

        public List<PosterEntity> GetAll()
        {
            List<PosterEntity> posters = _context.Posters.ToList();
            return posters;
        }

        public PosterEntity? GetPosterById(int posterId)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePoster(PosterRequest posterRequest)
        {
            throw new NotImplementedException();
        }
    }
}
