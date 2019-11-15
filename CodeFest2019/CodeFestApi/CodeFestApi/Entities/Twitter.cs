namespace CodeFest.Api.Entities
{
	public partial class Twitter
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public double? Score { get; set; }
    }
}
