namespace TinderProject.Repositories
{
    public class InteractionRepository
    {
        private readonly AppDbContext _context;

        public InteractionRepository(AppDbContext context)
        {
            _context = context;
        }
        //public ICollection<User?> GetLikedUsers(int userId)
        //{
        //    return _context.Interactions.Where(x => x.LikerId == userId).ToArray();
        //}
    }
}
