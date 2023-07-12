public class UseBaseImplementation
{
    public void SecurityDecision(BaseImplementation someImplementation)
    {
        if (someImplementation.UserIsValidated() == true)
        {
            Console.WriteLine("Account number & balance.");
        }
        else
        {
            Console.WriteLine("Please login.");
        }
    }
}