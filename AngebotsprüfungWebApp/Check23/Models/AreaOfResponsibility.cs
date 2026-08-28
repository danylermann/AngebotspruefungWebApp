namespace Check23.Models
{
    public class AreaOfResponsibility
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AreaOfResponsibility()
        {
        }

        public AreaOfResponsibility(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
