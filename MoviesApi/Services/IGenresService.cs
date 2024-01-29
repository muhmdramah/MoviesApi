﻿namespace MoviesApi.Services
{
    public interface IGenresService
    {
        Task<IEnumerable<Genre>> GetAll(); 
        Task<Genre> GetById(byte id); 
        Task<Genre> Create(Genre genre); 
        Genre Update(Genre genre); 
        Genre Delete(Genre genre); 
        Task<bool> IsValidGenre(byte id);
    }
}
