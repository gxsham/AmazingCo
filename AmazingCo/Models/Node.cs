namespace AmazingCo.Models
{
    public class Node
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string RootId { get; set; }
        public int Height { get; set; }
    }
}
