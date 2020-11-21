using AutoMapper;
using CodersAcademy.API.Model;
using CodersAcademy.API.Repository;
using CodersAcademy.API.ViewModel.Request;
using CodersAcademy.API.ViewModel.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersAcademy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserRepository UserRepository { get; set; }
        private IMapper Mapper { get; set; }
        private AlbumRepository AlbumRepository { get; set; }
        public UserController(UserRepository userRepository, IMapper mapper, AlbumRepository albumRepository)
        {
            this.UserRepository = userRepository;
            this.Mapper = mapper; ;
            this.AlbumRepository = albumRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await this.UserRepository.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = this.Mapper.Map<UserResponse>(user);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = this.UserRepository.GetAllAsync();
            var result = this.Mapper.Map<IList<UserResponse>>(users);
            return Ok(result);
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var password = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));
            var user = this.UserRepository.AuthenticateAsync(request.Email, password);

            if (user == null)
            {
                return Unauthorized(new
                {
                    Message = "Email/Senha invalidos"
                });
            }
            var result = this.Mapper.Map<UserResponse>(user);
            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Registrate([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = this.Mapper.Map<User>(request);
            user.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Password));
            user.Photo = $"https://robohash.org{Guid.NewGuid()}.png?bgset=any";
            await this.UserRepository.CreateAsync(user);
            var result = this.Mapper.Map<UserResponse>(user);

            return Created($"{result.Id}", result);
        }

        [HttpPost("{id}/favoriteMusic/{musicId}")]
        public async Task<IActionResult> SaveUserFavoriteMusic(Guid id, Guid musicId)
        {
            var music = await this.AlbumRepository.GetMusicAsync(musicId);
            var user = await this.UserRepository.GetUserAsync(id);
            if (user == null)
            {
                return UnprocessableEntity(new
                {
                    Message = "Usuario não encontrado"
                });
            }

            if (music == null)
            {
                return UnprocessableEntity(new
                {
                    Message = "Musica não encontrada"
                });
            }

            user.AddFavoriteMusic(music);
            await this.UserRepository.UpdateAsync(user);

            return Ok();
        }

        [HttpDelete("{id}/favoriteMusic/{musicId}")]
        public async Task<IActionResult> RemoveUserFavoriteMusic(Guid id, Guid musicId)
        {
            var music = await this.AlbumRepository.GetMusicAsync(musicId);
            var user = await this.UserRepository.GetUserAsync(id);
            if (user == null)
            {
                return UnprocessableEntity(new
                {
                    Message = "Usuario não encontrado"
                });
            }

            if (music == null)
            {
                return UnprocessableEntity(new
                {
                    Message = "Musica não encontrada"
                });
            }

            user.RemoveFavoriteMusic(music);
            await this.UserRepository.UpdateAsync(user);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser(Guid id, Guid musicId)
        {
            var user = await this.UserRepository.GetUserAsync(id);
            if (user == null)
            {
                return UnprocessableEntity(new
                {
                    Message = "Usuario não encontrado"
                });
            }
            await this.UserRepository.RemoveAsync(user);

            return NoContent();
        }

    }
}
