namespace SystemOcenianiaSimple.Models
{
    public abstract class PersonBase : EntityBase
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string FullName => (FirstName + " " + LastName).Trim();
    }
}
