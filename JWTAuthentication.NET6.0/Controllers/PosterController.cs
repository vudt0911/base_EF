using AutoMapper;
using JWTAuthentication.NET6._0.Models.DTO;
using JWTAuthentication.NET6._0.Models.Entities;
using JWTAuthentication.NET6._0.Models.Models;
using JWTAuthentication.NET6._0.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication.NET6._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosterController : Controller
    {
        private readonly IPosterRepository _posterRepository;
        private readonly IMapper _mapper;
        public PosterController(IPosterRepository posterRepository, IMapper mapper)
        {
            _posterRepository = posterRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<PosterEntity> posters = _posterRepository.GetAll();
            List<PosterDTO> listPosters = _mapper.Map<List<PosterDTO>>(posters);
            return Ok(listPosters);
        }

        [HttpPost]
        public IActionResult AddPoster([FromBody] PosterRequest posterRequest)
        {
            PosterEntity posterEntity = _posterRepository.AddPoster(posterRequest);
            PosterDTO posterViewModel = _mapper.Map<PosterDTO>(posterEntity);
            return Ok(posterViewModel);
        }
    }
}
