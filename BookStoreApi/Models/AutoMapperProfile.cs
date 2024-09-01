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
                config.CreateMap<AuthorDto, Author>().ReverseMap();
                config.CreateMap<BookDto, Book>().ReverseMap();
                config.CreateMap<GenreDto, Genre>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
