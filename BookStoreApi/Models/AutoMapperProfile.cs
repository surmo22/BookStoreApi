using AutoMapper;
using BookStoreApi.Models.DTO.Author;
using BookStoreApi.Models.DTO.Book;
using BookStoreApi.Models.DTO.Genre;

namespace BookStoreApi.Models
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<InputAuthorDto, Author>();
                config.CreateMap<InputBookDto, Book>();
                config.CreateMap<InputGenreDto, Genre>();
            });

            return mappingConfig;
        }
    }
}
