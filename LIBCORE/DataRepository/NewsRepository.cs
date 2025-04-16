namespace LIBCORE.DataRepository
{
    public partial class NewsRepository : INewsRepository
    {
        string INewsRepository.ExampleRepositoryNews()
        {
            return "Implementation for the ExampleRepositoryNews() located in the INewsRepository directly under the DataRepository folder";
        }
  
    }
}
