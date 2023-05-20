namespace TinderProject.Repositories
{
	public class InteractionRepository
	{
		private readonly AppDbContext _context;

		public InteractionRepository(AppDbContext context)
		{
			_context = context;
		}
	}
}
