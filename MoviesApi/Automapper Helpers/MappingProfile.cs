namespace MoviesApi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDetailsDto>();
            CreateMap<MovieBaseDto, Movie>()
                .ForMember(src => src.Poster, opt => opt.Ignore());
        }
    }
}
