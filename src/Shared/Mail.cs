namespace EctBlazorApp.Shared
{
    public abstract class Mail
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int EctUserId { get; set; }
    }
}
