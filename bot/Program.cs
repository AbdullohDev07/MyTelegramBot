namespace OneTask
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            BotHandler bt = new BotHandler();

            try
            {
                await bt.BotStart();
            }
            catch
            {
                await bt.BotStart();
            }
        }
    }
}