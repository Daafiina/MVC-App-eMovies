using eMovies.Data.Base;
using eMovies.Models;

namespace eMovies.Data.Services
{
    public class ProducersService : EntityBaseRepository<Producer>, IProducersService
    {
        public ProducersService(AppDbContext context) : base(context)
        {

        }
    }
}
