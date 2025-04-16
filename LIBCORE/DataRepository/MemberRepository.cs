namespace LIBCORE.DataRepository
{
    public partial class MemberRepository : IMemberRepository
    {
        string IMemberRepository.ExampleRepositoryMember()
        {
            return "Implementation for the ExampleRepositoryMember() located in the IMemberRepository directly under the DataRepository folder";
        }
    }
}
